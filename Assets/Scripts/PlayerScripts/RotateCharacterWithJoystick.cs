using UnityEngine;
using UnityEngine.InputSystem;

public class RotateCharacterWithJoystick : MonoBehaviour
{
    public float velocidadRotacion = 1440f;

    private Rigidbody rb;
    private PlayerInputActions input;
    private Vector2 lookDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            Debug.LogError("RotateCharacter: No tiene Rigidbody");

        input = new PlayerInputActions();
    }

    void OnEnable()
    {
        input.Enable();

        // Suscribirse a la acción correcta
        input.Player.LookDirection.performed += OnLook;
        input.Player.LookDirection.canceled += OnLookCanceled;
    }

    void OnDisable()
    {
        input.Player.LookDirection.performed -= OnLook;
        input.Player.LookDirection.canceled -= OnLookCanceled;
        input.Disable();
    }

    private void OnLook(InputAction.CallbackContext ctx)
    {
        lookDirection = ctx.ReadValue<Vector2>();
    }

    private void OnLookCanceled(InputAction.CallbackContext ctx)
    {
        lookDirection = Vector2.zero;
    }

    void FixedUpdate()
    {
        if (lookDirection.sqrMagnitude < 0.01f)
            return;

        float angulo = Mathf.Atan2(lookDirection.x, lookDirection.y) * Mathf.Rad2Deg;
        Quaternion rotacionObjetivo = Quaternion.Euler(0f, angulo, 0f);

        rb.MoveRotation(
            Quaternion.RotateTowards(
                rb.rotation,
                rotacionObjetivo,
                velocidadRotacion * Time.fixedDeltaTime
            )
        );
    }
}
