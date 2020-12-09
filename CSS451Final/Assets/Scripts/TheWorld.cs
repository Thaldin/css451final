using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TheWorld : MonoBehaviour {
    // test singleton
    public GameObject asteroid = null;

    // testing
    [SerializeField] int childCount = 0;
    [SerializeField] List<Transform> sceneObjects = new List<Transform>();

    List<GameObject> ObjColliders = new List<GameObject>();
    public GameObject objCollider = null;

    // root of M4x4 shader
    public SceneNode TheRoot;
    public bool init = false;
    // M4x4s  in scene
    [SerializeField] List<Matrix4x4> m4x4s = new List<Matrix4x4>();

    
    void Start() {
        ResetColliders();
        ResetSceneObjects();

        Debug.Assert(objCollider != null, "Please set Collider prefab in the Editor.");
        
        // get number of child transforms of root
        GetSceneObjects();
        childCount = sceneObjects.Count;
        
        // initialize planets
        InitSceneObjects();

        // creat planet colliders
        CreateSceneColliders();

    }


    // Update is called once per frame
    void Update() {
        m4x4s.Clear();
        Matrix4x4 i = Matrix4x4.identity;
        if (init) {
            TheRoot.CompositeXform(ref i, ref m4x4s);
        }
        //if childCount changes, update colliders
        SetSceneColliders();
        DrawTargets();
        DrawStarLines();
        Click();
    }

    void GetSceneObjects() {
        TheRoot.GetChildren(ref sceneObjects);
    }

    void InitSceneObjects() {
        TheRoot.InitializeSceneNode();
        init = true;
    }

    // create planet colliders
    void CreateSceneColliders() {
        ResetColliders();
        for (int i = 0; i < childCount; i++) {
            GameObject objColliderClone = Instantiate(objCollider);
            ObjColliders.Add(objColliderClone);
        }
    }

    // set planet colliders
    void SetSceneColliders() {
        for (int i = 0; i < childCount; i++) {
            // set the name of the collider obj
            ObjColliders[i].name = sceneObjects[i].name + "Collider";

            // set the radius of the collider obj
            sphereColliderScript scs;
            ObjColliders[i].TryGetComponent<sphereColliderScript>(out scs);
            if (scs) {
                float diameter = sceneObjects[i].GetComponent<SceneNode>().GetPlanetDiameter();
                scs.Initialize(diameter);
            }
        }
    }

    void ResetColliders() {
        foreach (var c in ObjColliders) {
            Destroy(c);
        }
        ObjColliders.Clear();
    }

    void ResetSceneObjects() {
        foreach (var c in sceneObjects) {
            Destroy(c);
        }
        sceneObjects.Clear();
    }
    
    void DrawTargets() {
        for (int i = 0; i < m4x4s.Count; i++) {
            Vector3 pos = new Vector3(m4x4s[i].m03, m4x4s[i].m13, m4x4s[i].m23);
            Debug.DrawLine(pos, asteroid.transform.position, Color.blue);
        }
    }

    void DrawStarLines() {
        for (int i = 0; i < ObjColliders.Count; i++) {
            Vector3 pos = new Vector3(m4x4s[i].m03, m4x4s[i].m13, m4x4s[i].m23);
            ObjColliders[i].transform.position = pos;
            // the star is the first trasform of the root
            Debug.DrawLine(pos, TheRoot.transform.GetChild(0).transform.position, Color.white);
        }
    }

    void Click() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                if (hit.transform.CompareTag("planet")) {
                    Debug.Log(hit.transform.name + " selected!");
                }
            }
        }
    }
}
