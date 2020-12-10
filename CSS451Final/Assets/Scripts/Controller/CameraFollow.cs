using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform cameraDefaultPosition = null;
    [SerializeField] Transform cameraTarget = null;
    [SerializeField] bool targetFollowIsOn = false;
    [SerializeField] float followDistance = 0f;

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
        Vector3 v = cameraTarget.transform.position - transform.position;
        v.Normalize();
        Quaternion r = Quaternion.LookRotation(v, Vector3.up);
        // --testing--
        //transform.LookAt(cameraTarget);
        transform.rotation = r;
        Vector3 p = cameraTarget.transform.position - -Vector3.forward * followDistance;
        p.y += followDistance;
        transform.position = p;
    }

    public void ToggleFollowTarget(Transform t = null, float distance = 0f) {
        if (t) {
            targetFollowIsOn = true;
            cameraTarget = t;
            followDistance = distance * 10f;
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
