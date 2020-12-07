using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour {
    public GameObject planetInfoPrefab;
    public GameObject planetPanel;
    public GameObject scrollBar;

    public int planetCount = 0;

    List<GameObject> planetChildren;
    // Start is called before the first frame update
    void Start() {
        planetChildren = new List<GameObject>();
    }

    // Update is called once per frame
    void Update() {

    }

    public void SetPlanetCount(string _count) {
        Int32.TryParse(_count, out planetCount);
    }
    public void SetPlanetCount(float _count) {
        planetCount = (int)_count;
    }

    public void FillPlanets() {
        RectTransform rt = planetPanel.GetComponent<RectTransform>();
        RectTransform scrollRT = scrollBar.GetComponent<RectTransform>();

        float width = planetInfoPrefab.GetComponent<RectTransform>().rect.width + 10f;
        float height = planetInfoPrefab.GetComponent<RectTransform>().rect.height * planetCount + 25f;
        float yPos = -1 * 150 + (float)(height / 2);
        rt.rect.Set(0f, yPos, width, height);
        scrollRT.rect.Set(-180f, yPos, height, 20f);
        //rt.sizeDelta = new Vector2(width, height);

        // kill babies
        foreach (var child in planetChildren) {
            Destroy(child);
        }
        planetChildren.Clear();

        // make new babies
        for (int i = 0; i < planetCount; i++) {
            GameObject prefabClone = (GameObject)Instantiate(planetInfoPrefab, rt);
            planetChildren.Add(prefabClone);
        }
    }


}
