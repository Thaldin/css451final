using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainController : MonoBehaviour {
    [Header("World Objects")]
    public MP5World theWorld = null;
    public CameraBehavior theCamera = null;

    [Header("Plane")]
    public Dropdown textureSelect = null;
    //public Texture[] selectableTextures = new Texture[0];
    public SliderWithEcho Tessellation = null;
    [Header("Cylinder")]
    public SliderWithEcho CylinderRes = null;
    public SliderWithEcho CylinderRot = null;
    [Header("Xform Controller")]
    public XfromControl TexturePanel = null;
    private Vector3 prevMousePos = Vector3.zero;

    // vertex
    [Header("Vertex Selection")]
    [SerializeField] private GameObject vertex = null;
    [SerializeField] private GameObject handle = null;
    private Vector3 delta = Vector3.zero;
    private Vector3 mouseDownPos = Vector3.zero;
    public planeGeneration planeGen = null;
    private bool triNormalsOn = false;

    // Start is called before the first frame update
    void Start() {
        Debug.Assert(theCamera != null, "Please set Camera Object.");
        Debug.Assert(theWorld != null, "Please set World Object.");
        Debug.Assert(CylinderRes != null, "Please set Cylinder Resolution Slider.");
        Debug.Assert(CylinderRot != null, "Please set Cylinder Rotation Slider.");
        Debug.Assert(planeGen != null, "Please set planeGeration Object.");
        Debug.Assert(textureSelect != null, "Please set texture select dropdown Object.");


        // Initialized Scene
        theWorld.SetLookAtPos(Vector3.zero);
        theCamera.SetLookAt(Vector3.zero);

        // Init UI Controls
        CylinderRes.InitSliderRange(4f, 20f, theWorld.GetCylinderResolution());
        CylinderRes.SetSliderLabel("Cylinder Res");
        CylinderRes.SetSliderListener(UpdateCylinderRes);

        CylinderRot.InitSliderRange(10f, 360f, theWorld.GetCylinderRotation());
        CylinderRot.SetSliderLabel("Cylinder Rot");
        CylinderRot.SetSliderListener(UpdateCylinderRot);

        Tessellation.InitSliderRange(2, 20, 2);
        Tessellation.SetSliderLabel("Tessellation");
        Tessellation.SetSliderListener(UpdateMeshRes);

        // texture select
        InitTextureSelection();
        // Hide UI components
        CylinderRes.gameObject.SetActive(false);
        CylinderRot.gameObject.SetActive(false);

        TexturePanel.mSelected = theWorld.GetCurrentSelection();
    }

    void InitTextureSelection() {
        textureSelect.options.Clear();
        Texture[] tex = theWorld.GetMeshSelectableTextures();
        foreach (var m in tex) {
            textureSelect.options.Add(new Dropdown.OptionData(m.name));
        }
    }

    // Update is called once per frame
    void Update() {
        CheckInput();
    }

    //
    // User Input
    //
    private void CheckInput() {
        // Alt = Camera manipulation
        #region Camera Manipulation
        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) {
            // Mouse wheel zoom
            if (Input.mouseScrollDelta.y != 0) {
                theCamera.SetZoom(Input.mouseScrollDelta.y);
            }

            // Set Previous Position for calculations on MouseDown
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) {
                prevMousePos = Input.mousePosition;
            }

            // Tumble
            if (Input.GetMouseButton(0)) {
                var delta = prevMousePos - Input.mousePosition;
                prevMousePos = Input.mousePosition;
                theCamera.Tumble(delta);
            }

            // Slide
            if (Input.GetMouseButton(1)) {
                var delta = prevMousePos - Input.mousePosition;
                prevMousePos = Input.mousePosition;
                theWorld.SlideLookAtPos(delta.x, delta.y);
                theCamera.Slide(delta);
                theCamera.SetLookAt(theWorld.GetLookAtPos());
            }

            // Return out of this because we want ignore other key presses
            return;
        }
        #endregion

        // Control = Vertex Key Controls
        #region Vertex Control
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
            theWorld.ShowSelectors(true);
        }

        if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl)) {
            theWorld.ShowSelectors(false);
        }


        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
            // vertex selection
            if (Input.GetMouseButtonDown(0)) {
                mouseDownPos = Input.mousePosition;
                delta = Vector3.zero;
                click(0);
            }

            if (Input.GetMouseButtonDown(1)) {
                click(1);
            }

            
            // Move Selected Vertex
            if (handle && Input.GetMouseButton(0)) {
                Debug.Log("Moving on " + handle.tag);
                // get delta
                delta = mouseDownPos - Input.mousePosition;
                mouseDownPos = Input.mousePosition;
                theWorld.MoveVertex(delta, vertex);
            }
            
        }

        #endregion

        // show triangles
        if (Input.GetKey(KeyCode.F1)) {
            theWorld.DrawDebugTriangles();
            triNormalsOn = true;
        }

        if (Input.GetKeyUp(KeyCode.F1)) {
            triNormalsOn = false;
        }

        theWorld.ShowDebugNormals(triNormalsOn);

        // Reset button
        if (Input.GetKey(KeyCode.R)) {
            SceneManager.LoadScene("ScottAndEdMP5");
        }

        // Quit
        if (Input.GetKey(KeyCode.Escape)) {
            Application.Quit();
        }
    }


    // UI elements
    public void RenderSelect(Dropdown ddRender) {
        if (ddRender.value == 0) {
            theWorld.RenderMesh = true;
            TexturePanel.gameObject.SetActive(true);
            textureSelect.gameObject.SetActive(true);
            Tessellation.gameObject.SetActive(true);
            CylinderRes.gameObject.SetActive(false);
            CylinderRot.gameObject.SetActive(false);
        } else {
            theWorld.RenderMesh = false;
            TexturePanel.gameObject.SetActive(false);
            Tessellation.gameObject.SetActive(false);
            textureSelect.gameObject.SetActive(false);
            CylinderRes.gameObject.SetActive(true);
            CylinderRot.gameObject.SetActive(true);
        }
        TexturePanel.mSelected = theWorld.GetCurrentSelection();
    }

    // 
    // Canvas Sliders
    // 
    public void UpdateCylinderRes(float v) {
        theWorld.SetCylinderResolution((int)v);
        theWorld.RenderCylinder();
    }

    public void UpdateCylinderRot(float v) {
        theWorld.SetCylinderRotation((int)v);
        theWorld.UpdateCylinder();
    }

    public void UpdateMeshRes(float v) {
        theWorld.SetPlaneResolution((int)v);
    }

    public void SetMeshMainTexture(float v) {
        int index = (int)v;
        theWorld.SetMeshMainTexture(index);
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
                        if (hit.transform.CompareTag("vertex") || hit.transform.CompareTag("selectvertex")) {
                            // if selection, clear it
                            if (vertex) {
                                vertex.GetComponent<VertexPrefab>().Selected(false);
                            }
                            // reassign selection
                            vertex = hit.transform.gameObject;
                            vertex.GetComponent<VertexPrefab>().Selected(true);
                            return;
                        }
                        // vertex handle
                        if (hit.transform.CompareTag("xHandle") ||
                            hit.transform.CompareTag("yHandle") ||
                            hit.transform.CompareTag("zHandle")) {
                            // MC handle
                            handle = hit.transform.gameObject;
                            // VP handle
                            vertex.GetComponent<VertexPrefab>().SetHandle(handle);
                            return;
                        }
                    }
                }
                else // Clear selection if nothing clicked
                {
                    if ((vertex) && !EventSystem.current.IsPointerOverGameObject())
                    {
                        Unselect();
                    }
                }
                return;
            // Right mouse
            case 1:
                if (vertex)
                { 
                    Unselect();
                }
                return;
            default:
                return;
        }
    }

    // unselect vertex prefab
    void Unselect() {
        vertex.GetComponent<VertexPrefab>().Unselect();
        vertex.GetComponent<VertexPrefab>().Selected(false);
        vertex = null;
        handle = null;
    }
}
