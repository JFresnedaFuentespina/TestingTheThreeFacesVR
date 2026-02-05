using System.Collections.Generic;
using UnityEngine;

public class ChangePlayerAttack : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool showingGhost = false;
    public GameObject sword;
    public GameObject fireball;
    public GameObject thunder;
    public bool isThunder = false;
    public GameObject explosionVFX;
    public float switchCooldown = 2f;
    private float lastSwitchTime = -Mathf.Infinity;
    public List<string> actions = new List<string>();
    private OVRInput.Controller controller = OVRInput.Controller.LTouch; // Left-hand controller
    private OVRInput.Button button = OVRInput.Button.PrimaryIndexTrigger; // Index trigger button

    void OnDestroy()
    {
        HourglassItemPickupBehaviour.OnNewChangeCharacterActionEvent -= AddAction;
        BombItemPickupBehaviour.OnNewChangeCharacterActionEvent -= AddAction;
        ThunderPickupItemBehaviour.OnPlayerAttackTypeEvent -= UpdateAttackType;
    }
    void Start()
    {
        HourglassItemPickupBehaviour.OnNewChangeCharacterActionEvent += AddAction;
        BombItemPickupBehaviour.OnNewChangeCharacterActionEvent += AddAction;
        ThunderPickupItemBehaviour.OnPlayerAttackTypeEvent += UpdateAttackType;
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(button, controller) && Time.time >= lastSwitchTime + switchCooldown)
        {
            SwitchCharacter();
            lastSwitchTime = Time.time;
        }
    }

    void UpdateAttackType(bool isThunder)
    {
        this.isThunder = isThunder;
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
            GameObject bomb = Instantiate(explosionVFX, gameObject.transform.position, Quaternion.identity, gameObject.transform);
            Destroy(bomb, 2f);
        }

        if (showingGhost)
        {
            sword.gameObject.SetActive(false);
            if (isThunder)
            {
                thunder.gameObject.SetActive(true);
            }
            else
            {
                fireball.gameObject.SetActive(true);
            }
        }
        else
        {
            sword.gameObject.SetActive(true);
            if (isThunder)
            {
                thunder.gameObject.SetActive(false);
            }
            else
            {
                fireball.gameObject.SetActive(false);
            }
        }

        // rotateCoin.rotate = true;
    }
    public void SubscribeToPickupItemsEvents()
    {
        // PickupItem.OnNewChangeCharacterActionEvent += AddAction;
        HourglassItemPickupBehaviour.OnNewChangeCharacterActionEvent += AddAction;
        BombItemPickupBehaviour.OnNewChangeCharacterActionEvent += AddAction;
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
