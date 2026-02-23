using UnityEngine;
using UnityEngine.UI;

public class ClassificationsButtonsManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Button exitButton;
    public Button restartButton;
    void Start()
    {
        Cursor.visible = true;
        exitButton.onClick.AddListener(ExitGame);
        restartButton.onClick.AddListener(RestartGame);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
