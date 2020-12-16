using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public partial class SceneNode : MonoBehaviour {


    [Header("Ring Prefab")]
    public GameObject orbitRing = null;
    bool ringUpdate = false;
    MeshFilter rmf;

    //debug
    public Vector3 forward = Vector3.zero;
    public Vector3 up = Vector3.zero;
    // components

    void SetRing() {
        orbitRing = Instantiate<GameObject>(orbitRing);
        orbitRing.name = transform.name + "Ring";
        rmf = orbitRing.GetComponent<MeshFilter>();
        //float rignDistance = distanceFromSun;

        float rignDistance = 0f;

        switch (transform.tag) {
            case "star":
                goto default;
            case "planet":
                goto default;
            case "moon":
                rignDistance = Mathf.Sqrt(offsetFromPlanet);
                
                break;
            case "ring":
                rignDistance = offsetFromPlanet;
                break;
            default:
                rignDistance = distanceFromSun;
                break;
        }
        // some planets have a negative distance to help offset them.  pure render hacks
        // normalize the distance
        rignDistance = (distanceFromSun < 0) ? -distanceFromSun : rignDistance;

        float disringInnerRadius = ringInnerRadius;
        // check for negative
        ringInnerRadius = (ringInnerRadius < 0) ? 0 : ringInnerRadius;
        // clamp size to 2.5f
        disringInnerRadius = (rignDistance >= 250f) ? disringInnerRadius : 2.5f;
        disringInnerRadius = (rignDistance <= 250f) ? disringInnerRadius : 1f;
        disringInnerRadius = (rignDistance <= 100f) ? 0.02f : disringInnerRadius; 
        rmf.mesh = Utils.Utils.CreateTorus(rignDistance);
        // if moon adjust for tilt
    }

    void UpdateRing() {
        //float dist = Vector3.Distance(colliderObj.transform.position, transform.parent.GetComponent<SceneNode>().colliderObj.transform.position);
        float dist = planetInfo.distanceFromSun;
        //rmf.mesh.Clear();
        //rmf.mesh = Utils.Utils.CreateTorus(dist,0.02f);
        // Extract new local rotation
        if (transform.CompareTag("ring")) { 
            forward = mCombinedParentXform.GetColumn(2);
            up = mCombinedParentXform.GetColumn(1);
            Quaternion rotation = Quaternion.LookRotation(forward,up);
            orbitRing.transform.rotation = rotation;
        }
        //orbitRing.transform.position = transform.parent.GetComponent<SceneNode>().colliderObj.transform.position;
        orbitRing.transform.position = Utils.Utils.Matrix4x4ToWorldPostion(ref transform.GetComponent<SceneNode>().mCombinedParentXform);

    }

    public List<string> GetPlanetInfo() {
        List<string> l = new List<string>();
        l.Add(name);
        l.Add(distanceFromSun.ToString());
        l.Add(orbitalPeriod.ToString());
        l.Add(planetDiameter.ToString());
        l.Add(rotationPeriod.ToString());
        if (transform.tag != "moon") { 
            l.Add(transform.childCount.ToString());
        }
        return l;
    }

    #region Runtime Set Functions
    // gets the node diamter
    public float GetPlanetDiameter() {
        if (np != null) {
            return np.GetComponent<NodePrimitive>().GetPlanetDiameter();
        }
        return 0f;
    }

    // set global time scale
    public void SetTimeScale(float v) {
        foreach (Transform child in transform) {
            SceneNode sn = child.GetComponent<SceneNode>();
            if (sn != null) {
                // init child
                sn.np.SetTimeScale(v);
            }
        }
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

    public void GetPlanetInfo(ref PlanetInfo _planetInfo) {
        _planetInfo = planetInfo;
        return;
    }

    // get collider object
    public Transform GetCollider() {
        return colliderObj.transform;
    }
    #endregion
}
