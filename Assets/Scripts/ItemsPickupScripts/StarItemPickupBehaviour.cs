using UnityEngine;

public class StarItemPickupBehaviour : MonoBehaviour, ItemPickupBehaviour
{
    public delegate void OnPlayerAttack(string item);
    public static event OnPlayerAttack OnPlayerAttackEvent;
    public delegate void OnPlayerSpeed(float amount);
    public static event OnPlayerSpeed OnPlayerSpeedEvent;
    public string ApplyItemEffects()
    {
        if (OnPlayerAttackEvent != null)
        {
            OnPlayerAttackEvent("Star");
        }
        if(OnPlayerSpeedEvent != null)
        {
            OnPlayerSpeedEvent(1f);
        }
        return "¡Mejoras en todas las estadísticas!";
    }
}
