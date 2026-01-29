using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndgameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("DeathCondition")]
    public TextMeshProUGUI killedByTxt;
    public TextMeshProUGUI enemiesKilledTxtDeath;
    public Button exitButtonDeath;
    public Button restartButtonDeath;
    public GameObject inventoryPanelDeath;
    public GameObject endgameDeathPanel;

    [Header("WinCondition")]
    public GameObject timerGO;
    public TextMeshProUGUI timerTxt;
    public TextMeshProUGUI enemiesKilledTxtWin;
    public Button exitButtonWin;
    public Button restartButtonWin;
    public GameObject inventoryPanelWin;
    public GameObject endgameWinPanel;

    [Header("PauseMenuManager")]
    public GameObject pauseMenuManager;

    public static event System.Action OnResetGameData;

    void Start()
    {
        exitButtonDeath.onClick.AddListener(ExitGame);
        restartButtonDeath.onClick.AddListener(RestartGame);

        exitButtonWin.onClick.AddListener(ExitGame);
        restartButtonWin.onClick.AddListener(RestartGame);
    }
    void OnEnable()
    {
        PlayerInventory.OnInventoryReadyForVictory += ShowEndgameVictory;
    }

    void OnDisable()
    {
        PlayerInventory.OnInventoryReadyForVictory -= ShowEndgameVictory;
    }


    public void ShowEndgameDeath(GameObject enemy, Inventory inventory)
    {
        float enemyKilledCount = GameObject.Find("EnemiesDeathCounterGO").GetComponent<EnemiesDeathCounter>().counter;
        string enemyName = enemy.name;
        switch (enemy.tag)
        {
            case "Enemy_Zombie":
                enemyName = "Zombie";
                break;
            case "Enemy_Ghost":
                enemyName = "Fantasma";
                break;
            case "EnemyProjectile":
                enemyName = "Fantasma";
                break;
            case "BossCara":
                enemyName = "Cara";
                break;
            case "BossCruz":
                enemyName = "Cruz";
                break;
            case "BossCanto":
                enemyName = "Canto";
                break;
        }
        pauseMenuManager.GetComponent<ShowPauseMenu>().enabled = false;
        endgameDeathPanel.SetActive(true);
        killedByTxt.text += " " + enemyName;
        enemiesKilledTxtDeath.text = "Mataste a " + enemyKilledCount + " enemigos!";
        ShowInventory(inventory, false);
    }

    public void ShowEndgameVictory(Inventory inventory)
    {
        pauseMenuManager.GetComponent<ShowPauseMenu>().enabled = false;
        float enemyKilledCount = GameObject.Find("EnemiesDeathCounterGO").GetComponent<EnemiesDeathCounter>().counter;
        endgameWinPanel.SetActive(true);
        float min = timerGO.GetComponent<GameTimer>().min;
        float sec = timerGO.GetComponent<GameTimer>().sec;
        timerGO.GetComponent<GameTimer>().PauseTimer();
        timerTxt.text = "Completaste el juego en " + min + " minutos y " + sec + " segundos!";
        enemiesKilledTxtWin.text = "Mataste a " + enemyKilledCount + " enemigos!";
        ShowInventory(inventory, true);
    }

    public void ExitGame()
    {
        ResetFiles();
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartGame()
    {
        ResetFiles();
        SceneManager.LoadScene("Level1Scene");
    }

    public void ResetFiles()
    {
        string path = Application.persistentDataPath + "/player.json";
        if (File.Exists(path))
            File.Delete(path);

        string timerPath = Application.persistentDataPath + "/timer.json";
        if (File.Exists(timerPath))
            File.Delete(timerPath);

        OnResetGameData?.Invoke();
    }


    public void ShowInventory(Inventory inventory, bool isWin)
    {
        GameObject inventoryPanel = isWin ? inventoryPanelWin : inventoryPanelDeath;
        // Limpiar iconos anteriores
        foreach (Transform child in inventoryPanel.transform)
            Destroy(child.gameObject);

        // Crear un Image por cada InventoryItem
        foreach (var item in inventory.items)
        {
            if (item.icon == null) continue;

            GameObject iconGO = new GameObject(item.itemID, typeof(RectTransform), typeof(Image));
            iconGO.transform.SetParent(inventoryPanel.transform, false);

            Image img = iconGO.GetComponent<Image>();
            img.sprite = item.icon;
            img.SetNativeSize();

            img.rectTransform.sizeDelta = new Vector2(80, 80);
        }
    }
}
