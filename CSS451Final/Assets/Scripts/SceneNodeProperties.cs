using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public partial class SceneNode : MonoBehaviour {

    [Header("Ring Prefab")]
    public GameObject orbitRing = null;

    // components
    MeshFilter rmf;

    void SetRing() {
        orbitRing = Instantiate(orbitRing);
        orbitRing.name = transform.name + "Ring";

        float rignDistance = distanceFromSun;
        // some planets have a negative distance to help offset them.  pure render hacks
        // normalize the distance
        rignDistance = (distanceFromSun < 0) ? -distanceFromSun : rignDistance;

        // check for negative
        ringInnerRadius = (ringInnerRadius < 0) ? 0 : ringInnerRadius;
        // clamp size to 2.5f
        ringInnerRadius = (rignDistance >= 250f) ? ringInnerRadius : 2.5f;
        ringInnerRadius = (rignDistance <= 250f) ? ringInnerRadius : 1f;
        ringInnerRadius = (rignDistance <= 100f) ? 0.05f : ringInnerRadius;


        rmf.mesh = Utils.Utils.CreateTorus(rignDistance);
    }

    void UpdateRing() {
        rmf.mesh.Clear();
        SetRing();

        orbitRing.transform.rotation = transform.rotation;
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

    // get collider object
    public Transform GetCollider() {
        return colliderObj.transform;
    }
    #endregion
}
