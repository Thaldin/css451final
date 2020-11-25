﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class meshGeneration : MonoBehaviour {

    [Header("Plane Properties")]
    private int zSize = 2;
    private int xSize = 2;
    private int tileSize = 1;

    public GameObject vertex = null;
    public Material matieral = null;


    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    public Vector3[] vertices = new Vector3[0];
    public Vector2[] uv = new Vector2[0];
    public int[] triangles = new int[0];
    private List<GameObject> vertexPrefabs = new List<GameObject>();

    public Vector3[] triangleNormals = new Vector3[0];
    private List<GameObject> meshNormals = new List<GameObject>();
    public GameObject meshNormal = null;
    //public GameObject[] vertexPrefabs = new GameObject[0];

    // Start is called before the first frame update
    void Start() {
        Debug.Assert(vertex != null);
        Debug.Assert(matieral != null);
        Debug.Assert(meshNormal != null);
        InitializeComponents();
        mesh = new Mesh();
        CreatePlane();
    }

    private void LateUpdate() {
        UpdateVertices();
    }

    void InitializeComponents() {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetSize(float newSize) {
        xSize = (int)newSize;
        zSize = (int)newSize;
    }

    public void SetTiling(float newTile) {
        tileSize = (int)newTile;
    }

    public void CreatePlane() {
        // set arrays
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        triangles = new int[xSize * zSize * 6];
        uv = new Vector2[(xSize + 1) * (zSize + 1)];
        triangleNormals = new Vector3[(xSize + 1) * (zSize + 1)];

        // generate verticies
        // generate uvs
        for (int i = 0, z = 0; z < (zSize + 1); z++) {
            for (int x = 0; x < (xSize + 1); x++) {
                vertices[i] = new Vector3(x, 0, z);
                uv[i] = new Vector2(x / (float)xSize * tileSize, z / (float)zSize * tileSize);
                i++;
            }
        }

        // generate triangles
        GenerateTrianges();

        // assign arrays
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;

        // assign components
        meshFilter.mesh = mesh;
        meshRenderer.material = matieral;
        //CalculateTriangleNormals();
        //mesh.RecalculateNormals(0);

        // recalculate vertex prefabs
        GenerateVertexPrefab();
    }

    public void UpdateMesh() {
        // update vertices
        UpdateVertices();

        // generate triangles
        GenerateTrianges();

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;

        // assign components
        meshFilter.mesh = mesh;
        meshRenderer.material = matieral;
        //CalculateTriangleNormals();
        //mesh.RecalculateNormals(0);
    }

    private void UpdateVertices() {
        for (int i = 0; i < vertices.Length; i++) {
            vertices[i] = vertexPrefabs[i].transform.position;
        }
    }

    private void GenerateTrianges() {
        // generate triangles
        // clockwise triangle for camera-facing triangles
        int vert = 0;
        int tris = 0;
        for (int z = 0; z < zSize; z++) {
            for (int x = 0; x < xSize; x++) {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    void GenerateVertexPrefab() {
        ClearVertexPrefabList();
        for (int i = 0; i < vertices.Length; i++) {
            GameObject vertexSpawn = Instantiate(vertex);
            vertexSpawn.name = "Vertex" + i;
            vertexPrefabs.Add(vertexSpawn);
            vertexSpawn.transform.position = vertices[i];
            // recalculate normal

        }
    }

    void ClearVertexPrefabList() {
        foreach (var v in vertexPrefabs) {
            Destroy(v);
        }
        vertexPrefabs.Clear();
    }

    void CalculateTriangleNormals() {
        // Take vertex A, B and C from a triangle. Then create vectors AB and AC. 
        // ABxAC will give you the normal of the triangle. 
        // ACxAB will give the opposite normal (ABxAC = - ACxAB).
        ClearTriangleNormals();
        
        for (int t = 0, i = 0; i < triangles.Length; i += 3) {
            Vector3 AB = vertices[triangles[i]] - vertices[triangles[i + 1]];
            Vector3 AC = vertices[triangles[i]] - vertices[triangles[i + 2]];

            Vector3 triNormal = Vector3.Cross(AC, AB);
            triangleNormals[t] = triNormal.normalized;
            GameObject triNormalClone = (GameObject)Instantiate(meshNormal);
            triNormalClone.transform.localPosition = triNormal;
            meshNormals.Add(triNormalClone);
            t++;
        }
        
        // (normal + normal).normalized
    }

    void ClearTriangleNormals() {
        foreach (var v in meshNormals) {
            Destroy(v);
        }
        meshNormals.Clear();
    }


}
