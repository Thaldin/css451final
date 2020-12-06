using UnityEngine;

public class cylinderGeneration : meshGeneration
{
    [Header("Cylinder Properties")]
    public float cylinderRadius = 10f;
    public int CylinderResolution = 10;
    public int CylinderRotation = 180;

    private const int nCylinderHeight = 10;

    public Mesh CreateCylinder()
    {

        // Compute Triangle and Vertex count
        int vertexCount = CylinderResolution * nCylinderHeight;
        int triangleCount = ((CylinderResolution - 1) * (nCylinderHeight - 1)) * 2;
        vertices = new Vector3[vertexCount];
        triangles = new int[triangleCount * 3];
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
                vertices[--arrayCounter] = new Vector3(cylinderRadius * Mathf.Cos(i * fTheta),
                                                       yValue,
                                                       cylinderRadius * Mathf.Sin(i * fTheta));
            }

            if (CylinderRotation == 360)
            {
                vertices[arrayCounter] = vertices[arrayCounter + (CylinderResolution - 1)];
            }
        }

        GenerateVertexPrefab();
        GenerateTrianges();
        CalculateNormals(vertices, CylinderResolution);

        return UpdateMesh();
    }

    public override void GenerateVertexPrefab() {
        ClearVertexPrefabList();
        for (int i = 0; i < vertices.Length; i++) {
            GameObject vertexSpawn = Instantiate(vertex);
            vertexSpawn.name = "Vertex" + i;
            vertexSpawn.GetComponent<VertexPrefab>().Id = i;
            vertexPrefabs.Add(vertexSpawn);
            vertexSpawn.transform.position = vertices[i];

            if (i % CylinderResolution == (CylinderResolution - 1)) {
                vertexSpawn.tag = "selectvertex";
                vertexSpawn.GetComponent<VertexPrefab>().SetColor(Color.white);
            } else {
                vertexSpawn.tag = "badvertex";
                vertexSpawn.GetComponent<VertexPrefab>().SetColor(Color.black);
            }

        }
    }

    public override Mesh UpdateMesh()
    {
        mesh = new Mesh();

        UpdateVertices();
        GenerateTrianges();
        CalculateNormals(vertices, CylinderResolution);

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;

        return mesh;
    }

    public override void UpdateVertices()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = vertexPrefabs[i].transform.position;
        }
    }

    public void UpdateRotation()
    {
        float fTheta = (CylinderRotation / (CylinderResolution - 1)) * Mathf.Deg2Rad;
        int vertexCount = (CylinderResolution * nCylinderHeight) - 1;

        // Center of circle
        Vector3 center = Vector3.zero;

        // Use magnitude of this to get new radius
        Vector3 translate = Vector3.zero;

        for (int h = 0; h < nCylinderHeight; h++)
        {
            center.y = vertexPrefabs[vertexCount].transform.localPosition.y;
            translate = vertexPrefabs[vertexCount].transform.localPosition - center;

            for (int i = 0; i < CylinderResolution; i++)
            {
                vertexPrefabs[vertexCount--].transform.localPosition = new Vector3(translate.magnitude * Mathf.Cos(i * fTheta),
                                                                                   center.y,
                                                                                   translate.magnitude * Mathf.Sin(i * fTheta));
            }

            if (CylinderRotation == 360)
            {
                vertices[vertexCount + 1] = vertices[(vertexCount + 1) + (CylinderResolution - 1)];
            }
        }
    }

    public void UpdateVertexRow(int vertexId, vertexHandle.axis dir, Vector3 mouseDelta)
    {
        var prefab = vertexPrefabs[vertexId];
        float fTheta = (CylinderRotation / (CylinderResolution - 1)) * Mathf.Deg2Rad;

        // Center of circle
        Vector3 center = Vector3.zero;
        center.y = prefab.transform.localPosition.y;

        // Use magnitude of this to get new radius
        Vector3 translate = prefab.transform.localPosition - center;

        int pos = 1;
        for (int i = (vertexId - 1); i > (vertexId - CylinderResolution); i--)
        {
            vertexPrefabs[i].transform.localPosition = new Vector3(translate.magnitude * Mathf.Cos(pos * fTheta),
                                                                   center.y,
                                                                   translate.magnitude * Mathf.Sin(pos++ * fTheta));
        }
    }

    protected override void GenerateTrianges()
    {
        // Compute Triangles
        var triCount = 0;
        for (int i = 0; i < (vertices.Length - CylinderResolution); i++)
        {
            int mod = i % CylinderResolution;
            if (mod == 0)       // Left side of mesh
            {
                triangles[triCount++] = i;
                triangles[triCount++] = i + 1;
                triangles[triCount++] = i + CylinderResolution;
            }
            else if (mod == (CylinderResolution - 1))  // Right side of mesh
            {
                triangles[triCount++] = i;
                triangles[triCount++] = i + CylinderResolution;
                triangles[triCount++] = i + CylinderResolution - 1;
            }
            else                // Middle of mesh
            {
                triangles[triCount++] = i;
                triangles[triCount++] = i + 1;
                triangles[triCount++] = i + CylinderResolution;
                triangles[triCount++] = i;
                triangles[triCount++] = i + CylinderResolution;
                triangles[triCount++] = i + CylinderResolution - 1;
            }
        }
    }

    protected override void CalculateNormals(Vector3[] v, int resolution)
    {
        int triangleCount = ((CylinderResolution - 1) * (nCylinderHeight - 1)) * 2;
        Vector3[] triNormals = new Vector3[triangleCount];
        int triPerRow = (resolution - 1) * 2;

        // Calculate the triangle normals
        for (int i = 0; i < triangleCount; i++)
        {
            int oddTri = ((i % 2) == 0) ? 0 : 1;                    // Odd or even triangle
            int startVertex = (i / 2) + (i / triPerRow) + oddTri;   // Starting vertex for triangle

            if (oddTri == 0)
            {
                // Even numbered triangles
                triNormals[i] = FaceNormals(v, startVertex, startVertex + 1, startVertex + resolution);
            }
            else
            {
                // Odd numbered triangles
                triNormals[i] = FaceNormals(v, startVertex, startVertex + resolution, startVertex + resolution - 1);
            }
        }

        // Calculate the vertex normals
        for (int i = 0; i < v.Length; i++)
        {
            // Used for determining left/right columns
            int mod = i % resolution;

            if (i < resolution)
            {
                // Top row, special case
                if (mod == 0)
                {
                    // Upper left corner
                    normals[i] = triNormals[0].normalized * -1;
                }
                else if (mod == (resolution - 1))
                {
                    // Upper right corner
                    normals[i] = (triNormals[triPerRow - 1] + triNormals[triPerRow - 2]).normalized * -1;
                }
                else
                {
                    // Middle top row
                    int triId = GetVertexBottomTriangle(i, triPerRow, resolution);
                    normals[i] = (triNormals[triId - 2] + triNormals[triId - 1] + triNormals[triId]).normalized * -1;
                }
            }
            else if (i >= v.Length - resolution)
            {
                // Bottom row, special case
                if (mod == 0)
                {
                    // Bottom left corner
                    normals[i] = (triNormals[triangleCount - triPerRow] + triNormals[triangleCount - triPerRow + 1]).normalized * -1;
                }
                else if (mod == (resolution - 1))
                {
                    // Bottom right corner
                    normals[i] = triNormals[triangleCount - 1].normalized * -1;
                }
                else
                {
                    // Middle bottom row
                    int triId = GetVertexTopTriangle(i, triPerRow, resolution);
                    normals[i] = (triNormals[triId - 1] + triNormals[triId] + triNormals[triId + 1]).normalized * -1;
                }
            }
            else
            {
                // Rest of vertices
                if (mod == 0)
                {
                    // Left column of vertices
                    int triId = ((i / resolution) - 1) * triPerRow;
                    normals[i] = (triNormals[triId] + triNormals[triId + 1] + triNormals[triId + triPerRow]).normalized * -1;
                }
                else if (mod == (resolution - 1))
                {
                    // Right column of vertices
                    int triId = ((i / resolution) * triPerRow) - 1;
                    normals[i] = (triNormals[triId] + triNormals[triId + triPerRow - 1] + triNormals[triId + triPerRow]).normalized * -1;
                }
                else
                {
                    // Center of mesh
                    int topTriId = GetVertexTopTriangle(i, triPerRow, resolution);
                    int bottomTriId = GetVertexBottomTriangle(i, triPerRow, resolution);
                    normals[i] = (triNormals[topTriId - 1] + triNormals[topTriId] + triNormals[topTriId + 1] + triNormals[bottomTriId - 2] + triNormals[bottomTriId - 1] + triNormals[bottomTriId]).normalized * -1;
                }
            }
        }

        UpdateNormals(v, normals);
    }
}