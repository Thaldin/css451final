using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class NodePrimitive : MonoBehaviour {


    [SerializeField] float systemScale = 1f;
    [SerializeField] float timeScale = 1f;

    // properties
    [SerializeField] float distanceFromSun = 0f;
    // rotation center
    float moonOffset = 0f;
    [SerializeField] Vector3 offsetFromPlanet;

    // 1 = 1 Earth Day
    // how fast the object will rotate on local axis
    // 1 = 24hrs
    const float SIDEREAL_ROTATIONAL_PERIOD = 24f;
    [SerializeField] float planetRotation = 24f;

    // the earth = 1
    // the earth = 12,756km
    const float EMI = 12756f;
    [SerializeField] float planetDiameter = 0;
    [SerializeField] private float pd = 0f;
    [SerializeField] float yAngle = 0f;

    [SerializeField] float ringInnerRadius = 0f;

    // components
    MeshFilter mf;
    Renderer mr;

    [SerializeField] Texture2D mainText;

    private void Awake() {
        mf = GetComponent<MeshFilter>();
        mr = GetComponent<Renderer>();
        pd = planetDiameter / EMI;
        mr.allowOcclusionWhenDynamic = true;
    }


    // initialize primitive

    public void Initiallize(Texture2D _mainTex = default, float _planetDiameter = 1000f, float _distanceFromSun = 10f, float _planetRotation = 24f, float _offsetFromPlanet = 0f, float _ringInnerRadius = 0f) {
        mainText = _mainTex;
        planetDiameter = _planetDiameter;
        distanceFromSun = _distanceFromSun;
        planetRotation = _planetRotation;
        ringInnerRadius = _ringInnerRadius;
        moonOffset = _offsetFromPlanet;
        offsetFromPlanet = new Vector3(moonOffset, 0f, 0f);

        // create mesh
        mf.mesh.Clear();
        // 20 for default slices and stacks
        Mesh mesh;
        switch (transform.tag) {
            case "star":
                goto default;
            case "planet":
                goto default;
            case "moon":
                goto default;
            case "ring":
                mesh = Utils.Utils.CreateTorus(planetDiameter / EMI, ringInnerRadius / EMI);
                break;
            default:
                mesh = Utils.Utils.CreateSphereMesh(planetDiameter / EMI);
                break;
        }
        
        mf.mesh = mesh;
    }

    public float GetPlanetDiameter() {
        return planetDiameter / EMI;
    }
    public Matrix4x4 LoadShaderMatrix(ref Matrix4x4 nodeMatrix) {

        // apply local roation
        // object rotaitons
        yAngle = (yAngle <= 360f) ? yAngle + (SIDEREAL_ROTATIONAL_PERIOD / planetRotation) * timeScale: 0f;
        //(0f, planet rotation, rotation axis)
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
        pd = planetDiameter / EMI;

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