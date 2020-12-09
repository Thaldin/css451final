using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MainController : MonoBehaviour {

    bool debugIsOn = false;
    bool ringIsOn = true;
    void CheckKeyboard() {
        if (Input.GetKeyDown(KeyCode.F1)) {
            KeyF1();
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            KeyR();
        }
    }

    void Click() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                string tag = hit.transform.tag;

                // potential
                switch (tag) {
                    case "star":
                        Debug.Log(hit.transform.name + " selected!");
                        return;
                    case "planet":
                        Debug.Log(hit.transform.name + " selected!");
                        return;
                    case "moon":
                        Debug.Log(hit.transform.name + " selected!");
                        return;
                    case "dwarf":
                        Debug.Log(hit.transform.name + " selected!");
                        return;
                    default:
                        return;
                }
            }
        }
    }

    #region Key Functions
    private void KeyF1() {
        debugIsOn = !debugIsOn;
        TheWorld.ToggleDebug(debugIsOn);
    }

    private void KeyR() {
        ringIsOn = !ringIsOn;
        TheWorld.ToggleRings(ringIsOn);
    }
    #endregion
}
