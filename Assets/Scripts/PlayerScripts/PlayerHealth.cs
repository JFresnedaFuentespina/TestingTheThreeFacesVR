using System;
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
    public float extraHealth = 0f;
    public float minHealth = 0f;
    public float healthPoints = 3f;
    public float extraHealthPoints = 0f;
    [Header("Health UI")]
    public GameObject heartContainer;
    public Sprite fullHeartSprite;
    public Sprite halfHeartSprite;
    public Sprite emptyHeartSprite;
    public Image bloodFrame;
    public AudioClip hitAudioClip;
    public AudioSource audioSource;
    private List<Image> hearts = new List<Image>();
    private List<Image> extraHearts = new List<Image>();
    public Sprite extraHeartSprite;
    public Sprite halfExtraHeart;

    public GameObject hud;
    private List<GameObject> corazones = new List<GameObject>();
    public bool canDie = false;
    private Rigidbody rb;
    private Animator animatorEsqueleto;
    private Animator animatorFantasma;
    private RotateCharacterToMouse rotateCharacterToMouse;
    private RotateCharacterWithJoystick rotateCharacterWithJoystick;
    private PlayerBehaviour playerBehaviour;
    private ChangeCharacter changeCharacter;

    private GameObject endgameManagerGO;
    private EndgameManager endgameManager;

    private GameObject lastHittedBy;

    void OnDestroy()
    {
        CantoDeathBehaviour.OnVictoryEvent -= BlockPlayerControl;
        HeartItemPickupBehaviour.OnHealthIncreasedEvent -= IncreaseMaxHealth;
        SkullItemPickupBehaviour.OnHealthDecreasedEvent -= DecreaseMaxHealth;
        HeartItemPickupBehaviour.OnFullyHealedEvent -= FullHeal;
        BluePillItemPickupBehaviour.OnSoulHeartEvent -= AddExtraHeart;
        DialogueManager.OnRestoreHealthEvent -= RefreshHearts;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rotateCharacterToMouse = GetComponent<RotateCharacterToMouse>();
        rotateCharacterWithJoystick = GetComponent<RotateCharacterWithJoystick>();
        playerBehaviour = GetComponent<PlayerBehaviour>();
        changeCharacter = GetComponent<ChangeCharacter>();
        endgameManagerGO = GameObject.Find("EndgameManagerGO");

        Transform esqueletoHijo = transform.Find("Esqueleto");
        animatorEsqueleto = esqueletoHijo != null ? esqueletoHijo.GetComponent<Animator>() : null;

        Transform ghostHijo = transform.Find("Ghost");
        animatorFantasma = ghostHijo != null ? ghostHijo.GetComponent<Animator>() : null;
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
                    extraHealthPoints = data.extraHealth;
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
        if (extraHealthPoints > 0)
        {
            RefreshExtraHearts();
        }
        Invoke(nameof(EnableDeath), 0.1f);
        SubscribeToPickupEvents();
    }

    public void SubscribeToPickupEvents()
    {
        CantoDeathBehaviour.OnVictoryEvent += BlockPlayerControl;
        HeartItemPickupBehaviour.OnHealthIncreasedEvent += IncreaseMaxHealth;
        SkullItemPickupBehaviour.OnHealthDecreasedEvent += DecreaseMaxHealth;
        HeartItemPickupBehaviour.OnFullyHealedEvent += FullHeal;
        BluePillItemPickupBehaviour.OnSoulHeartEvent += AddExtraHeart;
        DialogueManager.OnRestoreHealthEvent += RefreshHearts;
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
            lastHittedBy = other.gameObject;
            Damage();
        }
    }
    public void CheckDeath()
    {
        if (!canDie || healthPoints > 0) return;

        if (!changeCharacter.showingGhost)
            animatorEsqueleto.SetTrigger("Death");
        else
            animatorFantasma.SetTrigger("Death");

        // Bloquear movimiento y rotación
        BlockPlayerControl();

        // Borrar JSON si existe
        string path = Application.persistentDataPath + "/player.json";
        if (File.Exists(path))
            File.Delete(path);

        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
            col.enabled = false;

        // ⬇️ NO mostramos el panel aún
        StartCoroutine(WaitAndShowEndgame());
    }

    private IEnumerator WaitAndShowEndgame()
    {
        Animator currentAnimator = changeCharacter.showingGhost
            ? animatorFantasma
            : animatorEsqueleto;

        float animDuration = currentAnimator
            .GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(animDuration + 2f);

        endgameManager = endgameManagerGO.GetComponent<EndgameManager>();
        if (endgameManager != null)
        {
            PlayerInventory playerInventory = GetComponent<PlayerInventory>();
            endgameManager.ShowEndgameDeath(lastHittedBy, playerInventory.inventory);
        }
    }

    public void BlockPlayerControl()
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
        float deathDuration = animatorEsqueleto.GetCurrentAnimatorStateInfo(0).length;
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
        RefreshExtraHearts();

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

    private void RefreshExtraHearts()
    {
        float remainingExtraHp = extraHealthPoints;
        for (int i = 0; i < extraHearts.Count; i++)
        {
            if (remainingExtraHp >= 1f)
            {
                extraHearts[i].enabled = true;
                remainingExtraHp -= 1f;
            }

            else
            {
                extraHearts[i].enabled = false;
            }
        }
    }


    public void RebuildHearts()
    {
        InitializeHearts();
        RefreshHearts();
    }


    public void Damage(float amount = 0.5f)
    {
        if (audioSource != null)
        {
            audioSource.PlayOneShot(hitAudioClip);
        }

        if (extraHealthPoints > 0)
        {
            extraHealthPoints -= 1f;
            extraHealthPoints = Mathf.Clamp(extraHealthPoints, 0f, extraHealth);
        }
        else if (healthPoints > 0)
        {
            healthPoints -= amount;
            healthPoints = Mathf.Clamp(healthPoints, 0f, maxHealth);
        }
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

    public void AddExtraHeart(float amount)
    {
        extraHealth += amount;
        extraHealthPoints += amount; // agregamos vida extra real

        int heartsToAdd = Mathf.RoundToInt(amount);
        for (int i = 0; i < heartsToAdd; i++)
        {
            GameObject newExtraHeart = new GameObject(
                "ExtraHeart_" + extraHearts.Count,
                typeof(Image)
            );
            newExtraHeart.transform.SetParent(heartContainer.transform, false);
            Image img = newExtraHeart.GetComponent<Image>();
            img.sprite = extraHeartSprite;
            img.SetNativeSize();
            extraHearts.Add(img);
        }

        RefreshExtraHearts();
    }

}
