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
    private CruzAnimatorController cruzAI;
    private Animator animator;
    private CruzDialogManager dialogManager;
    private EnemyLife enemyLife;

    private bool isWalking = false;
    private bool isAttacking = false;
    private bool isFinishingAttack = false;

    private enum AttackType { None, Punch2, Punch3, Throw }
    private AttackType currentAttack = AttackType.None;
    public CruzBallAttack cruzBallAttack;

    void Start()
    {
        enemyLife = GetComponent<EnemyLife>();
        dialogManager = GetComponent<CruzDialogManager>();

        agent = GetComponent<NavMeshAgent>();
        agent.speed = 0f;

        BuscarJugador();
        cruzAI = GetComponent<CruzAnimatorController>();
        animator = cruzAI.animator;
    }

    void Update()
    {
        if (player == null) return;

        if (currentAttack != AttackType.Throw)
        {
            transform.LookAt(player.transform);
        }

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

        if (enemyLife.currentHp <= 0f)
        {
            cruzAI.SetDeath();
            dialogManager.ShowDeathDialog();
            enabled = false;
        }
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
                agent.ResetPath();
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
                cruzAI.SetThrow();
                cruzBallAttack.active = true;
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

        AttackType finishedAttack = currentAttack;

        isAttacking = false;
        currentAttack = AttackType.None;

        agent.speed = moveSpeed;
        agent.isStopped = false;

        if (finishedAttack == AttackType.Throw)
        {
            cruzBallAttack.active = false;
            Debug.Log("CRUZ BALL ATTACK ACTIVE? " + cruzBallAttack.active);
        }
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
