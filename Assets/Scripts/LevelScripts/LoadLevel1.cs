using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel1 : MonoBehaviour
{
    // Esta función se llamará al hacer clic en el botón
    public void CargarNivel1()
    {
        string path = Application.persistentDataPath + "/player.json";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        string timerPath = Application.persistentDataPath + "/timer.json";
        if (File.Exists(timerPath))
        {
            File.Delete(timerPath);
        }
        SceneManager.LoadScene("Level1Scene");
    }
}
