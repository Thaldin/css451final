using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VertexPrefab : MonoBehaviour {
    public enum Handle {
        xHandle,
        yHandle,
        zHandle
    }

    public Color selectionColor = Color.yellow;
    private Color color;
    private MeshRenderer meshRenderer;
    private Vector3 ogPosition = Vector3.zero;
    private bool isSelected = false;

    public  GameObject currentHandleSelected = null;
    public GameObject[] handles = new GameObject[3];
    // Start is called before the first frame update
    void Start() {
        InitializedComponents();
        meshRenderer.material = (Material)Instantiate(meshRenderer.material);
        color = meshRenderer.material.color;
        ogPosition = transform.position;
    }

    void InitializedComponents() {
        meshRenderer = GetComponent<MeshRenderer>();
    }


    private void Update() {
        if (isSelected) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                // clear UI
                if (!EventSystem.current.IsPointerOverGameObject()) {
                    if (hit.transform.CompareTag("xHandle") ||
                        hit.transform.CompareTag("yHandle") ||
                        hit.transform.CompareTag("zHandle")) {

                        if (currentHandleSelected) {
                            currentHandleSelected.transform.GetChild(0).GetComponent<vertexHandle>().Selected(false);
                        }
                        hit.transform.GetChild(0).GetComponent<vertexHandle>().Selected(true);
                        currentHandleSelected = hit.transform.gameObject;
                    }
                }
            } else {
                if (!currentHandleSelected) {
                    Unselect();
                }
            }
        }
    }
    

    public void Selected(bool _selected) {
        isSelected = _selected;
        meshRenderer.material.color = (isSelected) ? selectionColor : color;
        foreach (var h in handles) {
            h.SetActive(isSelected);
        }
    }

    public void Translate(Vector3 _detla) {
        Translate(currentHandleSelected.transform.GetChild(0).GetComponent<vertexHandle>().axisOfDirection, _detla);
    }

    private void Translate(Vector3 _axis, Vector3 _delta) {
        //_delta = (_delta < 0) ? _delta : 0;
        //_delta = (_delta > 1) ? _delta : 1;
        Vector3 position = _axis;
        position.x *= _delta.x /100f;
        position.y *= _delta.y /100f;
        position.z *= _delta.z /100f;

        Vector3 offset = position - ogPosition;
        transform.localPosition -= position;
        //https://docs.unity3d.com/ScriptReference/Vector3.ClampMagnitude.html
    }

    public void Unselect() {
        foreach (var h in handles) {
            h.transform.GetChild(0).GetComponent<vertexHandle>().Selected(false);
        }
        currentHandleSelected = null;
    }

    public GameObject GetHandle(string _tag) {
        foreach (var h in handles) {
            if (h.tag == _tag) {
                currentHandleSelected = h;
                return h;
            }
        }
        return null;
    }
}
