using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PickupItem : MonoBehaviour
{
    private GameObject pause;
    private GameObject menuItems;
    private GameObject stats;
    private TextMeshProUGUI damageText;
    private TextMeshProUGUI speedText;
    private TextMeshProUGUI attackSpeedText;
    private TextMeshProUGUI showItemMessageText;
    private Coroutine messageRoutine;
    private bool hudReady = false;

    // Eventos
    public delegate void OnAddItemToInventory(string id, Sprite icon);
    public static event OnAddItemToInventory OnAddItemToInventoryEvent;

    void OnEnable()
    {
        PlayerAttack.OnAttackStatsChangedEvent += UpdateAttackStats;
        PlayerBehaviour.OnSpeedStatsChangedEvent += UpdateSpeed;
        PlayerInventory.OnInventoryItemsProvidedEvent += BuildInventoryHUD;
    }

    void OnDisable()
    {
        PlayerAttack.OnAttackStatsChangedEvent -= UpdateAttackStats;
        PlayerBehaviour.OnSpeedStatsChangedEvent -= UpdateSpeed;
        PlayerInventory.OnInventoryItemsProvidedEvent -= BuildInventoryHUD;
    }


    IEnumerator Start()
    {
        yield return SetupHUD();
    }

    IEnumerator SetupHUD()
    {
        // Esperar a que el HUD exista
        GameObject hud = null;
        while (hud == null)
        {
            hud = GameObject.Find("HUD");
            yield return null;
        }
        // Setup menú de pausa
        pause = hud.transform.Find("Pause").gameObject;
        menuItems = pause.transform.Find("Items").gameObject;
        stats = pause.transform.Find("Stats").gameObject;
        damageText = stats.transform.Find("Damage").GetComponent<TextMeshProUGUI>();
        speedText = stats.transform.Find("Speed").GetComponent<TextMeshProUGUI>();
        attackSpeedText = stats.transform.Find("AttackInterval").GetComponent<TextMeshProUGUI>();
        showItemMessageText = hud.transform.Find("ItemMessage").GetComponent<TextMeshProUGUI>();

        hudReady = true;

        PlayerAttack.RequestAttackStats();
        PlayerBehaviour.RequestBehaviourStats();
        PlayerInventory.RequestInventoryItems();
    }

    public void BuildInventoryHUD(List<InventoryItem> items)
    {
        foreach (Transform child in menuItems.transform)
            Destroy(child.gameObject);

        foreach (var item in items)
        {
            if (item != null && item.icon != null)
                AddItemToHUD(item.icon, item.itemID);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Pedestal") &&
            !collision.gameObject.CompareTag("Key"))
            return;

        GameObject itemToPickup;

        // Caso Key
        if (collision.gameObject.CompareTag("Key"))
        {
            itemToPickup = collision.gameObject;
        }
        // Caso Pedestal
        else
        {
            if (collision.transform.childCount == 0)
                return;

            itemToPickup = collision.transform.GetChild(0).gameObject;
        }

        ItemIcon iconComp = itemToPickup.GetComponent<ItemIcon>();
        if (iconComp == null)
        {
            Debug.LogWarning("El objeto no tiene ItemIcon");
            return;
        }

        // Añadir al inventario
        OnAddItemToInventoryEvent?.Invoke(iconComp.itemID, iconComp.icon);
        AddItemToHUD(iconComp.icon, iconComp.itemID);

        // Aplicar efectos
        ApplyItemEffects(itemToPickup);

        Destroy(itemToPickup);
    }


    private void ApplyItemEffects(GameObject item)
    {
        ItemPickupBehaviour pickup = item.GetComponent<ItemPickupBehaviour>();
        if (pickup != null)
        {
            ShowMessage(pickup.ApplyItemEffects());
        }
        else
        {
            Debug.LogWarning("ItemPickupBehaviour NOT FOUND");
        }
    }

    private void AddItemToHUD(Sprite icon, string itemID)
    {
        if (icon == null)
        {
            Debug.LogWarning("Icono nulo para el item: " + itemID);
            return;
        }

        GameObject iconGO = new GameObject(itemID + "_Icon");
        iconGO.transform.SetParent(menuItems.transform, false); // GridLayoutGroup maneja la posición automáticamente

        Image img = iconGO.AddComponent<Image>();
        img.sprite = icon;

        RectTransform rt = iconGO.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(90, 100); // tamaño de la celda
    }

    private void UpdateAttackStats(float damage, float interval)
    {
        if (!hudReady) return;

        damageText.text = "Damage: " + damage.ToString("F1");
        attackSpeedText.text = "Attack Interval: " + interval.ToString("F1");
    }

    private void UpdateSpeed(float speed)
    {
        if (!hudReady) return;

        speedText.text = "Speed: " + speed.ToString("F1");
    }



    private void ShowMessage(string message)
    {
        showItemMessageText.gameObject.SetActive(true);
        showItemMessageText.text = message;

        if (messageRoutine != null)
            StopCoroutine(messageRoutine);

        messageRoutine = StartCoroutine(FadeMessage());
    }
    private IEnumerator FadeMessage()
    {
        // Primero poner el texto totalmente visible
        Color c = showItemMessageText.color;
        c.a = 1f;
        showItemMessageText.color = c;

        // Mantener el mensaje un momento
        yield return new WaitForSeconds(2f);

        // Tiempo total del fade
        float duration = 1.5f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / duration);

            c.a = alpha;
            showItemMessageText.color = c;

            yield return null;
        }

        // Asegurar que desaparece del todo
        c.a = 0f;
        showItemMessageText.color = c;
        showItemMessageText.gameObject.SetActive(false);
    }


}
