using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planeGeneration : meshGeneration {

    [SerializeField] private Vector3 planeTileScale = Vector3.one;
    [SerializeField] private Vector3 planeOffset = Vector3.zero;

    public Texture[] selectableTextures = new Texture[0];

    Matrix3x3 uvMatrix = new Matrix3x3();
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

    public void SetMainTexture(int index) {
        matieral.mainTexture = selectableTextures[index];
    }
    public Texture[] GetSelectableTextures() {
        return selectableTextures;
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
                float vertX = (x * meshSize) / res;
                float vertZ = (z * meshSize) / res;
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


    protected override void CalculateNormals(Vector3[] v, int resolution) {
        int triangleCount = triangles.Length / 3;
        Vector3[] triNormals = new Vector3[triangleCount];
        int triPerRow = (resolution - 1) * 2;

        //Debug.Log("Triangle Count: " + triangleCount);

        // Calculate the triangle normals
        for (int i = 0; i < triangleCount; i++) {
            int oddTri = ((i % 2) == 0) ? 0 : 1;                    // Odd or even triangle
            int startVertex = (i / 2) + (i / triPerRow) + oddTri;   // Starting vertex for triangle

            if (oddTri == 0) {
                // Even numbered triangles
                triNormals[i] = FaceNormals(v, startVertex, startVertex + 1, startVertex + resolution);
            } else {
                // Odd numbered triangles
                triNormals[i] = FaceNormals(v, startVertex, startVertex + resolution, startVertex + resolution - 1);
            }
            //Debug.Log("Tri: " + i + ", v1: " + startVertex + ", Odd: " + oddTri + ", Row: " + (int)(i / triPerRow));
        }

        // Calculate the vertex normals
        for (int i = 0; i < v.Length; i++) {
            //Debug.Log("Vertex/Normal: " + i);
            // Used for determining left/right columns
            int mod = i % resolution;

            if (i < resolution) {
                // Top row, special case
                if (mod == 0) {
                    // Upper left corner
                    normals[i] = triNormals[0].normalized;
                } else if (mod == (resolution - 1)) {
                    // Upper right corner
                    normals[i] = (triNormals[triPerRow - 1] + triNormals[triPerRow - 2]).normalized;
                } else {
                    // Middle top row
                    int triId = GetVertexBottomTriangle(i, triPerRow, resolution);
                    normals[i] = (triNormals[triId - 2] + triNormals[triId - 1] + triNormals[triId]).normalized;
                }
            } else if (i >= v.Length - resolution) {
                // Bottom row, special case
                if (mod == 0) {
                    // Bottom left corner
                    normals[i] = (triNormals[triangleCount - resolution] + triNormals[triangleCount - resolution + 1]).normalized;
                } else if (mod == (resolution - 1)) {
                    // Bottom right corner
                    normals[i] = triNormals[triangleCount - 1].normalized;
                } else {
                    // Middle bottom row
                    int triId = GetVertexTopTriangle(i, triPerRow, resolution);
                    normals[i] = (triNormals[triId - 1] + triNormals[triId] + triNormals[triId + 1]).normalized;
                }
            } else {
                // Rest of vertices
                if (mod == 0) {
                    // Left column of vertices
                    int triId = ((i / resolution) - 1) * triPerRow;
                    normals[i] = (triNormals[triId] + triNormals[triId + 1] + triNormals[triId + triPerRow]).normalized;
                } else if (mod == (resolution - 1)) {
                    // Right column of vertices
                    int triId = ((i / resolution) * triPerRow) - 1;
                    //Debug.Log("TriId: " + triId);
                    normals[i] = (triNormals[triId] + triNormals[triId + triPerRow - 1] + triNormals[triId + triPerRow]).normalized;
                } else {
                    // Center of mesh
                    int topTriId = GetVertexTopTriangle(i, triPerRow, resolution);
                    int bottomTriId = GetVertexBottomTriangle(i, triPerRow, resolution);
                    //Debug.Log("Top Tri: " + topTriId + ", Bottom Tri: " + bottomTriId);
                    normals[i] = (triNormals[topTriId - 1] + triNormals[topTriId] + triNormals[topTriId + 1] + triNormals[bottomTriId - 2] + triNormals[bottomTriId - 1] + triNormals[bottomTriId]).normalized;
                }
            }
        }

        UpdateNormals(v, normals);
    }

    /* TODO: Delete?
    private Vector3 FaceNormal(int index) {
        Vector3 V0 = vertices[triangles[index]];
        Vector3 V1 = vertices[triangles[index + 1]];
        Vector3 V2 = vertices[triangles[index + 2]];

        Vector3 a = V1 - V0;
        Vector3 b = V2 - V0;
        return Vector3.Cross(a, b).normalized;
    }
    */
    public void SetNormals() {
        for (int i = 0; i < normals.Length; i++) {
            vertexPrefabs[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, normals[i]);
            // this is cheese
            vertexPrefabs[i].transform.GetChild(1).transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }
    #endregion normals

    public override Mesh UpdateMesh() {
        //Debug.Log("Updating Mesh");
        //mesh.Clear();
        mesh = new Mesh();
        // calc tris
        GenerateTrianges();
        // calc normals
        CalculateNormals(vertices, (int)meshResolution.x);
        // set normals
        SetNormals();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;
        return mesh;
    }
}
