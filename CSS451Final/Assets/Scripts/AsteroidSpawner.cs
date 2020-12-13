using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour {
    public TheWorld theWorld = null;
    public GameObject asteroidPrefab = null;
    [SerializeField] List<GameObject> asteroidsInScene = new List<GameObject>();

    public float spawnRate = 6;
    public float max = 0.05f;
    const float GROW_CONST = 3000f;
    private Vector3 scale = Vector3.zero;
    private float size = 0f;

    public GameObject scaler;

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
        a.transform.localScale = new Vector3(max, max, max);

        a.Initialize(index);
    }

    public void TestFire() {
        GameObject asteroid = Instantiate(asteroidPrefab, transform.position, Quaternion.identity);
        asteroidsInScene.Add(asteroid);
        asteroid.transform.localScale = new Vector3(max,max,max);
    }
}
