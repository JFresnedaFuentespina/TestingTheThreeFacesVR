using UnityEngine;

public class HourglassItemPickupBehaviour : MonoBehaviour, ItemPickupBehaviour
{
    public delegate void OnNewChangeCharacterAction(string action);
    public static event OnNewChangeCharacterAction OnNewChangeCharacterActionEvent;
    public string ApplyItemEffects()
    {
        if (OnNewChangeCharacterActionEvent != null)
        {
            OnNewChangeCharacterActionEvent("Hourglass");
        }
        return "Ralentiza a los enemigos al girar la moneda";
    }
}
