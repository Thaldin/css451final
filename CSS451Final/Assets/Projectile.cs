using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    public TheWorld theWorld;

    public float speed = 100f;
    public float distanceToTarget = 0f;
    public float targetRadius = 0f;
    public Vector3 targetPos = Vector3.zero;

    public int pTarget = 0;
    public SceneNode target;
    public ParticleSystem explosion = null;
    public bool hit = false;
    public float time = 0f;
    float impact = 0f;

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
}
