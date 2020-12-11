using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PlanetInfo {
    [Header("km")]
    public float planetDiameter = 0f;
    [Header("10^6 km")]
    public float distanceFromSun = 0f;
    [Header("In Days")]
    public float orbitalPeriod = 0f;
    [Header("In Hours")]
    public float rotationPeriod = 0f;
    [Header("In Degrees")]
    public float axisTilt = 0f;
    [Header("Moon Only")]
    public float offsetFromPlanet = 0f;
    [Header("Ring Only")]
    public float ringInnerRadius = 0f;
    [Header("Object Center Rotation")]
    public Vector3 planetOrigin = Vector3.zero;
}

public partial class SceneNode : MonoBehaviour {
    [SerializeField] public PlanetInfo planetInfo;


    [SerializeField] float timeScale = 1f;

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

    [Header("In Degrees")]
    public float axisTilt = 0f;

    [Header("Moon Only")]
    public float offsetFromPlanet = 0f;

    [Header("Ring Only")]
    public float ringInnerRadius = 0f;

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
    public void InitializeSceneNode(ref List<GameObject> displayRings, ref List<GameObject> colliders) {
        // propagate to all children
        foreach (Transform child in transform) {
            SceneNode sn = child.GetComponent<SceneNode>();
            if (sn != null) {
                // init child
                sn.InitializeSceneNode(ref displayRings, ref colliders);
            }
        }
        // init this
        //np.Initiallize(planetInfo, mainTex);
        np.Initiallize(mainTex, planetDiameter, distanceFromSun, rotationPeriod, offsetFromPlanet, ringInnerRadius);
        SetRing();
        displayRings.Add(orbitRing);

        float d = np.GetPlanetDiameter();
        colliderObj.GetComponent<sphereColliderScript>().Initialize(d);
        colliders.Add(colliderObj);
    }

    // If planets are all rotating together, check the orbital period of the universe and 
    // make sure it is set to 0
    // This must be called _BEFORE_ each draw!! 
    public void CompositeXform(ref Matrix4x4 parentXform, ref List<Matrix4x4> sceneM4x4) {
        if (np != null) {
            // check for 0
            float v = (orbitalPeriod == 0f) ? 0f : (OP_STD / orbitalPeriod); // * timeScale
            yAngle = (yAngle <= 360f) ? yAngle + v * Time.deltaTime : 0f;

            /*
            float v = (orbitalPeriod == 0f) ? 0f : (OP_STD / orbitalPeriod); // * timeScale
            yAngle = (yAngle <= 360f) ? yAngle + v  * Time.deltaTime : 0f;
            */

            //Matrix4x4 offsetT = Matrix4x4.Translate(new Vector3(0f, offsetFromAxis, 0f)); ;
            Matrix4x4 rotOffset = Matrix4x4.Rotate(Quaternion.Euler(axisTilt, yAngle, 0f));
            Matrix4x4 posOffset = Matrix4x4.Translate(planetOrigin);
            Matrix4x4 trs = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);

            mCombinedParentXform = parentXform * posOffset * rotOffset * trs;

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

    #region Get and Set Functions
    
    #endregion

}
