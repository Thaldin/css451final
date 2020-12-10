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
    [SerializeField] List<GameObject> colliderObjects = new List<GameObject>();
    public GameObject objCollider = null;

    [Header("Rings")]
    public GameObject ringParent = null;
    [SerializeField] List<GameObject> ringObjects = new List<GameObject>();
    bool ringIsOn = true;

    [Header("Drawn Lines")]
    public GameObject starLineParent = null;
    [SerializeField] List<GameObject> starLines = new List<GameObject>();
    bool debugIsOn = false;

    [Header("M4x4 transforms")]
    [SerializeField] List<Matrix4x4> m4x4s = new List<Matrix4x4>();
    private void Start() {
        ResetColliders();
        ResetSceneObjects();

        Debug.Assert(objCollider != null, "Please set Collider prefab for " + name + " in the Editor.");
        Debug.Assert(colliderParent != null, "Please set Collider parent for " + name + " in the Editor.");
        Debug.Assert(starLineParent != null, "Please set starLineParent parent for " + name + " in the Editor.");
        Debug.Assert(ringParent != null, "Please set the Ring Parent for " + name + " in the Editor.");

        // get number of child transforms of root
        GetSceneObjects();
        childCount = sceneObjects.Count;

        // initialize planets
        InitSceneObjects();


        CreateStarLines(Color.white);
    }


    // Update is called once per frame
    private void FixedUpdate() {
        m4x4s.Clear();
        Matrix4x4 i = Matrix4x4.identity;
        if (init) {
            TheRoot.CompositeXform(ref i, ref m4x4s);
            //if childCount changes, update colliders
            UpdateColliderObjects();
            DrawTargets(asteroid, Color.red);

            if (debugIsOn) { 
                DrawStarLines();
            }
        }

    }

    #region Scene Objects
    // populates sceneObjects list
    private void GetSceneObjects() {
        TheRoot.GetChildren(ref sceneObjects);
    }

    // initialized scene objects
    private void InitSceneObjects() {
        TheRoot.InitializeSceneNode(ref ringObjects, ref colliderObjects);

        for (int i = 0; i < colliderObjects.Count; i++) { 
            ringObjects[i].transform.parent = ringParent.transform;
            colliderObjects[i].transform.parent = colliderParent.transform;
            colliderObjects[i].GetComponent<sphereColliderScript>().SetIndex(i);
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

    // set planet colliders
    private void UpdateColliderObjects() {
        for (int i = 0; i < childCount; i++) {
            Vector3 pos = new Vector3(m4x4s[i].m03, m4x4s[i].m13, m4x4s[i].m23);
            colliderObjects[i].transform.position = pos;
        }
    }

    private void ResetColliders() {
        foreach (var c in colliderObjects) {
            Destroy(c);
        }
        colliderObjects.Clear();
    }

    #endregion
    
    #region Draw Lines
    // draws lines between planets and object
    private void DrawTargets(GameObject tar, Color color = default) {
        for (int i = 0; i < m4x4s.Count; i++) {
            Vector3 pos = new Vector3(m4x4s[i].m03, m4x4s[i].m13, m4x4s[i].m23);
            Debug.DrawLine(pos, tar.transform.position, color);
        }
    }

    // draws lines between star and planets
    private void CreateStarLines(Color color) {
        ClearStarLines();
        for (int i = 0; i < colliderObjects.Count; i++) {
            // create new line obj
            GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            line.GetComponent<MeshRenderer>().material.color = color;
            line.SetActive(debugIsOn);
            line.transform.parent = starLineParent.transform;
            starLines.Add(line);
        }
    }

    private void DrawStarLines() {

        for (int i = 0; i < colliderObjects.Count; i++) {
            Vector3 pos = new Vector3(m4x4s[i].m03, m4x4s[i].m13, m4x4s[i].m23);
            colliderObjects[i].transform.position = pos;
            // the star is the first trasform of the root
            // Debug.DrawLine(pos, TheRoot.transform.GetChild(0).transform.position, Color.white);
            Utils.Utils.AdjustLine(starLines[i], pos, TheRoot.transform.GetChild(0).transform.position);
        }
    }

    private void ClearStarLines() {
        foreach (var l in starLines) {
            Destroy(l);
        }
        starLines.Clear();
    }

    private void ToggleStarLines(bool b) {
        debugIsOn = b;
        foreach (var l in starLines) {
            l.SetActive(debugIsOn);
        }
    }

    #endregion
}
