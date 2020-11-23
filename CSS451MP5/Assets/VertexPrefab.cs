using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexPrefab : MonoBehaviour {
    public Color selectionColor = Color.yellow;
    private Color color;
    private MeshRenderer meshRenderer;

    private bool isSelected = false;
    // Start is called before the first frame update
    void Start() {
        InitializedComponents();
        meshRenderer.material = (Material)Instantiate(meshRenderer.material);
        color = meshRenderer.material.color;
    }

    void InitializedComponents() {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update() {
        meshRenderer.material.color = (isSelected) ? selectionColor : color;
    }
}
