using UnityEngine;

public class BombItemPickupBehaviour : MonoBehaviour, ItemPickupBehaviour
{
    public delegate void OnNewChangeCharacterAction(string action);
    public static event OnNewChangeCharacterAction OnNewChangeCharacterActionEvent;
    public string ApplyItemEffects()
    {
        if (OnNewChangeCharacterActionEvent != null)
        {
            OnNewChangeCharacterActionEvent("Bomb");
        }
        return "¡Bomba recogida!";
    }
}
