﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    public TheWorld theWorld;
    public float tumbleSpeed = 5;
    public float speed = 100f;

    public enum ProjectileType {
        homing,      // by target
        heatSeekings // closest target
    }

    [SerializeField] float distanceToTarget = 0f;

    [SerializeField] int pTarget = 0;
    [SerializeField] ProjectileType pType;
    [SerializeField] SceneNode target;
    [SerializeField] Vector3 targetPos = Vector3.zero;
    private float targetRadius = 0f;

    public ParticleSystem explosion = null;
    public ParticleSystem particles;
    private bool hit = false;
    private float time = 0f;
    private float impact = 0f;

    bool debugIsOn = false;
    GameObject debugLine;

    public event Action<Projectile> OnCollision;

    private void Awake() {
        theWorld = GameObject.Find("god").GetComponent<TheWorld>();
        debugLine = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        debugLine.SetActive(debugIsOn);
        particles = Resources.Load<ParticleSystem>("Prefabs\\PlaneExp");
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
            if (CompareTag("asteroid")) {
                Destroy(debugLine);
                // invoke the event
                OnCollision?.Invoke(this);
            }
        }

        // update debug line
        debugLine.SetActive(debugIsOn);
        if (debugIsOn) {
            Utils.Utils.AdjustLine(debugLine, transform.position, targetPos);      
        }
    }

    public void Initialize(Transform mother, int targetIndex, ProjectileType projectileType = default, bool debug = false) {
        debugIsOn = debug;
        pTarget = targetIndex;
        pType = projectileType;
        target = theWorld.GetSceneObjectFromIndex(pTarget).GetComponent<SceneNode>();
        targetRadius = target.colliderRadius;
    }

    private void Hit() {
        Debug.Log("BOOM!");
        //explosion = Instantiate(explosion);
        //explosion.Play();
        hit = true;

        var exp = Instantiate(particles) as ParticleSystem;
        exp.transform.position = transform.position;
        exp.Play();
    }

    // Get the target position from the world by projectile type
    private void GetTargetPosition() {
        switch (pType) {
            case ProjectileType.heatSeekings:
                targetPos = theWorld.GetClosestTargetByPosition(transform.position, out target);
                break;
            case ProjectileType.homing:
                targetPos = theWorld.GetTargetPositionByIndex(pTarget);
                break;
            default:
                break;
        }
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

    public void ToggleDebug(bool b) {
        debugIsOn = b;
    }
}
