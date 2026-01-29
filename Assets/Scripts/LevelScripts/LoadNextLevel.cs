using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Newtonsoft.Json;
using System.IO;
using TMPro;
using System.Collections.Generic;

public class LoadNextLevel : MonoBehaviour
{
    public GameObject loadingPanel;
    public Image fadeImage;

    public GameObject timer;

    public float fadeDuration = 1f;

    void Start()
    {
        // Buscar el canvas de carga
        GameObject loadingCanvas = GameObject.Find("LoadingCanvas");
        if (loadingCanvas != null)
        {
            // Buscar los hijos por nombre dentro del canvas
            Transform loadingTransform = loadingCanvas.transform.Find("LoadingPanel");
            if (loadingTransform != null)
                loadingPanel = loadingTransform.gameObject;

            Transform fadeTransform = loadingCanvas.transform.Find("Fade");
            if (fadeTransform != null)
                fadeImage = fadeTransform.GetComponent<Image>();
        }

        // Verificar asignaciones
        if (fadeImage == null)
            Debug.LogWarning("Fade Image no asignado correctamente.");
        if (loadingPanel == null)
            Debug.LogWarning("Loading Panel no asignado correctamente.");
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        RemoveKeyFromPlayer();
        SavePlayerStats(other.gameObject);
        StartCoroutine(PreTransitionFade());
    }

    public void RemoveKeyFromPlayer()
    {
        PlayerInventory playerInventory = FindFirstObjectByType<PlayerInventory>();
        if (playerInventory != null)
        {
            playerInventory.RemoveItem("Key");
        }
    }


    private IEnumerator PreTransitionFade()
    {
        if (fadeImage == null || loadingPanel == null)
        {
            Debug.LogWarning("Fade o LoadingPanel no asignado correctamente.");
            yield break;
        }

        // Activar panel de carga y fade antes de cualquier fade para que se vea correctamente
        loadingPanel.SetActive(true);
        fadeImage.gameObject.SetActive(true);
        fadeImage.transform.SetAsLastSibling(); // asegurar que esté al frente

        // Alpha inicial 0
        fadeImage.color = new Color(0f, 0f, 0f, 0f);

        // Fade a negro
        yield return StartCoroutine(Fade(0f, 1f));

        // Cargar siguiente escena
        NextLevel();
    }

    public void NextLevel()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        string nextScene = "";

        switch (currentScene)
        {
            case "Level1Scene":
                nextScene = "Level2Scene";
                break;
            case "Level2Scene":
                nextScene = "Level3Scene";
                break;
            case "Level3Scene"://! Temporal, llevar a MainMenu
                nextScene = "MainMenu";
                break;
            default:
                return;
        }

        StartCoroutine(LoadSceneWithFade(nextScene));
    }

    private IEnumerator LoadSceneWithFade(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
            yield return null;

        op.allowSceneActivation = true;

        yield return null;

        // Fade-in en la nueva escena
        yield return StartCoroutine(Fade(1f, 0f));

        loadingPanel.SetActive(false);
        fadeImage.gameObject.SetActive(false);
    }

    private IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        Color c = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(from, to, t / fadeDuration);
            fadeImage.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }

        fadeImage.color = new Color(c.r, c.g, c.b, to);
    }

    public void SavePlayerStats(GameObject playerGO)
    {
        var data = new PlayerData();
        GameObject player = playerGO.transform.root.gameObject;
        var atk = player.GetComponent<PlayerAttack>();
        var bh = player.GetComponent<PlayerBehaviour>();
        var hp = player.GetComponent<PlayerHealth>();
        var changeCharacter = player.GetComponent<ChangeCharacter>();

        float enemiesDeathCounterFloat = 0f;

        EnemiesDeathCounter enemiesDeathCounter = GameObject.Find("EnemiesDeathCounterGO").GetComponent<EnemiesDeathCounter>();
        if (enemiesDeathCounter != null)
        {
            enemiesDeathCounterFloat = enemiesDeathCounter.counter;
        }

        data.maxHealth = hp.maxHealth;
        data.health = hp.healthPoints;
        data.extraHealth = hp.extraHealthPoints;
        data.velocity = bh.velocity;
        data.damage = atk.attackDamage;
        data.attackInterval = atk.attackInterval;
        data.attackRange = atk.attackRange;
        data.attackType = atk.isFireball ? "Fireball" : "Thunder";
        data.actions = changeCharacter.GetUnlockedActions();
        data.enemiesDeathCounter = enemiesDeathCounterFloat;
        data.appliesPoison = atk.appliesPoison;

        string json = JsonConvert.SerializeObject(data);
        string path = Application.persistentDataPath + "/player.json";
        File.WriteAllText(path, json);

    }

}
