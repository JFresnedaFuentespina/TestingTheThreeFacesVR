using System.Collections.Generic;
using UnityEngine;

public class GenerateRocksInRoom : MonoBehaviour
{
    [Header("Configuración de Rocas")]
    public List<GameObject> rockPrefabs;
    private int maxRocks = 6;
    private int minRocks = 6;

    [Header("Elementos de la Habitación")]
    public GameObject floor;
    public Transform wallLeft;
    public Transform wallRight;
    public Transform wallFront;
    public float spawnMargin = 2f;

    // Distancia mínima delante de la puerta donde no colocar rocas
    private float doorClearDistance = 1f;

    void Start()
    {
        GenerateRocks();
    }

    void GenerateRocks()
    {
        if (floor == null)
        {
            Debug.LogError("Floor no asignado en el inspector.");
            return;
        }

        MeshRenderer meshRenderer = floor.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogError("El floor no tiene MeshRenderer.");
            return;
        }

        Bounds bounds = meshRenderer.bounds;

        // Recopilar las puertas activas
        List<Transform> activeDoors = GetActiveDoors();

        int rockCount = Random.Range(minRocks, maxRocks);
        int spawned = 0;
        int attempts = 0;

        while (spawned < rockCount && attempts < rockCount * 10)
        {
            attempts++;

            float x = Random.Range(bounds.min.x + spawnMargin, bounds.max.x - spawnMargin);
            float z = Random.Range(bounds.min.z + spawnMargin, bounds.max.z - spawnMargin);
            float y = bounds.max.y;

            Vector3 pos = new Vector3(x, y, z);

            // Si está demasiado cerca de una puerta activa → saltar
            if (IsNearDoor(pos, activeDoors))
                continue;

            GameObject prefab = rockPrefabs[Random.Range(0, rockPrefabs.Count)];
            Instantiate(prefab, pos, Quaternion.identity, transform);
            spawned++;
        }
    }

    List<Transform> GetActiveDoors()
    {
        List<Transform> doors = new List<Transform>();

        // Buscar puertas dentro de las tres paredes si existen
        AddActiveDoorsFromWall(wallLeft, doors);
        AddActiveDoorsFromWall(wallRight, doors);
        AddActiveDoorsFromWall(wallFront, doors);

        return doors;
    }

    void AddActiveDoorsFromWall(Transform wall, List<Transform> list)
    {
        if (wall == null) return;

        foreach (Transform child in wall)
        {
            // Buscar objetos tipo Door_Prefab_Closed_Right (u otros) activos
            if (child.name.Contains("Door") && child.gameObject.activeInHierarchy)
            {
                list.Add(child);
            }
        }
    }

    bool IsNearDoor(Vector3 pos, List<Transform> doors)
    {
        foreach (Transform door in doors)
        {
            // Calcula la distancia en el plano XZ
            Vector3 doorPos = door.position;
            Vector2 pos2D = new Vector2(pos.x, pos.z);
            Vector2 door2D = new Vector2(doorPos.x, doorPos.z);

            if (Vector2.Distance(pos2D, door2D) < doorClearDistance)
                return true;
        }
        return false;
    }
}
