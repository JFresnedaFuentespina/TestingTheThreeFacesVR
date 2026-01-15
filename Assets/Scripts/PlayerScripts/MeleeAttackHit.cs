using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MeleeAttackHit : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float attackDamage;
    // public float pushForce = 10f;
    private void OnTriggerEnter(Collider other)
    {
        EnemyLife enemyLife = other.GetComponent<EnemyLife>();
        if (other.CompareTag("BossCara"))
        {
            CaraAI caraAi = other.GetComponent<CaraAI>();
            if (caraAi != null)
            {
                enemyLife.Damage(attackDamage);
                enemyLife.UpdateIsAlive();
            }
        }
        else if (other.CompareTag("BossCruz"))
        {
            CruzAI cruzAI = other.GetComponent<CruzAI>();
            if (cruzAI != null)
            {
                enemyLife.Damage(attackDamage);
                enemyLife.UpdateIsAlive();
            }
        }
        else if (other.CompareTag("BossCanto"))
        {
            CantoAI cantoAI = other.GetComponent<CantoAI>();
            if (cantoAI != null)
            {
                enemyLife.Damage(attackDamage);
                enemyLife.UpdateIsAlive();
            }
        }
        else if (other.CompareTag("Enemy_Zombie") || other.CompareTag("Enemy_Ghost"))
        {
            if (enemyLife != null)
            {
                enemyLife.Damage(attackDamage);
                enemyLife.UpdateIsAlive();
            }
        }
        if (other.CompareTag("BossCara") || other.CompareTag("BossCruz") || other.CompareTag("BossCanto") || other.CompareTag("Enemy_Zombie") || other.CompareTag("Enemy_Ghost"))
        {
            // Empujar enemigos al ser golpeados por ataque cuerpo a cuerpo
            if (other.GetComponent<NavMeshAgent>() != null)
            {
                other.GetComponent<EnemyMoveNavmesh>().SetStunned(2f);
            }
        }
    }


}