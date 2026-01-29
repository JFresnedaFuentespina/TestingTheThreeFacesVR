using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombAttackHit : MonoBehaviour
{
    public float bombDamage = 20f;
    public float bombRadius = 15f;
    public float waveDuration = 1f;
    public LayerMask enemyLayer;

    private HashSet<EnemyLife> damagedEnemies = new HashSet<EnemyLife>();

    void Start()
    {
        enemyLayer = LayerMask.GetMask("Enemy");
        StartCoroutine(BombWave());
    }

    IEnumerator BombWave()
    {
        float elapsed = 0f;
        while (elapsed < waveDuration)
        {
            float currentRadius = Mathf.Lerp(0f, bombRadius, elapsed / waveDuration);

            Collider[] hitColliders = Physics.OverlapSphere(
                transform.position,
                currentRadius,
                enemyLayer
            );

            foreach (Collider hitCollider in hitColliders)
            {
                EnemyLife enemyLife = hitCollider.GetComponent<EnemyLife>();
                if (enemyLife == null) continue;

                if (damagedEnemies.Contains(enemyLife)) continue;
                damagedEnemies.Add(enemyLife);
                enemyLife.Damage(bombDamage);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}

