using UnityEngine;

public class ThunderPickupItemBehaviour : MonoBehaviour, ItemPickupBehaviour
{
    public delegate void OnPlayerAttack(string item);
    public static event OnPlayerAttack OnPlayerAttackEvent;
    public delegate void OnPlayerAttackType(bool isThunder);
    public static event OnPlayerAttackType OnPlayerAttackTypeEvent;
    public string ApplyItemEffects()
    {
        if (OnPlayerAttackEvent != null)
        {
            OnPlayerAttackEvent("Thunder");
        }

        if(OnPlayerAttackTypeEvent != null)
        {
            OnPlayerAttackTypeEvent(true);
        }
        return "¡Disparo eléctrico!";
    }
}
