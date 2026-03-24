using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float maxHealth;
    public float health;
    public float extraHealth;
    public float velocity;
    public float damage;
    public float attackInterval;
    public float attackRange;
    public string attackType;
    public List<string> inventoryItems;
    public List<string> actions;
    public float enemiesDeathCounter;
    public bool appliesPoison;

    public string PlayerDataStr()
    {
        return "Max Health: " + maxHealth + ", Health: " + health
                + ", Velocity: " + velocity + ", Damage: " + damage
                + ", AttackInterval: " + attackInterval
                + ", AttackType: " + attackType;
    }
}
