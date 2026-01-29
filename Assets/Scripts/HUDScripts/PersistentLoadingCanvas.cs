using UnityEngine;

public class PersistentLoadingCanvas : MonoBehaviour
{
    private static PersistentLoadingCanvas instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
