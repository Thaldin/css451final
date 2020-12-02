using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planeGeneration : meshGeneration
{
    private int zSize = 2;
    private int xSize = 2;
    private Vector2 planeSize = new Vector2(2f,2f);
    [SerializeField] private int tileSizeX = 1;
    [SerializeField] private Vector3 planeTile = Vector3.one;
    [SerializeField] private Vector3 planeTileScale = Vector3.one;
    [SerializeField] private Vector3 planeOffset = Vector3.zero;


    public void SetSize(int v) {
        planeSize.x = v;
        planeSize.y = v;
    }

    // tile multiplier
    public void SetTiling(int v) {
        planeTile.x = v;
        planeTile.z = v;
}

    // set tiling
    public void SetTilingX(int v) {
        planeTile.x = v;
    }
    public void SetTilingZ(int v) {
        planeTile.z = v;
    }

    // set scales
    public void SetTileScaleX(int v) {
        planeTileScale.x = v;
    }
    public void SetTileScaleZ(int v) {
        planeTileScale.z = v;
    }

    // set offset
    public void SetTileOffsetX(int v) {
        planeOffset.x = v;
    }
    public void SetTileOffsetZ(int v) {
        planeOffset.z = v;
    }

    public Vector3 GetTile() {
        return planeTile;
    }

    public Vector3 GetTileScale() {
        return planeTileScale;
    }

    public Vector3 GetTileOffset() {
        return planeOffset;
    }

    public Mesh CreatePlane() {
        
        // set arrays
        vertices = new Vector3[((int)planeSize.x + 1) * ((int)planeSize.y + 1)];
        normals = new Vector3[vertices.Length];
        triangles = new int[(int)planeSize.x * (int)planeSize.y * 6];
        uv = new Vector2[((int)planeSize.x + 1) * ((int)planeSize.y + 1)];

        // generate verticies
        // generate uvs
        for (int i = 0, z = 0; z < (planeSize.y + 1); z++) {
            for (int x = 0; x < (planeSize.x + 1); x++) {
                vertices[i] = new Vector3(x, 0, z);
                uv[i] = new Vector2((x / planeSize.x * planeTile.x * planeTileScale.x) + planeOffset.x, 
                                    (z / planeSize.y * planeTile.z * planeTileScale.z) + planeOffset.z);
                i++;
            }
        }

        // generate triangles
        GenerateTrianges();
        return UpdateMesh();
    }

    public override Mesh UpdateMesh() {
        mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;

        GenerateVertexPrefab();

        return mesh;
    }

    public override void UpdateVertices() {
        for (int i = 0; i < vertices.Length; i++) {
            vertices[i] = vertexPrefabs[i].transform.position;
        }
    }

    public override void GenerateTrianges() {
        // generate triangles
        // clockwise triangle for camera-facing triangles
        int vert = 0;
        int tris = 0;
        for (int z = 0; z < (int)planeSize.y; z++) {
            for (int x = 0; x < (int)planeSize.x; x++) {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + (int)planeSize.x + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + (int)planeSize.x + 1;
                triangles[tris + 5] = vert + (int)planeSize.x + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }
}
