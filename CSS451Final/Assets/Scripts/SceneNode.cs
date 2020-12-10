using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public partial class SceneNode : MonoBehaviour {
    //private float timeScale = 10f;

    // https://www.solarsystemscope.com/textures/
    public Texture2D mainTex = null;
    public GameObject colliderObj;

    [Header("km")]
    public float planetDiameter = 0f;

    [Header("10^6 km")]
    public float distanceFromSun = 0f;

    // 1 = 365 days
    private float OP_STD = 3650f;
    [SerializeField] float yAngle = 0f;
    [Header("In Days")]
    public float orbitalPeriod = 1f;

    [Header("In Hours")]
    public float rotationPeriod = 1f;

    [Header("Axis Offset")]
    public Vector3 axisRotationOffset = Vector3.zero;
    [SerializeField] float offsetFromAxis = 0f;

    [Header("Moon Only")]
    public float offsetFromPlanet = 0f;

    // M4x4 for shader movement
    protected Matrix4x4 mCombinedParentXform;

    // this controls the object center rotation
    // a moon should match the pivot of the parent Node Primitve
    public Vector3 planetOrigin = Vector3.zero;

    // 
    protected List<NodePrimitive> PrimitiveList;
    public NodePrimitive np = null;
    // Use this for initialization
    protected void Awake() {
        Debug.Assert(mainTex != null, "Please set main texture for " + name + " in Editor");
        Debug.Assert(np != null, "Please set the Node Primitive for " + name + " in the Editor.");
        Debug.Assert(orbitRing != null, "Please set the Orbit Ring prefab for " + name + " in the Editor.");
        mCombinedParentXform = Matrix4x4.identity;

        //colliderObj = Instantiate<GameObject>(colliderObj) as GameObject;
        colliderObj.name +=  "Collider";

        // SNProperties
        rmf = orbitRing.GetComponent<MeshFilter>();
    }

    // init the node
    public void InitializeSceneNode(ref List<GameObject> rings, ref List<GameObject> colliders) {
        // propagate to all children
        foreach (Transform child in transform) {
            SceneNode sn = child.GetComponent<SceneNode>();
            if (sn != null) {
                // init child
                sn.InitializeSceneNode(ref rings, ref colliders);
            }
        }
        // init this
        np.Initiallize(mainTex, planetDiameter, distanceFromSun, rotationPeriod, offsetFromPlanet);
        SetRing();
        rings.Add(orbitRing);

        float d = np.GetPlanetDiameter();
        colliderObj.GetComponent<sphereColliderScript>().Initialize(d);
        colliders.Add(colliderObj);
    }


    // send child transform to world controller
    public void GetChildren(ref List<Transform> sceneObjects) {
        foreach (Transform child in transform) {
            SceneNode cn = child.GetComponent<SceneNode>();
            if (cn != null) {
                cn.GetChildren(ref sceneObjects);
                sceneObjects.Add(child);
            }
        }
    }

    

    // If planets are all rotating together, check the orbital period of the universe and 
    // make sure it is set to 0
    // This must be called _BEFORE_ each draw!! 
    public void CompositeXform(ref Matrix4x4 parentXform, ref List<Matrix4x4> sceneM4x4) {
        if (np != null) {
            // check for 0
            float v = (orbitalPeriod == 0f) ? 0f : (OP_STD / orbitalPeriod); // * timeScale
            yAngle = (yAngle <= 360f) ? yAngle + v  * Time.deltaTime : 0f;

            // does not work right
            float inc = (yAngle / 180f) * Mathf.PI;
            offsetFromAxis += Mathf.Cos(inc) * Time.deltaTime * axisRotationOffset.y;

            transform.eulerAngles = axisRotationOffset;

            //Matrix4x4 offsetT = Matrix4x4.Translate(new Vector3(0f, offsetFromAxis, 0f)); ;
            Matrix4x4 rot = Matrix4x4.Rotate(Quaternion.Euler(0f, yAngle, 0f));

            Matrix4x4 orgT = Matrix4x4.Translate(planetOrigin);
            Matrix4x4 trs = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);

            mCombinedParentXform = parentXform * orgT * trs * rot;

            // propagate to all children
            foreach (Transform child in transform) {
                SceneNode cn = child.GetComponent<SceneNode>();
                if (cn != null) {
                    cn.CompositeXform(ref mCombinedParentXform, ref sceneM4x4);
                }
            }

            // disenminate to primitives
            sceneM4x4.Add(np.LoadShaderMatrix(ref mCombinedParentXform));
        }
    }

    
}
