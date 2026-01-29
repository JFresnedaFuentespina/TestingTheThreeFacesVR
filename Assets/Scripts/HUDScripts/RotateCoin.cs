using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RotateCoin : MonoBehaviour
{
    public float rotationSpeed = 270f;
    private float rotated = 0f;
    public bool rotate = false;
    public GameObject fillCooldown;

    private Image cooldownImage;

    void Start()
    {
        if (fillCooldown != null)
            cooldownImage = fillCooldown.GetComponent<Image>();
        cooldownImage.fillAmount = 1f;
    }

    void Update()
    {
        if (!rotate)
            return;
        float step = rotationSpeed * Time.deltaTime;

        if (rotated + step > 180f)
            step = 180f - rotated;

        transform.Rotate(0f, step, 0f);
        rotated += step;

        if (cooldownImage != null)
            cooldownImage.fillAmount = rotated / 180f;

        if (rotated >= 180f)
        {
            rotate = false;
            rotated = 0f;

            if (cooldownImage != null)
                cooldownImage.fillAmount = 1f;
        }
    }
}
