using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public float rotationSpeed = 20f; // grados por segundo

    void Update()
    {
        Vector3 currentRotation = transform.localEulerAngles;

        float x = currentRotation.x;
        if (x > 180f) x -= 360f;

        float newX = Mathf.MoveTowards(x, 0f, rotationSpeed * Time.deltaTime);

        transform.localEulerAngles = new Vector3(
            newX,
            currentRotation.y,
            currentRotation.z
        );
    }
}
