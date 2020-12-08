using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class TheWorld : MonoBehaviour
{
    // test singleton
    public GameObject asteroid = null;

    // testing
    [SerializeField] int childCount = 0;
    [SerializeField] List<GameObject> colliders = new List<GameObject>();
    public GameObject collider = null;

    // root of M4x4 shader
    public SceneNode TheRoot;

    // M4x4s  in scene
    [SerializeField] List<Matrix4x4> objs = new List<Matrix4x4>();


    void Start() {
        Debug.Assert(!collider, "Please set Colliser prefab in the Editor.");
        // get number of child transforms of root
        childCount = TheRoot.transform.childCount;
    }

    // Update is called once per frame
    void Update()
    {
        objs.Clear();
        Matrix4x4 i = Matrix4x4.identity;
        TheRoot.CompositeXform(ref i, ref objs);
        DrawTargets();
    }

    void SetSceneColliders() {
        for (int i = 0; i < childCount; i++) {

        }
    }

    void DrawTargets() {
        foreach (var m in objs) {
            Vector3 pos = new Vector3(m.m03, m.m13, m.m23);
            Debug.DrawLine(pos, asteroid.transform.position, Color.blue);
        }
    }
}
