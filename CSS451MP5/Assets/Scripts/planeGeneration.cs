using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planeGeneration : meshGeneration {

    [SerializeField] private Vector3 planeTileScale = Vector3.one;
    [SerializeField] private Vector3 planeOffset = Vector3.zero;

    // the overall size of the plane
    public void SetResolution(int v) {
        meshResolution.x = v;
        meshResolution.y = v;
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
        int vertexCount = (int)meshResolution.x * (int)meshResolution.y;
        vertices = new Vector3[vertexCount];
        normals = new Vector3[vertices.Length];
        
        uv = new Vector2[vertexCount];


        // generate verticies
        // generate uvs
        // (uv - 0.5) * scale + 0.5
        int res = (int)meshResolution.x - 1;
        int i = 0;
        for (float x = 0; x < meshResolution.x; x++) {
            for (float z = 0; z < meshResolution.y; z++) {
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

        // return updated mesh
        return UpdateMesh();
    }

    //
    // Generate plane triangles
    //
    protected override void GenerateTrianges() {
        // generate triangles
        // clockwise triangle for camera-facing triangles
        int triangleCount = ((int)meshResolution.x - 1) * ((int)meshResolution.y - 1) * 2 * 3;
        triangles = new int[triangleCount];

        int vert = 0;
        int tris = 0;
        int res = (int)meshResolution.x - 1;
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

    public override void UpdateVertices() {
        for (int i = 0; i < vertices.Length; i++) {
            vertices[i] = vertexPrefabs[i].transform.position;
        }
    }
    //
    // Calculate plane Normals
    //
    #region Normals Calculation
    protected override void CalculateNormals() {
        // set arrays
        normals = new Vector3[vertices.Length];
        triNormals = new Vector3[triangles.Length / 3];
        triNormalPos = new Vector3[triNormals.Length];

        // calculate triangle normals
        for (int index = 0, i = 0; i < triNormals.Length; i++) {

            triNormals[i] = FaceNormal(i);
            //Debug.Log("Tri Normal Pos: " + i);
            triNormalPos[i] = TriNormalPos(index);
            index += 3;
        }

        // for fun, calculate the positions of the normals
        GenerateTriNormalsPrefab();

        int triStripSize = ((int)meshResolution.x - 1) * 2;
        int res = (int)meshResolution.x - 1;
        // left
        int triIndex = 0;
        int norIndex = 0;

        normals[norIndex] = triNormals[triIndex].normalized;
        Debug.Log("normals[" + norIndex + "] " + (triIndex));
        norIndex++;
        for (int i = 1; i <= res; i++) {
            // at the end? 2 tris
            if (i == res) {
                normals[norIndex] = (triNormals[triIndex] + triNormals[triIndex + 1]).normalized;
                Debug.Log("normals[" + norIndex + "] " + (triIndex) + ", " + (triIndex + 1));
                // end only goes up by one
            } else {
                // mid 3 tris
                normals[norIndex] = (triNormals[triIndex] + triNormals[triIndex + 1] + triNormals[triIndex + 2]).normalized;
                Debug.Log("normals[" + norIndex + "] " + (triIndex) + ", " + (triIndex + 1) + ", " + (triIndex + 2));
                // mid goes up by two
                triIndex += 2;
            }
            norIndex++;
        }

        // mid
        triIndex += 2;
        for (int x = 1; x < res; x++) {
            for (int y = 0; y <= res; y++) {
                // at 0 3 tris
                if (y == 0) {
                    normals[norIndex] = (triNormals[triIndex - triStripSize] +
                                        triNormals[triIndex - triStripSize + 1] +
                                        triNormals[triIndex]).normalized;

                    Debug.Log("normals[" + norIndex + "] " + (triIndex - triStripSize) + ", " + (triIndex - triStripSize + 1) + ", " + (triIndex));
                    // no triIndex inc
                } else if (y == res) {
                    // at end 3 tris
                    normals[norIndex] = (triNormals[triIndex] +
                                        triNormals[triIndex + 1] +
                                        triNormals[triIndex - triStripSize + 1]).normalized;

                    Debug.Log("normals[" + norIndex + "] " +
                        (triIndex) + ", " +
                        (triIndex + 1) + ", " +
                        (triIndex - triStripSize + 1));

                    // no triIndex inc
                } else {
                    // mid 6 tris
                    normals[norIndex] = (triNormals[triIndex] +
                                         triNormals[triIndex + 1] +
                                         triNormals[triIndex + 2] +
                                         triNormals[triIndex - triStripSize + 1] +
                                         triNormals[triIndex - triStripSize + 2] +
                                         triNormals[triIndex - triStripSize + 3]).normalized;

                    Debug.Log("normals[" + norIndex + "] " +
                                           (triIndex) + ", " +
                                           (triIndex + 1) + ", " +
                                           (triIndex + 2) + ", " +
                                           (triIndex - triStripSize + 1) + ", " +
                                           (triIndex - triStripSize + 2) + ", " +
                                           (triIndex - triStripSize + 3));

                    // triIndex inc 2
                    triIndex += 2;
                }
                norIndex++;
            }
            triIndex += 2;
        }

        // right
        triIndex = triNormals.Length - triStripSize;
        for (int i = 0; i < res; i++) {
            // at 0 2 tris
            if (i == 0) {
                normals[norIndex] = (triNormals[triIndex] + triNormals[triIndex + 1]).normalized;
                Debug.Log("normals[" + norIndex + "] " + (triIndex) + ", " + (triIndex + 1));

                // end only goes up by one
                triIndex++;
            } else {
                // mid 3 tris
                normals[norIndex] = (triNormals[triIndex] + triNormals[triIndex + 1] + triNormals[triIndex + 2]).normalized;
                Debug.Log("normals[" + norIndex + "] " + (triIndex) + ", " + (triIndex + 1) + ", " + (triIndex + 2));


                // mid goes up by two
                triIndex += 2;
            }
            norIndex++;
        }

        //last
        normals[norIndex] = triNormals[triNormals.Length - 1];
        Debug.Log("normals[" + norIndex + "] " + (triNormals.Length - 1));

        // calculate vertex normal
    }

    private Vector3 FaceNormal(int index) {
        Vector3 V0 = vertices[triangles[index]];
        Vector3 V1 = vertices[triangles[index + 1]];
        Vector3 V2 = vertices[triangles[index + 2]];

        Vector3 a = V1 - V0;
        Vector3 b = V2 - V0;
        return Vector3.Cross(a, b).normalized;
    }

    public void SetNormals() {
        for (int i = 0; i < normals.Length; i++) {
            vertexPrefabs[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, normals[i]);
            // this is cheese
            vertexPrefabs[i].transform.GetChild(1).transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }
    #endregion normals

    public override Mesh UpdateMesh() {
        Debug.Log("Updating Mesh");
        //mesh.Clear();
        mesh = new Mesh();
        // calc tris
        GenerateTrianges();
        // calc normals
        CalculateNormals();
        // set normals
        SetNormals();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;
        return mesh;
    }
}
