using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public float velocity = 0.5f;
    private float originalVelocity;
    private GameObject mainCharacter;
    private float fixedY;
    private Rigidbody rb;

    public bool isAlive = true;
    void Start()
    {
        fixedY = transform.position.y;
        BuscarJugador();
        rb = GetComponent<Rigidbody>();
        // Guardamos la velocidad original
        originalVelocity = (gameObject.CompareTag("BossCara")) ? 2f : velocity;
        velocity = originalVelocity;
    }

    void Update()
    {
        if (isAlive)
        {
            if (mainCharacter == null)
            {
                BuscarJugador();
                if (mainCharacter == null) return; // aún no existe
            }

            // Dirección horizontal hacia el jugador
            Vector3 targetPos = mainCharacter.transform.position;
            Vector3 direction = targetPos - transform.position;
            direction.y = 0; // ignorar altura
            direction.Normalize();

            // Mover usando Rigidbody
            Vector3 horizontalVelocity = direction * velocity;
            Vector3 currentVelocity = rb.linearVelocity;
            rb.linearVelocity = new Vector3(horizontalVelocity.x, currentVelocity.y, horizontalVelocity.z);

            // Girar suavemente hacia el jugador
            Vector3 lookPos = targetPos - transform.position;
            lookPos.y = 0;
            if (lookPos.sqrMagnitude > 0.001f)
            {
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 10f * Time.deltaTime);
            }
        }
    }

    public void Jump(float jumpForce = 5f)
    {
        if (rb == null) { return; }

        if (Mathf.Abs(rb.linearVelocity.y) < 0.1f)
        {
            jumpForce = Mathf.Clamp(jumpForce, 4f, 7f);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            velocity = originalVelocity;
        }
    }

    public void RestoreSpeed()
    {
        velocity = originalVelocity;
    }


    private void BuscarJugador()
    {
        mainCharacter = GameObject.Find("Character(Clone)");
        if (mainCharacter == null)
            mainCharacter = GameObject.FindGameObjectWithTag("Player");
    }
}
