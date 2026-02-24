using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class ChangeCharacter : MonoBehaviour
{
    public GameObject ghost;
    public GameObject esqueleto;
    public bool showingGhost = false;
    public List<string> actions = new List<string>();
    public GameObject explosionVFX;

    private GameObject monedaOriginal;
    private RotateCoin rotateCoin;
    private GameObject cursorManagerGO;
    private CursorManager cursorManager;


    public float switchCooldown = 2f;
    private float lastSwitchTime = -Mathf.Infinity;

    void OnDestroy()
    {
        // PickupItem.OnNewChangeCharacterActionEvent -= AddAction;
        HourglassItemPickupBehaviour.OnNewChangeCharacterActionEvent -= AddAction;
        BombItemPickupBehaviour.OnNewChangeCharacterActionEvent -= AddAction;
    }

    void Start()
    {
        cursorManagerGO = GameObject.Find("CursorManagerGO");
        if (cursorManagerGO != null)
        {
            cursorManager = cursorManagerGO.GetComponent<CursorManager>();
        }
        esqueleto.SetActive(true);
        ghost.SetActive(false);
        monedaOriginal = GameObject.Find("MonedaOriginal").gameObject;
        if (monedaOriginal != null)
            rotateCoin = monedaOriginal.GetComponent<RotateCoin>();
        SubscribeToPickupItemsEvents();
        string path = Application.persistentDataPath + "/player.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(json);
            if (playerData.actions != null)
                actions = playerData.actions;
            else
                actions = new List<string>();
        }
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("ChangeCharacter"))
            && Time.time >= lastSwitchTime + switchCooldown)
        {
            SwitchCharacter();
            lastSwitchTime = Time.time;
        }
    }

    public void SubscribeToPickupItemsEvents()
    {
        // PickupItem.OnNewChangeCharacterActionEvent += AddAction;
        HourglassItemPickupBehaviour.OnNewChangeCharacterActionEvent += AddAction;
        BombItemPickupBehaviour.OnNewChangeCharacterActionEvent += AddAction;
    }

    void SwitchCharacter()
    {
        showingGhost = !showingGhost;

        if (actions.Contains("Hourglass"))
        {
            Debug.Log("FREEZE TIME!");
        }
        if (actions.Contains("Bomb"))
        {
            GameObject bomb = Instantiate(explosionVFX, showingGhost ? esqueleto.transform.position : ghost.transform.position, Quaternion.identity, gameObject.transform);
            Destroy(bomb, 2f);
        }

        if (showingGhost)
        {
            if (cursorManager != null)
                cursorManager.ChangeCursorToCross();
            ghost.transform.position = esqueleto.transform.position;
            ghost.SetActive(true);
            esqueleto.SetActive(false);
        }
        else
        {
            if (cursorManager != null)
                cursorManager.ChangeCursorToSword();
            esqueleto.transform.position = ghost.transform.position;
            esqueleto.SetActive(true);
            ghost.SetActive(false);
        }

        if (rotateCoin != null)
        {
            rotateCoin.rotate = true;                // gira la moneda
            rotateCoin.StartCooldown(switchCooldown); // sincroniza la barra
        }
    }
    public void RemoveAction(string action)
    {
        actions.Remove(action);
    }

    public void AddAction(string action)
    {
        actions.Add(action);
    }

    public List<string> GetUnlockedActions()
    {
        return actions;
    }
}
