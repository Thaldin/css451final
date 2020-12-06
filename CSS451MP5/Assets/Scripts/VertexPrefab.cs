using UnityEngine;

public class VertexPrefab : MonoBehaviour {
    public Color selectionColor = Color.yellow;
    public Color unselectColor = Color.black;
    public Color cylinderColor = Color.white;

    public Vector3 ogPosition = Vector3.zero;
    public  GameObject currentHandleSelected = null;
    public int Id;

    public int row = 0;
    public bool isSelectable = true;
    private Color color;
    private MeshRenderer meshRenderer;
    [SerializeField] private bool isSelected = false;
    [SerializeField] private bool isOn = false;

    // must be set in editor
    public GameObject[] handles = new GameObject[3];

    // Start is called before the first frame update
    void Awake() {
        InitializedComponents();
        meshRenderer.material = (Material)Instantiate(meshRenderer.material);
        color = meshRenderer.material.color;
        ogPosition = transform.position;
    }

    void InitializedComponents() {
        meshRenderer = GetComponent<MeshRenderer>();
        gameObject.SetActive(isOn);
        meshRenderer.material.color = color;
    }

    private void Update() {
        gameObject.SetActive(isOn);
    }

    public void SetColor(Color c) {
        meshRenderer.material.color = c;
    }

    public void Selected(bool _selected) {
        isSelected = _selected;
        if (isSelected)
        {
            meshRenderer.material.color = selectionColor;
        }
        else
        {
            if (tag.Equals("selectvertex"))
            {
                meshRenderer.material.color = cylinderColor;
            }
            else
            {
                meshRenderer.material.color = unselectColor;
            }
        }

        foreach (var h in handles) {
            h.SetActive(isSelected);
        }
    }

    public void Translate(Vector3 _detla) {

        if (!currentHandleSelected) return;
        Translate(currentHandleSelected.GetComponent<vertexHandle>().direction, _detla);
    }

    public void Translate(Vector3 _detla, vertexHandle.axis dir)
    {
        Translate(dir, _detla);
    }

    // delta is vector2
    private void Translate(vertexHandle.axis _axis, Vector3 _delta) {
        switch (_axis) {
            // xAxis
            case vertexHandle.axis.xAxis:
                transform.position += (_delta.x / 3f * Vector3.right) * - 1;
                return;
            // yAxis
            case vertexHandle.axis.yAxis:
                transform.position -= _delta.y / 3f * Vector3.up;
                return;
            // zAxis 
            case vertexHandle.axis.zAxis:
                transform.position += _delta.x / 3f * Vector3.forward;
                break;
            default:
                break;
        }
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

    public void SetHandle(GameObject handle) {
        currentHandleSelected = handle;
    }

    public GameObject GetSelectedHandle()
    {
        return currentHandleSelected;
    }

    public void SetHandle(string tag) {
        switch (tag)
        {
            case "xHandle":
                currentHandleSelected = handles[0];
                return;
            case "yHandle":
                currentHandleSelected = handles[1];
                return;
            case "zHandle":
                currentHandleSelected = handles[2];
                return;
        }
        //currentHandleSelected = handle;
    }

    public void Destroy() {
        Destroy(gameObject);
    }
    
}
