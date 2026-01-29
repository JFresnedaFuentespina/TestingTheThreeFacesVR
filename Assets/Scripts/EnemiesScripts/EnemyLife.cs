using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyLife : MonoBehaviour
{
    // Start is called before the first frame update
    public float totalHp = 10f;
    public float currentHp;
    private bool isAlive = true;
    public GameObject healthBar;
    private Image fillImage;
    private float deathDelay = 2.5f;
    public AudioClip hitAudioClip;
    public AudioClip defaultAudioClip;
    public AudioClip deathAudioClip;
    public AudioSource audioSource;
    public bool poisoned = false;
    public GameObject enemiesDeathCounterGO;
    public EnemiesDeathCounter enemiesDeathCounter;

    void Start()
    {
        enemiesDeathCounterGO = GameObject.Find("EnemiesDeathCounterGO");
        if (enemiesDeathCounterGO != null)
        {
            enemiesDeathCounter = enemiesDeathCounterGO.GetComponent<EnemiesDeathCounter>();
        }

        audioSource.PlayOneShot(defaultAudioClip);
        currentHp = totalHp;
        if (healthBar != null)
        {
            Transform fillTransform = healthBar.transform.Find("Canvas/Fill");
            if (fillTransform != null)
                fillImage = fillTransform.GetComponent<Image>();
        }
    }

    public void Damage(float hit)
    {
        if (!isAlive) return;

        // Reproducir audio de hit
        if (audioSource != null && hitAudioClip != null)
        {
            StartCoroutine(PlayHitThenDefault());
        }

        currentHp -= hit;
        currentHp = Mathf.Clamp(currentHp, 0f, totalHp);
        UpdateHealthBar();
        UpdateIsAlive();

        if (poisoned)
        {
            StartCoroutine(PoisonDamage(hit * 0.2f));
            poisoned = false;
        }
    }

    private IEnumerator PlayHitThenDefault()
    {
        if (audioSource == null || hitAudioClip == null)
            yield break;

        audioSource.Stop();
        audioSource.loop = false;
        audioSource.clip = hitAudioClip;
        audioSource.Play();

        yield return new WaitForSeconds(hitAudioClip.length);

        if (audioSource != null && defaultAudioClip != null && isAlive)
        {
            audioSource.clip = defaultAudioClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private IEnumerator PoisonDamage(float damage)
    {
        int ticks = 3;
        float delay = 1f; // tiempo entre ticks

        for (int i = 0; i < ticks; i++)
        {
            if (!isAlive) yield break;

            yield return new WaitForSeconds(delay);

            currentHp -= damage;
            currentHp = Mathf.Clamp(currentHp, 0f, totalHp);


            UpdateHealthBar();
            UpdateIsAlive();
        }
        poisoned = true;
    }


    public void UpdateIsAlive()
    {
        if (currentHp <= 0 && isAlive)
        {
            isAlive = false;
            Die();
        }
    }

    public bool GetIsAlive()
    {
        return isAlive;
    }

    void UpdateHealthBar()
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = currentHp / totalHp;
        }
    }

    public void Die()
    {
        if (audioSource != null && deathAudioClip != null)
        {
            audioSource.PlayOneShot(deathAudioClip);
        }

        if (gameObject.tag.Contains("Boss"))
        {
            deathDelay = 5f;
        }
        else
        {
            gameObject.GetComponent<Collider>().enabled = false;
        }
        enemiesDeathCounter.counter++;
        if(gameObject.GetComponent<CantoDeathBehaviour>() != null)
        {
            gameObject.GetComponent<CantoDeathBehaviour>().NotifyVictory();
        }
        Destroy(gameObject, deathDelay);
    }
}
