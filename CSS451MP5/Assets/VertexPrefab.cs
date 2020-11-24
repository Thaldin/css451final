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

    private bool isSelected = false;
    private GameObject currentHandleSelected = null;
    public GameObject[] handles = new GameObject[3];
    // Start is called before the first frame update
    void Start() {
        InitializedComponents();
        meshRenderer.material = (Material)Instantiate(meshRenderer.material);
        color = meshRenderer.material.color;
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

                        hit.transform.GetChild(0).GetComponent<vertexHandle>().Selected(true);
                    } else { 
                        Unselect();
                    }

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

    public void Translate(string _tag, Vector3 _detla) {
        switch (_tag) {
            case "xHandle":
                Translate(Vector3.right, _detla.x);
                return;
            case "yHandle":
                Translate(Vector3.up, _detla.y);
                return;
            case "zHandle":
                Translate(Vector3.forward, _detla.z);
                return;
            default:
                return;
        }
    }

    private void Translate(Vector3 _axis, float _delta) {
        //_delta = (_delta < 0) ? _delta : 0;
        //_delta = (_delta > 1) ? _delta : 1;
        Vector3 position = _axis;
        position *= _delta;
        transform.localPosition += position;
    }

    public void Unselect() {
        foreach (var h in handles) { 
            h.transform.GetChild(0).GetComponent<vertexHandle>().Selected(false);
        }
        currentHandleSelected = null;
    }

    public GameObject GetHandle() {
        return currentHandleSelected;
    }
}
