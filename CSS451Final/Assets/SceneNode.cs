﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SceneNode : MonoBehaviour {
    bool selected = false;

    protected Matrix4x4 mCombinedParentXform;

    public Vector3 eulerAngles;
    public float yRotateSpeed = 100f;

    public float yAngle = 0f;
    public Vector3 NodeOrigin = Vector3.zero;
    public List<NodePrimitive> PrimitiveList;

    public Transform AxisFrame;
    //public Vector3 kDefaultTreeTip = new Vector3(0.19f, 12.69f, 3.88f);
    public Vector3 kDefaultTreeTip = Vector3.zero;

    public Vector3 headTip = Vector3.zero;
    
    public Transform head;
    public bool UseUnity = false;

    // Use this for initialization
    protected void Start() {
        InitializeSceneNode();
        // Debug.Log("PrimitiveList:" + PrimitiveList.Count);
    }

    private void InitializeSceneNode() {
        AxisFrame = new GameObject().transform;
        AxisFrame.name = transform.name + "AxisFrame";
        head = new GameObject().transform;
        head.name = transform.name + "head";

        mCombinedParentXform = Matrix4x4.identity;
    }

    void CalcHead() {
        Vector3 dir = AxisFrame.up;
        float distance = transform.localScale.y * 2f;
        Vector3 headTip = dir.normalized * distance + AxisFrame.position;
        head.position = headTip;
        head.rotation = Quaternion.FromToRotation(Vector3.up, AxisFrame.up);
    }

    // This must be called _BEFORE_ each draw!! 
    public void CompositeXform(ref Matrix4x4 parentXform) {
        Matrix4x4 orgT = Matrix4x4.Translate(NodeOrigin);

        // apply rotation
        yAngle = (yAngle <= 360f) ? yAngle + yRotateSpeed : 0f;
        transform.rotation = Quaternion.Euler(0f, yAngle, 0f);


        Matrix4x4 trs = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);

        mCombinedParentXform = parentXform * orgT * trs;

        // propagate to all children
        foreach (Transform child in transform) {
            SceneNode cn = child.GetComponent<SceneNode>();
            if (cn != null) {
                cn.CompositeXform(ref mCombinedParentXform);
            }
        }

        // disenminate to primitives
        foreach (NodePrimitive p in PrimitiveList) {
            p.LoadShaderMatrix(ref mCombinedParentXform);
        }


        // Compute AxisFrame
        if (AxisFrame != null) {
            AxisFrame.localPosition = mCombinedParentXform.MultiplyPoint(kDefaultTreeTip);
            
            // 
            // What is going on in the next two lines of code?
            Vector3 up = mCombinedParentXform.GetColumn(1).normalized;
            Vector3 forward = mCombinedParentXform.GetColumn(2).normalized;

            if (UseUnity) {
                AxisFrame.localRotation = Quaternion.LookRotation(forward, up);
            } else {
                // First align up direction, remember that the default AxisFrame.up is simply the y-axis
                float angle = Mathf.Acos(Vector3.Dot(Vector3.up, up)) * Mathf.Rad2Deg;
                Vector3 axis = Vector3.Cross(Vector3.up, up);
                AxisFrame.localRotation = Quaternion.AngleAxis(angle, axis);

                // Now, align the forward axis
                angle = Mathf.Acos(Vector3.Dot(AxisFrame.transform.forward, forward)) * Mathf.Rad2Deg;
                axis = Vector3.Cross(AxisFrame.transform.forward, forward);
                AxisFrame.localRotation = Quaternion.AngleAxis(angle, axis) * AxisFrame.localRotation;
            }
        }
        CalcHead();
    }
}