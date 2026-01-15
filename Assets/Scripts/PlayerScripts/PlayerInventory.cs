using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public Inventory inventory; // Asignado desde el Inspector

    public delegate void OnInventoryItemsProvided(List<InventoryItem> items);
    public static event OnInventoryItemsProvided OnInventoryItemsProvidedEvent;

    void Awake()
    {
        if (inventory == null)
        {
            Debug.LogError("Inventory ScriptableObject no asignado en PlayerInventory!");
        }
    }

    void Start()
    {
        SubscribeToPickupEvents();
    }

    void OnDestroy()
    {
        PickupItem.OnAddItemToInventoryEvent -= AddItem;
    }

    public void SubscribeToPickupEvents()
    {
        PickupItem.OnAddItemToInventoryEvent += AddItem;
    }

    public static void RequestInventoryItems()
    {
        var instance = FindFirstObjectByType<PlayerInventory>();
        if(instance == null || instance.inventory == null) return;

        OnInventoryItemsProvidedEvent?.Invoke(instance.inventory.items);
    }

    public void AddItem(string id, Sprite icon)
    {
        if (inventory != null)
        {
            inventory.AddItem(id, icon);
        }
    }

    public void ResetInventory()
    {
        if (inventory != null)
            inventory.ResetInventory();
    }
}
