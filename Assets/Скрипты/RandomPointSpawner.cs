using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPointSpawner : MonoBehaviour
{
    public GameObject prefab;
    public List<Transform> spawnPoints = new List<Transform>();

    [Header("Speed Range")]
    public float minSpeed = 5f;
    public float maxSpeed = 15f;

    [Header("Spawn")]
    public float spawnInterval = 1f;
    public bool autoSpawn = true;

    private void Start()
    {
        if (autoSpawn)
            StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnRandom();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnRandom()
    {
        if (spawnPoints.Count == 0 || prefab == null)
            return;

        Transform point = spawnPoints[Random.Range(0, spawnPoints.Count)];
        float speed = Random.Range(minSpeed, maxSpeed);

        GameObject obj = Instantiate(prefab, point.position, point.rotation);

        ConstantMove mover = obj.GetComponent<ConstantMove>();
        if (mover == null)
            mover = obj.AddComponent<ConstantMove>();

        mover.Init(point.forward, speed);
    }
}
