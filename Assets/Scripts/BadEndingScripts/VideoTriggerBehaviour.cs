using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

public class VideoTriggerBehaviour : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    public PostScore postScoreScript;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer == null)
        {
            Debug.LogError("No hay VideoPlayer en este GameObject");
            return;
        }

        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = Application.streamingAssetsPath + "/BadEndgame.mp4";

        videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
        videoPlayer.targetCamera = GameObject.Find("Camera").GetComponent<Camera>();
        videoPlayer.targetCameraAlpha = 1f;

        videoPlayer.loopPointReached += OnVideoFinished;
    }


    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Debug.Log("Player entered the bad ending trigger area.");

        other.gameObject.SetActive(false);
        postScoreScript.PostScoreToAPI();
        StartCoroutine(PrepareAndPlay());
    }
    private IEnumerator PrepareAndPlay()
    {
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }
        videoPlayer.Play();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene("Classifications");
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoFinished;
    }
}
