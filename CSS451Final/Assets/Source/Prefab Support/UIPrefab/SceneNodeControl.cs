using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneNodeControl : MonoBehaviour {
    public Dropdown TheMenu = null;
    public SceneNode TheRoot = null;
    public XfromControl XformControl = null;
    public UISelectionIndicator UISelection = null;

    public SliderWithEcho timeScale, SystemScale;

    const string kChildSpace = "  ";
    List<Dropdown.OptionData> mSelectMenuOptions = new List<Dropdown.OptionData>();
    [SerializeField] List<Transform> mSelectedTransform = new List<Transform>();

    // current selection
    [SerializeField] Transform currentSelection = null;
    public int selectIndex = 0;
    // Use this for initialization
    void Start() {
        Debug.Assert(TheMenu != null, "Please set " + TheMenu + " for " + name + " in the Editor");
        Debug.Assert(TheRoot != null, "Please set " + TheRoot + " for " + name + " in the Editor");
        Debug.Assert(XformControl != null, "Please set " + XformControl + " for " + name + " in the Editor");

        // dropdown menu
        TheMenu.options.Clear();
        mSelectedTransform.Add(TheRoot.transform);
        GetChildrenNames();
        TheMenu.AddOptions(mSelectMenuOptions);
        TheMenu.onValueChanged.AddListener(SelectionChange);

        // set sliders
        timeScale.SetSliderListener(TimeChange);
        

        // xform
        XformControl.SetSelectedObject(TheRoot.transform);

        // set first target
        SelectionChange(0);
    }

    // void GetChildrenNames(string blanks, Transform node) {
    void GetChildrenNames() {
        TheRoot.GetChildren(ref mSelectedTransform);
        foreach (var n in mSelectedTransform){
            mSelectMenuOptions.Add(new Dropdown.OptionData(n.name));
        }
    }

    public void SelectionChange(int index) {
        selectIndex = index;
        XformControl.SetSelectedObject(mSelectedTransform[index]);
        currentSelection = mSelectedTransform[index];
        Transform t = (index < 1) ? null : mSelectedTransform[index].GetComponent<SceneNode>().GetCollider();
        float d = (t) ? t.GetComponent<SphereCollider>().radius : 0f;

        Debug.Log("Change index to: " + index);

        // follow target
        if (selectIndex > 0) {
            //UISelection.SetSelection(currentSelection);
        } else { 
            // edge case universe is default no follow
            //UISelection.SetSelection(null);
        }
        ToggleFollowTarget(t, d);
    }

    public void SetMenuIndex(int i) {
        TheMenu.value = i;
    }

    public void ToggleFollowTarget(Transform t = null, float d = 0f) {
        //Camera.main.GetComponent<CameraFollow>().ToggleFollowTarget(t, d);
        CameraFollow miniCam = Camera.FindObjectOfType<CameraFollow>();
        miniCam.ToggleFollowTarget(t, d);
        
    }

    public void ResetMainCamera() {
        // set to star
        int i = mSelectedTransform.Count - 1;
        SetMenuIndex(i);
    }

    public void TimeChange(float v) {
        TheRoot.SetTimeScale(v);
    }
}
