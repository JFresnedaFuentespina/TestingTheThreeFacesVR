using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class EnemiesDeathCounter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float counter;

    void Start()
    {
        string path = Application.persistentDataPath + "/player.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(json);
            counter = playerData.enemiesDeathCounter;
        }
        else
        {
            counter = 0f;
        }
    }
}
