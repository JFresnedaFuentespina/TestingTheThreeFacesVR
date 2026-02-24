using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    public List<GameObject> items;

    void Start()
    {
        string path = Application.persistentDataPath + "/generatedItems.json";
        GeneratedItemsData data;

        // Cargar o crear datos
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            data = JsonConvert.DeserializeObject<GeneratedItemsData>(json);
        }
        else
        {
            data = new GeneratedItemsData();
        }

        // Obtener lista de índices disponibles
        List<int> availableIndexes = new List<int>();
        for (int i = 0; i < items.Count; i++)
        {
            if (!data.usedItemIndexes.Contains(i))
                availableIndexes.Add(i);
        }

        // Si ya se usaron todos → resetear
        if (availableIndexes.Count == 0)
        {
            data.usedItemIndexes.Clear();
            for (int i = 0; i < items.Count; i++)
                availableIndexes.Add(i);
        }

        // Elegir aleatorio entre los no usados
        int chosenIndex = availableIndexes[Random.Range(0, availableIndexes.Count)];

        // Guardar como usado
        data.usedItemIndexes.Add(chosenIndex);
        File.WriteAllText(path, JsonConvert.SerializeObject(data));

        // Instanciar
        Vector3 spawnPoint = transform.position + Vector3.up;
        GameObject spawned = Instantiate(items[chosenIndex], spawnPoint, Quaternion.identity);
        spawned.transform.SetParent(transform);
    }
}
