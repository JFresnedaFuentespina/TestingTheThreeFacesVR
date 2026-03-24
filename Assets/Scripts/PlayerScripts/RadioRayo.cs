using UnityEngine;

public class RadioRayo : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float radio = 2f;
    public int segmentos = 64;
    public float altura = 0.5f;
    LineRenderer lineRenderer;
    public bool isThunderActive = false;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        lineRenderer.loop = true;
        lineRenderer.positionCount = segmentos;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;

    }

    void Update()
    {
        if(isThunderActive)
        {
            DibujarCirculo();
        }
    }

    void DibujarCirculo()
    {
        Vector3 centro = transform.position + Vector3.up * altura;

        for (int i = 0; i < segmentos; i++)
        {
            float angulo = (float)i / segmentos * Mathf.PI * 2f;

            Vector3 punto = new Vector3(
                Mathf.Cos(angulo) * radio,
                0f,
                Mathf.Sin(angulo) * radio
            );

            lineRenderer.SetPosition(i, centro + punto);
        }
    }
}
