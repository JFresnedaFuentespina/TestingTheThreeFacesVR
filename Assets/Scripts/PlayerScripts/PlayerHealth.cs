using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 3f;
    public float minHealth = 0f;
    public float healthPoints = 3;
    [Header("Health UI")]
    public GameObject heartContainer;
    public Sprite fullHeartSprite;
    public Sprite halfHeartSprite;
    public Sprite emptyHeartSprite;
    public Image bloodFrame;
    public AudioClip hitAudioClip;
    public AudioSource audioSource;
    private List<Image> hearts = new List<Image>();

    public GameObject hud;
    private List<GameObject> corazones = new List<GameObject>();
    public bool canDie = false;
    private Rigidbody rb;
    private Animator animator;
    private RotateCharacterToMouse rotateCharacterToMouse;
    private RotateCharacterWithJoystick rotateCharacterWithJoystick;
    private PlayerBehaviour playerBehaviour;

    void OnDestroy()
    {
        PickupItem.OnFullyHealedEvent -= FullHeal;
        PickupItem.OnHealthIncreasedEvent -= IncreaseMaxHealth;
        PickupItem.OnHealthDecreasedEvent -= DecreaseMaxHealth;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rotateCharacterToMouse = GetComponent<RotateCharacterToMouse>();
        rotateCharacterWithJoystick = GetComponent<RotateCharacterWithJoystick>();
        playerBehaviour = GetComponent<PlayerBehaviour>();

        Transform esqueletoHijo = transform.Find("Esqueleto");
        animator = esqueletoHijo != null ? esqueletoHijo.GetComponent<Animator>() : null;

        // Cargar JSON
        string path = Application.persistentDataPath + "/player.json";
        bool loadedFromFile = false;
        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                PlayerData data = JsonConvert.DeserializeObject<PlayerData>(json);

                if (data != null && data.maxHealth > 0)
                {
                    maxHealth = data.maxHealth;
                    healthPoints = Mathf.Clamp(data.health, 0f, data.maxHealth);
                    loadedFromFile = true;
                }
            }
            catch
            {
                Debug.LogWarning("Error cargando JSON de vida");
            }
        }

        if (!loadedFromFile)
            healthPoints = maxHealth;

        // Buscar HUD dinámicamente
        if (heartContainer == null)
        {
            Canvas[] all = FindObjectsOfType<Canvas>();
            foreach (var c in all)
            {
                if (c.gameObject.name == "HUD")
                {
                    heartContainer = c.transform.Find("HealthPoints")?.gameObject;
                    break;
                }
            }
        }

        if (heartContainer == null)
        {
            Debug.LogError("No se encontró Healthpoints2 en el HUD");
            return;
        }

        bloodFrame = GameObject.Find("BloodFrame")?.GetComponent<Image>();
        if (bloodFrame == null)
        {
            Debug.LogWarning("No se encontró BloodFrame en el HUD");
        }

        audioSource = GetComponent<AudioSource>();

        InitializeHearts();
        RefreshHearts();
        Invoke(nameof(EnableDeath), 0.1f);
        SubscribeToPickupEvents();
    }

    public void SubscribeToPickupEvents()
    {
        PickupItem.OnFullyHealedEvent += FullHeal;
        PickupItem.OnHealthIncreasedEvent += IncreaseMaxHealth;
        PickupItem.OnHealthDecreasedEvent += DecreaseMaxHealth;
    }

    void EnableDeath() => canDie = true;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy_Zombie")
            || other.gameObject.CompareTag("Enemy_Ghost")
            || other.gameObject.CompareTag("BossCara")
            || other.gameObject.CompareTag("BossCruz")
            || other.gameObject.CompareTag("EnemyProjectile")
            )
        {
            Damage();
        }
    }
    public void CheckDeath()
    {
        if (!canDie || healthPoints > 0) return;

        animator.SetTrigger("Death");

        // Bloquear movimiento y rotación
        BlockPlayerControl();

        // Borrar JSON si existe
        string path = Application.persistentDataPath + "/player.json";
        if (File.Exists(path))
            File.Delete(path);

        // Volver al menú tras delay
        StartCoroutine(DeathAndReturnToMenu());
    }

    void BlockPlayerControl()
    {
        if (playerBehaviour != null)
            playerBehaviour.enabled = false;

        if (rotateCharacterToMouse != null)
            rotateCharacterToMouse.enabled = false;

        if (rotateCharacterWithJoystick != null)
            rotateCharacterWithJoystick.enabled = false;
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    private IEnumerator DeathAndReturnToMenu()
    {
        float deathDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(deathDuration + 5f);
        SceneManager.LoadScene("MainMenu");
    }

    private void InitializeHearts()
    {
        // Limpiar si había algo antes
        foreach (Transform child in heartContainer.transform)
            Destroy(child.gameObject);

        hearts.Clear();

        // Por cada punto de vida (1f = un corazón completo)
        for (int i = 0; i < (int)maxHealth; i++)
        {
            GameObject newHeart = new GameObject("Heart_" + i, typeof(Image));

            newHeart.transform.SetParent(heartContainer.transform, false);

            Image img = newHeart.GetComponent<Image>();
            img.sprite = emptyHeartSprite;

            hearts.Add(img);
        }
    }


    public void UpdateHUD(bool checkDeath = true)
    {
        RefreshHearts();

        if (checkDeath)
            CheckDeath();
    }
    private void RefreshHearts()
    {
        float hp = healthPoints;

        for (int i = 0; i < hearts.Count; i++)
        {
            if (hp >= 1f)
            {
                hearts[i].sprite = fullHeartSprite;
                hp -= 1f;
            }
            else if (hp >= 0.5f)
            {
                hearts[i].sprite = halfHeartSprite;
                hp -= 0.5f;
            }
            else
            {
                hearts[i].sprite = emptyHeartSprite;
            }
        }
    }
    public void RebuildHearts()
    {
        InitializeHearts();
        RefreshHearts();
    }

    public void Damage()
    {
        audioSource.PlayOneShot(hitAudioClip);
        healthPoints -= 0.5f;
        healthPoints = Mathf.Clamp(healthPoints, minHealth, maxHealth);
        UpdateHUD();
        BlinkBloodFrame();
    }

    public void BlinkBloodFrame()
    {
        if (bloodFrame == null)
        {
            return;
        }
        StartCoroutine(BlinkBloodCoroutine());
    }

    IEnumerator BlinkBloodCoroutine()
    {
        if (bloodFrame == null) yield break;

        float blinkDuration = 0.6f; // duración total
        int blinkCount = 2;          // número de “subidas y bajadas”
        float halfDuration = blinkDuration / (2f * blinkCount);

        Color originalColor = bloodFrame.color;
        Color transparent = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        Color opaque = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);

        for (int i = 0; i < blinkCount; i++)
        {
            // Fade in
            float t = 0f;
            while (t < halfDuration)
            {
                bloodFrame.color = Color.Lerp(transparent, opaque, t / halfDuration);
                t += Time.unscaledDeltaTime;
                yield return null;
            }
            bloodFrame.color = opaque;

            // Fade out
            t = 0f;
            while (t < halfDuration)
            {
                bloodFrame.color = Color.Lerp(opaque, transparent, t / halfDuration);
                t += Time.unscaledDeltaTime;
                yield return null;
            }
            bloodFrame.color = transparent;
        }

        bloodFrame.color = transparent; // asegurar que queda invisible
    }


    public void IncreaseMaxHealth(float amount)
    {
        maxHealth += amount;
        RebuildHearts();
    }

    public void FullHeal()
    {
        healthPoints = maxHealth;
        RebuildHearts();
    }

    public void DecreaseMaxHealth(float amount)
    {
        maxHealth -= amount;
        RebuildHearts();
    }

}
