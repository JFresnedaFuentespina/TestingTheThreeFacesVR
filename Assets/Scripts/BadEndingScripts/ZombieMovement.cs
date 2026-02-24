using UnityEngine;
using UnityEngine.AI;

public class ZombieMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private NavMeshAgent agent;
    private Animator animator;

    public float wanderRadius = 20f;
    public float waitTime = 2f;

    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        timer = waitTime;
        SetNewDestination();
    }

    void Update()
    {
        HandleAnimation();

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            timer += Time.deltaTime;

            if (timer >= waitTime)
            {
                SetNewDestination();
                timer = 0f;
            }
        }
    }

    void SetNewDestination()
    {
        Vector3 randomPoint = Random.insideUnitSphere * wanderRadius;
        randomPoint += transform.position;

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void HandleAnimation()
    {
        float currentSpeed = agent.velocity.magnitude;
        float normalizedSpeed = agent.speed > 0f ? currentSpeed / agent.speed : 0f;

        animator.SetFloat("speed", normalizedSpeed);
    }
}
