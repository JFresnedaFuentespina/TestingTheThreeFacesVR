using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SocialPlatforms.Impl;

public class PostScore : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private UserDTO user;
    private ScoreDTO scoreData;

    void Start()
    {
        user = new UserDTO();
        scoreData = new ScoreDTO();
    }

    public void PostScoreToAPI()
    {
        // Cargar el usuario desde user.json
        string userPath = Application.persistentDataPath + "/user.json";
        if (File.Exists(userPath))
        {
            string json = File.ReadAllText(userPath);
            user = JsonUtility.FromJson<UserDTO>(json);
        }
        else
        {
            Debug.LogWarning("User data not found at: " + userPath);
            user.email = "testing@testing.com";
            user.name = "Testing User";
        }

        // Cargar la puntuación desde score.json
        string scorePath = Application.persistentDataPath + "/score.json";
        if (File.Exists(scorePath))
        {
            string json = File.ReadAllText(scorePath);
            scoreData = JsonUtility.FromJson<ScoreDTO>(json);
        }
        else
        {
            Debug.LogWarning("Score data not found at: " + scorePath);
            scoreData.score = 10f;
        }
        StartCoroutine(TryPostScore());
    }

    private IEnumerator TryPostScore()
    {
        if (user != null && scoreData != null)
        {
            ApiDTO apiData = new ApiDTO();
            ScoreBody scoreBody = new ScoreBody
            {
                api_token = apiData.apiToken,
                name = user.name,
                puntuacion = scoreData.score
            };

            Debug.Log("Posting score: " + scoreBody.name + " - " + scoreBody.puntuacion);

            UnityWebRequest httpClient = new UnityWebRequest();
            httpClient.method = UnityWebRequest.kHttpVerbPOST;
            httpClient.url = apiData.apiUrl + "/api/classification";
            httpClient.SetRequestHeader("Content-Type", "application/json");
            httpClient.SetRequestHeader("Accept", "application/json");

            string jsonData = JsonUtility.ToJson(scoreBody);
            byte[] dataToSend = Encoding.UTF8.GetBytes(jsonData);

            httpClient.uploadHandler = new UploadHandlerRaw(dataToSend);
            httpClient.downloadHandler = new DownloadHandlerBuffer();

            yield return httpClient.SendWebRequest();
            if (httpClient.result == UnityWebRequest.Result.ConnectionError || httpClient.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Error: " + httpClient.error);
            }
            else
            {
                Debug.Log(httpClient.downloadHandler.text);
            }
            httpClient.Dispose();
        }
        yield return null;
    }
}
