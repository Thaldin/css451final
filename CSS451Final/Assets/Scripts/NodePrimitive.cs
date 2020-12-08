﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodePrimitive : MonoBehaviour {
    //public GameObject collider = null;

    public Color MyColor = new Color(0.1f, 0.1f, 0.2f, 1.0f);

    // rotation center
    [Header("Distance From Sun")]
    public Vector3 rotationCenter;

    // 1 = 1 Earth Day
    // how fast the object will rotate on local axis
    // 1 = 24hrs
    const float SIDEREAL_ROTATIONAL_PERIOD = 24f;
    [Header("In Hours")]
    public float planetRotation = 1f;

    // the earth = 1
    // the earth = 7,917.5 mi
    const float EMI = 7917.5f;
    [Header("In Miles")]
    public float planetDiameter = 10000f;
    float yAngle = 0f;

    private void Awake(){
        //Debug.Assert(collider != null, "Please set collider object in Editor");
    }

    public Matrix4x4 LoadShaderMatrix(ref Matrix4x4 nodeMatrix) {
        // apply local roation
        // object rotaitons
        yAngle = (yAngle <= 360f) ? yAngle + (planetRotation / SIDEREAL_ROTATIONAL_PERIOD) : 0f;
        Matrix4x4 rot = Matrix4x4.Rotate(Quaternion.Euler(0f, yAngle, 0f));
        // apply local scale
        // object diameter
        Matrix4x4 scale = Matrix4x4.Scale(new Vector3(planetDiameter / EMI, planetDiameter / EMI, planetDiameter / EMI));
        // apply pivot
        // object distance from center of system
        Matrix4x4 orgT = Matrix4x4.Translate(rotationCenter);

        /* TODO: legacy vertify delete
        //Matrix4x4 p = Matrix4x4.TRS(rotationCenter, Quaternion.identity, Vector3.one);
        //Matrix4x4 invp = Matrix4x4.TRS(-rotationCenter, Quaternion.identity, Vector3.one);
        */

        // apply trs of obj
        Matrix4x4 trs = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);
        // combine trs/ rotation/ sclae/ pivot to matrix
        Matrix4x4 m = nodeMatrix * orgT * trs * rot * scale;

        // send to shader
        GetComponent<MeshRenderer>().material.SetMatrix("MyXformMat", m);
        GetComponent<MeshRenderer>().material.SetColor("MyColor", MyColor);

        // return matrix to theWorld
        return m;
    }
}