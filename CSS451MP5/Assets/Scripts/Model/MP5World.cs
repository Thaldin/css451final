using System.Collections.Generic;
using UnityEngine;

public class MP5World : MonoBehaviour {
    public GameObject LookAt = null;
    public bool RenderMesh = true;

    [Header("Cylinder")]
    public int CylinderResolution = 10;
    public int MeshResolution = 4;
    public int CylinderRotation = 180;
    public GameObject VertexPrefab;
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
    private Vector3[] normals = null;
    
    private bool renderVertexSelectors = false;

    // testing
    [Header("Plane")]
    public Mesh planeMesh = null;
    public planeGeneration planeGen = null;
    private GameObject planeObject = null;
    private MeshFilter planeMeshFilter = null;
    private MeshRenderer planeMeshRender = null;

    [SerializeField] private Transform currentSelection = null;

    private bool vertexPrefabIsOn = false;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(LookAt != null, "Please set LookAt object.");
        cylinderMesh = new Mesh();
        planeMesh = new Mesh();

        sphereList = new List<GameObject>();

        // cylinder
        renderObject = new GameObject();
        renderObject.name = "Cylinder";
        cylinderFilter = renderObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        cylinderRenderer = renderObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        cylinderRenderer.material = testMaterial;

        // plane
        planeObject = new GameObject();
        planeObject.name = "Plane";
        planeMeshFilter = planeObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        planeMeshRender = planeObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        planeMeshRender.material = planeGen.matieral;

        //create start primitives
        //CalculateCylinder();
        RenderPlane();
        RenderCylinder();

        SetActiveMesh(0);
    }

    /*
    // Update is called once per frame
    void Update()
    {
        
        if (RenderMesh)
        {
            //RenderTextureMesh();
            RenderPlane();
        }
        else
        {
            RenderCylinderMesh();
        }
    }
    */

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
                Debug.Log("Plane vertexPrefabs " + enable);
                planeGen.ToggleVertexPrefabs(enable);
                return;
            // cylinder
            case "Cylinder":
                Debug.Log("Cylinder vertexPrefabs " + enable);
                ToggleVertexPrefabs(enable);
                return;
            default:
                return;
        }
    }

    private void ToggleVertexPrefabs(bool isOn) {
        foreach (var prefab in sphereList) {
            prefab.GetComponent<VertexPrefab>().ToggleVertexPrefab(isOn);
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

    public void RenderCylinder() {
        renderObject.SetActive(true);
        cylinderFilter.mesh.Clear();
        cylinderFilter.mesh = CalculateCylinder();
    }

    public void HideCylinder() {
        renderObject.SetActive(false);
        ClearVertexPrefabList();
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

    

    //public void RenderCylinderMesh()
    public Mesh RenderCylinderMesh()
    {
        
        //RenderSpheres(vertexArray, vertexCount);

        // Clear existing mesh
        cylinderMesh.Clear();
        cylinderMesh.vertices = vertexArray;
        cylinderMesh.triangles = triangleArray;
        cylinderMesh.normals = normals;
        cylinderFilter.mesh = cylinderMesh;
        cylinderRenderer.material = testMaterial;
        UpdateNormals(vertexArray, normals);

        return cylinderMesh;
    }

    private void RenderSpheres(Vector3[] array, int count)
    {
        // Clear previous spheres
        /*
        foreach (GameObject obj in sphereList)
        {
            Destroy(obj);
        }
        */

        ClearVertexPrefabList();

        sphereList.Clear();

        // Render Test Spheres
        var scale = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 pos;
        for (int i = 0; i < count; i++)
        {
            //var s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            var s = Instantiate(VertexPrefab);
            s.transform.localScale = scale;
            pos.x = array[i].x;
            pos.y = array[i].y;
            pos.z = array[i].z;
            s.transform.localPosition = pos;
            //s.SetActive(renderVertexSelectors);
            sphereList.Add(s);
        }
    }

    private void ClearVertexPrefabList() {
        foreach (var v in sphereList) {
            v.GetComponent<VertexPrefab>().Destroy();
        }
        sphereList.Clear();
    }



    //public void CalculateCylinder()
    public Mesh CalculateCylinder() {
        // Compute Triangle and Vertex count
        vertexCount = CylinderResolution * nCylinderHeight;
        triangleCount = ((CylinderResolution - 1) * (nCylinderHeight - 1)) * 2;
        vertexArray = new Vector3[vertexCount];
        triangleArray = new int[triangleCount * 3];
        normals = new Vector3[vertexCount];

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

        RenderSpheres(vertexArray, vertexCount);
        CalculateNormals(vertexArray, CylinderResolution);
        //RenderCylinderMesh();
        return RenderCylinderMesh();
    }

    private Vector3 FaceNormals(Vector3[] v, int v1, int v2, int v3)
    {
        Vector3 a = v[v2] - v[v1];
        Vector3 b = v[v3] - v[v1];
        return Vector3.Cross(a, b).normalized;
    }

    private void CalculateNormals(Vector3[] vertices, int resolution)
    {
        Vector3[] triNormals = new Vector3[triangleCount];
        int triPerRow = (resolution - 1) * 2;

        //Debug.Log("Triangle Count: " + triangleCount);

        // Calculate the triangle normals
        for (int i = 0; i < triangleCount; i++)
        {
            int oddTri = ((i % 2) == 0) ? 0 : 1;                    // Odd or even triangle
            int startVertex = (i / 2) + (i / triPerRow) + oddTri;   // Starting vertex for triangle

            if (oddTri == 0)
            {
                // Even numbered triangles
                triNormals[i] = FaceNormals(vertexArray, startVertex, startVertex + 1, startVertex + resolution);
            }
            else
            {
                // Odd numbered triangles
                triNormals[i] = FaceNormals(vertexArray, startVertex, startVertex + resolution, startVertex + resolution - 1);
            }
            //Debug.Log("Tri: " + i + ", v1: " + startVertex + ", Odd: " + oddTri + ", Row: " + (int)(i / triPerRow));
        }

        // Calculate the vertex normals
        for (int i = 0; i < vertexArray.Length; i++)
        {
            //Debug.Log("Vertex/Normal: " + i);
            // Used for determining left/right columns
            int mod = i % resolution;

            if (i < resolution)
            {
                // Top row, special case
                if (mod == 0)
                {
                    // Upper left corner
                    normals[i] = triNormals[0].normalized;
                }
                else if (mod == (resolution - 1))
                {
                    // Upper right corner
                    normals[i] = (triNormals[triPerRow - 1] + triNormals[triPerRow - 2]).normalized;
                }
                else
                {
                    // Middle top row
                    int triId = GetVertexBottomTriangle(i, triPerRow, resolution);
                    normals[i] = (triNormals[triId - 2] + triNormals[triId - 1] + triNormals[triId]).normalized;
                }
            }
            else if (i >= vertexArray.Length - resolution)
            {
                // Bottom row, special case
                if (mod == 0)
                {
                    // Bottom left corner
                    normals[i] = (triNormals[triangleCount - resolution] + triNormals[triangleCount - resolution + 1]).normalized;
                }
                else if (mod == (resolution - 1))
                {
                    // Bottom right corner
                    normals[i] = triNormals[triangleCount - 1].normalized;
                }
                else
                {
                    // Middle bottom row
                    int triId = GetVertexTopTriangle(i, triPerRow, resolution);
                    normals[i] = (triNormals[triId - 1] + triNormals[triId] + triNormals[triId + 1]).normalized;
                }
            }
            else
            {
                // Rest of vertices
                if (mod == 0)
                {
                    // Left column of vertices
                    int triId = ((i / resolution) - 1) * triPerRow;
                    normals[i] = (triNormals[triId] + triNormals[triId + 1] + triNormals[triId + triPerRow]).normalized;
                }
                else if (mod == (resolution - 1))
                {
                    // Right column of vertices
                    int triId = ((i / resolution) * triPerRow) - 1;
                    //Debug.Log("TriId: " + triId);
                    normals[i] = (triNormals[triId] + triNormals[triId + triPerRow - 1] + triNormals[triId + triPerRow]).normalized;
                }
                else
                {
                    // Center of mesh
                    int topTriId = GetVertexTopTriangle(i, triPerRow, resolution);
                    int bottomTriId = GetVertexBottomTriangle(i, triPerRow, resolution);
                    //Debug.Log("Top Tri: " + topTriId + ", Bottom Tri: " + bottomTriId);
                    normals[i] = (triNormals[topTriId - 1] + triNormals[topTriId] + triNormals[topTriId + 1] + triNormals[bottomTriId - 2] + triNormals[bottomTriId - 1] + triNormals[bottomTriId]).normalized;
                }
            }
        }

        UpdateNormals(vertexArray, normals);
    }

    private int GetVertexTopTriangle(int vertexNumber, int trianglesPer, int res)
    {
        int row = vertexNumber / res;
        int col = vertexNumber % res;

        // Returns the middle triangle of 3 (need to get +1 and -1 to either side of this one)
        return (row - 1) * trianglesPer + (col * 2);
    }

    private int GetVertexBottomTriangle(int vertexNumber, int trianglesPer, int res)
    {
        int row = vertexNumber / res;
        int col = vertexNumber % res;

        // Returns right most bottom triangle, need to get -1 and -2 also)
        return (row * trianglesPer) + (col * 2);
    }

    private void UpdateNormals(Vector3[] v, Vector3[] n)
    {
        int count = 0;
        foreach (var sphere in sphereList)
        {
            Quaternion q = Quaternion.FromToRotation(Vector3.up, n[count]);
            sphere.transform.localRotation = q;
            count++;
        }
    }

    // tessellation
    public void SetPlaneResolution(int newSize) {
        planeGen.SetResolution(newSize);
    }

    // set scales
    public void SetTileScaleX(float v) {
        planeGen.SetTileScaleX(v);
    }
    public void SetTileScaleY(float v) {
        planeGen.SetTileScaleY(v);
    }

    // set offset
    public void SetTileOffsetX(float v) {
        planeGen.SetTileOffsetX(v);
    }
    public void SetTileOffsetY(float v) {
        planeGen.SetTileOffsetY(v);
    }

    public Transform GetCurrentSelection() {
        return currentSelection;
    }
}
