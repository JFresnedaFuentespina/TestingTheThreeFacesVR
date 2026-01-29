using System.IO;
using UnityEngine;

public class InitGame : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    LevelGenerator levelGenerator;
    private float timer = 0f;
    private float timeLimit = 5f;

    void Start()
    {
        levelGenerator = GetComponent<LevelGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timeLimit)
        {
            levelGenerator.NextLevel(0);
        }
    }
}
