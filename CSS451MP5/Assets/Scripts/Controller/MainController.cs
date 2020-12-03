using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainController : MonoBehaviour {
    [Header("World Objects")]
    public MP5World theWorld = null;
    public CameraBehavior theCamera = null;

    [Header("Plane")]
    public SliderWithEcho TextureRes = null;
    public SliderWithEcho Tessellation = null;
    [Header("Cylinder")]
    public SliderWithEcho CylinderRes = null;
    public SliderWithEcho CylinderRot = null;
    [Header("Xform Controller")]
    public XfromControl TexturePanel = null;

    private Vector3 prevMousePos = Vector3.zero;

    // Start is called before the first frame update
    void Start() {
        Debug.Assert(theCamera != null, "Please set Camera Object.");
        Debug.Assert(theWorld != null, "Please set World Object.");
        Debug.Assert(CylinderRes != null, "Please set Cylinder Resolution Slider.");
        Debug.Assert(CylinderRot != null, "Please set Cylinder Rotation Slider.");
        Debug.Assert(TextureRes != null, "Please set Texture Resolution Slider.");

        // Initialized Scene
        theWorld.SetLookAtPos(Vector3.zero);
        theCamera.SetLookAt(Vector3.zero);

        // Init UI Controls
        CylinderRes.InitSliderRange(4f, 20f, theWorld.CylinderResolution);
        CylinderRes.SetSliderLabel("Cylinder Res");
        CylinderRes.SetSliderListener(UpdateCylinderRes);

        CylinderRot.InitSliderRange(10f, 360f, theWorld.CylinderRotation);
        CylinderRot.SetSliderLabel("Cylinder Rot");
        CylinderRot.SetSliderListener(UpdateCylinderRot);

        TextureRes.InitSliderRange(1, 20, 1);
        TextureRes.SetSliderLabel("Texture Res");
        TextureRes.SetSliderListener(UpdateTextureRes);

        Tessellation.InitSliderRange(2, 20, 2);
        Tessellation.SetSliderLabel("Tessellation");
        Tessellation.SetSliderListener(UpdateMeshRes);

        // Hide UI components
        CylinderRes.gameObject.SetActive(false);
        CylinderRot.gameObject.SetActive(false);

        TexturePanel.mSelected = theWorld.GetCurrentSelection();
    }

    // Update is called once per frame
    void Update() {
        CheckInput();
    }

    private void CheckInput() {
        // Alt = Camera manipulation
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

        // Control = Vertex Key Controls
        /*
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)) {
            theWorld.ShowSelectors(true);
        }
        */
        // Control = Vertex Key Controls
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
            theWorld.ShowSelectors(true);
        }

        if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl)) {
            theWorld.ShowSelectors(false);
        }


        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
            // Set Previous Position for calculations on MouseDown
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) {
                prevMousePos = Input.mousePosition;
            }

            // Move Selected Vertex
            if (Input.GetMouseButton(0)) {
                var delta = prevMousePos - Input.mousePosition;
                prevMousePos = Input.mousePosition;
                // TODO: Move the selected vertex
            }
        }

        // Reset button
        if (Input.GetKey(KeyCode.R)) {
            SceneManager.LoadScene("ScottAndEdMP5");
        }

        // Quit
        if (Input.GetKey(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    public void RenderSelect(Dropdown ddRender) {
        if (ddRender.value == 0) {
            theWorld.RenderMesh = true;
            TextureRes.gameObject.SetActive(true);
            TexturePanel.gameObject.SetActive(true);
            Tessellation.gameObject.SetActive(true);
            CylinderRes.gameObject.SetActive(false);
            CylinderRot.gameObject.SetActive(false);
        } else {
            theWorld.RenderMesh = false;
            TextureRes.gameObject.SetActive(false);
            TexturePanel.gameObject.SetActive(false);
            Tessellation.gameObject.SetActive(false);
            CylinderRes.gameObject.SetActive(true);
            CylinderRot.gameObject.SetActive(true);
            //theWorld.CalculateCylinder();
        }
        TexturePanel.mSelected = theWorld.GetCurrentSelection();
        //TexturePanel.SetSelectedObject(theWorld.GetCurrentSelection());
    }

    public void UpdateCylinderRes(float v) {
        theWorld.CylinderResolution = (int)v;
        theWorld.CalculateCylinder();
    }

    public void UpdateCylinderRot(float v) {
        theWorld.CylinderRotation = (int)v;
        theWorld.CalculateCylinder();
    }

    public void UpdateMeshRes(float v) {
        theWorld.SetPlaneResolution((int)v);
        theWorld.RenderPlane();
    }

    public void UpdateTextureRes(float v) {
        //theWorld.SetPlaneTiling((int)v);
        theWorld.RenderPlane();
    }
}
