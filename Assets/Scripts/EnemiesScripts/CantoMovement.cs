using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CantoMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float spawnDelay = 2.5f;
    public float attackDistance = 3f;
    public float attackCooldown = 2f;

    private bool hasSpawned = false;
    private float spawnTimer = 0f;
    private float attackTimer = 0f;

    private GameObject player;
    private NavMeshAgent agent;
    private CantoAnimatorController cantoAI;
    private Animator animator;
    private EnemyLife enemyLife;
    private CantoDialogueManager cantoDialogueManager;

    private bool isWalking = false;
    private bool isAttacking = false;
    private bool isFinishingAttack = false;
    private bool magicAttackCasted = false;

    private enum AttackType { None, Attack1, Attack2, Attack3, Attack4 }
    private AttackType currentAttack = AttackType.None;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;

        cantoDialogueManager = GetComponent<CantoDialogueManager>();

        BuscarJugador();
        cantoAI = GetComponent<CantoAnimatorController>();
        animator = cantoAI.animator;
        enemyLife = GetComponent<EnemyLife>();
    }

    void Update()
    {
        if (player == null) return;
        if (enemyLife.GetIsAlive() == false) return;

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
        // Ataques normales
        if (!isAttacking && attackTimer >= attackCooldown && distance <= attackDistance)
        {
            StartAttack();
        }

        // Ataque mágico al 50% de vida
        if (enemyLife != null
            && enemyLife.currentHp <= enemyLife.totalHp * 0.5f
            && !magicAttackCasted)
        {

            magicAttackCasted = true;
            isAttacking = true;
            isWalking = false;
            attackTimer = 0f;

            agent.ResetPath();
            agent.isStopped = false;

            cantoAI.SetWalking(false);
            cantoAI.SetCastMagicAttack();

            StartCoroutine(
                WaitForAttack(animator.GetCurrentAnimatorStateInfo(0).length)
            );
        }
    }


    private void StartAttack()
    {
        isAttacking = true;
        isWalking = false;
        attackTimer = 0f;

        agent.ResetPath();
        agent.isStopped = false;

        cantoAI.SetWalking(false);

        int randomAttack = Random.Range(1, 5);

        switch (randomAttack)
        {
            case 1:
                cantoAI.SetAttack(1);
                agent.speed = 0f;
                currentAttack = AttackType.Attack1;
                StartCoroutine(WaitForAttack(animator.GetCurrentAnimatorStateInfo(0).length));
                break;
            case 2:
                cantoAI.SetAttack(2);
                agent.speed = 0f;
                currentAttack = AttackType.Attack2;
                StartCoroutine(WaitForAttack(animator.GetCurrentAnimatorStateInfo(0).length));
                break;
            case 3:
                cantoAI.SetAttack(3);
                agent.speed = 0f;
                currentAttack = AttackType.Attack3;
                StartCoroutine(WaitForAttack(animator.GetCurrentAnimatorStateInfo(0).length));
                break;
            case 4:
                cantoAI.SetAttack(4);
                agent.speed = 0f;
                currentAttack = AttackType.Attack4;
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
                case AttackType.Attack1:
                    agent.isStopped = true;
                    break;
                case AttackType.Attack2:
                    agent.isStopped = true;
                    break;
                case AttackType.Attack3:
                    agent.isStopped = true;
                    break;
                case AttackType.Attack4:
                    agent.isStopped = true;
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
    public void ReactToHit()
    {
        if (isAttacking) return;
        StartCoroutine(ReactToHitCoroutine(0.9f));
    }

    public IEnumerator ReactToHitCoroutine(float hitDuration)
    {
        isWalking = false;
        agent.isStopped = true;
        cantoAI.SetWalking(false);
        cantoAI.SetHit();
        yield return new WaitForSeconds(hitDuration);
        StartWalking();
    }

    public void ReactToDeath()
    {
        isWalking = false;
        isAttacking = false;
        cantoDialogueManager.ShowDeathDialog();
        cantoAI.SetWalking(false);
        cantoAI.SetDeath();
        agent.isStopped = true;
        agent.ResetPath();
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
        cantoAI.SetWalking(true);
    }
    private void BuscarJugador()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
}
