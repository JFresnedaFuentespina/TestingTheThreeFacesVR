using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
// using System;

public class EnemiesGenerator : MonoBehaviour
{
    public GameObject enemyType1Prefab; // Zombie prefab
    public List<GameObject> enemyType2Prefabs; // Ghost prefabs
    public GameObject bossCaraPrefab;
    public GameObject bossCruzPrefab;
    public GameObject bossCantoPrefab;
    public int maxEnemies = 3;
    public float spawnAreaX = 2f;
    public float spawnAreaZ = 2f;

    private CameraDialogueManager cameraDialogueManager;

    private bool enemiesSpawned = false;
    private List<EnemyLife> spawnedEnemies = new List<EnemyLife>();
    public bool enemiesDefeated = false;

    public bool enemiesActuallySpawned = false;

    void Awake()
    {
        cameraDialogueManager = FindAnyObjectByType<CameraDialogueManager>();
        if (cameraDialogueManager == null)
        {
            Debug.LogWarning("CAMERA DIALOGUE MANAGER NOT FOUND");
        }
    }


    public void GenerateEnemiesInRoom(Vector3 roomPos)
    {
        if (enemiesDefeated)
        {
            return;
        }

        Transform suelo = transform.Find("Suelo");
        if (suelo == null)
        {
            Debug.LogWarning("No se encontró el objeto 'Suelo'. Se usará posición relativa.");
        }

        Renderer r = suelo != null ? suelo.GetComponent<Renderer>() : null;
        Bounds bounds = r != null ? r.bounds : new Bounds(transform.position, new Vector3(spawnAreaX * 2, 0, spawnAreaZ * 2));

        // Spawn de enemigos normales si hay prefabs asignados
        if (!enemiesActuallySpawned)
        {
            if (enemyType1Prefab != null || (enemyType2Prefabs != null && enemyType2Prefabs.Count > 0))
            {
                int enemyCount = UnityEngine.Random.Range(1, maxEnemies + 1);
                for (int i = 0; i < enemyCount; i++)
                {
                    Vector3 spawnPos = new Vector3(
                        Random.Range(bounds.min.x, bounds.max.x),
                        roomPos.y + 0.2f,
                        Random.Range(bounds.min.z, bounds.max.z)
                    );

                    GameObject prefab = null;
                    float random = Random.Range(0f, 2f);
                    if (random < 1f && enemyType1Prefab != null) prefab = enemyType1Prefab;
                    else if (enemyType2Prefabs != null && enemyType2Prefabs.Count > 0)
                        prefab = enemyType2Prefabs[Random.Range(0, enemyType2Prefabs.Count)];

                    if (prefab != null)
                    {
                        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
                        EnemyLife life = enemy.GetComponent<EnemyLife>();
                        if (life != null) spawnedEnemies.Add(life);
                    }
                }
            }
        }
        // Spawn de boss si es sala de boss
        if (gameObject.name.IndexOf("Boss", System.StringComparison.OrdinalIgnoreCase) >= 0)
        {
            if (SceneManager.GetActiveScene().name == "Level1Scene")
            {
                GenerateBoss(bossCaraPrefab, bounds, roomPos);
            }
            else if (SceneManager.GetActiveScene().name == "Level2Scene")
            {
                GenerateBoss(bossCruzPrefab, bounds, roomPos);
            }
            else if (SceneManager.GetActiveScene().name == "Level3Scene")
            {
                GenerateBoss(bossCantoPrefab, bounds, roomPos);
            }
        }

        // Marcar que ya se generó todo
        enemiesActuallySpawned = true;
        enemiesSpawned = true;
    }

    private void GenerateBoss(GameObject bossPrefab, Bounds bounds, Vector3 roomPos)
    {
        GameObject boss = bossPrefab;
        // Revisar si ya existe un BossCara en la escena
        bool bossExists = FindObjectsOfType<EnemyLife>()
            .Any(e => e != null && e.gameObject.CompareTag("BossCara"));

        if (bossExists)
        {
            Debug.Log("Ya existe un BossCara en escena, no se genera otro");
        }
        else if (boss != null)
        {
            Vector3 bossSpawn = new Vector3(bounds.center.x, roomPos.y, bounds.center.z);
            GameObject newBoss = Instantiate(boss, bossSpawn, Quaternion.identity);
            EnemyLife bossLife = newBoss.GetComponent<EnemyLife>();
            if (bossLife != null) spawnedEnemies.Add(bossLife);
<<<<<<< HEAD
            // Camera cameraBoss = newBoss.GetComponentInChildren<Camera>(true);
            // if (cameraBoss == null)
            // {
            //     Debug.LogError("BOSS CAMERA NOT FOUND IN PREFAB");
            // }
            // else
            // {
            //     Debug.Log("BOSS CAMERA FOUND: " + cameraBoss.name);
            // }

            // cameraDialogueManager.RegisterBossCamera(cameraBoss);
            // cameraDialogueManager.RefreshCamera();
=======
            Camera cameraBoss = newBoss.GetComponentInChildren<Camera>(true);
            if (cameraBoss == null)
            {
                Debug.LogError("BOSS CAMERA NOT FOUND IN PREFAB");
            }
            else
            {
                Debug.Log("BOSS CAMERA FOUND: " + cameraBoss.name);
            }

            cameraDialogueManager.RegisterBossCamera(cameraBoss);
            cameraDialogueManager.RefreshCamera();
>>>>>>> a1bfba5149275e362358ab35190c0a5522c77a6a

        }
    }

    public int GetAliveEnemiesCount()
    {
        spawnedEnemies.RemoveAll(e => e == null);
        int aliveCount = 0;

        foreach (var enemy in spawnedEnemies)
        {
            if (enemy != null)
                enemy.UpdateIsAlive();

            if (enemy != null && enemy.GetIsAlive())
                aliveCount++;
        }
        return aliveCount;
    }

    public bool AllEnemiesDead()
    {
        bool allDead = GetAliveEnemiesCount() == 0;
        return allDead;
    }

}
