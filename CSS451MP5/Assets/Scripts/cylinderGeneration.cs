using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cylinderGeneration : meshGeneration
{
    [Header("Cylinder Properties")]
    public int cylinderRadius = 2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void CreateCylinder() { 
        
    }
    public override Mesh UpdateMesh() {
        return new Mesh();
    }

}
