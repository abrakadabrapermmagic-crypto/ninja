using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject prefab;
    public float spawnDistance = 2f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Spawn();
        }
    }

    void Spawn()
    {
        Vector3 spawnPosition =
            transform.position + transform.forward * spawnDistance;

        Instantiate(prefab, spawnPosition, transform.rotation);
    }
}
