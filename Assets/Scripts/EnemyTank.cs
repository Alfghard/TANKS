using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTank : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;                // RigidBody du tank
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform turret;            // Objet Tourelle
    [SerializeField] private Transform firePoint;         // Point de tir
    [SerializeField] private GameObject missilePrefab;    // Prefab du missile
    [SerializeField] private float detectionRange = 50f;  // Portée de détection du joueur
    [SerializeField] private float angleThreshold = 60f;  // Angle de liberté pour autoriser le déplacement
    [SerializeField] private float tankSmoothness = 0.12f;// Temps de rotation du tank
    [SerializeField] private float turretSmoothness = 0.05f; // Temps de rotation de la tourelle
    [SerializeField] private float fireInterval = 3f;     // Intervalle de tir

    private Transform playerTank;                         // Référence au tank du joueur
    private float fireTimer = 0;
    private float baseCurrentSpeed = 0f;                  // État de la vitesse angulaire de la base du tank
    private float turretAngle;                            // État de l'angle de la tourelle
    private float turretCurrentSpeed = 0f;                // État de la vitesse angulaire de la tourelle

    private int layer_wall;
    private int layer_joueur;
    private int layer_ennemi;



    


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

        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        turretAngle = turret.eulerAngles.y;

        layer_wall = LayerMask.GetMask("Wall");
        Debug.Log("Wall is " + layer_wall);
        layer_joueur = LayerMask.GetMask("Joueur");
        Debug.Log("Player is " + layer_joueur);
        layer_ennemi = LayerMask.GetMask("Ennemi");
        Debug.Log("Ennemi is " + layer_ennemi);
    }

    void Update()
    {
        if (playerTank == null) return; // S'assure que playerTank est assigné avant d'exécuter le reste du code
        
        int layer = RayCheckForward();

        bool paused = Pause.isGamePaused();    // Récupère la valeur de paused
        
        if (!paused)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTank.position);
            if (distanceToPlayer <= detectionRange)
            {   
                MoveTowardsPlayer();
                TurretMovement();
                fireTimer += Time.deltaTime;
                //Debug.Log(layer);
                if (layer == layer_wall) {}
                else if (layer == layer_ennemi){}
                else if (layer == layer_joueur){
                    if (fireTimer >= fireInterval)
                    {
                    Shoot();
                    fireTimer = 0;
                    }

                }
                
            }
            else
            {
                Patrol();
            }
        }
        else
        {
            agent.velocity = new Vector3(0, 0, 0);
        }
    }

    public void OnCollisionEnter(Collision collision)   // Destruction du tank lors de la collision avec un Missile
    {
        if (collision.gameObject.CompareTag("Missile") | collision.gameObject.CompareTag("MissilePlayer"))
        {
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
            agent.stoppingDistance = 10;
            agent.SetDestination(playerTank.position); //Utilise NavMesh pour se déplacer vers le joueur
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

    private int RayCheckForward() {
        float dist_ennemi = Mathf.Infinity;
        float dist_wall = Mathf.Infinity;
        float dist_joueur = Mathf.Infinity;
        RaycastHit hit;
        Vector3 direction =  firePoint.position - turret.position ; 
        Ray ray = new Ray(turret.position, direction);
        //Debug.DrawRay(turret.position, direction*20, Color.red ); //Permet de voir les rayons lasers
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer_ennemi)){
            dist_ennemi = hit.distance;
        }
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, layer_wall)){
            //Debug.Log(hit.transform.name + " traverse le rayon.");
            //Debug.Log("La distance est de " + hit.distance);
            dist_wall = hit.distance;
        }
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer_joueur)){
            dist_joueur = hit.distance;
        }
        Dictionary<float, int> distances = new Dictionary<float, int>();
        distances[dist_ennemi] = layer_ennemi;
        distances[dist_wall] = layer_wall;
        distances[dist_joueur] = layer_joueur;
        float distMin = MathF.Min(MathF.Min(dist_ennemi,dist_joueur),dist_wall); //prend le minimum des 3 distances
        return distances[distMin];




    }
}
