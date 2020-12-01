using System.Collections.Generic;
using UnityEngine;

public class MP5World : MonoBehaviour
{
    public GameObject LookAt = null;
    public bool RenderMesh = true;

    public int CylinderResolution = 10;
    public int MeshResolution = 4;
    public int CylinderRotation = 180;

    public Material testMaterial;

    private Mesh cylinderMesh = null;
    private MeshFilter cylinderFilter = null;
    private MeshRenderer cylinderRenderer = null;

    private const int nCylinderHeight = 10;
    private List<GameObject> sphereList;
    private GameObject renderObject;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(LookAt != null, "Please set LookAt object.");
        cylinderMesh = new Mesh();

        sphereList = new List<GameObject>();
        renderObject = new GameObject();
        cylinderFilter = renderObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        cylinderRenderer = renderObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
    }

    // Update is called once per frame
    void Update()
    {
        if (RenderMesh)
        {
            RenderTextureMesh();
        }
        else
        {
            RenderCylinderMesh();
        }
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

    private void RenderTextureMesh()
    {
        Debug.Log("Rendering Mesh at Res: " + MeshResolution);
    }

    private void RenderCylinderMesh()
    {
        // Compute Triangle and Vertex count
        var vertexCount = CylinderResolution * nCylinderHeight;
        var triangleCount = ((CylinderResolution - 1) * (nCylinderHeight - 1)) * 2;
        var vertexArray = new Vector3[vertexCount];
        var triangleArray = new int[triangleCount * 3];

        // Store vertex count to load into array
        int arrayCounter = vertexCount;

        // Build Vertex Array
        float fTheta = (CylinderRotation / CylinderResolution) * Mathf.Deg2Rad;
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
            //Debug.Log("Vertex: " + i + ", Mod:" + mod + ", Count: " + vertexCount + ", Res: " + CylinderResolution);
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

        // Clear existing mesh
        cylinderMesh.Clear();
        cylinderMesh.vertices = vertexArray;
        cylinderMesh.triangles = triangleArray;

        cylinderFilter.mesh = cylinderMesh;
        cylinderRenderer.material = testMaterial;

        // TODO: Fix shader
    }

    private void RenderSpheres(Vector3[] array, int count)
    {
        // Clear previous spheres
        foreach (GameObject obj in sphereList)
        {
            Destroy(obj);
        }

        sphereList.Clear();

        // Render Test Spheres
        var scale = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 pos;
        for (int i = 0; i < count; i++)
        {
            var s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            s.transform.localScale = scale;
            pos.x = array[i].x;
            pos.y = array[i].y;
            pos.z = array[i].z;
            s.transform.localPosition = pos;
            sphereList.Add(s);
        }
    }
}
