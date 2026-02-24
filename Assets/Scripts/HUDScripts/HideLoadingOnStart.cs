using UnityEngine;

public class HideLoadingOnStart : MonoBehaviour
{
    void Start()
    {
        GameObject loadingCanvas = GameObject.Find("LoadingCanvas");
        if (loadingCanvas != null)
        {
            loadingCanvas.SetActive(false);
        }
    }
}
