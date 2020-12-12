using System; // for assert
using UnityEngine;
using UnityEngine.UI; // for GUI elements: Button, Toggle
using System.Collections;
using System.Collections.Generic;

public partial class MainController : MonoBehaviour {


    public Camera MainCamera = null;
    public Camera MiniCamera = null;
    public TheWorld TheWorld = null;
    public SceneNodeControl NodeControl = null;
    public PauseMenu pauseMenu = null;

    [Header("Time Scale")]
    public SliderWithEcho timeScale = null;
    public float minTimeScale = 0f;
    public float maxTimeScale = 10f;

    [Header("System Scale")]
    public SliderWithEcho systemScale = null;
    public float minSysScale = 0.5f;
    public float maxSysScale = 5.0f;

    [Header("Special Effects")]
    public GameObject ParticleSystems = null;

    void Awake() {
        Debug.Assert(NodeControl != null);
        NodeControl.TheRoot = TheWorld.TheRoot;
    }

    // Use this for initialization
    void Start() {
        Debug.Assert(MainCamera != null);
        Debug.Assert(TheWorld != null, "Please set The World for " + name + " in the Editor");
        Debug.Assert(NodeControl != null, "Please set The World for " + name + " in the Editor");
        Debug.Assert(pauseMenu != null, "Please set The World for " + name + " in the Editor");

        timeScale.InitSliderRange(minTimeScale, maxTimeScale, 1.0f);
        timeScale.SetSliderListener(TimeScaleListener);

        systemScale.InitSliderRange(minSysScale, maxSysScale, 1.0f);
        systemScale.SetSliderListener(SysScaleListener);
    }

    // Update is called once per frame
    void Update() {        
        Click();
        CheckKeyboard();
    }

    public void TimeScaleListener(float v)
    {
        TheWorld.SetTimeScale(v);
    }

    public void SysScaleListener(float v)
    {
        TheWorld.SetSystemScale(v);
    } 
}