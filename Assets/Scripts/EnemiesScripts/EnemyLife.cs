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

    void Start()
    {
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
        if (audioSource != null && hitAudioClip != null)
        {
            audioSource.PlayOneShot(hitAudioClip);
        }
        audioSource.PlayOneShot(defaultAudioClip);
        currentHp -= hit;
        currentHp = Mathf.Clamp(currentHp, 0f, totalHp);

        UpdateHealthBar();
        UpdateIsAlive();
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
        if(gameObject.tag.Contains("Boss"))
        {
            deathDelay = 5f;
        }
        if (audioSource != null && deathAudioClip != null)
        {
            audioSource.PlayOneShot(deathAudioClip);
        }
        Destroy(gameObject, deathDelay);
    }
}
