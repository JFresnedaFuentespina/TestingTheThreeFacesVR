using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CruzMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float punch3MoveSpeed = 6f;
    public float spawnDelay = 2.5f;
    public float attackDistance = 3f;
    public float attackCooldown = 2f;

    private bool hasSpawned = false;
    private float spawnTimer = 0f;
    private float attackTimer = 0f;

    private GameObject player;
    private NavMeshAgent agent;
    private CruzAI cruzAI;
    private Animator animator;

    private bool isWalking = false;
    private bool isAttacking = false;
    private bool isFinishingAttack = false;

    private enum AttackType { None, Punch2, Punch3, Throw }
    private AttackType currentAttack = AttackType.None;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = 0f;

        BuscarJugador();
        cruzAI = GetComponent<CruzAI>();
        animator = cruzAI.animator;
    }

    void Update()
    {
        if (player == null) return;

        transform.LookAt(player.transform);

        spawnTimer += Time.deltaTime;
        attackTimer += Time.deltaTime;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        // Spawn inicial
        if (!hasSpawned && spawnTimer >= spawnDelay)
        {
            hasSpawned = true;
            StartWalking();
        }

        // Intentar atacar
        TryAttack(distance);

        // Movimiento según estado
        UpdateMovement(distance);
    }

    private void TryAttack(float distance)
    {
        if (!isAttacking && attackTimer >= attackCooldown && distance <= attackDistance)
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        isAttacking = true;
        isWalking = false;
        attackTimer = 0f;

        agent.ResetPath();
        agent.isStopped = false;

        cruzAI.SetWalking(false);
        cruzAI.ResetAttackTriggers();

        int randomAttack = Random.Range(0, 3);

        switch (randomAttack)
        {
            case 0: // Punch2
                currentAttack = AttackType.Punch2;
                agent.speed = 0f;
                cruzAI.SetPunch2();
                StartCoroutine(WaitForAttack(animator.GetCurrentAnimatorStateInfo(0).length));
                break;

            case 1: // Punch3
                currentAttack = AttackType.Punch3;
                agent.speed = punch3MoveSpeed;
                cruzAI.SetPunch3();
                StartCoroutine(WaitForAttack(animator.GetCurrentAnimatorStateInfo(0).length));
                break;

            case 2: // Throw
                currentAttack = AttackType.Throw;
                agent.speed = 0f;
                cruzAI.SetThrow();
                StartCoroutine(WaitForAttack(animator.GetCurrentAnimatorStateInfo(0).length));
                break;
        }
    }

    private IEnumerator WaitForAttack(float duration)
    {
        isFinishingAttack = true;
        yield return new WaitForSeconds(duration);
        FinishAttack();
        isFinishingAttack = false;
    }

    private void UpdateMovement(float distance)
    {
        if (isAttacking)
        {
            // Movimiento durante ataque
            switch (currentAttack)
            {
                case AttackType.Punch2:
                case AttackType.Throw:
                    agent.isStopped = true;
                    break;
                case AttackType.Punch3:
                    agent.isStopped = false;
                    agent.SetDestination(player.transform.position);
                    break;
            }
        }
        else if (isWalking)
        {
            agent.isStopped = false;
            agent.speed = moveSpeed;
            agent.SetDestination(player.transform.position);
        }
        else
        {
            agent.isStopped = true;
        }
    }

    private void FinishAttack()
    {
        if (!isFinishingAttack) return;

        isAttacking = false;
        currentAttack = AttackType.None;
        agent.speed = moveSpeed;
        agent.isStopped = false;

        StartWalking();
    }

    private void StartWalking()
    {
        if (isAttacking) return;

        isWalking = true;
        agent.isStopped = false;
        cruzAI.SetWalking(true);
    }

    private void BuscarJugador()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
}
