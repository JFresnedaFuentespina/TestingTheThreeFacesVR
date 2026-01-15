// AttackHit.cs
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class FireAttackHit : MonoBehaviour
{
    public float attackDamage = 5;

    public float fireballPushForce = 1f;

    void Start()
    {
        PlayerAttack playerAttack = FindAnyObjectByType<PlayerAttack>();
        if (playerAttack != null)
        {
            attackDamage = playerAttack.attackDamage;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        float destroyDelay = 0f;
        if (gameObject.CompareTag("Thunderbolt"))
        {
            destroyDelay = 0.5f;
        }
        EnemyLife enemyLife = other.GetComponent<EnemyLife>();
        if (other.CompareTag("BossCara"))
        {
            CaraAI caraAi = other.GetComponent<CaraAI>();
            if (caraAi != null)
            {
                enemyLife.Damage(attackDamage);
                caraAi.ReactToHit();
                enemyLife.UpdateIsAlive();
                if(!enemyLife.GetIsAlive())
                {
                    caraAi.ReactToDeath();
                }
            }
            Destroy(gameObject, destroyDelay);
        }
        else if (other.CompareTag("BossCruz"))
        {
            CruzAI cruzAI = other.GetComponent<CruzAI>();
            if (cruzAI != null)
            {
                enemyLife.Damage(attackDamage);
                enemyLife.UpdateIsAlive();
            }
            Destroy(gameObject, destroyDelay);
        }
        else if (other.CompareTag("BossCanto"))
        {
            CantoAI cantoAI = other.GetComponent<CantoAI>();
            CantoMovement cantoMovement = other.GetComponent<CantoMovement>();
            if (cantoAI != null && cantoMovement != null)
            {
                enemyLife.Damage(attackDamage);
                cantoMovement.ReactToHit();
                enemyLife.UpdateIsAlive();
                if(!enemyLife.GetIsAlive())
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
