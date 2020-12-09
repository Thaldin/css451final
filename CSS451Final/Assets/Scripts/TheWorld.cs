using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public partial class TheWorld : MonoBehaviour {
    // root of M4x4 shader
    [Header("The universe")]
    public SceneNode TheRoot;
    private bool init = false;

    // test singleton
    public GameObject asteroid = null;

    [Header("Scene Objects")]
    [SerializeField] List<Transform> sceneObjects = new List<Transform>();
    private int childCount = 0;

    [Header("Colliders")]
    public GameObject colliderParent = null;
    List<GameObject> ObjColliders = new List<GameObject>();
    public GameObject objCollider = null;

    [Header("Rings")]
    public GameObject ringParent = null;
    [SerializeField] List<GameObject> ringObjects = new List<GameObject>();

    [Header("Drawn Lines")]
    [SerializeField] List<GameObject> starLines = new List<GameObject>();

    [Header("M4x4 transforms")]
    [SerializeField] List<Matrix4x4> m4x4s = new List<Matrix4x4>();
    private void Start() {
        ResetColliders();
        ResetSceneObjects();

        Debug.Assert(objCollider != null, "Please set Collider prefab in the Editor.");
        Debug.Assert(colliderParent != null, "Please set Collider parent in the Editor.");

        Debug.Assert(ringParent != null, "Please set the Ring Parent in the Editor.");

        // get number of child transforms of root
        GetSceneObjects();
        childCount = sceneObjects.Count;

        // initialize planets
        InitSceneObjects();

        // creat planet colliders
        CreateSceneColliders();

    }


    // Update is called once per frame
    private void Update() {
        m4x4s.Clear();
        Matrix4x4 i = Matrix4x4.identity;
        if (init) {
            TheRoot.CompositeXform(ref i, ref m4x4s);
            //if childCount changes, update colliders
            SetSceneColliders();
            DrawTargets(asteroid, Color.red);
            DrawStarLines(Color.white);
            Click();
        }

    }

    #region Scene Objects
    // populates sceneObjects list
    private void GetSceneObjects() {
        TheRoot.GetChildren(ref sceneObjects);
    }

    // initialized scene objects
    private void InitSceneObjects() {
        TheRoot.InitializeSceneNode(ref ringObjects);
        foreach (var r in ringObjects) {
            r.transform.parent = ringParent.transform;
        }

        init = true;
    }

    // resets sceneObjects list
    private void ResetSceneObjects() {
        foreach (var c in sceneObjects) {
            Destroy(c);
        }
        sceneObjects.Clear();
    }
    #endregion

    #region Scene Colliders
    // create planet colliders
    private void CreateSceneColliders() {
        ResetColliders();
        for (int i = 0; i < childCount; i++) {
            GameObject objColliderClone = Instantiate(objCollider);
            objColliderClone.transform.parent = colliderParent.transform;
            ObjColliders.Add(objColliderClone);
        }
    }

    // set planet colliders
    private void SetSceneColliders() {
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

    private void ResetColliders() {
        foreach (var c in ObjColliders) {
            Destroy(c);
        }
        ObjColliders.Clear();
    }

    #endregion

    void Click() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                string tag = hit.transform.tag;
                /*
                if (hit.transform.CompareTag("planet")) {
                    Debug.Log(hit.transform.name + " selected!");
                }
                */

                // potential
                switch (tag) {
                    case "star":
                        Debug.Log(hit.transform.name + " selected!");
                        return;
                    case "planet":
                        Debug.Log(hit.transform.name + " selected!");
                        return;
                    case "moon":
                        Debug.Log(hit.transform.name + " selected!");
                        return;
                    case "dwarf":
                        Debug.Log(hit.transform.name + " selected!");
                        return;
                    default:
                        return;
                }
            }
        }
    }

    #region Runtime Set Functions
    // set global time scale
    public void SetTimeScale(float v) {
        foreach (var sn in sceneObjects) {
            sn.GetComponent<SceneNode>().SetTimeScale(v);
        }
    }
    #endregion
}
