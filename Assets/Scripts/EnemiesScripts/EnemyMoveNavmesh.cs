using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMoveNavmesh : MonoBehaviour
{
    public float velocity = 1f;
    private GameObject mainCharacter;
    private NavMeshAgent agent;

    // Empuje
    private bool isPushed = false;
    private Vector3 pushDirection;
    private float pushForce;
    private float pushDuration;
    private float pushElapsed;
    private float stunnedSpeed = 0f;
    private EnemyLife enemyLife;

    void Start()
    {
        enemyLife = GetComponent<EnemyLife>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = velocity;
        agent.angularSpeed = 120f;
        agent.acceleration = 8f;
        agent.stoppingDistance = 0.5f;

        agent.updateRotation = true;
        agent.updateUpAxis = true;

        BuscarJugador();
        if (mainCharacter != null)
        {
            agent.SetDestination(mainCharacter.transform.position);
        }
    }

    void Update()
    {
        if (!enemyLife.GetIsAlive())
        {
            agent.isStopped = true;
            return;
        }
        if (isPushed)
        {
            // Aplicar empuje con física
            transform.position += pushDirection * pushForce * Time.deltaTime;
            pushElapsed += Time.deltaTime;
            if (pushElapsed >= pushDuration)
            {
                isPushed = false;
                agent.isStopped = false; // Reanudar navegación
            }
            return; // No hacemos navegación mientras es empujado
        }

        if (mainCharacter == null) return;
        // Solo actualizamos destino si el jugador se ha movido lo suficiente
        if (Vector3.Distance(agent.destination, mainCharacter.transform.position) > 0.5f)
        {
            agent.SetDestination(mainCharacter.transform.position);
        }
    }

    public void GetPushed(Vector3 direction, float force, float duration)
    {
        pushDirection = direction.normalized;
        pushForce = force;
        pushDuration = duration;
        pushElapsed = 0f;

        isPushed = true;
        agent.isStopped = true;
    }

    public void SetStunned(float duration)
    {
        StartCoroutine(StunCoroutine(duration));
    }

    IEnumerator StunCoroutine(float duration)
    {
        float originalSpeed = agent.speed;
        agent.speed = stunnedSpeed;
        yield return new WaitForSeconds(duration);
        agent.speed = originalSpeed;
    }

    private void BuscarJugador()
    {
        mainCharacter = GameObject.Find("Character(Clone)") ?? GameObject.FindGameObjectWithTag("Player");
    }
}

