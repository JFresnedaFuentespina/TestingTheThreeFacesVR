using System.Collections;
using UnityEngine;

public class MoveCameraAfterDialogue : MonoBehaviour
{
    public float moveDistance = 6f;   // cuánto sube la cámara
    public float moveDuration = 1f;   // cuánto tarda
    private bool isMoving = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isMoving)
        {
            StartCoroutine(MoveCameraUp());
        }
    }

    IEnumerator MoveCameraUp()
    {
        isMoving = true;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * moveDistance;

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveDuration;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        transform.position = endPos;
        isMoving = false;
    }
}