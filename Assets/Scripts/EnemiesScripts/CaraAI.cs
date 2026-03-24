using System.Collections;
using UnityEngine;

public class CaraAI : MonoBehaviour
{
    private float distanceToPlayerFloat;
    private Animator animator;
    private EnemyMove enemyMove;

    private bool hasJumped = false;       // indica si está actualmente en el aire
    private bool wasInAir = false;        // para detectar aterrizaje real
    private bool isBeingHit = false;
    private bool jumpOnCooldown = false;  // evita saltos dobles inmediatos

    void Start()
    {
        animator = GetComponent<Animator>();
        enemyMove = GetComponent<EnemyMove>();
        animator.applyRootMotion = false;
    }

    void Update()
    {
        // Raycast hacia el suelo para detectar si está en el aire
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.55f);
        animator.SetBool("isGrounded", isGrounded);

        // Detectar aterrizaje real
        if (!isGrounded)
        {
            wasInAir = true;
        }
        else if (isGrounded && wasInAir)
        {
            // Ha aterrizado este frame
            wasInAir = false;
            hasJumped = false;           // permite saltar de nuevo
            enemyMove.RestoreSpeed();

            // Animación de aterrizar
            animator.ResetTrigger("Jump");
            animator.SetTrigger("Land");

            // Forzar animación de caminar tras aterrizar
            animator.SetFloat("Action", 0, 0.1f, Time.deltaTime);
        }

        // Raycast hacia adelante al jugador
        Ray rayForward = new Ray(transform.position, transform.forward);
        bool rayHitPlayer = Physics.Raycast(rayForward, out RaycastHit hit, 20f);

        if (!rayHitPlayer || !hit.collider.CompareTag("Player"))
            return;

        distanceToPlayerFloat = (hit.point - transform.position).magnitude;

        // Solo decidir nuevas acciones si no está en el aire
        if (!wasInAir)
        {
            if (distanceToPlayerFloat > 8)
            {
                animator.SetFloat("Action", 2, 0.2f, Time.deltaTime); // idle
            }
            else if (distanceToPlayerFloat > 5)
            {
                animator.SetFloat("Action", 0, 0.2f, Time.deltaTime); // caminar
            }
            else if (distanceToPlayerFloat > 1 && distanceToPlayerFloat <= 5
                     && !hasJumped && !jumpOnCooldown && isGrounded && !isBeingHit)
            {
                // Saltar solo si está en el suelo y no está en cooldown
                animator.ResetTrigger("Hit");
                animator.SetTrigger("Jump");
                enemyMove.Jump(7f);
                hasJumped = true;
                wasInAir = true;

                // Iniciar cooldown para evitar saltos repetidos inmediatos
                StartCoroutine(JumpCooldown());
            }
            else if (distanceToPlayerFloat <= 1)
            {
                animator.SetFloat("Action", 4, 0.2f, Time.deltaTime); // ataque
            }
        }
    }

    public void ReactToHit()
    {
        if (wasInAir || isBeingHit)
            return;
        StartCoroutine(ReactToHitCoroutine());
    }

    private IEnumerator ReactToHitCoroutine()
    {
        isBeingHit = true;

        animator.ResetTrigger("Hit");
        animator.SetTrigger("Hit");

        enemyMove.velocity = 0f;

        yield return new WaitForSeconds(1f);

        enemyMove.RestoreSpeed();
        isBeingHit = false;
    }


    public void ReactToDeath()
    {
        enemyMove.isAlive = false;
        animator.SetTrigger("Death");
    }

    private IEnumerator JumpCooldown()
    {
        jumpOnCooldown = true;           // bloquea saltos
        yield return new WaitForSeconds(12f); // duración del cooldown
        jumpOnCooldown = false;          // permite saltar nuevamente
    }
}
