using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorsEnabler : MonoBehaviour
{
    private EnemiesGenerator generator;
    private NextRoomCalculator calc;
    private bool doorsReenabled = false;
    public List<GameObject> torches;

    void Start()
    {
        calc = GetComponentInChildren<NextRoomCalculator>();
        generator = GetComponent<EnemiesGenerator>();
    }

    public void StartCheckEnemies()
    {
        StartCoroutine(CheckEnemiesCoroutine());
    }

    IEnumerator CheckEnemiesCoroutine()
    {
        yield return new WaitUntil(() => generator.enemiesActuallySpawned);
        yield return new WaitUntil(() => generator.GetAliveEnemiesCount() == 0);
        SetAllTorchesGreen();
        ReenableAllDoors();
        doorsReenabled = true;
        generator.enemiesDefeated = true;
    }

    public void ReenableAllDoors()
    {
        string[] doorPaths =
        {
        "ParedIzquierda/Door_Prefab_Closed_Left",
        "ParedDerecha/Door_Prefab_Closed_Right",
        "ParedFrontal/Door_Prefab_Closed_Front"
    };
        if (!generator.enemiesActuallySpawned)
        {
            return;
        }

        foreach (string path in doorPaths)
        {
            Transform door = transform.Find(path);
            if (door != null)
            {
                Collider collider = door.GetComponent<Collider>();
                if (collider != null && !collider.enabled)
                {
                    collider.enabled = true;
                }

                NextRoomCalculator doorCalc = door.GetComponent<NextRoomCalculator>() ?? door.GetComponentInChildren<NextRoomCalculator>();
                if (doorCalc != null)
                {
                    doorCalc.enabledTemporarily = false;
                }
            }
            else
            {
                Debug.LogWarning($"No se encontró la puerta: {path}");
            }
        }
    }
    private void SetAllTorchesGreen()
    {
        if (torches == null || torches.Count == 0)
        {
            torches = new List<GameObject>();
            if (transform.Find("ParedIzquierda/TorchLeft") != null)
                torches.Add(transform.Find("ParedIzquierda/TorchLeft").gameObject);
            if (transform.Find("ParedDerecha/TorchRight") != null)
                torches.Add(transform.Find("ParedDerecha/TorchRight").gameObject);
            if (transform.Find("ParedFrontal/TorchFront") != null)
                torches.Add(transform.Find("ParedFrontal/TorchFront").gameObject);
        }
        foreach (GameObject torch in torches)
        {
            if (torch == null) continue;

            Transform red = torch.transform.Find("FireRed");
            Transform green = torch.transform.Find("FireGreen");

            if (red != null) red.gameObject.SetActive(false);
            if (green != null) green.gameObject.SetActive(true);
        }
    }
}
