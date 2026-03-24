using UnityEngine;

public class HealthBarLookAtCamera : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main; // toma la cámara principal
    }

    void LateUpdate()
    {
        // Hace que el objeto apunte hacia la cámara
        if (mainCamera != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
        }
    }
}
