using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTank : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;                // RigidBody du tank
    [SerializeField] private Transform turret;            // Objet Tourelle
    [SerializeField] private Transform firePoint;         // Point de tir
    [SerializeField] private GameObject missilePrefab;    // Prefab du missile
    [SerializeField] private float detectionRange = 50f;  // Portée de détection du joueur
    [SerializeField] private float tankSpeed = 35f;       // Vitesse de déplacement du tank
    [SerializeField] private float angleThreshold = 60f;  // Angle de liberté pour autoriser le déplacement
    [SerializeField] private float tankSmoothness = 0.12f;// Temps de rotation du tank
    [SerializeField] private float turretSmoothness = 0.05f; // Temps de rotation de la tourelle
    [SerializeField] private float fireInterval = 3f;     // Intervalle de tir
    [SerializeField] private float minMoveSpeed = 5f;

    private Transform playerTank;                         // Référence au tank du joueur
    private GameManager gameManager;                      // Référence au GameManager
    private float fireTimer = 0;
    private float baseCurrentSpeed = 0f;                  // État de la vitesse angulaire de la base du tank
    private float turretAngle;                            // État de l'angle de la tourelle
    private float turretCurrentSpeed = 0f;                // État de la vitesse angulaire de la tourelle

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTank = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player Tank not found. Make sure the player tank has the tag 'Player'", gameObject);
        }

        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found. Make sure there is a GameManager script in the scene.", gameObject);
        }

        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        turretAngle = turret.eulerAngles.y;
    }

    void Update()
    {
        if (playerTank == null) return; // S'assure que playerTank est assigné avant d'exécuter le reste du code

        bool paused = Pause.isGamePaused();    // Récupère la valeur de paused
        if (!paused)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTank.position);
            if (distanceToPlayer <= detectionRange)
            {
                MoveTowardsPlayer();
                TurretMovement();
                fireTimer += Time.deltaTime;
                if (fireTimer >= fireInterval)
                {
                    Shoot();
                    fireTimer = 0;
                }
            }
            else
            {
                Patrol();
            }
        }
    }

    public void OnCollisionEnter(Collision collision)   // Destruction du tank lors de la collision avec un Missile
    {
        if (collision.gameObject.CompareTag("Missile"))
        {
            gameManager.AddScore(10); // Ajouter 10 points lorsque le tank ennemi est détruit
            Destroy(gameObject);
        }
    }

    private void Patrol()
    {
        // Logique de patrouille simple : tourner sur place
        transform.Rotate(0, 20 * Time.deltaTime, 0);
    }

    private void MoveTowardsPlayer()
    {
        // Calcule la direction vers le joueur
        Vector3 direction = (playerTank.position - transform.position).normalized;

        // Calcule l'angle de rotation nécessaire pour faire face à la direction
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref baseCurrentSpeed, tankSmoothness);

        // Fait pivoter le tank
        rb.MoveRotation(Quaternion.Euler(0f, angle, 0f));

        // Recalcule la direction après la rotation pour s'assurer qu'il se déplace vers le joueur
        direction = (playerTank.position - transform.position).normalized;

        // Vérifie si le tank est orienté presque dans la bonne direction
        float angleToTarget = Vector3.Angle(transform.forward, direction);
        if (angleToTarget < angleThreshold)
        {
            // Calcule la norme de la vitesse entre 0 et 1 selon l'inclinaison du joystick
            float norme = Mathf.Min(direction.magnitude, 1f);

            // Applique la vitesse minimale pour éviter le patinage
            Vector3 move = direction * Mathf.Max(norme * tankSpeed, minMoveSpeed);
            rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
        }
        else
        {
            // Réduit progressivement la vitesse jusqu'à ce que le tank soit réaligné
            rb.velocity = rb.velocity * 0.9f;
        }
    }

    private void TurretMovement()
    {
        // Calcule la direction vers le joueur
        Vector3 direction = turret.position - playerTank.position;

        // Vérifie si la direction est suffisante pour enclencher le mouvement
        if (direction.magnitude >= 0.1f)
        {
            // Calcule l'angle cible basé sur la direction vers le joueur
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            // Obtient l'angle actuel de la tourelle et le lisse pour suivre la cible
            turretAngle = Mathf.SmoothDampAngle(turret.eulerAngles.y, targetAngle, ref turretCurrentSpeed, turretSmoothness);

            // Applique la rotation vers l'angle cible
            turret.rotation = Quaternion.Euler(0f, turretAngle, 0f);
        }
    }

    private void Shoot()
    {
        // Instantiate un missile et le tire
        GameObject missile = Instantiate(missilePrefab, firePoint.position, firePoint.rotation);
    }
}
