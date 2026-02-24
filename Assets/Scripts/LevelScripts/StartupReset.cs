using UnityEngine;

public static class StartupReset
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void ResetInventory()
    {
        Inventory inv = Resources.Load<Inventory>("Inventory"); // busca Assets/Resources/Inventory.asset
        if (inv != null)
        {
            inv.items.Clear();
        }
        else
        {
            Debug.LogError("No se pudo cargar Inventory desde Resources!");
        }
    }
}
