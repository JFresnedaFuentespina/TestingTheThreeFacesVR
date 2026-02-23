using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class GetClassifications : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private List<ScoreResponse> classifications;
    private ApiDTO apiData;
    // public GameObject classificationsTable;
    public GameObject contentContainer;
    public GameObject headerContainer;
    public GameObject classificationItemPrefab;
    public GameObject classificationItemHeaderPrefab;
    public GameObject loadingPanel;
    private bool isLoading = true;
    void Start()
    {
        apiData = new ApiDTO();
        classifications = new List<ScoreResponse>();
        StartCoroutine(ShowLoadingPanel());
        StartCoroutine(GetClassificationsFromAPI());
    }

    private IEnumerator ShowLoadingPanel()
    {
        if (isLoading)
        {
            loadingPanel.SetActive(true);
        }
        else
        {
            loadingPanel.SetActive(false);
        }
        yield return null;
    }

    private IEnumerator GetClassificationsFromAPI()
    {
        string url = apiData.apiUrl + "/api/classification/" + apiData.apiToken;

        UnityWebRequest httpRequest = UnityWebRequest.Get(url);
        httpRequest.SetRequestHeader("Accept", "application/json");

        yield return httpRequest.SendWebRequest();

        if (httpRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + httpRequest.error);
            yield break;
        }

        ClassificationResponse response = JsonConvert.DeserializeObject<ClassificationResponse>(httpRequest.downloadHandler.text);
        classifications = response.data;

        foreach (ScoreResponse score in classifications)
        {
            Debug.Log("Player: " + score.name + " - Score: " + score.puntuacion);
        }
        DisplayClassifications();
    }


    public void DisplayClassifications()
    {
        // Limpia hijos anteriores (por si recargas)
        foreach (Transform child in contentContainer.transform)
        {
            Destroy(child.gameObject);
        }

        Instantiate(classificationItemHeaderPrefab, headerContainer.transform); // Instancia el encabezado de la tabla

        foreach (ScoreResponse score in classifications)
        {
            GameObject item = Instantiate(classificationItemPrefab, contentContainer.transform);

            ClassificationItemUI itemUI = item.GetComponent<ClassificationItemUI>();
            itemUI.Setup(score.name, score.puntuacion);
        }
        isLoading = false;
        loadingPanel.SetActive(false);
    }

}
