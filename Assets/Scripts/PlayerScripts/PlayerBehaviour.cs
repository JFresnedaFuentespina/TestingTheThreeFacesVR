using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public float velocity = 5.0f;
    public float currentSpeed;
    private Rigidbody rb;

    private Animator animatorEsqueleto;
    private Animator animatorFantasma;
    private ChangeCharacter changeCharacter;

    private Vector3 lastPosition;

    public delegate void OnSpeedStatsChanged(float speed);
    public static event OnSpeedStatsChanged OnSpeedStatsChangedEvent;
    public delegate void OnSpeedStatsRequested();
    public static event OnSpeedStatsRequested OnSpeedStatsRequestedEvent;

    void OnEnable()
    {
        OnSpeedStatsRequestedEvent += SendCurrentStats;
    }
    void OnDestroy()
    {
        OnSpeedStatsRequestedEvent -= SendCurrentStats;
    }

    void Start()
    {
        SubscribeToPickupEvents();
        changeCharacter = GetComponent<ChangeCharacter>();

        Transform esqueletoHijo = transform.Find("Esqueleto");
        if (esqueletoHijo == null)
        {
            Debug.LogError("No se encontró el hijo llamado 'Esqueleto'");
            return;
        }

        // Obtener el Animator SOLO de ese hijo
        animatorEsqueleto = esqueletoHijo.GetComponent<Animator>();
        if (animatorEsqueleto == null)
        {
            Debug.LogError("El hijo 'Esqueleto' existe, pero no tiene Animator");
            return;
        }

        Transform ghostHijo = transform.Find("Ghost");
        if (ghostHijo == null)
        {
            Debug.LogError("No se encontró el hijo llamado 'Ghost'");
            return;
        }

        // Obtener el Animator SOLO de ese hijo
        animatorFantasma = ghostHijo.GetComponent<Animator>();
        if (animatorFantasma == null)
        {
            Debug.LogError("El hijo 'Ghost' existe, pero no tiene Animator");
            return;
        }


        // Load JSON stats
        string path = Application.persistentDataPath + "/player.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(json);
            velocity = playerData.velocity;
        }

        rb = GetComponent<Rigidbody>();
        NotifySpeedStatsChanged();
    }
    public static void RequestBehaviourStats()
    {
        OnSpeedStatsRequestedEvent?.Invoke();
    }
    public void SubscribeToPickupEvents()
    {
        IncreaseSpeedItemPickupBehaviour.OnPlayerSpeedEvent += UpdateSpeed;
        StarItemPickupBehaviour.OnPlayerSpeedEvent += UpdateSpeed;
    }

    public void UpdateSpeed(float amount)
    {
        velocity += amount;
    }
    void SendCurrentStats()
    {
        NotifySpeedStatsChanged();
    }
    public void NotifySpeedStatsChanged()
    {
        OnSpeedStatsChangedEvent?.Invoke(this.velocity);
    }
    private void FixedUpdate()
    {
        if (animatorEsqueleto == null || rb == null) return;

        animatorEsqueleto.applyRootMotion = false;

        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(inputH, 0f, inputV);

        if (inputDir.magnitude > 1f)
            inputDir.Normalize();

        Vector3 movement = inputDir * velocity * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        currentSpeed = (rb.position - lastPosition).magnitude / Time.fixedDeltaTime;
        lastPosition = rb.position;

        if (!changeCharacter.showingGhost)
        {
            animatorEsqueleto.SetFloat("Speed", currentSpeed);
        }
        else
        {
            animatorFantasma.SetFloat("Speed", currentSpeed);
        }
    }


}
