using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour {
    public TheWorld theWorld = null;
    public GameObject asteroidPrefab = null;
    [SerializeField] List<Projectile> asteroidsInScene = new List<Projectile>();

    [SerializeField] float spawnRate = 6;
    public float max = 0.05f;
    const float GROW_CONST = 3000f;
    private Vector3 scale = Vector3.zero;
    private float size = 0f;
    private int pTypeIndex = 0;
    public GameObject scaler;

    bool debugIsOn = false;
    private void Awake() {
        //theWorld = GameObject.Find("god").GetComponent<TheWorld>();
        //asdasd
        //scaler = Instantiate(asteroidPrefab, transform.position, Quaternion.identity, transform);
    }

    // Update is called once per frame
    void Update() {
        scaler.transform.localScale = Vector3.zero;
        //asteroidsInScene.Add(a);

        size += (spawnRate) / GROW_CONST * max;
        if (size >= max) {
            size = 0f;
            FireAsteroid(0);
        }

        scale = new Vector3(size, size, size);
        scaler.transform.localScale = scale;

    }
    private void FireAsteroid(int index) {
        GameObject asteroid = Instantiate(asteroidPrefab, transform.position, Quaternion.identity);
        Projectile a = asteroid.GetComponent<Projectile>();
        a.OnCollision += HandleAteroidCOllision;
        a.transform.localScale = new Vector3(max, max, max);

        a.Initialize(transform, index, (Projectile.ProjectileType)pTypeIndex, debugIsOn);
        asteroidsInScene.Add(a);
    }

    public void SetProjectileType(int tIndex) {
        pTypeIndex = tIndex;
    }

    public void SetSpawnRate(float v) {
        spawnRate = v;
    }

    public int GetAsteroidCOunt() {
        return asteroidsInScene.Count;
    }

    public Projectile.ProjectileType GetProjectileType() {
        return (Projectile.ProjectileType)pTypeIndex;
    }

    void HandleAteroidCOllision(Projectile p) { 
        asteroidsInScene.Remove(p);
        Destroy(p.transform.gameObject);
    }

    public void DestroyAsteroid(Transform asteroid) {

    }

    public void ToggleDebug(bool b) {
        debugIsOn = b;
        foreach (var a in asteroidsInScene) {
            a.GetComponent<Projectile>().ToggleDebug(debugIsOn);
        }
    }
}
