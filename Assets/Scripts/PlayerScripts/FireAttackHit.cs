// AttackHit.cs
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class FireAttackHit : MonoBehaviour
{
    public float attackDamage = 5;

    public float fireballPushForce = 1f;
    public bool appliesPoison = false;
    public PlayerAttack playerAttack;

    void Start()
    {
        playerAttack = FindAnyObjectByType<PlayerAttack>();
        if (playerAttack != null)
        {
            attackDamage = playerAttack.attackDamage;
            appliesPoison = playerAttack.appliesPoison;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        appliesPoison = playerAttack.appliesPoison;
        float destroyDelay = 0f;
        if (gameObject.CompareTag("Thunderbolt"))
        {
            destroyDelay = 0.5f;
        }
        EnemyLife enemyLife = other.GetComponent<EnemyLife>();
        if (appliesPoison && enemyLife != null)
        {
            enemyLife.poisoned = true;
        }
        if (other.CompareTag("BossCara"))
        {
            CaraAnimatorController caraAi = other.GetComponent<CaraAnimatorController>();
            if (caraAi != null)
            {
                enemyLife.Damage(attackDamage);
                caraAi.ReactToHit();
                enemyLife.UpdateIsAlive();
                if (!enemyLife.GetIsAlive())
                {
                    caraAi.ReactToDeath();
                }
            }
            Destroy(gameObject, destroyDelay);
        }
        else if (other.CompareTag("BossCruz"))
        {
            CruzAnimatorController cruzAI = other.GetComponent<CruzAnimatorController>();
            if (cruzAI != null)
            {
                enemyLife.Damage(attackDamage);
                enemyLife.UpdateIsAlive();
            }
            Destroy(gameObject, destroyDelay);
        }
        else if (other.CompareTag("BossCanto"))
        {
            CantoAnimatorController cantoAI = other.GetComponent<CantoAnimatorController>();
            CantoMovement cantoMovement = other.GetComponent<CantoMovement>();
            if (cantoAI != null && cantoMovement != null)
            {
                enemyLife.Damage(attackDamage);
                cantoMovement.ReactToHit();
                enemyLife.UpdateIsAlive();
                if (!enemyLife.GetIsAlive())
                {
                    cantoMovement.ReactToDeath();
                }
            }
            Destroy(gameObject, destroyDelay);
        }
        else if (other.CompareTag("Enemy_Zombie") || other.CompareTag("Enemy_Ghost"))
        {
            if (enemyLife != null)
            {
                enemyLife.Damage(attackDamage);
                enemyLife.UpdateIsAlive();
            }
            Destroy(gameObject, destroyDelay);
        }
        else if (other.CompareTag("Pared"))
        {
            Destroy(gameObject, destroyDelay);
        }
        // Empujar enemigos al ser golpeados por una bola de fuego
        if (gameObject.CompareTag("Fireball"))
        {
            EnemyMoveNavmesh enemyMove = other.GetComponent<EnemyMoveNavmesh>();
            if (enemyMove != null)
            {
                Vector3 pushDirection = other.transform.position - transform.position;
                enemyMove.GetPushed(pushDirection, fireballPushForce, 0.2f);
            }
        }
    }

}
