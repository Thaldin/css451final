using System.Collections.Generic;
using UnityEngine;

public class MP5World : MonoBehaviour {
   
    public GameObject LookAt = null;

    [HideInInspector]
    public bool RenderMesh = true;

    [Header("Cylinder")]
    public cylinderGeneration cylGen;
    private MeshFilter cylinderFilter = null;
    private MeshRenderer cylinderRenderer = null;

    private GameObject renderObject;

    // testing
    [Header("Plane")]
    public Mesh planeMesh = null;
    public planeGeneration planeGen = null;
    private GameObject planeObject = null;
    private MeshFilter planeMeshFilter = null;
    private MeshRenderer planeMeshRender = null;

    [SerializeField] private Transform currentSelection = null;

    // Start is called before the first frame update
    void Start() {
        Debug.Assert(LookAt != null, "Please set LookAt object.");

        // cylinder
        renderObject = new GameObject();
        renderObject.name = "Cylinder";
        cylinderFilter = renderObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        cylinderRenderer = renderObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        cylinderRenderer.material = cylGen.matieral;

        // plane
        planeObject = new GameObject();
        planeObject.name = "Plane";
        planeMeshFilter = planeObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        planeMeshRender = planeObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        planeMeshRender.material = planeGen.matieral;

        //create start primitives
        RenderPlane();
        RenderCylinder();

        SetActiveMesh(0);
    }

    public void SetActiveMesh(int value) {
        planeMeshFilter.mesh.Clear();
        switch (value) {
            case 0:
                currentSelection = planeObject.transform;
                HideCylinder();
                RenderPlane();
                return;
            case 1:
                currentSelection = renderObject.transform;
                HidePlane();
                RenderCylinder();
                return;
            default:
                return;
        }
    }

    public void ShowSelectors(bool enable) {
        // check mesh
        string str = currentSelection.name;
        switch (str) {
            // plane
            case "Plane":
                planeGen.ToggleVertexPrefabs(enable);
                return;
            // cylinder
            case "Cylinder":
                cylGen.ToggleVertexPrefabs(enable);
                return;
            default:
                return;
        }
    }

    public void RenderPlane() {
        planeObject.SetActive(true);
        planeMeshFilter.mesh.Clear();
        planeMeshFilter.mesh = planeGen.CreatePlane();
    }

    public void HidePlane() {
        planeObject.SetActive(false);
        planeGen.ClearVertexPrefabList();
    }

    public void UpdatePlane() {
        planeMeshFilter.mesh.Clear();
        planeMeshFilter.mesh = planeGen.UpdateMesh();
    }

    public void RenderCylinder() {
        renderObject.SetActive(true);
        cylinderFilter.mesh.Clear();
        cylinderFilter.mesh = cylGen.CreateCylinder();
    }

    public void UpdateCylinder()
    {
        cylinderFilter.mesh.Clear();
        cylinderFilter.mesh = cylGen.UpdateMesh();
    }

    public void HideCylinder() {
        renderObject.SetActive(false);
        cylGen.ClearVertexPrefabList();
    }

    public void SetLookAtPos(Vector3 pos) {
        LookAt.transform.localPosition = pos;
    }

    public Vector3 GetLookAtPos() {
        return LookAt.transform.localPosition;
    }

    public GameObject GetVert(int i) {
        return cylGen.GetVert(i);
    }

    public void SlideLookAtPos(float deltaX, float deltaY) {
        LookAt.transform.position += deltaX * LookAt.transform.right;
        LookAt.transform.position += deltaY * LookAt.transform.up;
    }
    #region Cylinder Sliders
    public void SetCylinderResolution(int res)
    {
        cylGen.CylinderResolution = res;
    }

    public void SetCylinderRotation(int rot)
    {
        cylGen.CylinderRotation = rot;
    }

    public int GetCylinderResolution()
    {
        return cylGen.CylinderResolution;
    }

    public int  GetCylinderRotation()
    {
        return cylGen.CylinderRotation;
    }

    #endregion

    #region Plane Functions
    // tessellation
    public void SetPlaneResolution(int newSize) {
        planeGen.SetResolution(newSize);
        RenderPlane();
    }

    // set scales
    public void SetTileScaleX(float v) {
        planeGen.SetUVTileX(v);
    }
    public void SetTileScaleY(float v) {
        planeGen.SetUVTileY(v);
    }

    public Vector3 GetUVTile() {
        return planeGen.GetUVTile();
    }

    // set offset
    public void SetTileOffsetX(float v) {
        planeGen.SetUVOffsetX(v);
    }
    public void SetTileOffsetY(float v) {
        planeGen.SetUVOffsetY(v);
    }

    public Vector3 GetUVOffset() {
        return planeGen.GetUVOffset();
    }

    public void SetUVRotation(float v) {
        planeGen.SetUVRotation(v);
    }

    public float GetUVRotation() {
        return planeGen.GetUVRotation();
    }

    #endregion
    public Transform GetCurrentSelection() {
        return currentSelection;
    }

    public void MoveVertex(Vector3 mouseDelta, GameObject vertex)
    {
        int row = vertex.GetComponent<VertexPrefab>().row;
        Debug.Log("Row" + row);
        

        Debug.Log("Moving Vertex: " + vertex.name);

        switch (currentSelection.name) {
            case "Plane":
                // update vertices
                vertex.GetComponent<VertexPrefab>().Translate(mouseDelta);
                planeGen.GetComponent<planeGeneration>().UpdateVertices();
                // redraw mesh
                UpdatePlane();
                return;
            case "Cylinder":
                int id = vertex.GetComponent<VertexPrefab>().Id;
                vertexHandle.axis dir = vertex.GetComponent<VertexPrefab>().GetSelectedHandle().GetComponent<vertexHandle>().direction;
                cylGen.GetComponent<cylinderGeneration>().UpdateVertexRow(id, dir, mouseDelta);
                /*
                GameObject h = vertex.GetComponent<VertexPrefab>().GetHandle();
                for (int i = row; i < cylGen.GetComponent<cylinderGeneration>().CylinderResolution; i++) {
                    GameObject v = cylGen.GetComponent<cylinderGeneration>().GetVert(i);
                    v.GetComponent<VertexPrefab>().SetHandle(h.tag);
                    v.GetComponent<VertexPrefab>().Translate(mouseDelta);
                }
                */
                cylGen.GetComponent<cylinderGeneration>().UpdateVertices();
                UpdateCylinder();
                return;
        }
        
    }

    public Texture[] GetMeshSelectableTextures() {
        // check mesh
        string str = currentSelection.name;
        switch (str) {
            // plane
            case "Plane":
                return planeGen.GetSelectableTextures();
            // cylinder
            case "Cylinder":
                Debug.Log("Not using textures yet");
                return null;
            default:
                return null;
        }
    }

    public void SetMeshMainTexture(int index) {
        // check mesh
        string str = currentSelection.name;
        switch (str) {
            // plane
            case "Plane":
                planeGen.SetMainTexture(index);
                return;
            // cylinder
            case "Cylinder":
                Debug.Log("Not using textures yet");
                return;
            default:
                return;
        }
    }

    public void ShowDebugNormals(bool b) {
        switch (currentSelection.name) {
            case "Plane":
                planeGen.ToggleTriNormalPreFabs(b);
                return;
            case "Cylinder":
                cylGen.ToggleTriNormalPreFabs(b);
                return;
        }
    }

    public void DrawDebugTriangles() {
        switch (currentSelection.name) {
            case "Plane":
                planeGen.DrawTriangles();
                return;
            case "Cylinder":
                cylGen.DrawTriangles();
                return;
        }
    }
}
 