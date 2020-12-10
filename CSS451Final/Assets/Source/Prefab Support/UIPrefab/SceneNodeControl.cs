using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneNodeControl : MonoBehaviour {
    public Dropdown TheMenu = null;
    public SceneNode TheRoot = null;
    public XfromControl XformControl = null;
    public UISelectionIndicator UISelection = null;

    const string kChildSpace = "  ";
    List<Dropdown.OptionData> mSelectMenuOptions = new List<Dropdown.OptionData>();
    [SerializeField] List<Transform> mSelectedTransform = new List<Transform>();

    // current selection
    [SerializeField] Transform currentSelection = null;

    // Use this for initialization
    void Start() {
        Debug.Assert(TheMenu != null, "Please set " + TheMenu + " for " + name + " in the Editor");
        Debug.Assert(TheRoot != null, "Please set " + TheRoot + " for " + name + " in the Editor");
        Debug.Assert(XformControl != null, "Please set " + XformControl + " for " + name + " in the Editor");

        TheMenu.options.Clear();
        mSelectedTransform.Add(TheRoot.transform);
        GetChildrenNames();
        TheMenu.AddOptions(mSelectMenuOptions);
        TheMenu.onValueChanged.AddListener(SelectionChange);

        XformControl.SetSelectedObject(TheRoot.transform);
        SelectionChange(0);
    }
    // void GetChildrenNames(string blanks, Transform node) {
    void GetChildrenNames() {
        /*
        for (int i = 0; i < node.childCount; i++) {
            Transform child = node.GetChild(i);
            SceneNode cn = child.GetComponent<SceneNode>();
            if (cn != null) {
                mSelectMenuOptions.Add(new Dropdown.OptionData(space + child.name));
                mSelectedTransform.Add(child);
                GetChildrenNames(blanks + kChildSpace, child);
            }
        }
        */
        TheRoot.GetChildren(ref mSelectedTransform);
        foreach (var n in mSelectedTransform){
            mSelectMenuOptions.Add(new Dropdown.OptionData(n.name));
        }
    }

    public void SelectionChange(int index) {
        XformControl.SetSelectedObject(mSelectedTransform[index]);
        currentSelection = mSelectedTransform[index];
        // edge case universe is default no follow
        Transform t = (index < 1) ? null : mSelectedTransform[index].GetComponent<SceneNode>().GetCollider();
        float d = (t) ? t.GetComponent<SphereCollider>().radius : 0f;
        Debug.Log("Change index to: " + index);
        if (index < 0) {

            UISelection.SetSelection(currentSelection);
        }
        
        // follow target
        ToggleFollowTarget(t, d);
    }

    public void SetMenuIndex(int i) {
        TheMenu.value = i;
    }

    public void ToggleFollowTarget(Transform t = null, float d = 0f) {
        Camera.main.GetComponent<CameraFollow>().ToggleFollowTarget(t, d);
    }

    public void ResetMainCamera() {
        Camera.main.GetComponent<CameraFollow>().ResetPosition();
    }

    public void SetCurrentSelection(Transform s) {
        currentSelection = s;
    }
}
