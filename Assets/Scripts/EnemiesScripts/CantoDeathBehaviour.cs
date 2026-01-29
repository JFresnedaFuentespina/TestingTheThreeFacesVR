using System;
using UnityEngine;

public class CantoDeathBehaviour : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static event Action OnVictoryEvent;
    void Start()
    {
        
    }

    public void NotifyVictory()
    {
        OnVictoryEvent?.Invoke();
    }
}
