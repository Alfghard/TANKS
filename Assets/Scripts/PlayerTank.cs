using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public class PlayerTank : MonoBehaviour
{
    // Rigid body, Transforms et GameObject
    [SerializeField] public Rigidbody rb;                      // RigidBody du tank
    [SerializeField] public Transform turret;                  // objet Tourelle
    [SerializeField] public Transform firePoint;               // Point a la sortie du canon
    [SerializeField] public GameObject missilePrefab;          // Missile a tirer

    // Variables de mouvement
    [SerializeField] private float tankSpeed = 35f;             // Vitesse de deplacement du tank
    [SerializeField] private float angleThreshold =  0f;        // Angle de liberte permis pour autoriser le d�placement
    [SerializeField] private float tankSmoothness = 0.12f;      // Temps d'execution de la rotation du tank
    
    // Variables de tir et de la tourelle
    [SerializeField] private float turretSmoothness = 0.05f;    // Temps d'execution de la rotation de la tourelle
    [SerializeField] private float shootThreshold = 0.5f;       // Seuil d'activation du tir du bouton R2 entre 0 compris et 1 non compris                 

    // Variables d'etats (ne pas toucher)
    private float turretAngle;                  // Etat de l'angle de la tourelle
    private float baseCurrentSpeed = 0f;        // Etat de la vitesse angulaire de la base du tank
    private float turretCurrentSpeed = 0f;      // Etat de la vitesse angulaire de la tourelle du tank
    private float shootTriggerAxis = 0f;        // Etat de la valeur precedente de l'axe du bouton R2


    void Start()
    {
        turretAngle = turret.eulerAngles.y;
        Physics.gravity = new Vector3(0, -1000, 0);
    }

    
    void Update()
    {    

       
        bool paused = Pause.isGamePaused();    //Récupère la valeur de paused
        if (!paused) {

            // Obtient les inputs du joystick ou des fl�ches directionnelles
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // G�n�re le vecteur du mouvement du joystick
            Vector3 direction = new Vector3(horizontal, 0f, vertical);

            TankMovement(direction);             // Mecanisme de mouvement de la base du tank

            float rJoyX = -Input.GetAxis("RJoyX");
            float rJoyY = Input.GetAxis("RJoyY");

            Vector3 shootDir = new Vector3(rJoyX, 0f, rJoyY);

            TurretMovement(shootDir);   // Mecanisme de rotation de la tourelle du tank
            //TurretMovement_Souris();  // Mecanisme de rotation de la tourelle du tank avec la souris

            // Si le bouton RTrigger est enclenche
            if (Input.GetButton("Fire"))
            {
                // Calcul de l'activation du tir à la moitié de la pression du bouton
                float normShootAxis = (Input.GetAxis("RTrigger") + 1) / 2;
                Debug.Log($"normShootAxis: {normShootAxis}");

                // Tire si l'axe du bouton depasse le seuil fixé
                if (shootTriggerAxis <= shootThreshold && normShootAxis > shootThreshold)
                {
                    Shoot();            // Mecanisme de tir
                }
                shootTriggerAxis = normShootAxis;
            }
            else if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                Shoot();
            }
        }
    }

    public List<Vector3> TankMovement(Vector3 direction)
    {
        float angle = transform.eulerAngles.y;
        Vector3 move = Vector3.zero;

        // N'ex�cute le mouvement que si une norme minimale de direction est enclench�e (zone morte)
        if (direction.magnitude >= 0.1f)
        {
            // Calcule l'angle de rotation necessaire pour faire face a la directione
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref baseCurrentSpeed, tankSmoothness);
            Quaternion tankAngle = Quaternion.Euler(0f, angle, 0f);

            // Applique les rotations
            rb.MoveRotation(Quaternion.Euler(0f, angle, 0f));
            turret.rotation = Quaternion.Euler(0f, turretAngle, 0f);

            // Avance seulement si le tank est oriente presque dans la bonne direction, d�fini par un angle de libert�
            if (Vector3.Angle(transform.forward, direction) < angleThreshold)
            {
                // Calcule la norme de la vitesse entre 0 et 1 selon l'inclinaison du joystick
                float norme = Mathf.Min(direction.magnitude, 1f);

                // D�place le rigid body 
                move = transform.forward * (float)norme * tankSpeed;
                rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
            }
        }
        return new List<Vector3>{ new Vector3(0f, angle, 0f), move };
    }

    public Vector3 TurretMovement(Vector3 shootDir)
    {
        if (shootDir.magnitude >= 0.1f)
        {
            // Calcule l'angle cible base sur la direction du joystick
            float targetAngle = Mathf.Atan2(shootDir.x, shootDir.z) * Mathf.Rad2Deg;
            // Obtient l'angle actuel de la tourelle

            turretAngle = Mathf.SmoothDampAngle(turret.eulerAngles.y, targetAngle, ref turretCurrentSpeed, turretSmoothness);

            // Applique la rotation vers l'angle cible
            turret.rotation = Quaternion.Euler(0f, turretAngle, 0f);
        }

        return turret.rotation.eulerAngles;
    }

    //private void TurretMovement_Souris()
    //{
    //    Vector3 screenPosition;
    //    Vector3 worldPosition;


    //    screenPosition = Input.mousePosition;
    //    screenPosition.z = Camera.main.nearClipPlane + 1;

    //    worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

    //     Vector3 direction = turret.position - worldPosition;
    //     direction.z += direction.y*Mathf.Cos(60 * Mathf.Deg2Rad);
    //     //direction.x += direction.y*Mathf.Sin(60 * Mathf.Deg2Rad);
     
    //    Vector3 projection = Vector3.ProjectOnPlane(direction, Plan.transform.up);
    
    //    turret.rotation = Quaternion.LookRotation(projection);
    //    //Debug.Log(direction);
    //    //Debug.Log(projection);
    
    //}



    public void Shoot()
    {
        GameObject missile = Instantiate(missilePrefab, firePoint.position, firePoint.rotation);
        missile.transform.Rotate(180f, 0, 0);
    }
}
