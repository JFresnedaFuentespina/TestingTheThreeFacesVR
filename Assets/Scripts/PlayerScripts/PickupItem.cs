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
    public delegate void OnPlayerAttack(string item);
    public static event OnPlayerAttack OnPlayerAttackEvent;

    public delegate void OnPlayerSpeed(float amount);
    public static event OnPlayerSpeed OnPlayerSpeedEvent;

    public delegate void OnHealthIncreased(float amunt);
    public static event OnHealthIncreased OnHealthIncreasedEvent;

    public delegate void OnHealthDecreased(float amount);
    public static event OnHealthDecreased OnHealthDecreasedEvent;

    public delegate void OnFullyHealed();
    public static event OnFullyHealed OnFullyHealedEvent;

    public delegate void OnNewChangeCharacterAction(string action);
    public static event OnNewChangeCharacterAction OnNewChangeCharacterActionEvent;

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
        if (!collision.gameObject.CompareTag("Pedestal") || collision.transform.childCount == 0) return;

        Transform child = collision.transform.GetChild(0);
        ItemIcon iconComp = child.GetComponent<ItemIcon>();
        if (iconComp == null)
        {
            Debug.LogWarning("El objeto en el pedestal no tiene ItemIcon");
            return;
        }

        // Añadir al inventario y HUD
        if (OnAddItemToInventoryEvent != null)
        {
            OnAddItemToInventoryEvent(iconComp.itemID, iconComp.icon);
        }
        AddItemToHUD(iconComp.icon, iconComp.itemID);

        // Aplicar efectos del item
        ApplyItemEffects(child);

        Destroy(child.gameObject);
    }

    private void ApplyItemEffects(Transform item)
    {
        string msg = "";
        if (item.CompareTag("ThunderItem"))
        {
            if (OnPlayerAttackEvent != null)
                OnPlayerAttackEvent("Thunder");
            msg = "¡Disparo eléctrico!";
        }
        else if (item.CompareTag("IncreaseSpeedItem"))
        {
            if (OnPlayerSpeedEvent != null)
                OnPlayerSpeedEvent(0.5f);
            msg = "¡Velocidad aumentada!";
        }
        else if (item.CompareTag("IncreaseAttackDamageItem"))
        {
            if (OnPlayerAttackEvent != null)
                OnPlayerAttackEvent("IncreaseAttackDamageItem");
            msg = "¡Daño de ataque aumentado!";
        }
        else if (item.CompareTag("IncreaseAttackSpeedItem"))
        {
            if (OnPlayerAttackEvent != null)
                OnPlayerAttackEvent("IncreaseAttackSpeedItem");
            msg = "¡Velocidad de ataque aumentada!";
        }
        else if (item.CompareTag("Hourglass"))
        {
            if (OnNewChangeCharacterActionEvent != null)
                OnNewChangeCharacterActionEvent("Hourglass");
            msg = "Ralentiza a los enemigos al girar la moneda";
        }
        else if (item.CompareTag("Star"))
        {
            if (OnPlayerSpeedEvent != null)
                OnPlayerSpeedEvent(1.0f);
            if (OnPlayerAttackEvent != null)
                OnPlayerAttackEvent("Star");
            msg = "¡Mejoras en todas las estadísticas!";
        }
        else if (item.CompareTag("BluePill")) // Corazón extra azul (temporal)
        {
            msg = "¡Pastilla azul recogida!";
        }
        else if (item.CompareTag("Bomb")) // Explosión alrededor del jugador que daña a los enemigos al girar la moneda
        {
            if (OnNewChangeCharacterActionEvent != null)
                OnNewChangeCharacterActionEvent("Bomb");
            msg = "¡Bomba recogida!";
        }
        else if (item.CompareTag("Key")) // Llave para abrir la puerta final
        {
            msg = "¡Llave recogida!";
        }
        else if (item.CompareTag("GreenPotion")) // Curación de medio corazón
        {
            msg = "¡Poción verde recogida!";
        }
        else if (item.CompareTag("RedVial")) // Curación de un corazón
        {
            msg = "¡Vial rojo recogido!";
        }
        else if (item.CompareTag("Heart")) // Vida extra
        {
            msg = "¡Vida extra!";
            if (OnHealthIncreasedEvent != null)
                OnHealthIncreasedEvent(1);
            if (OnFullyHealedEvent != null)
                OnFullyHealedEvent();

        }
        else if (item.CompareTag("Shield")) // Escudo que bloquea algunos ataques
        {
            msg = "¡Escudo recogido!";
        }
        else if (item.CompareTag("Skull")) // Calavera que aumenta el daño pero reduce la vida
        {
            msg = "¡Calavera recogida!";
            if (OnPlayerAttackEvent != null)
                OnPlayerAttackEvent("Skull");
            if (OnHealthDecreasedEvent != null)
                OnHealthDecreasedEvent(1);
        }
        ShowMessage(msg);
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
