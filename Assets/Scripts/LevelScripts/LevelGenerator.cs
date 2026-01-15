using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject roomPrefab;
    public GameObject treasureRoomPrefab;
    public GameObject bossRoomPrefab;
    public GameObject characterPrefab;

    [Header("Level Settings")]
    public int levelWidth;
    public int levelId;
    public float levelBaseY = 0f;
    public float offsetW = 50f;
    public int maxEnemiesPerRoom = 3;

    [Header("Generation Settings")]
    private List<bool> levelMap = new List<bool>();
    private int bossRoomIndex = -1;
    private bool bossRoomSpawned = false;
    private Vector3? forcedBossRoomPos = null;
    public GameObject character;

    public Dictionary<string, Vector3> roomsDictionary = new Dictionary<string, Vector3>();

    private MinimapBehaviour minimapBehaviour;
    public bool fogEnabled = false;

    public void GenerateLevel(int width, int minRooms, int levelId)
    {
        levelWidth = width;
        this.levelId = levelId;
        levelMap.Clear();
        roomsDictionary.Clear();

        int totalRooms = Random.Range(minRooms, levelWidth + 1);

        for (int i = 0; i < totalRooms; i++)
            levelMap.Add(true);

        for (int i = totalRooms; i < levelWidth; i++)
            levelMap.Add(false);
    }


    public int SpawnRooms()
    {
        minimapBehaviour = GetComponent<MinimapBehaviour>();
        int generatedRooms = 0;
        List<GameObject> roomList = new List<GameObject>();

        // Primero generamos todas las habitaciones normales y el boss si toca
        for (int i = 0; i < levelMap.Count; i++)
        {
            if (levelMap[i])
            {
                Vector3 position = new Vector3(i * offsetW, levelBaseY, 0);

                GameObject room = Instantiate(roomPrefab, position, Quaternion.identity, transform);
                room.name = $"Room_{i}";
                roomList.Add(room);

                if (fogEnabled)
                {
                    Transform fog = room.GameObject().transform.Find("Smoke");
                    if(fog != null)
                    {
                        fog.gameObject.SetActive(true);
                    }
                }

                if (i == 0)
                {
                    character = Instantiate(characterPrefab, position, Quaternion.identity);
                }

                roomsDictionary.Add($"Room_{i}", position);
                generatedRooms++;

                TrySpawnBossRoom(i, position);
            }
        }

        //  Generar la sala del tesoro ANTES de configurar las puertas
        Vector3? treasurePos = SpawnTreasureRoom();

        //  Si no se generó bossRoom, forzarla
        if (!bossRoomSpawned && forcedBossRoomPos.HasValue)
        {
            Instantiate(bossRoomPrefab, forcedBossRoomPos.Value, Quaternion.identity, transform);
            roomsDictionary.Add("Boss_Forced", forcedBossRoomPos.Value);
            bossRoomSpawned = true;
        }
        // Ahora configuramos las puertas correctamente para cada habitación instanciada
        for (int i = 0; i < roomList.Count; i++)
        {
            SetupRoomDoors(roomList[i], i, treasurePos);
        }
        minimapBehaviour.initMinimap(this.roomsDictionary, character);
        minimapBehaviour.MovePlayerToRoom("Room_0");
        return generatedRooms;
    }

    public void TrySpawnBossRoom(int i, Vector3 position)
    {
        if (bossRoomSpawned) return;

        if (Random.value < 0.3f)
        {
            Vector3 bossPos = position + new Vector3(0, 0, offsetW);
            Instantiate(bossRoomPrefab, bossPos, Quaternion.identity, transform);
            roomsDictionary.Add("Boss", bossPos);
            bossRoomSpawned = true;
            bossRoomIndex = i;
        }
        else if (!forcedBossRoomPos.HasValue)
        {
            forcedBossRoomPos = position + new Vector3(0, 0, offsetW);
        }
    }

    public void SetupRoomDoors(GameObject room, int x, Vector3? treasurePos = null)
    {
        Transform leftDoor = room.transform.Find("ParedIzquierda/Door_Prefab_Closed_Left");
        Transform rightDoor = room.transform.Find("ParedDerecha/Door_Prefab_Closed_Right");
        Transform frontDoor = room.transform.Find("ParedFrontal/Door_Prefab_Closed_Front");

        Vector3 currentPos = room.transform.position;

        //  Determinar si hay habitaciones vecinas en los lados
        bool hasLeft = (x > 0 && levelMap[x - 1]);
        bool hasRight = (x < levelMap.Count - 1 && levelMap[x + 1]);
        bool hasFront = false;

        // Comprobar si la BossRoom (normal o forzada) está justo delante (+Z)
        var bossEntry = roomsDictionary.FirstOrDefault(r =>
            r.Key.StartsWith("Boss") || r.Key.StartsWith("Boss_Forced"));
        if (!bossEntry.Equals(default(KeyValuePair<string, Vector3>)))
        {
            if (Vector3.Distance(bossEntry.Value, currentPos + new Vector3(0, 0, offsetW)) < 1f)
                hasFront = true;
        }

        // Comprobar si el tesoro está justo a la izquierda o derecha
        if (treasurePos.HasValue)
        {
            if (Vector3.Distance(treasurePos.Value, currentPos + new Vector3(-offsetW, 0, 0)) < 1f)
                hasLeft = true;
            if (Vector3.Distance(treasurePos.Value, currentPos + new Vector3(offsetW, 0, 0)) < 1f)
                hasRight = true;
        }

        // Activar o desactivar las puertas
        if (leftDoor != null) leftDoor.gameObject.SetActive(hasLeft);
        if (rightDoor != null) rightDoor.gameObject.SetActive(hasRight);
        if (frontDoor != null) frontDoor.gameObject.SetActive(hasFront);
    }


    public Vector3? SpawnTreasureRoom()
    {
        List<int> edgeIndices = new List<int>();
        if (levelMap[0]) edgeIndices.Add(0);
        if (levelMap[levelMap.Count - 1]) edgeIndices.Add(levelMap.Count - 1);
        if (edgeIndices.Count == 0) return null;

        int chosen = edgeIndices[Random.Range(0, edgeIndices.Count)];
        Vector3 pos = new Vector3(chosen * offsetW, levelBaseY, 0);
        Vector3 treasurePos = chosen == 0
            ? pos + new Vector3(-offsetW, 0, 0)   // a la izquierda
            : pos + new Vector3(offsetW, 0, 0);   // a la derecha

        GameObject treasureRoom = Instantiate(treasureRoomPrefab, treasurePos, Quaternion.identity, transform);
        treasureRoom.name = "TreasureRoom";
        roomsDictionary.Add(treasureRoom.name, treasurePos);

        Transform leftDoor = treasureRoom.transform.Find("ParedIzquierda/Door_Prefab_Closed_Left");
        Transform rightDoor = treasureRoom.transform.Find("ParedDerecha/Door_Prefab_Closed_Right");

        if (chosen == 0) // tesoro a la izquierda del todo
        {
            if (leftDoor != null) leftDoor.gameObject.SetActive(false);  // izquierda sin salida
            if (rightDoor != null) rightDoor.gameObject.SetActive(true); // salida hacia la derecha
        }
        else // tesoro a la derecha del todo
        {
            if (leftDoor != null) leftDoor.gameObject.SetActive(true);  // salida hacia la izquierda
            if (rightDoor != null) rightDoor.gameObject.SetActive(false); // derecha sin salida
        }
        return treasurePos;
    }

    public void NextLevel(int actualLevel)
    {
        string nextScene = "";
        switch (actualLevel)
        {
            case 0: nextScene = "MainMenu"; break;
            case 1: nextScene = "Level1Scene"; break;
            case 2: nextScene = "Level2Scene"; break;
            case 3: nextScene = "Level3Scene"; break;
            case 4: nextScene = "CretditsScene"; break;// créditos
            default: Debug.LogWarning("Nivel {actualLevel} no tiene escena siguiente."); break;
        }
        SceneManager.LoadScene(nextScene);
    }
}
