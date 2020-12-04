using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VertexPrefab : MonoBehaviour {

    public Color selectionColor = Color.yellow;
    private Color color;
    private MeshRenderer meshRenderer;
    public Vector3 ogPosition = Vector3.zero;
    [SerializeField] private bool isSelected = false;
    private bool handleSelected = false;
    [SerializeField] private bool isOn = false;

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
        gameObject.SetActive(isOn);
    }
    

    public void Selected(bool _selected) {
        // 3
        Debug.Log("3. "+ _selected);
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

    public void ToggleVertexPrefab(bool _isOn) {
        //Debug.Log(_isOn + " " + name + " prefab Level");
        isOn = _isOn;
        gameObject.SetActive(isOn);
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

    public GameObject GetHandle() {
        return currentHandleSelected;
    }

    public void Destroy() {
        Destroy(gameObject);
    }
    
}
