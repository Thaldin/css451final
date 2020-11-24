using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TheWorld : MonoBehaviour {
    [SerializeField]
    private GameObject currentSelection = null;

    [SerializeField]
    private GameObject currentHandleSelected = null;

    private Vector3 delta = Vector3.zero;
    private Vector3 mouseDownPos = Vector3.zero;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            mouseDownPos = Input.mousePosition;
            delta = Vector3.zero;
        }

        if (Input.GetMouseButtonDown(0)) {
            click(0);
        }
        if (Input.GetMouseButtonDown(1)) {
            click(1);
        }

        
    }

    private void click(int _button) {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        switch (_button) {
            // left mouse
            case 0:
                if (Physics.Raycast(ray, out hit)) {
                    // clear UI
                    if (!EventSystem.current.IsPointerOverGameObject()) {
                        // vertex
                        if (hit.transform.CompareTag("vertex")) {
                            // if selection, clear it
                            if (currentSelection) {
                                currentSelection.GetComponent<VertexPrefab>().Selected(false);
                            }
                            // reassign selection
                            currentSelection = hit.transform.gameObject;
                            currentSelection.GetComponent<VertexPrefab>().Selected(true);
                            return;
                        }

                        // vertex handle
                        delta = mouseDownPos - Input.mousePosition;
                        mouseDownPos = Input.mousePosition;
                        currentSelection.GetComponent<VertexPrefab>().Translate(hit.transform.tag, delta);
                        currentHandleSelected = currentSelection.GetComponent<VertexPrefab>().GetHandle();

                    }
                }
                return;
            case 1:
                if (currentSelection) { 
                    currentSelection.GetComponent<VertexPrefab>().Unselect();
                    currentSelection.GetComponent<VertexPrefab>().Selected(false);
                    currentSelection = null;
                    currentHandleSelected = null;
                    
                }
                return;
            default:
                return;
        }
    }
}
