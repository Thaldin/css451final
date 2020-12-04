using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class meshGeneration : MonoBehaviour {
    

    // vertex prefab
    [SerializeField] protected List<GameObject> vertexPrefabs = new List<GameObject>();
    [SerializeField] protected GameObject vertex = null;
    [SerializeField] protected GameObject triangleNormal = null;


    // components
    public Material matieral = null;
    protected Mesh mesh;

    // mesh components
    [SerializeField] protected Vector3[] vertices = new Vector3[0];
    [SerializeField] protected Vector2[] uv = new Vector2[0];
    [SerializeField] protected int[] triangles = new int[0];
    [SerializeField] protected Vector3[] normals = new Vector3[0];

    // Start is called before the first frame update
    private void Start() {
        Debug.Assert(vertex != null);
        Debug.Assert(matieral != null);
        InitializeComponents();
    }

    private void InitializeComponents() {
        mesh = new Mesh();
    }

    public Mesh UpdateMesh() {
        //mesh.Clear();
        mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;
        return mesh;
    }

    public void UpdateVertices() {
        for (int i = 0; i < vertices.Length; i++) {
            vertices[i] = vertexPrefabs[i].transform.position;
        }
    }

    public virtual void GenerateTrianges() { }

    public virtual void CalculateNormals() { }

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


}
