using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLevelSelector : MonoBehaviour
{
    // Start is called before the first frame update
    public LevelGenerator levelGenerator;
    public int currentLevel = 1;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentLevel = 1;
            levelGenerator.NextLevel(currentLevel);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentLevel = 2;
            levelGenerator.NextLevel(currentLevel);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentLevel = 3;
            levelGenerator.NextLevel(currentLevel);
        }
    }
}
