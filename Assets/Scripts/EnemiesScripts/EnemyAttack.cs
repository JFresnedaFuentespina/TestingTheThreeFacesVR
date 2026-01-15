using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject projectilePrefab;
    public EnemyLife enemyLife;
    public float attackInterval = 2.0f;
    private float attackTimer = 0.0f;
    public float projectileSpeed = 2.0f;

    // Update is called once per frame
    void Update()
    {
        enemyLife = GetComponent<EnemyLife>();
        if (attackTimer < attackInterval)
        {
            attackTimer += Time.deltaTime;
        }
        else
        {
            Attack();
            attackTimer = 0.0f;
        }
    }

    private void Attack()
    {
        if (projectilePrefab == null) return;
        if (enemyLife.GetIsAlive() == false) return;

        Vector3 spawnPos = transform.position + transform.forward + Vector3.up * 0.3f;

        GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        Collider myCol = GetComponent<Collider>() ?? GetComponentInChildren<Collider>();
        Collider projCol = projectile.GetComponent<Collider>();

        if (myCol != null && projCol != null)
        {
            Physics.IgnoreCollision(projCol, myCol, true);
        }

        FireballBehaviour fireballBehaviour = projectile.GetComponent<FireballBehaviour>();
        if (fireballBehaviour != null)
        {
            fireballBehaviour.direction = transform.forward;
            fireballBehaviour.speed = projectileSpeed;
        }
    }


}
