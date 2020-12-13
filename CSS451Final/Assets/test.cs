using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class test : MonoBehaviour {
    public float speed = 10f;

    // Update is called once per frame
    void Update() {
        Vector3 pos = transform.position;
        pos.z -= speed * Time.deltaTime;
        transform.position = pos;
        Rotate();
    }

    private void Rotate() {
        Vector3 rotation = transform.eulerAngles;

        rotation.x += 5f * speed * Time.deltaTime;
        rotation.x = (rotation.x <= 360f) ? rotation.x : 0f;

        rotation.y += 5f * speed * Time.deltaTime / 2f;
        rotation.y = (rotation.y <= 360f) ? rotation.y : 0f;

        rotation.z += 5f * speed * Time.deltaTime;
        rotation.z = (rotation.z <= 360f) ? rotation.z : 0f;

        transform.localEulerAngles = rotation;
    }
}
