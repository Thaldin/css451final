using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public float MinZoomDistance = 1.0f;
    public float RotationDegrees = 10.0f;

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
        var v = vLookAt - transform.localPosition;
        float distance;

        // Check Zoom distance
        if (v.magnitude >= MinZoomDistance)
        {
            distance = v.magnitude + -delta;
        }
        else
        {
            distance = MinZoomDistance;
        }

        transform.localPosition = vLookAt - (distance * v.normalized);
    }

    // Take mouse input and tumble the camera
    public void Tumble(Vector3 tumble)
    {
        // Checks to prevent large camera jumps that flip the camera
        if ((tumble.y < 30) && (tumble.y > -30))
        {
            tumbleCam(tumble.y, transform.right);
        }

        if ((tumble.x < 30) && (tumble.x > -30))
        {
            tumbleCam(tumble.x, Vector3.up);
        }
    }

    private void tumbleCam(float value, Vector3 direction)
    {
        // Set rotation point and position values
        var quat = Quaternion.AngleAxis(value * rotationDelta, direction);
        var r = Matrix4x4.Rotate(quat);
        var invP = Matrix4x4.TRS(-vLookAt, Quaternion.identity, Vector3.one);
        r = invP.inverse * r * invP;
        var pos = r.MultiplyPoint(transform.localPosition);

        // Store old position in case min/max angle exceeded
        var oldPos = transform.localPosition;

        // Transform to new position/rotation
        transform.localPosition = pos;
        transform.localRotation = quat * transform.localRotation;

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
        Camera.main.transform.localPosition += slide.x * Vector3.right;
        Camera.main.transform.localPosition += slide.y * Vector3.up;
    }
}
