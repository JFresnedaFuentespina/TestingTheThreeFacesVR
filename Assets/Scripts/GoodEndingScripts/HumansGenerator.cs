using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumansGenerator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public List<GameObject> humansPrefabs;
    public MeshRenderer groundMeshRenderer;
    public int numberOfHumans = 20;
    public GameObject humansParent;

    void Start()
    {
        for (int i = 0; i < numberOfHumans; i++)
        {
            Vector3 randomPosition = GetRandomPointOnGround();
            int randomIndex = Random.Range(0, humansPrefabs.Count);
            var human = Instantiate(humansPrefabs[randomIndex], randomPosition, Quaternion.identity, humansParent.transform);
            human.GetComponent<NavMeshAgent>().Warp(randomPosition);
        }
    }

    Vector3 GetRandomPointOnGround()
    {
        Bounds bounds = groundMeshRenderer.bounds;
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);

        return new Vector3(randomX, 0f, randomZ);
    }
}
