﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathStar : MonoBehaviour {
    public Transform pivot = null;
    public GameObject projectile = null;
    public AudioClip tarkin = null;
    public AudioClip laserFire = null;
    public Transform tar;
    [SerializeField] Vector3 targetPosition = Vector3.zero;
    AudioSource audioSource;


    public GameObject[] laserPoints = new GameObject[5];
    public GameObject[] lasers = new GameObject[5];
    bool isFiring = false;
    public float time = 0f;
    // Start is called before the first frame update
    void Awake() {
        //Debug.Assert(projectile != null, "Please set projectile prefab for " + name + " in the Editor.");
        audioSource = GetComponent<AudioSource>();
        CreateLasers();
    }

    // Update is called once per frame
    void Update() {
        for (int i = 0; i < 4; i++) {
            Utils.Utils.AdjustLine(lasers[i], laserPoints[0].transform.position, laserPoints[i + 1].transform.position, 0.15f);
        }
        Utils.Utils.AdjustLine(lasers[4], laserPoints[0].transform.position, targetPosition, 0.3f);

        Vector3 dir = pivot.position - targetPosition;
        dir.Normalize();
        pivot.rotation = Quaternion.FromToRotation(Vector3.forward, -dir);
    }

    public void HandleOnTarget(Vector3 tar) {
        if (!isFiring) {
            targetPosition = tar;
        }
    }

    private void CreateLasers() {
        for (int i = 0; i < 4; i++) {
            GameObject l = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            l.GetComponent<MeshRenderer>().material.color = Color.green;
            lasers[i] = l;
            Utils.Utils.AdjustLine(l, laserPoints[0].transform.position, laserPoints[i + 1].transform.position);
        }
        GameObject p = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        p.GetComponent<MeshRenderer>().material.color = Color.green;
        lasers[4] = p;

        foreach (var l in lasers) {
            l.SetActive(false);
        }
    }

    public void HandleOnFire() {
        isFiring = true;
        StartCoroutine(FireLaser());
    }

    IEnumerator FireLaser() {
        audioSource.clip = tarkin;
        audioSource.Play();
        yield return new WaitForSeconds(tarkin.length);
        audioSource.clip = laserFire;
        audioSource.Play();
        yield return new WaitForSeconds(laserFire.length / 2f);

        for (int i = 0; i < 5; i++) {
            lasers[i].SetActive(true);
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(3f);

        foreach (var l in lasers) {
            l.SetActive(false);
        }
        isFiring = false;
    }

    public Vector3 GetPosition() {
        return transform.position;
    }

    IEnumerator playFireRoutine(int index) {
        audioSource.clip = tarkin;
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length * 1.1f);
        audioSource.clip = laserFire;
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length * 0.9f);
        FireProjectile(index);
    }

    private void FireProjectile(int index) {
        GameObject pc = Instantiate(projectile, transform.position, Quaternion.identity);
        Projectile p = pc.GetComponent<Projectile>();
        p.Initialize(transform, index);
    }

}
