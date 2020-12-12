﻿using System.Collections;
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

    public Transform GetSceneObjectFromIndex(int i) {
        return sceneObjects[i].transform;
    }

    public List<Transform> GetSceneObjectsNames() {
        return sceneObjects;
    }

    public List<Matrix4x4> GetM4x4() {
        return m4x4s;
    }

    #region Runtime Set Functions
    // set global time scale
    public void SetTimeScale(float v) {
        foreach (var sn in sceneObjects) {
            sn.GetComponent<SceneNode>().SetTimeScale(v);
        }
    }

    // Set System Scale
    public void SetSystemScale(float v)
    {
        Debug.Log("Setting System Scale: " + v);
    }
    #endregion

}
