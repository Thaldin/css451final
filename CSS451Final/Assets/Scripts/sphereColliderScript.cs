using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sphereColliderScript : MonoBehaviour
{
    [SerializeField] float radius = 0.5f;
    public void Initialize(float r) {
        radius = r;
        GetComponent<SphereCollider>().radius = radius;
    }
}
