using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MinimapBehaviour : MonoBehaviour
{
    public GameObject minimapPanel;
    public GameObject roomIconPrefab;
    public GameObject playerIconPrefab;

    public Dictionary<string, Vector3> roomsDictionary = new Dictionary<string, Vector3>();
    private Dictionary<string, GameObject> minimapIcons = new Dictionary<string, GameObject>();

    private GameObject playerIcon;
    private GameObject characterRef;

    private float mapScale = 1.5f;

    public void initMinimap(Dictionary<string, Vector3> levelRoomsDictionary, GameObject character)
    {
        minimapPanel = GameObject.Find("Minimap");
        roomsDictionary = levelRoomsDictionary;
        characterRef = character;

        GenerateMinimapIcons();
        GeneratePlayerIcon(character);
    }

    private void GeneratePlayerIcon(GameObject character)
    {
        if (playerIconPrefab == null)
        {
            Debug.LogError("PlayerIconPrefab no asignado en el inspector");
            return;
        }

        playerIcon = Instantiate(playerIconPrefab, minimapPanel.transform);
        playerIcon.name = "PlayerIcon";
    }

    public void MovePlayerToRoom(string roomName)
    {
        string key = roomsDictionary.Keys.FirstOrDefault(k => roomName.Contains(k) || k.Contains(roomName));
        if (key == null)
        {
            Debug.LogWarning("Room not found in minimap: " + roomName);
            return;
        }

        Vector2 minimapPos = WorldToMinimap(roomsDictionary[key]);
        if (playerIcon != null)
            playerIcon.GetComponent<RectTransform>().anchoredPosition = minimapPos;
    }



    private void GenerateMinimapIcons()
    {
        foreach (KeyValuePair<string, Vector3> room in roomsDictionary)
        {
            Vector2 minimapPos = WorldToMinimap(room.Value);
            GameObject icon = Instantiate(roomIconPrefab, minimapPanel.transform);

            RectTransform rt = icon.GetComponent<RectTransform>();
            rt.anchoredPosition = minimapPos;

            icon.name = "MinimapIcon_" + room.Key;
            minimapIcons.Add(room.Key, icon);
        }
    }

    private Vector2 WorldToMinimap(Vector3 worldPos)
    {
        Vector3 firstRoom = roomsDictionary["Room_0"];
        Vector3 offset = worldPos - firstRoom;

        // Escalar con mapScale
        Vector2 pos = new Vector2(offset.x * mapScale, offset.z * mapScale);

        // Limitar para que no se salga del panel
        RectTransform panelRect = minimapPanel.GetComponent<RectTransform>();
        float halfWidth = panelRect.rect.width / 2f;
        float halfHeight = panelRect.rect.height / 2f;

        pos.x = Mathf.Clamp(pos.x, -halfWidth, halfWidth);
        pos.y = Mathf.Clamp(pos.y, -halfHeight, halfHeight);

        return pos;
    }


}
