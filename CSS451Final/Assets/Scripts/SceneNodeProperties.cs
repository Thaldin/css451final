using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public partial class SceneNode : MonoBehaviour {

    public GameObject orbitRing = null;
    MeshFilter rmf;

    void SetRing() {
        orbitRing = Instantiate(orbitRing);
        Mesh mesh;
        orbitRing.GetComponent<TorusWire>().Torus(out mesh, distanceFromSun);
        rmf.mesh = mesh;
    }

    void UpdateRing() {
        rmf.mesh.Clear();
        SetRing();

        orbitRing.transform.rotation = transform.rotation;
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
        np.SetTimeScale(v);
    }
    #endregion
}
