using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public Material material = null;
    public float radius = 1f;
    public int slices = 5;
    public int stacks = 5;
    GameObject testSphere;
    MeshFilter mf;
    MeshRenderer mr;
    // Start is called before the first frame update
    void Start()
    {
        testSphere = new GameObject();
        mf = testSphere.AddComponent<MeshFilter>();
        mr = testSphere.AddComponent<MeshRenderer>();

       
        mr.material = material;
    }

    // Update is called once per frame
    void Update()
    {
        mf.mesh.Clear();
        Mesh mesh;
        mesh = Utils.Utils.CreateSphereMesh(radius, slices, stacks);
        mf.mesh = mesh;
    }
}
