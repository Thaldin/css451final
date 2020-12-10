using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform cameraDefaultPosition = null;
    [SerializeField] Transform cameraTarget = null;
    [SerializeField] bool targetFollowIsOn = false;
    [SerializeField] float followDistance = 0f;

    public float yCamOffset = 3f;
    public float yCenterOffset = -10f;

    private void Awake() {
        Debug.Assert(cameraDefaultPosition != null, "Please set " + cameraDefaultPosition + " for " + name + "in the Editor.");
    }
    // Update is called once per frame
    private void Update() {
        if (targetFollowIsOn) {
            UpdateCamera();
        }
    }

    private void UpdateCamera() {

        // target - transform
        Vector3 t = cameraTarget.transform.position;
        Vector3 c = new Vector3(0f, yCenterOffset, 0f);
        Vector3 v = c - t;
        //Vector3 v = cameraTarget.transform.position - transform.position;
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
            targetFollowIsOn = true;
            cameraTarget = t;
            followDistance = distance * 10f;
            followDistance = (followDistance <= 20f) ? 20f : followDistance;
            followDistance = (followDistance >= 400f) ? 400f : followDistance;

        }

        // if null
        if (t == null) {
            targetFollowIsOn = false;
            cameraTarget = null;
            followDistance = 0f;
        }
    }

    public void ResetPosition(){
        transform.position = cameraDefaultPosition.position;
        transform.position = cameraDefaultPosition.rotation.eulerAngles;
    }
}
