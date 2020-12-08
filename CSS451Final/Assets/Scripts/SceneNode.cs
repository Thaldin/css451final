﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SceneNode : MonoBehaviour {
    public float timeScale = 10f;

    // 1 = 365 days
    float OP_STD = 3650f;
    [Header("In Days")]
    public float orbitalPeriod = 1f;
    public float yAngle = 0f;

    protected Matrix4x4 mCombinedParentXform;

    // this controls the object center rotation
    // a moon should match the pivot of the parent Node Primitve
    public Vector3 planetOrigin = Vector3.zero;

    // 
    public List<NodePrimitive> PrimitiveList;

    // TODO: legacy code. queue for delete
    // public Transform AxisFrame;
    // public Vector3 kDefaultTreeTip = new Vector3(0.19f, 12.69f, 3.88f);
    // public Vector3 kDefaultTreeTip = Vector3.zero;
    // public bool UseUnity = false;

    // Use this for initialization
    protected void Start() {
        InitializeSceneNode();
        // Debug.Log("PrimitiveList:" + PrimitiveList.Count);
        //orbitalPeriod /= 3650f;

    }

    private void InitializeSceneNode() {
        // TODO: legacy code. queue for delete
        // AxisFrame = new GameObject().transform;
        // AxisFrame.name = transform.name + "AxisFrame";

        mCombinedParentXform = Matrix4x4.identity;
        //OP_STD = 365f * timeScale;
    }

    public void GetChildren(ref List<Transform> sceneObjects) {
        foreach (Transform child in transform) {
            SceneNode cn = child.GetComponent<SceneNode>();
            if (cn != null) {
                cn.GetChildren(ref sceneObjects);
                sceneObjects.Add(child);
            }
        }
    }

    public float GetPlanetDiameter() {
        return PrimitiveList[0].GetComponent<NodePrimitive>().GetPlanetDiameter();
    }

    // This must be called _BEFORE_ each draw!! 
    public void CompositeXform(ref Matrix4x4 parentXform, ref List<Matrix4x4> sceneObjs) {

        float v = (orbitalPeriod == 0f) ? 0f : (OP_STD / orbitalPeriod);
        yAngle = (yAngle <= 360f) ? yAngle + v * Time.deltaTime: 0f;
        Matrix4x4 rot = Matrix4x4.Rotate(Quaternion.Euler(0f, yAngle, 0f));

        Matrix4x4 orgT = Matrix4x4.Translate(planetOrigin);
        Matrix4x4 trs = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);

        mCombinedParentXform = parentXform * orgT * trs * rot;

        // propagate to all children
        foreach (Transform child in transform) {
            SceneNode cn = child.GetComponent<SceneNode>();
            if (cn != null) {
                cn.CompositeXform(ref mCombinedParentXform, ref sceneObjs);
            }
        }

        // disenminate to primitives
        foreach (NodePrimitive p in PrimitiveList) {
            sceneObjs.Add(p.LoadShaderMatrix(ref mCombinedParentXform));
            //p.LoadShaderMatrix(ref mCombinedParentXform);
        }
    }
}
