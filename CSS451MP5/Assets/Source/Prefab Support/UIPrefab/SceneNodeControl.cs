using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneNodeControl : MonoBehaviour {
    //public Dropdown TheMenu = null;
    //public SceneNode TheRoot = null;
    public XfromControl XformControl = null;

    const string kChildSpace = "  ";
    List<Dropdown.OptionData> mSelectMenuOptions = new List<Dropdown.OptionData>();
    List<Transform> mSelectedTransform = new List<Transform>();    

    // Use this for initialization
    void Start () {
       //Debug.Assert(TheMenu != null);
       //Debug.Assert(TheRoot != null);
        Debug.Assert(XformControl != null);

        //mSelectMenuOptions.Add(new Dropdown.OptionData(TheRoot.transform.name));
        //mSelectedTransform.Add(TheRoot.transform);
        //GetChildrenNames("", TheRoot.transform);
        //TheMenu.AddOptions(mSelectMenuOptions);
        //TheMenu.onValueChanged.AddListener(SelectionChange);

        //XformControl.SetSelectedObject(TheRoot.transform);
    }
}
