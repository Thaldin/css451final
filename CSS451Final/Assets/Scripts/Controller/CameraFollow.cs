using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    [SerializeField] Transform cameraTarget = null;
    [SerializeField] bool targetFollowIsOn = false;
    [SerializeField] float followDistance = 0f;

    public float yCamOffset = 3f;
    public float yCenterOffset = -10f;

    private void Awake() {
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
            followDistance = (followDistance <= 20f) ? 10f : followDistance;
            followDistance = (followDistance >= 400f) ? 400f : followDistance;

        }

        // if null
        if (t == null) {
            targetFollowIsOn = false;
            cameraTarget = null;
            followDistance = 0f;
        }
    }
}
