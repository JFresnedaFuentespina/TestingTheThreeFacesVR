using TMPro;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System.Collections;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    private float elapsedTime = 0f;
    private bool isRunning = false;
    private Coroutine timerCoroutine;

    private string timerPath;

    void Awake()
    {
        timerPath = Application.persistentDataPath + "/timer.json";

        if (File.Exists(timerPath))
        {
            try
            {
                string json = File.ReadAllText(timerPath);
                TimerData data = JsonConvert.DeserializeObject<TimerData>(json);
                if (data != null)
                {
                    elapsedTime = data.time;
                }
            }
            catch
            {
                Debug.LogWarning("Error al leer timer.json, se iniciará desde 0.");
                elapsedTime = 0f;
            }
        }
    }

    void Start()
    {
        ResumeTimer();
    }

    private IEnumerator TimerCoroutine()
    {
        while (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerText();
            yield return null; // espera un frame
        }
    }

    private void UpdateTimerText()
    {
        int min = Mathf.FloorToInt(elapsedTime / 60f);
        int sec = Mathf.FloorToInt(elapsedTime % 60f);
        timerText.text = $"Timer: {min:00}:{sec:00}";
    }

    public void PauseTimer()
    {
        isRunning = false;

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }

        SaveTimer();
    }

    public void ResumeTimer()
    {
        if (isRunning) return;

        isRunning = true;
        timerCoroutine = StartCoroutine(TimerCoroutine());
    }

    public void ResetTimer()
    {
        PauseTimer();
        elapsedTime = 0f;
        UpdateTimerText();
        SaveTimer();
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    public void SaveTimer()
    {
        TimerData data = new TimerData { time = elapsedTime };
        string json = JsonConvert.SerializeObject(data);
        File.WriteAllText(timerPath, json);
    }
}

[System.Serializable]
public class TimerData
{
    public float time;
}
