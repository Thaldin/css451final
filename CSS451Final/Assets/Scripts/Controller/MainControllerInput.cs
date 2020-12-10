using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class MainController : MonoBehaviour {

    bool debugIsOn = false;
    bool ringIsOn = true;
    bool targetFollowIsOn = false;
    void CheckKeyboard() {
        // toggles debug lines
        if (Input.GetKeyDown(KeyCode.F1)) {
            KeyF1();
        }

        // reset main camera to default position
        if (Input.GetKeyDown(KeyCode.F1)) {
            KeyF2();
        }

        // toggles rings
        if (Input.GetKeyDown(KeyCode.R)) {
            KeyR();
        }
    }

    void Click() {
        if (Input.GetMouseButtonDown(0)) {
            // ray
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //camera
            Transform cameraLookAtTarget = null;
            float cameraFollowDistance = 0f;

            if (Physics.Raycast(ray, out hit)) {
                if (!EventSystem.current.IsPointerOverGameObject()) {
                    string tag = hit.transform.tag;

                    // potential
                    switch (tag) {
                        case "star":
                            goto default;
                        case "planet":
                            goto default;

                        case "moon":
                            goto default;

                        case "dwarf":
                            goto default;

                        default:
                            Debug.Log(hit.transform.name + " selected!");
                            cameraLookAtTarget = hit.transform;
                            cameraFollowDistance = hit.transform.GetComponent<SphereCollider>().radius;
                            break;
                    }
                }
            }
            ToggleCameraFollowTarget(cameraLookAtTarget, cameraFollowDistance);
        }
    }

    #region Key Functions
    private void KeyF1() {
        debugIsOn = !debugIsOn;
        TheWorld.ToggleDebug(debugIsOn);
    }

    private void KeyF2() {
        NodeControl.ResetMainCamera();
    }

    private void KeyR() {
        ringIsOn = !ringIsOn;
        TheWorld.ToggleRings(ringIsOn);
    }
    #endregion

    #region Mouse Clicks
    private void ToggleCameraFollowTarget(Transform t, float d = 0f) {
        NodeControl.ToggleFollowTarget(t, d);
        
    }
    #endregion
}
