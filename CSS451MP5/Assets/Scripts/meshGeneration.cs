using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class meshGeneration : MonoBehaviour {

    // vertex prefab
    protected List<GameObject> vertexPrefabs = new List<GameObject>();
    // user defined in editor
    [SerializeField] protected GameObject vertex = null;
    [SerializeField] protected GameObject triangleNormal = null;


    // components
    public Material matieral = null;
    protected Mesh mesh;
    public int planeSize = 10;
    protected Vector2 meshResolution = new Vector2(2f, 2f);

    // mesh components
    protected Vector3[] vertices = new Vector3[0];
    protected Vector2[] uv = new Vector2[0];
    protected int[] triangles = new int[0];
    [SerializeField] protected Vector3[] normals = new Vector3[0];
    protected Vector3[] triNormals = new Vector3[0];

    // mesh normals
    protected Vector3[] triNormalPos = new Vector3[0];
    protected List<GameObject> triNormalPrefabs = new List<GameObject>();
    protected bool triNormalsOn = false;

    // Start is called before the first frame update
    private void Start() {
        Debug.Assert(vertex != null, "Please set vertex prefab in editor.");
        Debug.Assert(triangleNormal != null, "Please set triangle normal prefab in editor.");
        Debug.Assert(matieral != null);
        InitializeComponents();
    }

    void Update() {
        
        if (Input.GetKey(KeyCode.F1)) { 
            DrawTriangles();
            triNormalsOn = true;
        }

        if (Input.GetKeyUp(KeyCode.F1)) {
            triNormalsOn = false;
        }

        ToggleTriNormalPreFabs(triNormalsOn);
    }

    private void InitializeComponents() {
        mesh = new Mesh();
    }

    void DrawTriangles() {
        for (int i = 0; i < triangles.Length; i+=3) {
            Vector3 V0 = vertices[triangles[i]];
            Vector3 V1 = vertices[triangles[i + 1]];
            Vector3 V2 = vertices[triangles[i + 2]];
            Debug.DrawLine(V0, V1, Color.blue);
            Debug.DrawLine(V0, V2, Color.blue);
            Debug.DrawLine(V1, V2, Color.blue);
        }
    }

    public abstract Mesh UpdateMesh();

    public virtual void UpdateVertices() {}

    protected virtual void GenerateTrianges() { }

    protected virtual void CalculateNormals() {}

    protected virtual void CalculateNormals(Vector3[] v, int resolution) { }


    // 
    //  Display mesh normals on triangles
    //
    #region Mesh Normals
    protected Vector3 TriNormalPos(int index) {
        Vector3 V0 = vertices[triangles[index]];
        Vector3 V1 = vertices[triangles[index + 1]];
        Vector3 V2 = vertices[triangles[index + 2]];
        return (V0 + V1 + V2) / 3;
    }

    protected void GenerateTriNormalsPrefab() {
        int i = 0;
        ClearTriNormalsPrefab();
        foreach (var n in triNormalPos) {
            // debug --show tri normal--
            GameObject triNormalClone = Instantiate(triangleNormal);
            triNormalClone.transform.position = n;
            triNormalClone.transform.rotation = Quaternion.FromToRotation(Vector3.up, triNormals[i]);
            triNormalPrefabs.Add(triNormalClone);
            i++;
        }
    }

    protected void ClearTriNormalsPrefab() {
        foreach (var c in triNormalPrefabs) {
            Destroy(c);
        }
        triNormalPrefabs.Clear();
    }

    protected void ToggleTriNormalPreFabs(bool isOn) {
        foreach (var p in triNormalPrefabs) {
            p.SetActive(isOn);
        }
    }
    #endregion

    // 
    //  Vertex display prefab generation
    //
    #region
    public void GenerateVertexPrefab() {
        ClearVertexPrefabList();
        for (int i = 0; i < vertices.Length; i++) {
            GameObject vertexSpawn = Instantiate(vertex);
            vertexSpawn.name = "Vertex" + i;
            vertexPrefabs.Add(vertexSpawn);
            vertexSpawn.transform.position = vertices[i];
        }
    }

    public void ClearVertexPrefabList() {
        foreach (var v in vertexPrefabs) {
            v.GetComponent<VertexPrefab>().Destroy();
        }
        vertexPrefabs.Clear();
    }

    public void ToggleVertexPrefabs(bool isOn) {
        //Debug.Log(isOn + " planeGen Level");
        foreach (var v in vertexPrefabs) {
            v.GetComponent<VertexPrefab>().ToggleVertexPrefab(isOn);
        }
    }
    #endregion

    #region Merged code

    protected void UpdateNormals(Vector3[] v, Vector3[] n)
    {
        int count = 0;
        foreach (var prefab in vertexPrefabs)
        {
            Debug.Log("ncount: " + count);
            Quaternion q = Quaternion.FromToRotation(Vector3.up, n[count]);
            prefab.transform.localRotation = q;
            count++;
        }
    }

    protected Vector3 FaceNormals(Vector3[] v, int v1, int v2, int v3)
    {
        Vector3 a = v[v2] - v[v1];
        Vector3 b = v[v3] - v[v1];
        return Vector3.Cross(a, b).normalized;
    }

    protected int GetVertexTopTriangle(int vertexNumber, int trianglesPer, int res)
    {
        int row = vertexNumber / res;
        int col = vertexNumber % res;

        // Returns the middle triangle of 3 (need to get +1 and -1 to either side of this one)
        return (row - 1) * trianglesPer + (col * 2);
    }

    protected int GetVertexBottomTriangle(int vertexNumber, int trianglesPer, int res)
    {
        int row = vertexNumber / res;
        int col = vertexNumber % res;

        // Returns right most bottom triangle, need to get -1 and -2 also)
        return (row * trianglesPer) + (col * 2);
    }
    #endregion
}
