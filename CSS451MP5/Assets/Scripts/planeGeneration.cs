using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planeGeneration : meshGeneration {


    public int planeSize = 10;
    [SerializeField] private Vector2 planeResolution = new Vector2(2f, 2f);
    [SerializeField] private Vector3 planeTileScale = Vector3.one;
    [SerializeField] private Vector3 planeOffset = Vector3.zero;

    // the overall size of the plane
    public void SetResolution(int v) {
        planeResolution.x = v;
        planeResolution.y = v;
    }

    // set scales
    public void SetTileScaleX(float v) {
        planeTileScale.x = v;
    }
    public void SetTileScaleY(float v) {
        planeTileScale.y = v;
    }
    public Vector3 GetTileScale() {
        return planeTileScale;
    }

    // set offset
    public void SetTileOffsetX(float v) {
        planeOffset.x = v;
    }
    public void SetTileOffsetY(float v) {
        planeOffset.y = v;
    }
    public Vector3 GetTileOffset() {
        return planeOffset;
    }

    public Mesh CreatePlane() {
        // set arrays
        int vertexCount = (int)planeResolution.x * (int)planeResolution.y;
        vertices = new Vector3[vertexCount];
        normals = new Vector3[vertices.Length];
        
        uv = new Vector2[vertexCount];


        // generate verticies
        // generate uvs
        // (uv - 0.5) * scale + 0.5
        int res = (int)planeResolution.x - 1;
        int i = 0;
        for (float x = 0; x < planeResolution.x; x++) {
            for (float z = 0; z < planeResolution.y; z++) {
                // vertex
                float vertX = (x * planeSize) / res;
                float vertZ = (z * planeSize) / res;
                vertices[i] = new Vector3(vertX, 0, vertZ);
                // uv
                float uvX = (x * planeTileScale.x) / res;
                float uvZ = (z * planeTileScale.y) / res;
                uv[i] = new Vector2(uvX, uvZ) + (Vector2)planeOffset;
                // normal
                normals[i] = Vector3.up;
                                    
                i++;
            }
        }

        GenerateVertexPrefab();
        GenerateTrianges();
        // calc normals
        return UpdateMesh();
    }
    public override void GenerateTrianges() {
        // generate triangles
        // clockwise triangle for camera-facing triangles
        int triangleCount = ((int)planeResolution.x - 1) * ((int)planeResolution.y - 1) * 2 * 3;
        triangles = new int[triangleCount];

        int vert = 0;
        int tris = 0;
        int res = (int)planeResolution.x - 1;
        for (int z = 0; z < res; z++) {
            for (int x = 0; x < res; x++) {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + 1;
                triangles[tris + 2] = vert + res + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + res + 2;
                triangles[tris + 5] = vert + res + 1;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }
}
