using UnityEngine;

public class IncreaseAttackSpdItemPickupBehaviour : MonoBehaviour, ItemPickupBehaviour
{
    public delegate void OnPlayerAttack(string item);
    public static event OnPlayerAttack OnPlayerAttackEvent;
    public string ApplyItemEffects()
    {
        if (OnPlayerAttackEvent != null)
        {
            OnPlayerAttackEvent("IncreaseAttackSpeedItem");
        }
        return "¡Velocidad de ataque aumentada!";
    }
}
