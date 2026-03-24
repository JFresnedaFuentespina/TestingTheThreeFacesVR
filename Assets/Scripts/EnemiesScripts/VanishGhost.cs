using System.Collections;
using UnityEngine;

public class VanishGhost : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private EnemyLife enemyLife;
    public float fadeDuration = 2.0f;
    void Start()
    {
        enemyLife = GetComponent<EnemyLife>();
        if (enemyLife == null)
        {
            Debug.LogError("EnemyLife component not found on the GameObject.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyLife.GetIsAlive() == false)
        {
            StartCoroutine(ShrinkAndDestroy());
            this.enabled = false;
        }
    }
    private IEnumerator ShrinkAndDestroy(float duration = 1f)
    {
        Vector3 originalScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            yield return null;
        }
    }
}

