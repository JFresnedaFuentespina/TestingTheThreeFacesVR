using UnityEngine;

public class IncreaseSpeedItemPickupBehaviour : MonoBehaviour, ItemPickupBehaviour
{
    public delegate void OnPlayerSpeed(float amount);
    public static event OnPlayerSpeed OnPlayerSpeedEvent;

    public string ApplyItemEffects()
    {
        if(OnPlayerSpeedEvent != null)
        {
            OnPlayerSpeedEvent(0.5f);
        }
        return "¡Velocidad aumentada!";
    }
}
