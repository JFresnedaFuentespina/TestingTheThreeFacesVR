using UnityEngine;

public class CruzBallAttack : MonoBehaviour
{
    public GameObject fireballPrefab;
    public bool active = false;
    public Transform spawnPoint;

    public float radius = 1f;
    public float fireRate = 0.5f;
    public float initialDelay = 1.5f;

    private float timer = 0f;
    private float delayTimer = 0f;
    private bool canShoot = false;

    void Update()
    {
        if (!active)
        {
            // Resetear cuando se desactiva
            delayTimer = 0f;
            timer = 0f;
            canShoot = false;
            return;
        }

        // Esperar delay inicial
        if (!canShoot)
        {
            delayTimer += Time.deltaTime;

            if (delayTimer >= initialDelay)
            {
                canShoot = true;
                delayTimer = 0f;
            }

            return;
        }

        // Disparo normal
        timer += Time.deltaTime;

        if (timer >= fireRate)
        {
            ShootFireball();
            timer = 0f;
        }
    }

    void ShootFireball()
    {
        Vector3 randomDir = Random.insideUnitSphere * radius;
        Vector3 spawnPos = spawnPoint.position + randomDir;

        GameObject fireball = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);

        Vector3 dir = (spawnPos - spawnPoint.position).normalized;

        FireballBehaviour fb = fireball.GetComponent<FireballBehaviour>();
        if (fb != null)
        {
            fb.speed = 2f;
            fb.direction = dir;
        }
    }
}