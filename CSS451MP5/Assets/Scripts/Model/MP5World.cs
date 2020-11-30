using System.Collections.Generic;
using UnityEngine;

public class MP5World : MonoBehaviour
{
    public GameObject LookAt = null;
    public bool RenderMesh = true;

    public int CylinderResolution = 10;
    public int MeshResolution = 4;
    public int CylinderRotation = 180;

    public Material firstMaterial;
    public Material lastMaterial;
    public Material testMaterial;

    private Mesh cylinderMesh = null;
    private const int nCylinderHeight = 10;
    private List<GameObject> sphereList;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(LookAt != null, "Please set LookAt object.");
        cylinderMesh = new Mesh();

        sphereList = new List<GameObject>();
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
        var triangleCount = ((CylinderResolution - 1) * (CylinderResolution - 1)) * 2;
        var vertexArray = new Vector3[vertexCount];
        var triangleArray = new int[triangleCount];

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


        RenderSpheres(vertexArray, vertexCount);

        // Clear existing mesh
        cylinderMesh.Clear();
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
            if (i == (count - 1))
            {
                s.GetComponent<MeshRenderer>().material = lastMaterial;
            }
            else if (i == 0)
            {
                s.GetComponent<MeshRenderer>().material = firstMaterial;
            }
            else if ((i == (CylinderResolution - 1)) || (i == (count - CylinderResolution)))
            {
                s.GetComponent<MeshRenderer>().material = testMaterial;
            }

            sphereList.Add(s);
        }
    }
}
