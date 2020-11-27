using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public float MinZoomDistance = 1.0f;
    public float RotationDegrees = 10.0f;

    private float rotationDelta = 0;
    private Vector3 vLookAt = Vector3.zero;

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
}
