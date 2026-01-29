using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorManager : MonoBehaviour
{
    public Texture2D swordCursor;
    public Texture2D crossCursor;
    public Texture2D normalCursor;
    public Vector2 hotspot = Vector2.zero;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Cursor.SetCursor(normalCursor, hotspot, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(swordCursor, hotspot, CursorMode.Auto);
        }
    }

    public void ChangeCursorToCross()
    {
        Cursor.SetCursor(crossCursor, hotspot, CursorMode.Auto);
    }

    public void ChangeCursorToSword()
    {
        Cursor.SetCursor(swordCursor, hotspot, CursorMode.Auto);
    }
}
