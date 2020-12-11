﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class MainController : MonoBehaviour {

    bool debugIsOn = false;
    bool ringIsOn = true;
    bool targetFollowIsOn = false;
    bool particleSystemIsOn = false;
    void CheckKeyboard() {
        // toggles debug lines
        if (Input.GetKeyDown(KeyCode.F1)) {
            KeyF1();
        }

        // reset main camera to default position
        if (Input.GetKeyDown(KeyCode.F2)) {
            KeyF2();
        }

        // reset main camera to default position
        if (Input.GetKeyDown(KeyCode.F3)) {
            KeyF3();
        }

        // toggles rings
        if (Input.GetKeyDown(KeyCode.R)) {
            KeyR();
        }

        Vector2 mouseScrollDelta = Input.mouseScrollDelta;
        if (mouseScrollDelta.y != 0f) {
            MouseScroll(mouseScrollDelta.y);
        }
    }

    void Click() {
        if (Input.GetMouseButtonDown(0)) {
            // ray
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //camera
            if (Physics.Raycast(ray, out hit)) {
                if (!EventSystem.current.IsPointerOverGameObject()) {
                    Transform cameraLookAtTarget = null;
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
                            break;
                    }
                    Debug.Log("Click" + cameraLookAtTarget);
                    ToggleCameraFollowTarget(cameraLookAtTarget);
                }
                Debug.Log("Clicked UI");
            }

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

    private void KeyF3() {
        particleSystemIsOn = !particleSystemIsOn;
        ParticleSystems.SetActive(particleSystemIsOn);
    }

    private void KeyR() {
        ringIsOn = !ringIsOn;
        TheWorld.ToggleRings(ringIsOn);
    }
    #endregion

    #region Mouse Clicks
    private void ToggleCameraFollowTarget(Transform t) {
        if (t != null) {

            // get selection index
            sphereColliderScript scs;
            t.TryGetComponent<sphereColliderScript>(out scs);
            int i = (scs) ? scs.GetIndex() : 0;

            NodeControl.SetMenuIndex(i + 1);
        } else {
            NodeControl.SetMenuIndex(0);
        }
    }

    private void MouseScroll(float v) { 
        
    }

    #endregion
}
