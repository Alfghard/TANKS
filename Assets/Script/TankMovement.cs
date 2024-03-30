using UnityEngine;

public class TankMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;               // RigidBody du tank
    [SerializeField] private float speed = 35f;          // Vitesse de déplacement du tank
    [SerializeField] private float libertyAngle = 30f;   // Angle de liberté permis pour le déplacement
    [SerializeField] private float smoothAngle = 0.13f;  // Temps d'exécution de la rotation du tank
    private float turnSpeed = 0f;                        // Vitesse de rotation du tank

    void Start()
    {
        Physics.gravity = new Vector3(0, -1000, 0);
    }

    void Update()
    {
        Movement();

        // Saut
        if (Input.GetMouseButtonDown(0))
        {
            rb.AddForce(Vector3.up * 10000);
        }

        // Détecte l'appui sur n'importe quelle touche caractère
        foreach (char c in Input.inputString)
        {
            Debug.Log($"Touche pressée: {c}");
        }

        // Exemples pour des touches spécifiques non caractères
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Touche pressée: Espace");
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("Touche pressée: Flèche gauche");
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Touche pressée: Flèche droite");
        }
        // Ajoutez ici des vérifications supplémentaires si nécessaire
        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log("Saut demandé !");
        }

        if (Input.GetButtonUp("Fire1"))
        {
            Debug.Log("Tir terminé !");
        }
    }

    private void Movement()
    {
        // Obtient les inputs du joystick ou des flèches directionnelles
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Génère le vecteur du mouvement du joystick
        Vector3 direction = new Vector3(horizontal, 0f, vertical);

        // N'exécute le mouvement que si une norme minimale de direction est enclenchée
        if (direction.magnitude >= 0.1f)
        {
            // Calcule l'angle de rotation nécessaire pour faire face à la direction
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSpeed, smoothAngle);
            rb.MoveRotation(Quaternion.Euler(0f, angle, 0f));  // Fait pivoter le tank

            // Avance seulement si le tank est orienté presque dans la bonne direction, défini par un angle de liberté
            if (Vector3.Angle(transform.forward, direction) < libertyAngle)
            {
                // Calcule la norme de la vitesse entre 0 et 1 selon l'inclinaison du joystick
                float norme = Mathf.Min(direction.magnitude, 1f);

                // Déplace le rigid body 
                Vector3 move = transform.forward * (float)norme * speed;
                rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
            }
        }
    }
}