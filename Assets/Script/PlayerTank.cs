using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public class PlayerTank : MonoBehaviour
{
    // Rigid body, Transforms et GameObject
    [SerializeField] private Rigidbody rb;                      // RigidBody du tank
    [SerializeField] private Transform turret;                  // objet Tourelle
    [SerializeField] private Transform firePoint;               // Point � la sortie du canon
    [SerializeField] private GameObject missilePrefab;          // Missile � tirer

    // Variables de mouvement
    [SerializeField] private float tankSpeed = 35f;             // Vitesse de d�placement du tank
    [SerializeField] private float angleThreshold =  0f;        // Angle de libert� permis pour autoriser le d�placement
    [SerializeField] private float tankSmoothness = 0.12f;      // Temps d'ex�cution de la rotation du tank
    
    // Variables de tir et de la tourelle
    [SerializeField] private float turretSmoothness = 0.05f;    // Temps d�ex�cution de la rotation de la tourelle
    [SerializeField] private float shootThreshold = 0.5f;       // Seuil d'activation du tir du bouton R2 entre 0 compris et 1 non compris

    [SerializeField] private GameObject Plan;                   

    // Variables d'�tats (ne pas toucher)
    private float turretAngle;                  // �tat de l'angle de la tourelle
    private float baseCurrentSpeed = 0f;        // �tat de la vitesse angulaire de la base du tank
    private float turretCurrentSpeed = 0f;      // �tat de la vitesse angulaire de la tourelle du tank
    private float shootTriggerAxis = 0f;        // �tat de la valeur pr�c�dente de l'axe du bouton R2


    void Start()
    {
        turretAngle = turret.eulerAngles.y;
        Physics.gravity = new Vector3(0, -1000, 0);
    }

    
    void Update()
    {    

       
        bool paused = Pause.isGamePaused();    //Récupère la valeur de paused
        if (!paused) {
            TankMovement();     // M�canisme de mouvement de la base du tank
            //TurretMovement_Manette();   // M�canisme de rotation de la tourelle du tank
            TurretMovement_Souris();        // M�canisme de rotation de la tourelle du tank avec la souris
            Shoot();            // M�canisme de tir
             // tirer
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) )
                {
                    //rb.AddForce(Vector3.up * 10000); //pour sauter
                    GameObject missile = Instantiate(missilePrefab, firePoint.position, firePoint.rotation);
                    missile.transform.Rotate(180,0,0);
                    Debug.Log("Fire");

                }
        }
    }

    private void TankMovement()
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
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref baseCurrentSpeed, tankSmoothness);
            rb.MoveRotation(Quaternion.Euler(0f, angle, 0f));  // Fait pivoter le tank
            turret.rotation = Quaternion.Euler(0f, turretAngle, 0f);

            // Avance seulement si le tank est orient� presque dans la bonne direction, d�fini par un angle de libert�
            if (Vector3.Angle(transform.forward, direction) < angleThreshold)
            {
                // Calcule la norme de la vitesse entre 0 et 1 selon l'inclinaison du joystick
                float norme = Mathf.Min(direction.magnitude, 1f);

                // D�place le rigid body 
                Vector3 move = transform.forward * (float)norme * tankSpeed;
                rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
            }
        }
    }

    private void TurretMovement_Manette()
    {
        float rJoyX = -Input.GetAxis("RJoyX");
        float rJoyY = Input.GetAxis("RJoyY");

        Vector3 shootDir = new Vector3(rJoyX, 0f, rJoyY);

        if (shootDir.magnitude >= 0.1f)
        {
            // Calcule l'angle cible bas� sur la direction du joystick
            float targetAngle = Mathf.Atan2(shootDir.x, shootDir.z) * Mathf.Rad2Deg;
            // Obtient l'angle actuel de la tourelle
            turretAngle = Mathf.SmoothDampAngle(turret.eulerAngles.y, targetAngle, ref turretCurrentSpeed, turretSmoothness);

            // Applique la rotation vers l'angle cible
            turret.rotation = Quaternion.Euler(0f, turretAngle, 0f);
        }
    }

    private void TurretMovement_Souris()
    {
    Vector3 screenPosition;
    Vector3 worldPosition;


    screenPosition = Input.mousePosition;
    screenPosition.z = Camera.main.nearClipPlane + 1;

    worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

     Vector3 direction = turret.position - worldPosition;
     direction.z += direction.y*Mathf.Cos(60 * Mathf.Deg2Rad);
     //direction.x += direction.y*Mathf.Sin(60 * Mathf.Deg2Rad);
     
    Vector3 projection = Vector3.ProjectOnPlane(direction, Plan.transform.up);
    
    turret.rotation = Quaternion.LookRotation(projection);
    //Debug.Log(direction);
    //Debug.Log(projection);
    
    }



    public void Shoot()
    {
        // Si le bouton RTrigger est enclench�
        if (Input.GetButton("Fire"))
        {
            float normShootAxis = (Input.GetAxis("RTrigger") + 1) / 2;
            Debug.Log($"normShootAxis: {normShootAxis}");
            // Tire si l'axe du bouton d�passe 
            if (shootTriggerAxis <= shootThreshold && normShootAxis > shootThreshold)
            {
                GameObject missile = Instantiate(missilePrefab, firePoint.position, firePoint.rotation);
                missile.transform.Rotate(180f, 0, 0);
            }
            shootTriggerAxis = normShootAxis;
        }
    }
}
