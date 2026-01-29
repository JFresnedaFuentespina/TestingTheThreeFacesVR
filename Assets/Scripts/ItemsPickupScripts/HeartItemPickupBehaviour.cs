using UnityEngine;

public class HeartItemPickupBehaviour : MonoBehaviour, ItemPickupBehaviour
{
    public delegate void OnHealthIncreased(float amunt);
    public static event OnHealthIncreased OnHealthIncreasedEvent;
    public delegate void OnFullyHealed();
    public static event OnFullyHealed OnFullyHealedEvent;
    public string ApplyItemEffects()
    {
        if (OnHealthIncreasedEvent != null)
        {
            OnHealthIncreasedEvent(1f);
        }
        if(OnFullyHealedEvent != null)
        {
            OnFullyHealedEvent();
        }
        return "¡Vida máxima aumentada!";
    }
}
