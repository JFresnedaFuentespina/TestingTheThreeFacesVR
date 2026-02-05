using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private ChangePlayerAttack changePlayerAttack;
    public GameObject fireball;
    public GameObject thunderPrefab;
    public float attackDamage = 5f;
    public float attackSpeed = 5f;
    public float spawnHeight = 1.0f;
    public float attackRange = 2f;
    public float attackInterval = 1f;
    private float lastAttackTime = -999f;
    private float thunderSpawnY = 5f;
    public float thunderLifeTime = 0.4f;
    public bool isFireball = false;
    public bool isThunder = true;
    private bool isAttacking = false;
    public AudioClip swordSwingAudioClip;
    public AudioClip fireballAudioClip;
    private AudioSource audioSource;
    private Animator animatorEsqueleto;
    private Animator animatorFantasma;

    public GameObject swordGO;
    public bool appliesPoison = false;

    public delegate void OnAttackStatsChanged(float damage, float interval);
    public static event OnAttackStatsChanged OnAttackStatsChangedEvent;
    public delegate void OnAttackStatsRequested();
    public static event OnAttackStatsRequested OnAttackStatsRequestedEvent;

    private OVRInput.Controller controller = OVRInput.Controller.RTouch; // Right-hand controller
    private OVRInput.Button button = OVRInput.Button.PrimaryIndexTrigger; // Index trigger button
    public Transform spawnPoint;

    void OnEnable()
    {
        OnAttackStatsRequestedEvent += SendCurrentStats;
    }

    void OnDestroy()
    {
        OnAttackStatsRequestedEvent -= SendCurrentStats;
    }

    // Start is called before the first frame update
    void Start()
    {
        SubscribeToPickupEvents();
        audioSource = GetComponent<AudioSource>();
        string path = Application.persistentDataPath + "/player.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(json);
            attackInterval = playerData.attackInterval;
            attackRange = playerData.attackRange;
            attackDamage = playerData.damage;
            isFireball = playerData.attackType == "Fireball";
            isThunder = playerData.attackType == "Thunder";
            appliesPoison = playerData.appliesPoison;
        }
        changePlayerAttack = GetComponent<ChangePlayerAttack>();
        animatorEsqueleto = FindEsqueletoAnimator(transform);
        animatorFantasma = FindGhostAnimator(transform);
        MeleeAttackHit weapon = this.gameObject.GetComponentInChildren<MeleeAttackHit>();
        weapon.attackDamage = attackDamage;
        swordGO = GameObject.Find("Sword");
    }

    public static void RequestAttackStats()
    {
        OnAttackStatsRequestedEvent?.Invoke();
    }

    public void SubscribeToPickupEvents()
    {
        ThunderPickupItemBehaviour.OnPlayerAttackEvent += DecideChanges;
        IncreaseAttackDmgItemPickupBehaviour.OnPlayerAttackEvent += DecideChanges;
        StarItemPickupBehaviour.OnPlayerAttackEvent += DecideChanges;
        GreenPotionItemPickupBehaviour.OnPlayerAttackEvent += DecideChanges;
        SkullItemPickupBehaviour.OnPlayerAttackEvent += DecideChanges;
    }

    // Buscar el animator del esqueleto
    Animator FindEsqueletoAnimator(Transform raiz)
    {
        foreach (Transform t in raiz)
        {
            if (t.name == "Esqueleto")
            {
                return t.GetComponent<Animator>();
            }

            Animator encontrado = FindEsqueletoAnimator(t);
            if (encontrado != null)
                return encontrado;
        }
        return null;
    }
    // Buscar el animator del fantasma
    Animator FindGhostAnimator(Transform raiz)
    {
        foreach (Transform t in raiz)
        {
            if (t.name == "Ghost")
            {
                return t.GetComponent<Animator>();
            }

            Animator encontrado = FindGhostAnimator(t);
            if (encontrado != null)
                return encontrado;
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        if (changePlayerAttack != null)
        {
            if (OVRInput.GetDown(button, controller))
            {
                TryAttack();
            }
        }
    }
    void TryAttack()
    {
        if (Time.time < lastAttackTime + attackInterval)
            return;
        lastAttackTime = Time.time;
        if (changePlayerAttack.showingGhost)
            Shoot();
    }
    void Shoot()
    {
        // animatorFantasma.SetTrigger("Attack");
        if (isFireball)
        {
            ShootFire();
        }
        else if (isThunder)
        {
            ShootThunder();
        }
    }

    void ShootFire()
    {
        // audioSource.PlayOneShot(fireballAudioClip);
        isThunder = false;
        Vector3 direction = spawnPoint.forward;

        GameObject newFireball = Instantiate(fireball, spawnPoint.position, Quaternion.LookRotation(direction));
        // Asignar la dirección al script del proyectil
        FireballBehaviour fbMove = newFireball.GetComponent<FireballBehaviour>();
        if (fbMove != null)
        {
            fbMove.direction = direction;
            fbMove.speed = attackSpeed;
        }
    }

    void ShootThunder()
    {
        isFireball = false;

        Vector3 direction = transform.forward;
        Vector3 spawnPos = transform.position + direction * attackRange;
        spawnPos.y = thunderSpawnY; // forzar la altura exacta

        GameObject newThunder = Instantiate(
            thunderPrefab,
            spawnPos,
            Quaternion.identity
        );

        if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit hitInfo, 20f))
        {
            if (hitInfo.collider.tag.Contains("Boss") || hitInfo.collider.CompareTag("Enemy_Zombie") || hitInfo.collider.CompareTag("Enemy_Ghost"))
            {
                EnemyLife enemyLife = hitInfo.collider.GetComponent<EnemyLife>();
                if (enemyLife != null)
                {
                    enemyLife.Damage(attackDamage);
                    enemyLife.UpdateIsAlive();
                }
            }
        }

        Destroy(newThunder, thunderLifeTime);
    }

    public void DecideChanges(string item)
    {
        switch (item)
        {
            case "Thunder":
                PickupThunder();
                break;
            case "IncreaseAttackDamageItem":
                PickupIncreaseDamage();
                break;
            case "IncreaseAttackSpeedItem":
                OnPickupIncreaseAttackSpeed();
                break;
            case "Star":
                OnPickupStar();
                break;
            case "Skull":
                OnPickupSkull();
                break;
            case "GreenPotion":
                ApplyPoisonEffect();
                break;
        }
    }

    public void PickupThunder()
    {
        isFireball = false;
        isThunder = true;
        attackDamage += 2f;
        NotifyAttackStatsChanged();
    }

    public void PickupIncreaseDamage()
    {
        attackDamage += 2.5f;
        NotifyAttackStatsChanged();
    }

    public void OnPickupIncreaseAttackSpeed()
    {
        attackInterval -= 1f;
    }

    public void OnPickupStar()
    {
        attackDamage += 2f;
        attackInterval -= 0.5f;
        NotifyAttackStatsChanged();
    }

    public void OnPickupSkull()
    {
        attackDamage += 5f;
        NotifyAttackStatsChanged();
    }

    void SendCurrentStats()
    {
        NotifyAttackStatsChanged();
    }


    public void NotifyAttackStatsChanged()
    {
        OnAttackStatsChangedEvent?.Invoke(attackDamage, attackInterval);
    }

    public void ApplyPoisonEffect()
    {
        appliesPoison = true;
    }
}
