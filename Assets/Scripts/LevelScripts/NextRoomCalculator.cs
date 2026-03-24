using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NextRoomCalculator : MonoBehaviour
{
    private LevelGenerator level;
    public bool enabledTemporarily = false;
    public List<GameObject> torches;
    public GameObject audioManagerGO;
    public AudioManager audioManager;
    public GameObject camera1;
    public GameObject cameraCenital;

    void Start()
    {

        level = FindAnyObjectByType<LevelGenerator>();
        audioManagerGO = GameObject.Find("Music");
        audioManager = audioManagerGO?.GetComponent<AudioManager>();
        if (audioManager == null)
        {
            Debug.Log("AUDIO MANAGER NOT FOUND!!");
        }
        else
        {
            audioManager.level = level.levelWidth;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (enabledTemporarily)
            return;

        enabledTemporarily = true;

        Collider doorCollider = GetComponent<Collider>() ?? GetComponentInChildren<Collider>();
        if (doorCollider != null)
            Physics.IgnoreCollision(doorCollider, other, true);

        Vector3 targetPos = CalculateTargetRoomPosition(gameObject.name, transform.parent.parent.position);

        // Encontrar la siguiente habitación válida
        GameObject nextRoomObj = FindRoomObject(FindNextRoom(targetPos).Value);
        if (nextRoomObj == null)
        {
            Debug.LogWarning("No se encontró la habitación válida. Se mantiene la posición actual del jugador.");
            StartCoroutine(ReenableCollisionBetween(doorCollider, other, 0.5f));
            return;
        }

        if (nextRoomObj.GetComponent<BossRoom>() != null)
        {
            if (!PlayerHasKey())
            {
                Debug.Log("No tienes la llave para entrar a la Boss Room");

                // Rehabilitar colisión y salir
                StartCoroutine(ReenableCollisionBetween(doorCollider, other, 0.1f));
                enabledTemporarily = false;
                return;
            }
            audioManager?.PlayBossMusic();
        }

        // Desactivar puertas de la habitación de destino temporalmente
        DisableDoorsInRoom(nextRoomObj);

        // Calcular spawn seguro del jugador
        Transform oppositeDoor = FindOppositeDoor(nextRoomObj, gameObject.name);
        Vector3 spawnPos = (oppositeDoor != null) ? CalculateSpawnPosition(oppositeDoor) : other.transform.position;

        other.transform.root.position = spawnPos;

        MoveCamera(targetPos);

        StartCoroutine(ReenableCollisionBetween(doorCollider, other, 0.5f));
    }

    private bool PlayerHasKey()
    {
        PlayerInventory playerInventory = FindFirstObjectByType<PlayerInventory>();
        if (playerInventory == null || playerInventory.inventory == null)
            return false;
        return playerInventory.inventory.items.Exists(item => item.itemID == "Key");
    }

    private IEnumerator ReenableCollisionBetween(Collider a, Collider b, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (a == null || b == null) yield break;

        Physics.IgnoreCollision(a, b, false);

        var calc = a.GetComponent<NextRoomCalculator>();
        if (calc != null)
            calc.enabledTemporarily = false;
    }

    Vector3 CalculateTargetRoomPosition(string doorName, Vector3 currentRoomPos)
    {
        if (level == null)
            level = FindAnyObjectByType<LevelGenerator>();

        if (doorName.EndsWith("Left", System.StringComparison.OrdinalIgnoreCase))
            return currentRoomPos + new Vector3(-level.offsetW, 0, 0);
        if (doorName.EndsWith("Right", System.StringComparison.OrdinalIgnoreCase))
            return currentRoomPos + new Vector3(level.offsetW, 0, 0);
        if (doorName.EndsWith("Front", System.StringComparison.OrdinalIgnoreCase))
            return currentRoomPos + new Vector3(0, 0, level.offsetW);

        Debug.LogWarning($"Dirección no reconocida para la puerta {doorName}");
        return currentRoomPos;
    }

    KeyValuePair<string, Vector3> FindNextRoom(Vector3 targetPos)
    {
        if (level.roomsDictionary.Count == 0)
            return default;

        return level.roomsDictionary
            .Where(r => r.Key.Contains("Room") || r.Key.Contains("Boss") || r.Key.Contains("Treasure"))
            .OrderBy(r => Vector3.Distance(r.Value, targetPos))
            .FirstOrDefault();
    }

    GameObject FindRoomObject(Vector3 position)
    {
        return FindObjectsOfType<Transform>()
            .Select(t => t.gameObject)
            .FirstOrDefault(go =>
                go.GetComponent<EnemiesGenerator>() != null &&
                Vector3.Distance(go.transform.position, position) < 0.5f);
    }

    Transform FindOppositeDoor(GameObject targetRoomObj, string currentDoorName)
    {
        if (targetRoomObj == null) return null;

        string oppositeDoorName = "";
        if (currentDoorName.EndsWith("Left", System.StringComparison.OrdinalIgnoreCase))
            oppositeDoorName = "Door_Prefab_Closed_Right";
        else if (currentDoorName.EndsWith("Right", System.StringComparison.OrdinalIgnoreCase))
            oppositeDoorName = "Door_Prefab_Closed_Left";
        else if (currentDoorName.EndsWith("Front", System.StringComparison.OrdinalIgnoreCase))
            oppositeDoorName = "Door_Prefab_Closed_Front";

        if (string.IsNullOrEmpty(oppositeDoorName)) return null;

        return targetRoomObj.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(t => t.name.Equals(oppositeDoorName, System.StringComparison.OrdinalIgnoreCase));
    }

    Vector3 CalculateSpawnPosition(Transform oppositeDoor)
    {
        if (oppositeDoor == null)
            return Vector3.zero;

        Vector3 dir = Vector3.zero;
        if (oppositeDoor.name.EndsWith("Left", System.StringComparison.OrdinalIgnoreCase))
            dir = Vector3.right;
        else if (oppositeDoor.name.EndsWith("Right", System.StringComparison.OrdinalIgnoreCase))
            dir = Vector3.left;
        else if (oppositeDoor.name.EndsWith("Front", System.StringComparison.OrdinalIgnoreCase))
            dir = Vector3.back;

        Vector3 spawnPos = oppositeDoor.position + dir * 2f;
        spawnPos.y = 0f;
        return spawnPos;
    }

    private void DisableDoorsInRoom(GameObject room)
    {
        if (room == null) return;
        UpdateTorchesState(room);
        string[] doorPaths =
        {
            "ParedIzquierda/Door_Prefab_Closed_Left",
            "ParedDerecha/Door_Prefab_Closed_Right",
            "ParedFrontal/Door_Prefab_Closed_Front",
            "ParedFrontal/Door_Prefab_Closed_Front (Bad)",
            "ParedFrontal/Door_Prefab_Closed_Front (Good)"
        };

        foreach (string path in doorPaths)
        {
            Transform door = room.transform.Find(path);
            if (door != null)
            {
                Collider collider = door.GetComponent<Collider>();
                if (collider != null && collider.enabled)
                    collider.enabled = false;
            }
        }
    }

    private void UpdateTorchesState(GameObject room)
    {
        if (room == null) return;
        Transform torchLeft = room.transform.Find("ParedIzquierda/TorchLeft");
        Transform torchRight = room.transform.Find("ParedDerecha/TorchRight");
        Transform torchFront = room.transform.Find("ParedFrontal/TorchFront");
        SetTorchState(torchLeft);
        SetTorchState(torchRight);
        SetTorchState(torchFront);
    }

    private void SetTorchState(Transform torch)
    {
        if (torch == null) return;

        Transform red = torch.Find("FireRed");
        Transform green = torch.Find("FireGreen");

        if (red != null) red.gameObject.SetActive(true);
        if (green != null) green.gameObject.SetActive(false);
    }


    void MoveCamera(Vector3 roomPos)
    {
        if (Camera.main == null)
            return;

        FindCameras();

        if (camera1 != null)
        {
            camera1.transform.position = new Vector3(roomPos.x - 1.5f, camera1.transform.position.y, roomPos.z - 11.5f);
            camera1.transform.rotation = Quaternion.Euler(40f, 0f, 0f);
        }

        // if (cameraCenital != null)
        // {
        //     cameraCenital.transform.position = new Vector3(roomPos.x - 1.5f, cameraCenital.transform.position.y, roomPos.z - 3.95f);
        //     cameraCenital.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        //     Debug.Log("NextRoomCalculator - Moving Cenital Camera to new room position. New position: " + cameraCenital.transform.position);
        // }

        GameObject roomObj = FindRoomObject(roomPos);
        if (roomObj != null)
        {
            var minimap = FindAnyObjectByType<MinimapBehaviour>();
            minimap?.MovePlayerToRoom(roomObj.name);

            var generator = roomObj.GetComponentInChildren<EnemiesGenerator>();
            var doorsEnabler = roomObj.GetComponentInParent<DoorsEnabler>();
            var keyGenerator = level.GetComponentInChildren<SpawnKeyInRoom>();

            if (generator != null && doorsEnabler != null)
            {
                DisableDoorsInRoom(roomObj);
                generator.GenerateEnemiesInRoom(roomPos);
                keyGenerator?.GenerateKey(roomPos);
                doorsEnabler.StartCheckEnemies();
            }
        }
    }

    void FindCameras()
    {
        Camera[] cams = GameObject.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var cam in cams)
        {
            if (cam.name == "Main Camera")
                camera1 = cam.gameObject;
            // else if (cam.name == "CamaraCenital")
            //     cameraCenital = cam.gameObject;
        }
    }

}
