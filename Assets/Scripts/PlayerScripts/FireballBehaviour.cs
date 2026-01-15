using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballBehaviour : MonoBehaviour
{
    public float speed = 10f;      // velocidad de la bola
    [HideInInspector]
    public Vector3 direction;      // dirección de movimiento
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 3f); // se destruye tras 3 segundos
    }

    // Update is called once per frame
    void Update()
    {
        // Mover la bola en la dirección indicada
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
    }
}
