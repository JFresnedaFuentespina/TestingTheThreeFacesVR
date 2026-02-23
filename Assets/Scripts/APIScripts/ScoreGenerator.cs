using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class ScoreGenerator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float baseScore = 1000f;
    public float enemiesDeathCounter;
    public float time;
    public float mins;
    public float secs;
    public float score;

    void Start()
    {
        // Cargar el número de enemigos muertos desde player.json
        string enemiesKilledPath = Application.persistentDataPath + "/player.json";
        if (File.Exists(enemiesKilledPath))
        {
            string json = File.ReadAllText(enemiesKilledPath);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);
            enemiesDeathCounter = playerData.enemiesDeathCounter;
        }
        else
        {
            enemiesDeathCounter = 0f;
        }

        // Cargar el tiempo desde timer.json
        string timerPath = Application.persistentDataPath + "/timer.json";
        if (File.Exists(timerPath))
        {
            string json = File.ReadAllText(timerPath);
            TimerData timerData = JsonUtility.FromJson<TimerData>(json);
            time = timerData.time;
        }
        else
        {
            time = 0f;
        }
        CalculateScore();
    }

    public void CalculateScore()
    {
        mins = Mathf.FloorToInt(time / 60f);
        float modifier;
        if (mins < 3f)
        {
            modifier = 1.5f;
        }
        else if (mins >= 3f && mins < 7f)
        {
            modifier = 1f;
        }
        else
        {
            modifier = 0.7f;
        }
        score = (baseScore + enemiesDeathCounter) * modifier;

        string json = JsonConvert.SerializeObject(new ScoreDTO { score = score });
        string path = Application.persistentDataPath + "/score.json";
        File.WriteAllText(path, json);
    }
}
