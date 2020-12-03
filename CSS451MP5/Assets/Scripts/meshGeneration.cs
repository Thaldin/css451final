using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class meshGeneration : MonoBehaviour {
    

    // vertex prefab
    [SerializeField] protected List<GameObject> vertexPrefabs = new List<GameObject>();
    [SerializeField] protected GameObject vertex = null;

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

    public virtual Mesh UpdateMesh() { return null; }
    public virtual void UpdateVertices() { }

    public virtual void GenerateTrianges() { }

    public virtual void CalculateNormals() { }

    protected void GenerateVertexPrefab() {
        ClearVertexPrefabList();
        for (int i = 0; i < vertices.Length; i++) {
            GameObject vertexSpawn = Instantiate(vertex);
            vertexSpawn.name = "Vertex" + i;
            vertexPrefabs.Add(vertexSpawn);
            vertexSpawn.transform.position = vertices[i];
            //vertexSpawn.transform.rotation = Quaternion.FromToRotation(Vector3.up, Vector3.forward);
            // recalculate normal

        }
    }

    public void ClearVertexPrefabList() {
        foreach (var v in vertexPrefabs) {
            v.GetComponent<VertexPrefab>().Destroy();
        }
        vertexPrefabs.Clear();
    }


}
