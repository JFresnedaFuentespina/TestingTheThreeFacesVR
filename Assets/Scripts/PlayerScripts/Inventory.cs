using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "Scriptable Objects/Inventory")]
public class Inventory : ScriptableObject
{
    public List<InventoryItem> items = new List<InventoryItem>();

    public void AddItem(string id, Sprite sprite)
    {
        if (!items.Exists(i => i.itemID == id))
        {
            items.Add(new InventoryItem { itemID = id, icon = sprite });
        }
    }

    public void ResetInventory()
    {
        items.Clear();
    }

    public void RemoveItem(string id)
    {
        items.RemoveAll(i => i.itemID == id);
    }

    public bool HasItem(string id)
    {
        return items.Exists(i => i.itemID == id);
    }
}
