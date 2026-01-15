using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2Generator : MonoBehaviour
{
    private int levelWidth = 7;

    void Start()
    {
        LevelGenerator levelGenerator = GetComponent<LevelGenerator>();

        if (levelGenerator == null)
        {
            Debug.LogError("No se encontró un componente LevelGenerator en este GameObject.");
            return;
        }
        levelGenerator.fogEnabled = true;

        levelGenerator.GenerateLevel(levelWidth, 5, 2); // Genera el mapa lógico
        int totalRooms = levelGenerator.SpawnRooms(); // Genera las habitaciones físicas

        Debug.Log($"Nivel 2 generado con {totalRooms} habitaciones normales + Boss + Tesoro");
    }
}
