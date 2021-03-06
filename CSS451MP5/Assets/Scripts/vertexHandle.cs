﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vertexHandle : MonoBehaviour
{
    public enum axis : int{
        xAxis,
        yAxis,
        zAxis
    }

    public axis direction;
    public Color selectionColor = Color.yellow;
    private Color color;
    private MeshRenderer meshRenderer;

    private bool isSelected = false;

    void Start() {
        InitializedComponents();
        meshRenderer.material = (Material)Instantiate(meshRenderer.material);
        color = meshRenderer.material.color;
    }

    void InitializedComponents() {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Selected(bool _selected) {
        isSelected = _selected;
        meshRenderer.material.color = (isSelected) ? selectionColor : color;
    }

    public axis GetHandle() {
        return direction;
      }
}
