using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerTank : MonoBehaviour
{
    // Rigid body, Transforms et GameObject
    [SerializeField] private Rigidbody rb;                      // RigidBody du tank
    [SerializeField] private Transform turret;                  // objet Tourelle
    [SerializeField] private Transform firePoint;               // Point à la sortie du canon
    [SerializeField] private GameObject missilePrefab;          // Missile à tirer

    // Variables de mouvement
    [SerializeField] private float tankSpeed = 35f;             // Vitesse de déplacement du tank
    [SerializeField] private float angleThreshold = 30f;        // Angle de liberté permis pour autoriser le déplacement
    [SerializeField] private float tankSmoothness = 0.12f;      // Temps d'exécution de la rotation du tank
    
    // Variables de tir et de la tourelle
    [SerializeField] private float turretSmoothness = 0.05f;    // Temps d’exécution de la rotation de la tourelle
    [SerializeField] private float shootThreshold = 0.5f;       // Seuil d'activation du tir du bouton R2 entre 0 compris et 1 non compris

    // Variables d'états (ne pas toucher)
    private float turretAngle;                  // État de l'angle de la tourelle
    private float baseCurrentSpeed = 0f;        // État de la vitesse angulaire de la base du tank
    private float turretCurrentSpeed = 0f;      // État de la vitesse angulaire de la tourelle du tank
    private float shootTriggerAxis = 0f;        // État de la valeur précédente de l'axe du bouton R2


    void Start()
    {
        turretAngle = turret.eulerAngles.y;
        Physics.gravity = new Vector3(0, -1000, 0);
    }

    
    void Update()
    {
        TankMovement();     // Mécanisme de mouvement de la base du tank
        TurretMovement();   // Mécanisme de rotation de la tourelle du tank
        Shoot();            // Mécanisme de tir

        // Saut
        if (Input.GetMouseButtonDown(0))
        {
            rb.AddForce(Vector3.up * 10000);
        }
    }

    private void TankMovement()
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
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref baseCurrentSpeed, tankSmoothness);
            rb.MoveRotation(Quaternion.Euler(0f, angle, 0f));  // Fait pivoter le tank
            turret.rotation = Quaternion.Euler(0f, turretAngle, 0f);

            // Avance seulement si le tank est orienté presque dans la bonne direction, défini par un angle de liberté
            if (Vector3.Angle(transform.forward, direction) < angleThreshold)
            {
                // Calcule la norme de la vitesse entre 0 et 1 selon l'inclinaison du joystick
                float norme = Mathf.Min(direction.magnitude, 1f);

                // Déplace le rigid body 
                Vector3 move = transform.forward * (float)norme * tankSpeed;
                rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
            }
        }
    }

    private void TurretMovement()
    {
        float rJoyX = -Input.GetAxis("RJoyX");
        float rJoyY = Input.GetAxis("RJoyY");

        Vector3 shootDir = new Vector3(rJoyX, 0f, rJoyY);

        if (shootDir.magnitude >= 0.1f)
        {
            // Calcule l'angle cible basé sur la direction du joystick
            float targetAngle = Mathf.Atan2(shootDir.x, shootDir.z) * Mathf.Rad2Deg;
            // Obtient l'angle actuel de la tourelle
            turretAngle = Mathf.SmoothDampAngle(turret.eulerAngles.y, targetAngle, ref turretCurrentSpeed, turretSmoothness);

            // Applique la rotation vers l'angle cible
            turret.rotation = Quaternion.Euler(0f, turretAngle, 0f);
        }
    }

    private void Shoot()
    {
        // Si le bouton RTrigger est enclenché
        if (Input.GetButton("Fire"))
        {
            float normShootAxis = (Input.GetAxis("RTrigger") + 1) / 2;
            Debug.Log($"normShootAxis: {normShootAxis}");
            // Tire si l'axe du bouton dépasse 
            if (shootTriggerAxis <= shootThreshold && normShootAxis > shootThreshold)
            {
                GameObject missile = Instantiate(missilePrefab, firePoint.position, firePoint.rotation);
                missile.transform.Rotate(180f, 0, 0);
            }
            shootTriggerAxis = normShootAxis;
        }
    }
}
