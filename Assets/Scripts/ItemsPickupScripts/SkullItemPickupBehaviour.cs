using UnityEngine;

public class SkullItemPickupBehaviour : MonoBehaviour, ItemPickupBehaviour
{
    public delegate void OnHealthDecreased(float amount);
    public static event OnHealthDecreased OnHealthDecreasedEvent; public delegate void OnPlayerAttack(string item);
    public static event OnPlayerAttack OnPlayerAttackEvent;
    public string ApplyItemEffects()
    {
        if (OnPlayerAttackEvent != null)
        {
            OnPlayerAttackEvent("Skull");
        }
        if(OnHealthDecreasedEvent != null)
        {
            OnHealthDecreasedEvent(1f);
        }
        return "Menos vida, ¡pero más daño!";
    }
}
