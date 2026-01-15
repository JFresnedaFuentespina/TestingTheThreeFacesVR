using UnityEngine;

public class EnemyFireAttackHit : MonoBehaviour
{
    public float attackDamage = 0.5f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth ph = other.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            ph.Damage();
        }

        Destroy(gameObject); // destruir proyectil tras impactar al Player
    }
}
