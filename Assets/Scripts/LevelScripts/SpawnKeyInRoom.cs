using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnKeyInRoom : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject keyPrefab;
    private LevelGenerator levelGenerator;
    private Dictionary<string, Vector3> roomsDictionary;
    private Vector3 selectedRoomPos;
    public GameObject suelo;
    public bool spawned = false;
    void Start()
    {
        levelGenerator = GetComponent<LevelGenerator>();
        roomsDictionary = levelGenerator.roomsDictionary;
        suelo = GameObject.Find("Suelo");
    }
    public IEnumerator WaitAndChooseRandomRoom()
    {
        // Esperar hasta que el diccionario esté listo
        while (roomsDictionary == null || roomsDictionary.Count == 0)
        {
            yield return null; // esperar 1 frame
        }

        ChooseRandomRoom();
    }
    public void ChooseRandomRoom()
    {
        if (roomsDictionary == null || roomsDictionary.Count == 0)
        {
            Debug.LogWarning("No hay habitaciones en el diccionario.");
            return;
        }

        if (selectedRoomPos != Vector3.zero)
        {
            Debug.Log("Ya se ha seleccionado una habitación para la llave: " + selectedRoomPos);
            return;
        }
        // Filtrar habitaciones
        List<Vector3> validRooms = new List<Vector3>();
        foreach (var kvp in roomsDictionary)
        {
            if (!kvp.Key.Contains("Boss") && kvp.Value != Vector3.zero)
            {
                validRooms.Add(kvp.Value);
            }
        }

        if (validRooms.Count == 0)
        {
            Debug.LogWarning("No hay habitaciones válidas para la llave (todas son Boss).");
            return;
        }

        // Elegir una posición aleatoria
        selectedRoomPos = validRooms[Random.Range(0, validRooms.Count)];

        Debug.Log("Habitación seleccionada para la llave: " + selectedRoomPos);
    }



    public void GenerateKey(Vector3 roomPos)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        if (inventory.hasKey || spawned)
        {
            Debug.Log("El jugador ya tiene la llave o ya fue generada.");
            return;
        }
        Debug.Log("Intentando generar llave en habitación: " + roomPos);
        // Solo generar si es la habitación seleccionada
        if (roomPos != selectedRoomPos)
            return;

        // Offset para que la llave no aparezca clavada en el suelo
        Vector3 spawnOffset = new Vector3(0f, 0.5f, 0f);

        Instantiate(
            keyPrefab,
            roomPos + spawnOffset,
            Quaternion.identity
        );
        spawned = true;
    }

}
