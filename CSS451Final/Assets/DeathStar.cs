using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathStar : MonoBehaviour {
    public GameObject projectile = null;
    // Start is called before the first frame update
    void Awake() {
        //Debug.Assert(projectile != null, "Please set projectile prefab for " + name + " in the Editor.");
    }

    // Update is called once per frame
    void Update() {

    }

    public void Fire(List<Matrix4x4> m4x4) {
        

    }

    public Vector3 GetPosition() {
        return transform.position;
    }
}
