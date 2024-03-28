using UnityEngine;

public class MissileMovementReflect : MonoBehaviour
{
    [SerializeField] public float speed = 10f;
    [SerializeField] public int nbRebond = 1;
    private Vector3 currentDirection;
    

    void Start()
    {
        // Initialise la direction du missile pour correspondre � celle du lanceur
        currentDirection = transform.forward;
    }

    void Update()
    {
        // D�place le missile dans la direction actuelle
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
        if (collision.gameObject.tag == "Wall")
        {
            if (nbRebond <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                nbRebond--;
                // Calcule le sym�trique de la direction actuelle par rapport � la normale de la collision
                currentDirection = Vector3.Reflect(currentDirection, collision.contacts[0].normal).normalized;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
