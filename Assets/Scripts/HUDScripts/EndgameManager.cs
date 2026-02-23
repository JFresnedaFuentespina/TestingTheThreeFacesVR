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

        foreach (Transform child in inventoryPanel.transform)
            Destroy(child.gameObject);

        GridLayoutGroup grid = inventoryPanel.GetComponent<GridLayoutGroup>();
        RectTransform panelRect = inventoryPanel.GetComponent<RectTransform>();

        int itemCount = inventory.items.Count;
        if (itemCount == 0) return;

        // --- CONFIGURACIÓN ---
        int columns = Mathf.CeilToInt(Mathf.Sqrt(itemCount));
        int rows = Mathf.CeilToInt((float)itemCount / columns);

        float spacingX = grid.spacing.x;
        float spacingY = grid.spacing.y;

        float totalWidth = panelRect.rect.width - (spacingX * (columns - 1)) - grid.padding.left - grid.padding.right;
        float totalHeight = panelRect.rect.height - (spacingY * (rows - 1)) - grid.padding.top - grid.padding.bottom;

        float cellWidth = totalWidth / columns;
        float cellHeight = totalHeight / rows;

        grid.cellSize = new Vector2(cellWidth, cellHeight);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;

        // --- Crear iconos ---
        foreach (var item in inventory.items)
        {
            if (item.icon == null) continue;

            GameObject iconGO = new GameObject(item.itemID, typeof(RectTransform), typeof(Image));
            iconGO.transform.SetParent(inventoryPanel.transform, false);

            Image img = iconGO.GetComponent<Image>();
            img.sprite = item.icon;
            img.preserveAspect = true;
        }
    }
}
