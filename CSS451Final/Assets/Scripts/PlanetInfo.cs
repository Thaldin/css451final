using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// READ ONLY 
/// This class just holds info for each planet so that it can be easily retrieved.
/// Eventually, this class could be used to read info from a JSON or data file to
/// complete generate planets procedurally.  Out side the scope of this project.
/// </summary>

public class PlanetInfo {
    public PlanetInfo(Texture2D _mainTex = default, float _planetDiameter = 0f,
                      float _distanceFromSun = 0f, float _orbitalPeriod = 0f,
                      float _rotationPeriod = 0f, float _axisTilt = 0f,
                      float _offsetFromPlanet = 0f, float _ringInnerRadius = 0f, int _moonCount = 0, int _ringCount = 0) {
        mTex = _mainTex;
        pDiameter = _distanceFromSun;
        dFromSun = _distanceFromSun;
        oPeriod = _orbitalPeriod;
        rPeriod = _rotationPeriod;
        aTilt = _axisTilt;
        oFromPlanet = _offsetFromPlanet;
        rInnerRadius = _ringInnerRadius;
        pOrigin = new Vector3(oFromPlanet, 0f, 0f);

    }
    private Texture2D mTex;
    public Texture2D mainTexture { get { return mTex; } }
    private float pDiameter;
    public float planetDiameter { get { return pDiameter; } }
    private float dFromSun = 0f;
    public float distanceFromSun { get { return dFromSun; } }
    private float oPeriod = 0f;
    public float orbitalPeriod { get { return oPeriod; } }
    private float rPeriod = 0f;
    public float rotationPeriod { get { return rPeriod; } }
    private float aTilt = 0f;
    public float axisTilt { get { return aTilt; } }
    private float oFromPlanet = 0f;
    private float rInnerRadius = 0f;
    public float ringInnerRadius { get { return rInnerRadius; } }
    private Vector3 pOrigin = Vector3.zero;
    public Vector3 planetorigin { get { return pOrigin; } }
    private int mCount = 0;
    public float moonCount { get { return mCount; } }
    private int rCount = 0;
    public float ringCount { get { return rCount; } }

}
