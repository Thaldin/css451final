using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XfromControl : MonoBehaviour {
    public MP5World theWorld = null;

    public Toggle T, R, S;
    public SliderWithEcho X, Y, Z;
    public Text ObjectName;

    [SerializeField] public Transform mSelected;
    private Vector3 mPreviousSliderValues = Vector3.one;

    planeGeneration plane;

    // Use this for initialization
    void Start() {
        T.onValueChanged.AddListener(SetToTranslation);
        R.onValueChanged.AddListener(SetToRotation);
        S.onValueChanged.AddListener(SetToScaling);
        X.SetSliderListener(XValueChanged);
        Y.SetSliderListener(YValueChanged);
        Z.SetSliderListener(ZValueChanged);

        T.isOn = true;
        R.isOn = false;
        S.isOn = false;
        SetToTranslation(true);

        SetSelectedObject(theWorld.GetCurrentSelection());

    }

    //---------------------------------------------------------------------------------
    // Initialize slider bars to specific function
    void SetToTranslation(bool v) {
        Vector3 p = ReadObjectXfrom();
        mPreviousSliderValues = p;
        X.InitSliderRange(-20, 20, p.x);
        Y.InitSliderRange(-20, 20, p.y);
        Z.InitSliderRange(-20, 20, p.z);
    }

    void SetToScaling(bool v) {
        Vector3 s = ReadObjectXfrom();
        mPreviousSliderValues = s;
        X.InitSliderRange(0.1f, 20, s.x);
        Y.InitSliderRange(0.1f, 20, s.y);
        Z.InitSliderRange(0.1f, 20, s.z);
    }

    void SetToRotation(bool v) {
        Vector3 r = ReadObjectXfrom();
        mPreviousSliderValues = r;
        X.InitSliderRange(-180, 180, r.x);
        Y.InitSliderRange(-180, 180, r.y);
        Z.InitSliderRange(-180, 180, r.z);
        mPreviousSliderValues = r;
    }
    //---------------------------------------------------------------------------------

    //---------------------------------------------------------------------------------
    // resopond to sldier bar value changes
    void XValueChanged(float v) {
        Vector3 p = ReadObjectXfrom();
        // if not in rotation, next two lines of work would be wasted
        float dx = v - mPreviousSliderValues.x;
        mPreviousSliderValues.x = v;
        Quaternion q = Quaternion.AngleAxis(dx, Vector3.right);
        p.x = v;
        UISetObjectXform(ref p, ref q);

        //MP5
        if (T.isOn) {
            theWorld.SetTileOffsetX((int)v);
        } else if (S.isOn) {
            theWorld.SetTileScaleX((int)v);
        }
        theWorld.RenderPlane();
    }

    void YValueChanged(float v) {
        Vector3 p = ReadObjectXfrom();
        // if not in rotation, next two lines of work would be wasted
        float dy = v - mPreviousSliderValues.y;
        mPreviousSliderValues.y = v;
        Quaternion q = Quaternion.AngleAxis(dy, Vector3.up);
        p.y = v;
        UISetObjectXform(ref p, ref q);
    }

    void ZValueChanged(float v) {
        Vector3 p = ReadObjectXfrom();
        // if not in rotation, next two lines of work would be wasterd
        float dz = v - mPreviousSliderValues.z;
        mPreviousSliderValues.z = v;
        Quaternion q = Quaternion.AngleAxis(dz, Vector3.forward);
        p.z = v;
        UISetObjectXform(ref p, ref q);

        //MP5
        if (T.isOn) {
            theWorld.SetTileOffsetZ((int)v);
        } else if (S.isOn) { 
            theWorld.SetTileScaleZ((int)v);
        }
        theWorld.RenderPlane();
    }
    //---------------------------------------------------------------------------------

    // new object selected
    public void SetSelectedObject(Transform xform) {
        mSelected = xform;
        mPreviousSliderValues = Vector3.zero;
        if (xform != null)
            ObjectName.text = "Selected:" + xform.name;
        else
            ObjectName.text = "Selected: none";
        ObjectSetUI();
    }

    public void ObjectSetUI() {
        Vector3 p = ReadObjectXfrom();
        X.SetSliderValue(p.x);  // do not need to call back for this comes from the object
        Y.SetSliderValue(p.y);
        Z.SetSliderValue(p.z);
    }

    // modified for MP5, texture map modification
    private Vector3 ReadObjectXfrom() {

        if (T.isOn) {
            if (mSelected != null) {
        
                if (mSelected.name == "Plane") {
                    return theWorld.planeGen.GetTileOffset();
                }
            } else {
                return Vector3.zero;
            }
        } else if (S.isOn) {
            if (mSelected != null) {
                if (mSelected.name == "Plane") {
                    return theWorld.planeGen.GetTileScale();
                }
            } else {
                return Vector3.one;
            }
        } else {
            return Vector3.zero;
        }
        return Vector3.zero;
    }

    private void UISetObjectXform(ref Vector3 p, ref Quaternion q) {
        if (mSelected == null)
            return;

        if (T.isOn) {
            mSelected.localPosition = p;
        } else if (S.isOn) {
            mSelected.localScale = p;
        } else {
            mSelected.localRotation *= q;
        }
    }
}