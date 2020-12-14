using System; // for assert
using UnityEngine;
using UnityEngine.UI; // for GUI elements: Button, Toggle
using System.Collections;
using System.Collections.Generic;

public partial class MainController : MonoBehaviour {
    bool hudIsOn = true;


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

    [Header("Asteroids")]
    public GameObject asteroidUIPanel = null;
    public SliderWithEcho asteroidSpawnRate = null;
    public float minSpawnRate = 0f;
    public float maxSpawnRate = 100f;
    public AsteroidSpawner asteroidSpawner = null;
    public Text asteroidCountText = null;
    public Text pTypeText = null;
    void Awake() {
        Debug.Assert(NodeControl != null);
        NodeControl.TheRoot = TheWorld.TheRoot;

        OnFire += deathStar.HandleOnFire;
        OnToggleHud += NodeControl.HandleOnToggleHud;
        OnToggleHud += HandleOnTogglehud;
    }

    // Use this for initialization
    void Start() {
        Debug.Assert(MainCamera != null);
        Debug.Assert(MiniCamera != null);
        Debug.Assert(TheWorld != null, "Please set The World for " + name + " in the Editor");
        Debug.Assert(NodeControl != null, "Please set The World for " + name + " in the Editor");
        Debug.Assert(pauseMenu != null, "Please set The World for " + name + " in the Editor");

        
        timeScale.InitSliderRange(minTimeScale, maxTimeScale, 1.0f);
        timeScale.SetSliderListener(TimeScaleListener);
        timeScale.gameObject.SetActive(false);

        systemScale.InitSliderRange(minSysScale, maxSysScale, 1.0f);
        systemScale.SetSliderListener(SysScaleListener);
        systemScale.gameObject.SetActive(false);
        
        asteroidSpawnRate.InitSliderRange(minSpawnRate, maxSpawnRate, 10f);
        asteroidSpawnRate.SetSliderListener(AsteroidSpawnRateListener);
        asteroidSpawnRate.gameObject.SetActive(hudIsOn);

        NodeControl.AddToDropdownMenu(deathStar.transform.parent);
        NodeControl.AddToDropdownMenu(asteroidSpawner.transform);

    }

    // Update is called once per frame
    void Update() {        
        Click();
        CheckKeyboard();
        UpdateUI();
    }

    public void TimeScaleListener(float v)
    {
        TheWorld.SetTimeScale(v);
    }

    public void SysScaleListener(float v)
    {
        MiniCamera.gameObject.GetComponent<CameraFollow>().SetZoom(v);
    }

    public void AsteroidSpawnRateListener(float v) {
        asteroidSpawner.SetSpawnRate(v);
    }

    private void UpdateUI() {
        UpdateAsteroidCount();
        UpdateProjectileType();
    }

    private void UpdateAsteroidCount() { 
        int asC = asteroidSpawner.GetAsteroidCOunt();
        asteroidCountText.text = asC.ToString();
    }

    private void UpdateProjectileType() {
        Projectile.ProjectileType pt = asteroidSpawner.GetProjectileType();
        pTypeText.text = pt.ToString();
    }

    private void HandleOnTogglehud() {
        hudIsOn = !hudIsOn;
        asteroidUIPanel.SetActive(hudIsOn);
    }


}