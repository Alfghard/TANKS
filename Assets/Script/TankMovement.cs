using UnityEngine;

public class TankMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;               // RigidBody du tank
    [SerializeField] private float speed = 35f;          // Vitesse de d�placement du tank
    [SerializeField] private float libertyAngle = 30f;   // Angle de libert� permis pour le d�placement
    [SerializeField] private float smoothAngle = 0.13f;  // Temps d'ex�cution de la rotation du tank
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

        // D�tecte l'appui sur n'importe quelle touche caract�re
        foreach (char c in Input.inputString)
        {
            Debug.Log($"Touche press�e: {c}");
        }

        // Exemples pour des touches sp�cifiques non caract�res
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Touche press�e: Espace");
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("Touche press�e: Fl�che gauche");
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Touche press�e: Fl�che droite");
        }
        // Ajoutez ici des v�rifications suppl�mentaires si n�cessaire
        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log("Saut demand� !");
        }

        if (Input.GetButtonUp("Fire1"))
        {
            Debug.Log("Tir termin� !");
        }
    }

    private void Movement()
    {
        // Obtient les inputs du joystick ou des fl�ches directionnelles
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // G�n�re le vecteur du mouvement du joystick
        Vector3 direction = new Vector3(horizontal, 0f, vertical);

        // N'ex�cute le mouvement que si une norme minimale de direction est enclench�e
        if (direction.magnitude >= 0.1f)
        {
            // Calcule l'angle de rotation n�cessaire pour faire face � la direction
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSpeed, smoothAngle);
            rb.MoveRotation(Quaternion.Euler(0f, angle, 0f));  // Fait pivoter le tank

            // Avance seulement si le tank est orient� presque dans la bonne direction, d�fini par un angle de libert�
            if (Vector3.Angle(transform.forward, direction) < libertyAngle)
            {
                // Calcule la norme de la vitesse entre 0 et 1 selon l'inclinaison du joystick
                float norme = Mathf.Min(direction.magnitude, 1f);

                // D�place le rigid body 
                Vector3 move = transform.forward * (float)norme * speed;
                rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
            }
        }
    }
}