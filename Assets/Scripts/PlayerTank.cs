using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


/// <summary>
/// Classe gérant les intéractions avec le tank du joueur
/// </summary>
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
    internal static bool ControlManette;

    private static int nbMissiles = 0;
    private static GameObject[] EnnTab;

    void Start()
    {
        turretAngle = turret.eulerAngles.y;
        Physics.gravity = new Vector3(0, -1000, 0);
    }

    /// <summary>
    /// Boucle de jeu gérant entre autre les entrées clavier et/ou manette, et lance les scripts de mouvements et de tir
    /// </summary>
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

            //Permet de choisir les contrôles voulues
            if(ControlManette)  
            {
                TurretMovement(shootDir);   // Mecanisme de rotation de la tourelle du tank
            }
            else
            {
                TurretMovement_Souris();  // Mecanisme de rotation de la tourelle du tank avec la souris
            }
            
            
            UpdateNbMissiles();
            // Si le bouton RTrigger est enclenche
            if (nbMissiles < 5) {
                if (Input.GetButton("Fire"))
                {
                    // Calcul de l'activation du tir à la moitié de la pression du bouton
                    float normShootAxis = (Input.GetAxis("RTrigger") + 1) / 2;

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
    }

    /// <summary>
    /// Déplace le tank dans la direction demandée</br>
    /// Crée la logique de déplacement en deux temps :
    /// <ol>
    ///   <li>Ajuste la rotation du tank si celui-ci n'est pas orienté dans la même direction que le déplacement et l'empêche d'avancer</li>
    ///   <li>Si la rotation du tank selon la direction demandée est compris dans l'angle de seuil, alors le déplacement du tank est enclenché</li>
    /// </ol>
    /// </summary>
    /// <param name="direction">Vecteur indiquant la direction vers laquelle se déplacer</param>
    
    public static void UpdateNbMissiles()
    {
        EnnTab = GameObject.FindGameObjectsWithTag("MissilePlayer");
        nbMissiles = 0;
        foreach(GameObject Enn in EnnTab) {
            nbMissiles += 1;
        }
        return;
    }
    
    public void TankMovement(Vector3 direction)
    {
        // N'ex�cute le mouvement que si une norme minimale de direction est enclench�e (zone morte)
        if (direction.magnitude >= 0.1f)
        {
            // Calcule l'angle de rotation necessaire pour faire face a la directione
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref baseCurrentSpeed, tankSmoothness);
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
                Vector3 move = transform.forward * (float)norme * tankSpeed;
                rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
            }
        }
    }

    /// <summary>
    /// Déclenche le mouvement de la tourelle</br>
    /// Calcule l'angle de rotation de la tourelle à partir du vecteur des coordonnées X et Y du joystick 
    /// et effectue la rotation à partir d'une fonction qui s'assure que le mouvement ne soit pas brusque
    /// </summary>
    /// <param name="shootDir">Vecteur des coordonnées en X et Y du joystick</param>
    public void TurretMovement(Vector3 shootDir)
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
    }

    /// <summary>
    /// Lance un tir du tank<\br>
    /// Instancie un objet missile à la sortie du canon et oriente le missile correctement
    /// </summary>

    private void TurretMovement_Souris()
    {   //gère les commandes avec la souris
        Vector3  mouseInScreen = Input.mousePosition;
    
        mouseInScreen.z = mouseInScreen.y;
        //Centrage
        mouseInScreen.x += -950;
        mouseInScreen.z += -500;
        //Rescale
        mouseInScreen = mouseInScreen/36;
        mouseInScreen.y= 1.07F;
        //tilted
        mouseInScreen.z = mouseInScreen.z * Mathf.Cos(mouseInScreen.z * Mathf.Deg2Rad);
        mouseInScreen.z = mouseInScreen.z * 1.5F - 2;

        Vector3 direction = turret.position - mouseInScreen;
        turret.rotation = Quaternion.LookRotation(direction);

    }
    public void Shoot()
    {
        GameObject missile = Instantiate(missilePrefab, firePoint.position, firePoint.rotation);
        missile.transform.Rotate(180f, 0, 0);
    }
}
