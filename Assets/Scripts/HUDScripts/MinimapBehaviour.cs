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

    private float mapScale;

    public void initMinimap(Dictionary<string, Vector3> levelRoomsDictionary, GameObject character)
    {
        minimapPanel = GameObject.Find("Minimap");
        roomsDictionary = levelRoomsDictionary;

        mapScale = CalculateDynamicScale();

        GenerateMinimapIcons();
        GeneratePlayerIcon();
    }

    private void GeneratePlayerIcon()
    {
        if (playerIconPrefab == null)
        {
            Debug.LogError("PlayerIconPrefab no asignado en el inspector");
            return;
        }

        playerIcon = Instantiate(playerIconPrefab, minimapPanel.transform);
        playerIcon.name = "PlayerIcon";

        RectTransform rt = playerIcon.GetComponent<RectTransform>();

        float iconSize = GetIconSize();

        rt.localScale = Vector3.one;

        float aspect = 170f / 100f;
        rt.sizeDelta = new Vector2(iconSize * aspect, iconSize);
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
        float iconSize = GetIconSize();

        foreach (KeyValuePair<string, Vector3> room in roomsDictionary)
        {
            Vector2 minimapPos = WorldToMinimap(room.Value);
            GameObject icon = Instantiate(roomIconPrefab, minimapPanel.transform);

            RectTransform rt = icon.GetComponent<RectTransform>();
            rt.anchoredPosition = minimapPos;
            rt.sizeDelta = new Vector2(iconSize, iconSize);

            icon.name = "MinimapIcon_" + room.Key;
            minimapIcons.Add(room.Key, icon);
        }
    }

    private float GetIconSize()
    {
        int roomCount = roomsDictionary.Count;

        // Tamaño basado en número de habitaciones, no en escala
        float size = 30f - roomCount * 0.5f;

        return Mathf.Clamp(size, 14f, 30f);
    }

    private Vector2 WorldToMinimap(Vector3 worldPos)
    {
        Vector3 firstRoom = roomsDictionary["Room_0"];
        Vector3 offset = worldPos - firstRoom;

        Vector2 pos = new Vector2(offset.x * mapScale, offset.z * mapScale);

        RectTransform panelRect = minimapPanel.GetComponent<RectTransform>();
        float halfWidth = panelRect.rect.width / 2f;
        float halfHeight = panelRect.rect.height / 2f;

        pos.x = Mathf.Clamp(pos.x, -halfWidth, halfWidth);
        pos.y = Mathf.Clamp(pos.y, -halfHeight, halfHeight);

        return pos;
    }

    private float CalculateDynamicScale()
    {
        if (roomsDictionary.Count == 0)
            return 1f;

        float minX = roomsDictionary.Values.Min(v => v.x);
        float maxX = roomsDictionary.Values.Max(v => v.x);
        float minZ = roomsDictionary.Values.Min(v => v.z);
        float maxZ = roomsDictionary.Values.Max(v => v.z);

        float worldWidth = maxX - minX;
        float worldHeight = maxZ - minZ;

        RectTransform panelRect = minimapPanel.GetComponent<RectTransform>();

        float panelWidth = panelRect.rect.width;
        float panelHeight = panelRect.rect.height;

        float padding = 40f;

        float scaleX = (panelWidth - padding) / worldWidth;
        float scaleY = (panelHeight - padding) / worldHeight;

        float finalScale = Mathf.Min(scaleX, scaleY);

        // Evita escalas ridículamente pequeñas
        return Mathf.Clamp(finalScale, 0.05f, 2f);
    }
}