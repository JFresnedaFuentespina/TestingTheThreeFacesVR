using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3Generator : MonoBehaviour
{
    // Start is called before the first frame update
    private int levelWidth = 10;

    void Start()
    {
        LevelGenerator levelGenerator = GetComponent<LevelGenerator>();
        SpawnKeyInRoom keySpawner = GetComponent<SpawnKeyInRoom>();

        if (levelGenerator == null)
        {
            Debug.LogError("No se encontró un componente LevelGenerator en este GameObject.");
            return;
        }

        levelGenerator.GenerateLevel(levelWidth, 7, 3); // Genera el mapa lógico
        int totalRooms = levelGenerator.SpawnRooms(); // Genera las habitaciones físicas
        StartCoroutine(keySpawner.WaitAndChooseRandomRoom());

        Debug.Log($"Nivel 3 generado con {totalRooms} habitaciones normales + Boss + Tesoro");
    }
}
