using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class test : MonoBehaviour
{
    public Material material = null;
    public float radius = 1f;
    public float tr = 1f;

    public int s = 5;
    public int t = 5;
    GameObject testSphere;
    MeshFilter mf;
    MeshRenderer mr;
    Mesh mesh;
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
        mesh = Utils.Utils.CreateTorus(radius, tr, s, t);
        mf.mesh = mesh;
    }
}
