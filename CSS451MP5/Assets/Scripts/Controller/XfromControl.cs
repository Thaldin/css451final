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

    private bool onToggle = false;

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

    private void LateUpdate() {
    }

    //---------------------------------------------------------------------------------
    // Initialize slider bars to specific function
    void SetToTranslation(bool v) {
        onToggle = true;
        Vector3 p = ReadObjectXfrom();
        mPreviousSliderValues = p;
        X.InitSliderRange(-10, 10, p.x);
        Y.InitSliderRange(-10, 10, p.y);
        Z.InitSliderRange(-10, 10, p.z);
        onToggle = false;

    }

    void SetToScaling(bool v) {
        onToggle = true;
        Vector3 s = ReadObjectXfrom();
        mPreviousSliderValues = s;
        X.InitSliderRange(1f, 20, s.x);
        Y.InitSliderRange(1f, 20, s.y);
        Z.InitSliderRange(1f, 20, s.z);
        onToggle = false;

    }

    void SetToRotation(bool v) {
        onToggle = true;
        Vector3 r = ReadObjectXfrom();
        mPreviousSliderValues = r;
        X.InitSliderRange(-180, 180, r.x);
        Y.InitSliderRange(-180, 180, r.y);
        Z.InitSliderRange(-180, 180, r.z);
        mPreviousSliderValues = r;

        onToggle = false;
    }
    //---------------------------------------------------------------------------------

    //---------------------------------------------------------------------------------
    // resopond to sldier bar value changes
    void XValueChanged(float v) {
        if (!onToggle) {
            Vector3 p = ReadObjectXfrom();
            // if not in rotation, next two lines of work would be wasted
            //float dx = v - mPreviousSliderValues.x;
            //mPreviousSliderValues.x = v;
            //Quaternion q = Quaternion.AngleAxis(dx, Vector3.right);
            p.x = v;
            //UISetObjectXform(ref p, ref q);

            //MP5
            if (T.isOn) {
                theWorld.SetUVOffset(p);
            } else if (S.isOn) {
                theWorld.SetTileOffset(p);
            }
            theWorld.RenderPlane();
        }
    }

    void YValueChanged(float v) {
        if (!onToggle) {
            Vector3 p = ReadObjectXfrom();
            // if not in rotation, next two lines of work would be wasted
            //float dy = v - mPreviousSliderValues.y;
            //mPreviousSliderValues.y = v;
            //Quaternion q = Quaternion.AngleAxis(dy, Vector3.up);
            p.y = v;
            //UISetObjectXform(ref p, ref q);

            //MP5
            if (T.isOn) {
                theWorld.SetTileOffsetY(p);
            } else if (S.isOn) {
                theWorld.SetTileOffset(p);
            }
            theWorld.RenderPlane();
        }
    }

    void ZValueChanged(float v) {
        if (!onToggle) {
            Vector3 p = ReadObjectXfrom();
            // if not in rotation, next two lines of work would be wasterd
            float dz = v - mPreviousSliderValues.z;
            mPreviousSliderValues.z = v;
            Quaternion q = Quaternion.AngleAxis(dz, Vector3.forward);
            p.z = v;
            UISetObjectXform(ref p, ref q);

            //MP5
            if (R.isOn) {
                theWorld.SetUVRotation(v);
            }
            theWorld.RenderPlane();
        }
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
        Vector3 p = Vector3.zero;
        if (T.isOn) {
            if (mSelected != null) {

                if (mSelected.name == "Plane") {
                    p = theWorld.GetUVOffset();
                }
            } else {
                p = Vector3.zero;
            }
        } else if (S.isOn) {
            if (mSelected != null) {
                if (mSelected.name == "Plane") {
                    p = theWorld.GetUVTile();
                }
            } else {
                p = Vector3.one;
            }
        } else {
            if (mSelected != null) {
                if (mSelected.name == "Plane") {
                    float uvRot = theWorld.GetUVRotation();
                    p = new Vector3(0f, 0f, uvRot);
                }
            } else {
                p = Vector3.one;
            }
        }
        return p;
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