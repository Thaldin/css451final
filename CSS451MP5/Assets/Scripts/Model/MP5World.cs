﻿using UnityEngine;

public class MP5World : MonoBehaviour
{
    public GameObject LookAt = null;
    public bool RenderMesh = true;

    public int CylinderResolution = 10;
    public int MeshResolution = 4;
    public int CylinderRotation = 180;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(LookAt != null, "Please set LookAt object.");
    }

    // Update is called once per frame
    void Update()
    {
        if (RenderMesh)
        {
            RenderTextureMesh();
        }
        else
        {
            RenderCylinderMesh();
        }
    }

    public void SetLookAtPos(Vector3 pos)
    {
        LookAt.transform.localPosition = pos;
    }

    public Vector3 GetLookAtPos()
    {
        return LookAt.transform.localPosition;
    }

    public void SlideLookAtPos(float deltaX, float deltaY)
    {
        LookAt.transform.position += deltaX * LookAt.transform.right;
        LookAt.transform.position += deltaY * LookAt.transform.up;
    }

    private void RenderTextureMesh()
    {
        Debug.Log("Rendering Mesh at Res: " + MeshResolution);
    }

    private void RenderCylinderMesh()
    {
        Debug.Log("Rendering Cylinder at Res: " + CylinderResolution + " & Rot: " + CylinderRotation);
    }
}
