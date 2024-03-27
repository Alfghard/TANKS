using UnityEngine;

public class MissileMovementReflect : MonoBehaviour
{
    public float speed = 10f;
    private Vector3 currentDirection;

    void Start()
    {
        // Initialise la direction du missile pour correspondre à celle du lanceur
        currentDirection = transform.forward;
    }

    void Update()
    {
        // Déplace le missile dans la direction actuelle
        transform.Translate(currentDirection * speed * Time.deltaTime, Space.World);

        // Ajuste la rotation du missile pour qu'il "regarde" vers sa direction de mouvement
        if (currentDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(currentDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, speed * Time.deltaTime * 500);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Calcule le symétrique de la direction actuelle par rapport à la normale de la collision
        currentDirection = Vector3.Reflect(currentDirection, collision.contacts[0].normal).normalized;
    }
}
