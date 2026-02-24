using UnityEngine;
using System.Collections.Generic;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    public List<GameObject> itemPrefabs;

    void Awake()
    {
        Instance = this;
    }

    public Transform GetItemPrefabByID(string id)
    {
        foreach (var item in itemPrefabs)
        {
            var icon = item.GetComponent<ItemIcon>();
            if (icon != null && icon.itemID == id)
                return item.transform;

        }
        Debug.LogWarning("Item ID no encontrado: " + id);
        return null;
    }
}
