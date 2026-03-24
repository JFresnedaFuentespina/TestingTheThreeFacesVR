using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiesGenerator : MonoBehaviour
{
    public List<GameObject> zombiesPrefabs;
    public List<GameObject> spawnPoints; // 4 puntos
    public int numberOfZombies = 20;
    public GameObject zombiesParent;

    void Start()
    {
        for (int i = 0; i < numberOfZombies; i++)
        {
            Vector3 randomPosition = GetRandomPointBetweenSpawnPoints();
            int randomIndex = Random.Range(0, zombiesPrefabs.Count);

            var zombie = Instantiate(
                zombiesPrefabs[randomIndex],
                randomPosition,
                Quaternion.identity,
                zombiesParent.transform
            );

            zombie.GetComponent<NavMeshAgent>().Warp(randomPosition);
        }
    }

    Vector3 GetRandomPointBetweenSpawnPoints()
    {
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogError("No hay spawnPoints asignados");
            return transform.position;
        }

        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minZ = float.MaxValue;
        float maxZ = float.MinValue;

        foreach (var point in spawnPoints)
        {
            Vector3 pos = point.transform.position;
            minX = Mathf.Min(minX, pos.x);
            maxX = Mathf.Max(maxX, pos.x);
            minZ = Mathf.Min(minZ, pos.z);
            maxZ = Mathf.Max(maxZ, pos.z);
        }

        Vector3 randomPoint = new Vector3(
            Random.Range(minX, maxX),
            0f,
            Random.Range(minZ, maxZ)
        );

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return randomPoint;
    }
}
