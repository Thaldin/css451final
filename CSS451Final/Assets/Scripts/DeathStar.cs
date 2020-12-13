using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathStar : MonoBehaviour {
    public GameObject projectile = null;
    public AudioClip tarkin = null;
    public AudioClip laserFire = null;
    public Transform tar;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Awake() {
        //Debug.Assert(projectile != null, "Please set projectile prefab for " + name + " in the Editor.");
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {

    }

    public void Fire(int index) {
        Debug.Log(" when ready");
        FireProjectile(index);

        //StartCoroutine(playFireRoutine(index));
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
        p.Initialize(index);
    }

}
