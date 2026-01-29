using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class LoadLevel1 : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public VideoClip loreVideo;
    public Button playButton;

    public float fadeDuration = 0.5f;

    private GameObject loadingPanel;
    private Image fadeImage;
    private bool isLoading = false;

    void Start()
    {
        // Buscar LoadingCanvas y sus hijos
        GameObject loadingCanvas = GameObject.Find("LoadingCanvas");
        if (loadingCanvas != null)
        {
            Transform loadingTransform = loadingCanvas.transform.Find("LoadingPanel");
            if (loadingTransform != null)
                loadingPanel = loadingTransform.gameObject;

            Transform fadeTransform = loadingCanvas.transform.Find("Fade");
            if (fadeTransform != null)
                fadeImage = fadeTransform.GetComponent<Image>();
        }

        if (loadingPanel != null)
            loadingPanel.SetActive(false);

        if (fadeImage != null)
            fadeImage.gameObject.SetActive(false);

        playButton.onClick.AddListener(ShowLoreVideo);
        videoPlayer.loopPointReached += OnVideoEnd;

        if (loreVideo != null)
            videoPlayer.clip = loreVideo;
    }

    void Update()
    {
        if (!isLoading && videoPlayer.isPlaying && Input.GetKeyDown(KeyCode.Space))
        {
            SkipVideo();
        }
    }

    public void ShowLoreVideo()
    {
        videoPlayer.clip = null; //! quitar el clip temporalmente hasta ver si es compatible con WEBGL
        if (videoPlayer.clip == null)
        {
            StartCoroutine(PreTransitionFade());
            return;
        }

        videoPlayer.Play();
    }

    public void SkipVideo()
    {
        if (isLoading) return;

        isLoading = true;
        StartCoroutine(PreTransitionFade());
    }

    private IEnumerator PreTransitionFade()
    {
        // if (loadingPanel == null || fadeImage == null)
        // {
        //     SceneManager.LoadScene("Level1Scene");
        //     yield break;
        // }

        // // Activar UI de carga
        // loadingPanel.SetActive(true);
        // fadeImage.gameObject.SetActive(true);
        // fadeImage.transform.SetAsLastSibling();

        // // Alpha inicial
        // fadeImage.color = new Color(0f, 0f, 0f, 0f);

        yield return null;

        // Cargar escena async
        yield return StartCoroutine(LoadLevelAsync());
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

    private IEnumerator LoadLevelAsync()
    {
        string path = Application.persistentDataPath + "/player.json";
        if (File.Exists(path)) File.Delete(path);

        string timerPath = Application.persistentDataPath + "/timer.json";
        if (File.Exists(timerPath)) File.Delete(timerPath);

        AsyncOperation op = SceneManager.LoadSceneAsync("Level1Scene");
        op.allowSceneActivation = false;

        // Mientras se carga, seguimos cediendo frames a Unity
        // while (op.progress < 0.9f)
        // {
        //     // Aquí podrías actualizar barra de progreso: op.progress
        //     yield return null; // deja que Unity dibuje spinner
        // }

        // Pequeña espera para asegurarnos que el último frame se dibuje
        yield return null;

        // Activar la escena
        op.allowSceneActivation = true;
    }



    private void OnVideoEnd(VideoPlayer vp)
    {
        SkipVideo();
    }
}
