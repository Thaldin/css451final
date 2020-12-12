using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sphereColliderScript : MonoBehaviour
{
    [SerializeField] int index = 0;
    [SerializeField] float radius = 0.5f;
    public void Initialize(float d, out float r) {
        
        // this is cheese
        radius = Mathf.Pow(d, 2f);
        GetComponent<SphereCollider>().radius = radius;
        r = radius;
    }

    public void SetIndex(int i) {
        index = i;
    }
    public int GetIndex() {
        return index;
    }
}
