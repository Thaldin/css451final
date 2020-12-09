using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TheWorld : MonoBehaviour
{
    
    public void ToggleDebug(bool b) {
        ToggleStarLines(b);
    }

    public void ToggleRings(bool b) {
        ringIsOn = b;
        foreach (var r in ringObjects) {
            r.SetActive(ringIsOn);
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
