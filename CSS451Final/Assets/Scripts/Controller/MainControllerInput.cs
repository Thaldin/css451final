using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class MainController : MonoBehaviour {

    public DeathStar deathStar = null;
    bool debugIsOn = false;
    bool ringIsOn = true;
    public bool pauseMenuIsOn = false;
    bool targetFollowIsOn = false;
    bool particleSystemIsOn = false;
    bool HUDIsOn = true;

    bool moveCam = false;
    Vector3 vPrevMouseDownPos = Vector3.zero;

    void CheckKeyboard() {
        // toggles menu
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Escape();
        }

        // set asteroid type to heatseeking (targeted)
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            Key1();
        }

        // set asteroid type to homing (closest target)
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            Key2();
        }

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

        // toggle HUD
        if (Input.GetKeyDown(KeyCode.H)) {
            KeyH();

        }
        // deathstar fire
        if (Input.GetKeyDown(KeyCode.Space)) {
            KeySpace();
        }

        Vector2 mouseScrollDelta = Input.mouseScrollDelta;
        if (mouseScrollDelta.y != 0f) {
            MouseScroll(mouseScrollDelta.y);
        }
        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
        {
            moveCam = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt))
        {
            moveCam = false;
        }
    }

    void Click()
    {
        // Left mouse click
        if (Input.GetMouseButtonDown(0)) {

            if (moveCam)
            {
                vPrevMouseDownPos = Input.mousePosition;
            }
            else
            {
                // ray
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                Transform cameraLookAtTarget = null;
                //camera
                if (EventSystem.current.IsPointerOverGameObject()) { return; }

                //            if (!EventSystem.current.IsPointerOverGameObject()) {
                if (Physics.Raycast(ray, out hit))
                {
                    string tag = hit.transform.tag;

                    // potential
                    switch (tag)
                    {
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
                    //ToggleCameraFollowTarget(cameraLookAtTarget);
                }
                Debug.Log("Clicked Space");
                //            }
                Debug.Log("Clicked UI");
                ToggleCameraFollowTarget(cameraLookAtTarget);
            }
        }

        // Left mouse down
        if (Input.GetMouseButton(0))
        {
            if (moveCam)
            {
                var delta = vPrevMouseDownPos - Input.mousePosition;
                vPrevMouseDownPos = Input.mousePosition;
                MainCamera.gameObject.GetComponent<CameraBehavior>().Tumble(delta);
            }
        }
        
        // Right mouse click
        if (Input.GetMouseButtonDown(1))
        {
            if (moveCam)
            {
                vPrevMouseDownPos = Input.mousePosition;
            }
        }

        // Right mouse down
        if (Input.GetMouseButton(1))
        {
            if (moveCam)
            {
                var delta = vPrevMouseDownPos - Input.mousePosition;
                vPrevMouseDownPos = Input.mousePosition;
                TheWorld.SlideLookAtPos(delta.x, delta.y);
                MainCamera.gameObject.GetComponent<CameraBehavior>().Slide(delta);
                MainCamera.gameObject.GetComponent<CameraBehavior>().SetLookAt(TheWorld.GetLookAtPos());
            }
        }
    }

    #region Key Functions

    private void Escape() {
        // TODO: if you hit resume, it does not set pauseMenuIsOn in this scope
        //  have to double tap esc to trigger
        // pauseMenuIsOn = !pauseMenuIsOn;
        // pauseMenu.Set(pauseMenuIsOn);
        pauseMenu.Set(true);
    }
    private void KeyF1() {
        debugIsOn = !debugIsOn;
        TheWorld.ToggleDebug(debugIsOn);
        asteroidSpawner.ToggleDebug(debugIsOn);
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

    private void KeyH() {
        HUDIsOn = !HUDIsOn;
        NodeControl.ToggleHUD(HUDIsOn);
    }

    private void KeySpace() {
        Debug.Log("You may fire ");
        deathStar.Fire(TheWorld.closestTarget);
    }

    // set asteroid projectile type to heatseeking (single target)
    private void Key1() {
        asteroidSpawner.SetProjectileType(0);
    }
    // set asteroid projectile type to homing (closet target)
    private void Key2() {
        asteroidSpawner.SetProjectileType(1);
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
            timeScale.gameObject.SetActive(true);
            systemScale.gameObject.SetActive(true);
        } else {
            NodeControl.SetMenuIndex(0);
            timeScale.gameObject.SetActive(false);
            systemScale.gameObject.SetActive(false);
        }
    }

    private void MouseScroll(float v) {

        if (moveCam)
        {
            if (Input.mouseScrollDelta.y != 0)
            {
                MainCamera.gameObject.GetComponent<CameraBehavior>().SetZoom(Input.mouseScrollDelta.y);
            }
        }
    }

    #endregion
}
