using UnityEngine;

public class IncreaseAttackDmgItemPickupBehaviour : MonoBehaviour, ItemPickupBehaviour
{
    public delegate void OnPlayerAttack(string item);
    public static event OnPlayerAttack OnPlayerAttackEvent;
    public string ApplyItemEffects()
    {
        if (OnPlayerAttackEvent != null)
        {
            OnPlayerAttackEvent("IncreaseAttackDamageItem");
        }
        return "¡Daño de ataque aumentado!";
    }
}
