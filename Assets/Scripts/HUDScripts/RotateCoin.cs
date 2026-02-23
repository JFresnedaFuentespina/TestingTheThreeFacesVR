using UnityEngine;
using UnityEngine.UI;

public class RotateCoin : MonoBehaviour
{
    public float rotationSpeed = 270f;
    private float rotated = 0f;
    public bool rotate = false;
    public GameObject fillCooldown;

    private Image cooldownImage;

    // NUEVO: tiempo del cooldown real
    private float cooldownDuration = 2f;
    private float lastSwitchTime = -Mathf.Infinity;

    void Start()
    {
        if (fillCooldown != null)
            cooldownImage = fillCooldown.GetComponent<Image>();

        if (cooldownImage != null)
            cooldownImage.fillAmount = 1f;
    }

    void Update()
    {
        // 🔹 ACTUALIZAR BARRA SEGÚN TIEMPO REAL
        if (cooldownImage != null)
        {
            float t = (Time.time - lastSwitchTime) / cooldownDuration;
            cooldownImage.fillAmount = Mathf.Clamp01(t);
        }

        // 🔹 ROTACIÓN (exactamente como lo tenías)
        if (!rotate)
            return;

        float step = rotationSpeed * Time.deltaTime;

        if (rotated + step > 180f)
            step = 180f - rotated;

        transform.Rotate(0f, step, 0f);
        rotated += step;

        if (rotated >= 180f)
        {
            rotate = false;
            rotated = 0f;
        }
    }

    // 🔹 LLAMAR DESDE ChangeCharacter
    public void StartCooldown(float cooldown)
    {
        cooldownDuration = cooldown;
        lastSwitchTime = Time.time;
    }
}
