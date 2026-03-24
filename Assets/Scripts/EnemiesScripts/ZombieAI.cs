using UnityEngine;
using System.Collections;

public class ZombieAI : MonoBehaviour
{
    public ZombieAnimatorManager animatorManager;
    public EnemyLife enemyLife;
    public float minAttackInterval = 2f;
    public float maxAttackInterval = 5f;

    private bool canAttack = true;

    void Start()
    {
        animatorManager = GetComponent<ZombieAnimatorManager>();
        if (animatorManager == null)
            Debug.LogError("ZombieAnimatorManager component not found on ZombieAI.");

        enemyLife = GetComponent<EnemyLife>();
        if (enemyLife == null)
            Debug.LogError("EnemyLife component not found on ZombieAI.");

        StartCoroutine(RandomAttackRoutine());
    }

    void Update()
    {
        if (!enemyLife.GetIsAlive())
        {
            animatorManager.SetDeath();
            canAttack = false;
        }
    }

    private IEnumerator RandomAttackRoutine()
    {
        while (true)
        {
            if (canAttack)
            {
                float waitTime = Random.Range(minAttackInterval, maxAttackInterval);
                yield return new WaitForSeconds(waitTime);

                if (enemyLife.GetIsAlive())
                {
                    animatorManager.SetAttack();
                }
            }
            else
            {
                yield return null;
            }
        }
    }
}
