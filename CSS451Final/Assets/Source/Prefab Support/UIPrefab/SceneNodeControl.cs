using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneNodeControl : MonoBehaviour {
    public Dropdown TheMenu = null;
    public SceneNode TheRoot = null;
    public XfromControl XformControl = null;
    public UISelectionIndicator UISelection = null;

    //public SliderWithEcho timeScale, SystemScale;

    const string kChildSpace = "  ";
    List<Dropdown.OptionData> mSelectMenuOptions = new List<Dropdown.OptionData>();
    [SerializeField] List<Transform> mSelectedTransform = new List<Transform>();

    // current selection
    [SerializeField] Transform currentSelection = null;
    [SerializeField] PlanetInfo currentSelectionPlanetInfo = null;
    public int selectIndex = 0;

    public event Action OnSelect;

    bool hudIsOn = true;
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

        // add asteroid spawner and death star to drop down

        // set sliders
        //timeScale.SetSliderListener(TimeChange);


        // xform
        XformControl.SetSelectedObject(TheRoot.transform);

        // set first target
        SetMenuIndex(2);
    }

    // void GetChildrenNames(string blanks, Transform node) {
    void GetChildrenNames() {
        TheRoot.GetChildren(ref mSelectedTransform);
        foreach (var n in mSelectedTransform){
            mSelectMenuOptions.Add(new Dropdown.OptionData(n.name));
        }
    }

    public void SelectionChange(int index) {
        if (index != 2) {
            OnSelect?.Invoke();
        }
        Transform t = null;
        float d = 0f;
        selectIndex = index;
        // set the current planet selection
        XformControl.SetSelectedObject(mSelectedTransform[index]);
        currentSelection = mSelectedTransform[index];
        // get current planet information for UI
        if (index <= 1) {
            t = mSelectedTransform[index];
            d = 25f;
        } else { 
            // get the collider of the current planet
            t = (index == 2) ? null : mSelectedTransform[index].GetComponent<SceneNode>().GetCollider();
            d = (t) ? t.GetComponent<SphereCollider>().radius : 0f;
            SetCurrentPlanetInfo();
            currentSelection = (index == 2) ? null : currentSelection;
            UISelection.SetSelection(currentSelection, currentSelectionPlanetInfo);
        }
        ToggleFollowTarget(t, d);

        //Debug.Log("Change index to: " + index);
        /*
        // follow target
        if (selectIndex > 2) {
            UISelection.SetSelection(currentSelection, currentSelectionPlanetInfo);
            //timeScale.gameObject.SetActive(true);
           // SystemScale.gameObject.SetActive(true);
        } else {
            // edge case universe is default no follow
            UISelection.SetSelection(null, currentSelectionPlanetInfo);
            //timeScale.gameObject.SetActive(false);
            //SystemScale.gameObject.SetActive(false);
        }*/
        //ToggleFollowTarget(t, d);

        
    }

    public void SetMenuIndex(int i) {
        if (hudIsOn) { 
            TheMenu.value = i;
        
        }
    }

    public void SetCurrentPlanetInfo() {
        currentSelection.GetComponent<SceneNode>().GetPlanetInfo(ref currentSelectionPlanetInfo);
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

    public void HandleOnToggleHud() {
        hudIsOn = !hudIsOn;
        TheMenu.gameObject.SetActive(hudIsOn);
        XformControl.gameObject.SetActive(hudIsOn);
        UISelection.gameObject.SetActive(hudIsOn);
        //timeScale.gameObject.SetActive(hudIsOn);
        //SystemScale.gameObject.SetActive(hudIsOn);
    }

    public void AddToDropdownMenu(Transform t) {
        mSelectedTransform.Add(t);
    }
}
