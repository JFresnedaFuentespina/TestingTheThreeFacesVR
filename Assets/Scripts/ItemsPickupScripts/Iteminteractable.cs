using UnityEngine;

public class ItemInteractable : MonoBehaviour
{
    private GameObject player;
    private PickupItem playerPickup;

    private PickupItem GetPlayerPickup()
    {
        if (playerPickup == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogWarning("Player no encontrado en la escena");
                return null;
            }

            playerPickup = player.GetComponent<PickupItem>();
            if (playerPickup == null)
            {
                Debug.LogWarning("PickupItem no encontrado en el Player");
                return null;
            }
        }
        return playerPickup;
    }

    public void OnSelected()
    {
        var pickup = GetPlayerPickup();
        if (pickup == null)
        {
            Debug.LogWarning("PickupItem no encontrado en el Player");
            return;
        }

        pickup.PickupItemRayInteraction(gameObject);
    }
}
