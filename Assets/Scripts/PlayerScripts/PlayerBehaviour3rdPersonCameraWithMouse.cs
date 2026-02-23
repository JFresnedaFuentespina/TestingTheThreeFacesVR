using UnityEngine;

public class PlayerBehaviour3rdPersonCameraWithMouse : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float movementSpeed = 2f;
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;
    public float verticalRotation = 0f;
    private Rigidbody rb;
    public Animator animatorEsqueleto;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up * mouseX * mouseSensitivity);
    }
    void FixedUpdate()
    {
        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");

        Vector3 movement =
            (transform.forward * inputV + transform.right * inputH).normalized *
            movementSpeed;

        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);

        // Parámetros para Blend Tree
        // Velocidad vertical: adelante (+) o atrás (-)
        animatorEsqueleto.SetFloat("Vertical", inputV * movementSpeed);
        // Velocidad horizontal: izquierda/derecha si quieres strafing
        animatorEsqueleto.SetFloat("Horizontal", inputH * movementSpeed);
    }

}
