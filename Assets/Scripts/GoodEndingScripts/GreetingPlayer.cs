using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GreetingPlayer : MonoBehaviour
{
    public Transform raycastOrigin;
    public Animator animator;
    public float rayDistance = 2f;
    public float rayRadius = 2f;

    private NavMeshAgent agent;
    private bool isGreeting;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (isGreeting) return;

        if (Physics.SphereCast(
            raycastOrigin.position,
            rayRadius,
            raycastOrigin.forward,
            out RaycastHit hit,
            rayDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                StartCoroutine(GreetCoroutine());
            }
        }
    }

    IEnumerator GreetCoroutine()
    {
        isGreeting = true;
        agent.isStopped = true;

        animator.ResetTrigger("greet");
        animator.SetTrigger("greet");

        yield return null;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        while (stateInfo.normalizedTime < 1f)
        {
            yield return null;
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        }

        agent.isStopped = false;
        isGreeting = false;
    }
}
