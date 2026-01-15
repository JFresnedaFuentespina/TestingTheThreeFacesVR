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

    private Animator animator;
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
    void Oestroy()
    {
        OnSpeedStatsRequestedEvent -= SendCurrentStats;
    }

    void Start()
    {
        SubscribeToPickupEvents();
        changeCharacter = GetComponent<ChangeCharacter>();

        // Buscar el TRANSFORM del hijo que se llama "Esqueleto"
        Transform esqueletoHijo = transform.Find("Esqueleto");
        if (esqueletoHijo == null)
        {
            Debug.LogError("No se encontró el hijo llamado 'Esqueleto'");
            return;
        }

        // Obtener el Animator SOLO de ese hijo
        animator = esqueletoHijo.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("El hijo 'Esqueleto' existe, pero no tiene Animator");
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
        PickupItem.OnPlayerSpeedEvent += UpdateSpeed;
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
        if (animator == null || rb == null) return;

        animator.applyRootMotion = false;

        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(inputH, 0f, inputV);

        if (inputDir.magnitude > 1f)
            inputDir.Normalize();

        Vector3 movement = inputDir * velocity * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        currentSpeed = (rb.position - lastPosition).magnitude / Time.fixedDeltaTime;
        lastPosition = rb.position;

        if (!changeCharacter.showingGhost)//! ANIMACIÓN CON LA VELOCIDAD DEL RIGIDBODY
        {
            animator.SetFloat("Speed", currentSpeed);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }
    }


}
