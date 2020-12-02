using System.Collections.Generic;
using UnityEngine;

public class MP5World : MonoBehaviour
{
    public GameObject LookAt = null;
    public bool RenderMesh = true;

    public int CylinderResolution = 10;
    public int MeshResolution = 4;
    public int CylinderRotation = 180;

    public Material testMaterial;

    private Mesh cylinderMesh = null;
    private MeshFilter cylinderFilter = null;
    private MeshRenderer cylinderRenderer = null;

    private const int nCylinderHeight = 10;
    private List<GameObject> sphereList;
    private GameObject renderObject;

    
    private int vertexCount = 0;
    private int triangleCount = 0;
    private Vector3[] vertexArray = null;
    private int[] triangleArray = null;
    private Vector3[] triangleNormals = null;
    
    private bool renderVertexSelectors = false;

    // testing
    [Header("Plane")]
    public Mesh planeMesh = null;
    public planeGeneration planeGen = null;
    private GameObject planeObject = null;
    private MeshFilter planeMeshFilter = null;
    private MeshRenderer planeMeshRender = null;

    [SerializeField] private Transform currentSelection = null;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(LookAt != null, "Please set LookAt object.");
        cylinderMesh = new Mesh();
        planeMesh = new Mesh();

        sphereList = new List<GameObject>();
        renderObject = new GameObject();
        renderObject.name = "Cylinder";

        cylinderFilter = renderObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        cylinderRenderer = renderObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;

        // plane
        planeObject = new GameObject();
        planeObject.name = "Plane";
        planeMeshFilter = planeObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        planeMeshRender = planeObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        planeMeshRender.material = planeGen.matieral;

        //create start primitives
        CalculateCylinder();
        RenderPlane();

        SetActiveMesh(0);
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (RenderMesh)
        {
            RenderTextureMesh();
        }
        else
        {
            RenderCylinderMesh();
        }
        */
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
                RenderCylinderMesh();
                return;
            default:
                return;
        }
    }

    public void SetLookAtPos(Vector3 pos)
    {
        LookAt.transform.localPosition = pos;
    }

    public Vector3 GetLookAtPos()
    {
        return LookAt.transform.localPosition;
    }

    public void SlideLookAtPos(float deltaX, float deltaY)
    {
        LookAt.transform.position += deltaX * LookAt.transform.right;
        LookAt.transform.position += deltaY * LookAt.transform.up;
    }

    public void RenderPlane()
    {
        planeObject.SetActive(true);
        planeMeshFilter.mesh.Clear();
        planeMeshFilter.mesh = planeGen.CreatePlane();
    }

    public void HidePlane() {
        planeObject.SetActive(false);
        planeGen.ClearVertexPrefabList();
    }

    public void HideCylinder() {
        renderObject.SetActive(false);
    }

    public void RenderCylinderMesh()
    {
        renderObject.SetActive(true);
        // TODO: Make this the selectors?
        RenderSpheres(vertexArray, vertexCount);

        // Clear existing mesh
        cylinderMesh.Clear();
        cylinderMesh.vertices = vertexArray;
        cylinderMesh.triangles = triangleArray;
        cylinderMesh.normals = triangleNormals;
        //temp
        cylinderMesh.RecalculateNormals();
        cylinderFilter.mesh = cylinderMesh;
        cylinderRenderer.material = testMaterial;

        // TODO: Fix shader
    }

    private void RenderSpheres(Vector3[] array, int count)
    {
        // Clear previous spheres
        foreach (GameObject obj in sphereList)
        {
            Destroy(obj);
        }

        sphereList.Clear();

        // Render Test Spheres
        var scale = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 pos;
        for (int i = 0; i < count; i++)
        {
            var s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            s.transform.localScale = scale;
            pos.x = array[i].x;
            pos.y = array[i].y;
            pos.z = array[i].z;
            s.transform.localPosition = pos;
            s.SetActive(renderVertexSelectors);
            sphereList.Add(s);
        }
    }

    public void ShowSelectors(bool enable)
    {
        renderVertexSelectors = enable;
    }

    public void CalculateCylinder()
    {
        // Compute Triangle and Vertex count
        vertexCount = CylinderResolution * nCylinderHeight;
        triangleCount = ((CylinderResolution - 1) * (nCylinderHeight - 1)) * 2;
        vertexArray = new Vector3[vertexCount];
        triangleArray = new int[triangleCount * 3];
        triangleNormals = new Vector3[vertexCount];

        // Store vertex count to load into array
        int arrayCounter = vertexCount;

        // Build Vertex Array
        float fTheta = (CylinderRotation / (CylinderResolution - 1)) * Mathf.Deg2Rad;
        for (int h = 0; h < nCylinderHeight; h++)
        {
            float yValue = (h * 3) - 10;

            for (int i = 0; i < CylinderResolution; i++)
            {
                vertexArray[--arrayCounter] = new Vector3(10f * Mathf.Cos(i * fTheta),
                                                          yValue,
                                                          10f * Mathf.Sin(i * fTheta));
            }
        }

        // Compute Triangles
        var triCount = 0;
        for (int i = 0; i < (vertexCount - CylinderResolution); i++)
        {
            int mod = i % CylinderResolution;
            if (mod == 0)       // Left side of mesh
            {
                triangleArray[triCount++] = i;
                triangleArray[triCount++] = i + 1;
                triangleArray[triCount++] = i + CylinderResolution;
            }
            else if (mod == (CylinderResolution - 1))  // Right side of mesh
            {
                triangleArray[triCount++] = i;
                triangleArray[triCount++] = i + CylinderResolution;
                triangleArray[triCount++] = i + CylinderResolution - 1;
            }
            else                // Middle of mesh
            {
                triangleArray[triCount++] = i;
                triangleArray[triCount++] = i + 1;
                triangleArray[triCount++] = i + CylinderResolution;
                triangleArray[triCount++] = i;
                triangleArray[triCount++] = i + CylinderResolution;
                triangleArray[triCount++] = i + CylinderResolution - 1;
            }
        }

        CalculateNormals();

        RenderSpheres(vertexArray, vertexCount);
    }

    private void CalculateNormals()
    {
        // Take vertex A, B and C from a triangle. Then create vectors AB and AC. 
        // ABxAC will give you the normal of the triangle. 
        // ACxAB will give the opposite normal (ABxAC = - ACxAB).
        int t = 0;
        for (int i = 0; i < triangleArray.Length; i += 3)
        {
            //Vector3 AB = vertices[triangles[i]] - vertices[triangles[i + 1]];
            //Vector3 AC = vertices[triangles[i]] - vertices[triangles[i + 2]];

            //Vector3 triNormal = Vector3.Cross(AC, AB);
            //triangleNormals[t] = triNormal.normalized;
            t++;
        }
        // (normal + normal).normalized
    }

    // tessellation
    public void SetPlaneTessellation(int newSize) {
        planeGen.SetSize(newSize);
    }

    // tile multiplier
    public void SetPlaneTiling(int newTile) {
        planeGen.SetTiling(newTile);
    }

    // tiling
    public void SetPlaneTilingX(float newTile) {
        planeGen.SetTilingX((int)newTile);
    }
    public void SetPlaneTilingZ(float newTile) {
        planeGen.SetTilingZ((int)newTile);
    }

    // set scales
    public void SetTileScaleX(float v) {
        planeGen.SetTileScaleX((int)v);
    }
    public void SetTileScaleZ(float v) {
        planeGen.SetTileScaleZ((int)v);
    }

    // set offset
    public void SetTileOffsetX(float v) {
        planeGen.SetTileOffsetX((int)v);
    }
    public void SetTileOffsetZ(float v) {
        planeGen.SetTileOffsetZ((int)v);
    }

    public Transform GetCurrentSelection() {
        return currentSelection;
    }
}
