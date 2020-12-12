using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    [SerializeField] Transform cameraTarget = null;
    [SerializeField] bool targetFollowIsOn = false;
    [SerializeField] float followDistance = 0f;
    Camera cam;

    public RectTransform miniCamBorder;
    public SliderWithEcho zoomSlider;

    public float yCamOffset = 3f;
    public float yCenterOffset = -10f;

    private void Awake() {
        cam = GameObject.FindGameObjectWithTag("MiniCam").GetComponent<Camera>();
    }

    // Update is called once per frame
    private void FixedUpdate() {
        if (targetFollowIsOn) {
            UpdateCamera();
        }
    }

    private void UpdateCamera() {
        Vector3 t;
        // target - transform
        // not the star
        switch (cameraTarget.tag) {
            case "star":
                t = new Vector3(followDistance, 0f, 0f);
                break;
            case "planet":
                goto default;
            case "moon":
                goto default;
            case "dwarf":
                goto default;
            default:
                t = cameraTarget.transform.position;
                break;
        }

        Vector3 c = new Vector3(0f, yCenterOffset, 0f);
        Vector3 v = c - t;
        v.Normalize();
        Quaternion r = Quaternion.LookRotation(v, Vector3.up);
        // look at center
        transform.rotation = r;

        Vector3 p = t - v * followDistance;
        p.y += yCamOffset;
        transform.position = p;
    }

    public void ToggleFollowTarget(Transform t = null, float distance = 0f) {
        if (t) {
            cam.enabled = true;
            miniCamBorder.gameObject.SetActive(true);
            targetFollowIsOn = true;
            cameraTarget = t;

            var radius = GameObject.Find(t.name).GetComponent<SphereCollider>().radius;
            followDistance = radius * 2.5f;
            followDistance = (followDistance < 1.0f) ? 1.0f : followDistance;
            zoomSlider.InitSliderRange(followDistance, 100f, followDistance);
        }

        // if null
        if (t == null) {
            targetFollowIsOn = false;
            cameraTarget = null;
            followDistance = 0f;
            cam.enabled = false;
            miniCamBorder.gameObject.SetActive(false);
        }
    }

    private void OnPreCull()
    {
        cam.cullingMatrix = Matrix4x4.Ortho(-99999, 99999, -99999, 99999, 0.001f, 99999) *
                            Matrix4x4.Translate(Vector3.forward * -99999 / 2f) *
                            cam.worldToCameraMatrix;
    }

    public void SetZoom(float delta)
    {
        Debug.Log("Set follow: " + delta);
        followDistance = delta;
        ////delta *= ZoomModifier;
        //var v = cameraTarget.localPosition - transform.localPosition;
        //float distance;

        //distance = v.magnitude + -delta;

        //transform.localPosition = cameraTarget.localPosition - (distance * v.normalized);

        //// Check Zoom distance
        ////if (distance >= MinZoomDistance)
        ////{
        ////    transform.localPosition = vLookAt - (distance * v.normalized);
        ////}
    }
}
