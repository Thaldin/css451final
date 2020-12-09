using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class NodePrimitive : MonoBehaviour {

    //public Color MyColor = new Color(0.1f, 0.1f, 0.2f, 1.0f);

    float systemScale = 1f;
    // properties
    [SerializeField] float distanceFromSun = 10;
    // rotation center
    float planetOffset = 0f;
    [SerializeField] Vector3 offsetFromPlanet;

    float yPos = 0f;

    // 1 = 1 Earth Day
    // how fast the object will rotate on local axis
    // 1 = 24hrs
    const float SIDEREAL_ROTATIONAL_PERIOD = 24f;
    [SerializeField] float planetRotation = 1f;
    float timeScale = 1f;

    // the earth = 1
    // the earth = 7,917.5 mi
    const float EMI = 7917.5f;
    [SerializeField] float planetDiameter = 10000f;
    [SerializeField] float pd; // debug
    [SerializeField] float yAngle = 0f;

    // components
    MeshFilter mf;
    Renderer mr;

    [SerializeField] Texture2D mainText;

    private void Awake() {
        mf = GetComponent<MeshFilter>();
        mr = GetComponent<Renderer>();
        pd = planetDiameter / EMI;
        //mr.material = (Material)Instantiate(mr.material);
    }


    // initialize primitive
    public void Initiallize(Texture2D mt = default, float d = 1000f, float dfs = 10f, float srp = 24f, float ofp = 0f) {

        //Debug.Log("Initalizing " + name);
        // set params
        mainText = mt;
        planetDiameter = d;
        distanceFromSun = dfs;
        planetRotation = srp;
        planetOffset = ofp;
        offsetFromPlanet = new Vector3(planetOffset, 0f, 0f);
        // create mesh
        mf.mesh.Clear();
        // 20 for default slices and stacks
        Mesh mesh = Utils.Utils.CreateSphereMesh(planetDiameter / EMI);
        mf.mesh = mesh;
    }

    public float GetPlanetDiameter() {
        return pd;
    }
    public Matrix4x4 LoadShaderMatrix(ref Matrix4x4 nodeMatrix) {
        
        // apply local roation
        // object rotaitons
        yAngle = (yAngle <= 360f) ? yAngle + (planetRotation / SIDEREAL_ROTATIONAL_PERIOD) * timeScale : 0f;
        Matrix4x4 rot = Matrix4x4.Rotate(Quaternion.Euler(0f, yAngle, 0f));
        // apply local scale
        // object diameter
        Matrix4x4 scale = Matrix4x4.Scale(new Vector3(planetDiameter / EMI, planetDiameter / EMI, planetDiameter / EMI));
        // apply pivot
        // object distance from center of system
        Vector3 pos = new Vector3(distanceFromSun / systemScale, 0f, 0f);
        Matrix4x4 orgT = Matrix4x4.Translate(pos);

        
        Matrix4x4 offsetT = Matrix4x4.Translate(offsetFromPlanet);

        // apply trs of obj
        Matrix4x4 trs = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);
        // combine trs/ rotation/ sclae/ pivot to matrix
        Matrix4x4 m = nodeMatrix * orgT * trs * rot * scale * offsetT;
        // send to shader
        GetComponent<MeshRenderer>().material.SetMatrix("MyXformMat", m);
        //GetComponent<MeshRenderer>().material.SetColor("MyColor", MyColor);
        GetComponent<MeshRenderer>().material.SetTexture("_MainTex", mainText);
        // return matrix to theWorld

        Vector3 s = new Vector3(m.GetColumn(0).magnitude, m.GetColumn(2).magnitude, m.GetColumn(2).magnitude);
        pd = s.x;

        // return 4x4
        return m;

    }

    #region Runtime Set Functions
    // set global time scale
    public void SetTimeScale(float v) {
        timeScale = v;
    }

    public void SetSystemScale(float v) {
        systemScale = v;
    }
    #endregion
}