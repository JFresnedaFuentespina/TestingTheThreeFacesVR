using UnityEngine;

public class Level1Generator : MonoBehaviour
{
    private int levelWidth = 5;

    void Start()
    {
        LevelGenerator levelGenerator = GetComponent<LevelGenerator>();
        SpawnKeyInRoom keySpawner = GetComponent<SpawnKeyInRoom>();
        if (levelGenerator == null)
        {
            Debug.LogError("No se encontró un componente LevelGenerator en este GameObject.");
            return;
        }

        levelGenerator.GenerateLevel(levelWidth, 2, 1); // Genera el mapa
        int totalRooms = levelGenerator.SpawnRooms(); // Genera las habitaciones físicas
        StartCoroutine(keySpawner.WaitAndChooseRandomRoom());

        Debug.Log($"Nivel 1 generado con {totalRooms} habitaciones normales + Boss + Tesoro");
    }
}
