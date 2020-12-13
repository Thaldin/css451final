using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    public TheWorld theWorld;
    public float tumbleSpeed = 5;
    public float speed = 100f;

    [SerializeField] float distanceToTarget = 0f;

    [SerializeField] int pTarget = 0;
    [SerializeField] SceneNode target;
    [SerializeField] Vector3 targetPos = Vector3.zero;
    private float targetRadius = 0f;

    public ParticleSystem explosion = null;
    private bool hit = false;
    private float time = 0f;
    private float impact = 0f;

    private void Awake() {
        theWorld = GameObject.Find("god").GetComponent<TheWorld>();
    }

    // Update is called once per frame
    void Update() {
        if (!hit) {
            GetTargetPosition();
            // look at target
            Vector3 dir = transform.position - targetPos;
            dir.Normalize();
            transform.rotation = Quaternion.FromToRotation(Vector3.up, -dir);
            // cal distance
            distanceToTarget = Vector3.Distance(targetPos, transform.position);
            // move towards target
            transform.Translate(Vector3.up * Time.deltaTime * speed);

            if (distanceToTarget <= targetRadius) {
                Hit();
            }
        }

        if (hit) {
            impact = Time.time;
        }

        time = impact - Time.deltaTime;
        if (time > 3f) {
            Destroy(explosion);
            Destroy(gameObject);
        }
    }

    public void Initialize(int targetIndex) {
        pTarget = targetIndex;
        target = theWorld.GetSceneObjectFromIndex(pTarget).GetComponent<SceneNode>();
        targetRadius = target.colliderRadius;
    }

    private void Hit() {
        Debug.Log("BOOM!");
        explosion = Instantiate(explosion);
        explosion.Play();
        hit = true;

    }

    private void GetTargetPosition() {
        targetPos = theWorld.GetTargetPosition(pTarget);
    }

    private void Rotate() {
        Vector3 rotation = transform.eulerAngles;

        rotation.x += tumbleSpeed * speed * Time.deltaTime;
        rotation.x = (rotation.x <= 360f) ? rotation.x : 0f;

        rotation.y += tumbleSpeed * speed * Time.deltaTime / 2f;
        rotation.y = (rotation.y <= 360f) ? rotation.y : 0f;

        rotation.z += tumbleSpeed * speed * Time.deltaTime;
        rotation.z = (rotation.z <= 360f) ? rotation.z : 0f;

        transform.localEulerAngles = rotation;
    }
}
