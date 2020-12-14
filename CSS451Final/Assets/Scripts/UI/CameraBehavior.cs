using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public float MinZoomDistance = 3.0f;
    public float RotationDegrees = 10.0f;
    public float ZoomModifier = 2.0f;

    private float rotationDelta = 0;
    private Vector3 vLookAt = Vector3.zero;
    private const float minYCameraAngle = 20.0f;
    private const float maxYCameraAngle = 120.0f;

    void Start()
    {
        // Set degrees per second rotation
        rotationDelta = RotationDegrees / 60;
    }

    // Update is called once per frame
    void Update()
    {
        LookAtPosition();
    }

    public void SetLookAt(Vector3 lookAtPos)
    {
        vLookAt = lookAtPos;
    }

    // Make camera look at position
    private void LookAtPosition()
    {
        var v = vLookAt - transform.localPosition;
        var w = Vector3.Cross(-v, transform.up);
        var u = Vector3.Cross(w, -v);

        transform.localRotation = Quaternion.FromToRotation(Vector3.up, u);
        var align = Quaternion.FromToRotation(transform.forward, v);
        transform.localRotation = align * transform.localRotation;
    }

    public void SetZoom(float delta)
    {
        delta *= ZoomModifier;
        var v = vLookAt - transform.localPosition;
        float distance;

        distance = v.magnitude + -delta;

        // Check Zoom distance
        if (distance >= MinZoomDistance)
        {
            transform.localPosition = vLookAt - (distance * v.normalized);
        }
    }

    // Take mouse input and tumble the camera
    public void Tumble(Vector3 tumble)
    {
        tumbleCam(tumble.y, transform.right);
        tumbleCam(-tumble.x, Vector3.up);
    }

    private void tumbleCam(float value, Vector3 direction)
    {
        // Set rotation point and position values
        var quat = Quaternion.AngleAxis(value * rotationDelta, direction);
        Matrix4x4 r = Matrix4x4.TRS(Vector3.zero, quat, Vector3.one);
        var invP = Matrix4x4.TRS(-vLookAt, Quaternion.identity, Vector3.one);
        Matrix4x4 m = invP.inverse * r * invP;

        var pos = m.MultiplyPoint(transform.localPosition);

        // Store old position in case min/max angle exceeded
        var oldPos = transform.localPosition;

        // Transform to new position/rotation
        transform.localPosition = pos;
        Vector3 v = (vLookAt - transform.localPosition).normalized;
        Vector3 w = Vector3.Cross(v, transform.up).normalized;
        Vector3 u = Vector3.Cross(w, v).normalized;

        transform.up = u;
        transform.right = w;
        transform.forward = v;

        // Check if camera has exceeded min/max angles on Y axis
        var degrees = Mathf.Acos(Vector3.Dot(pos.normalized, Vector3.up)) * Mathf.Rad2Deg;
        if (degrees <= minYCameraAngle || degrees >= maxYCameraAngle)
        {
            transform.localPosition = oldPos;
        }
    }

    // Slide camera base on mouse input
    public void Slide(Vector3 slide)
    {
        //Camera.main.transform.localPosition += slide.x * Vector3.right;
        Camera.main.transform.localPosition += slide.y * Vector3.up;
        Camera.main.transform.localPosition += slide.z * Vector3.forward;

    }

    // Try to limit the (0,0,0) camera object culling issue
    private void OnPreCull()
    {
        Camera.main.cullingMatrix = Matrix4x4.Ortho(-99999, 99999, -99999, 99999, 0.001f, 99999) *
                                    Matrix4x4.Translate(Vector3.forward * -99999 / 2f) *
                                    Camera.main.worldToCameraMatrix;
    }

    
}
