using System; // for assert
using UnityEngine;
using UnityEngine.UI; // for GUI elements: Button, Toggle

public partial class MainController : MonoBehaviour {

    // reference to all UI elements in the Canvas
    public Camera MainCamera = null;
    public Camera MiniCamera = null;
    public TheWorld TheWorld = null;
    public SceneNodeControl NodeControl = null;

    void Awake() {
        Debug.Assert(TheWorld != null, "Please set The Word for " + name + " in the Editor");

        //Debug.Assert(NodeControl != null);
        //NodeControl.TheRoot = TheWorld.TheRoot;
    }

    // Use this for initialization
    void Start() {
        //Debug.Assert(MainCamera != null);
        
    }

    // Update is called once per frame
    void Update() {
        //ProcessMouseEvents();
        Click();
        CheckKeyboard();
    }

}