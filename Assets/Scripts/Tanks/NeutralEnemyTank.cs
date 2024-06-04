using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class NeutralEnemyTank : MonoBehaviour
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
    private float behaviourTimer;
    private bool focusTowardPlayer;
    private float baseCurrentSpeed = 0f;                  // État de la vitesse angulaire de la base du tank
    private float turretAngle;                            // État de l'angle de la tourelle
    private float turretCurrentSpeed = 0f;                // État de la vitesse angulaire de la tourelle
    Vector3 target;
    float angle;
    Vector3 targetMinus;
    Vector3 targetPlus;
    float norma;
    Vector3 direction;

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
            
            TankFunctions.TurretMovementTowardPlayer(playerTank, turret, turretCurrentSpeed, turretSmoothness);

            fireTimer += Time.deltaTime;
            behaviourTimer -= Time.deltaTime;
            
            if (behaviourTimer <= 0) {
                focusTowardPlayer = UnityEngine.Random.value > 0.5;
                focusTowardPlayer = false;

                Vector3 direction = (playerTank.position - transform.position);        
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

                /*float angle = targetAngle;
                print("targetAngle" + targetAngle);
                print("angle " + angle);

                Vector3 tagetPlus = new Vector3(Mathf.Cos(angle)*direction.x - Mathf.Sin(angle)*direction.z, direction.y, Mathf.Sin(angle)*direction.x + Mathf.Cos(angle)*direction.z) + transform.position;
                print("cos(angle) = " + Mathf.Cos(angle));
                //print(targetPlus);

                angle = targetAngle - Mathf.PI/2;
                Vector3 targetMinus = new Vector3(Mathf.Cos(angle)*direction.x - Mathf.Sin(angle)*direction.z, direction.y, Mathf.Sin(angle)*direction.x + Mathf.Cos(angle)*direction.z) + transform.position;

                float norma = targetPlus.magnitude - targetMinus.magnitude;

                if (norma >= 0) { Vector3 target = targetPlus; }
                else { Vector3 target = targetMinus; }*/
                target = new Vector3(UnityEngine.Random.Range(-10f, 10f), 0, UnityEngine.Random.Range(-10f, 10f)) + transform.position;
                

                behaviourTimer = UnityEngine.Random.Range(3f, 5f);
            }

            if (focusTowardPlayer) {
                TankFunctions.MoveTowardPlayer(transform, playerTank, baseCurrentSpeed, tankSmoothness, rb, angleThreshold, agent); 
            }
            else {
                TankFunctions.MovePerpendicularPlayer(transform, playerTank, baseCurrentSpeed, tankSmoothness, rb, angleThreshold, agent, target); 
            }

            if (layer == layer_wall) {}
            else if (layer == layer_ennemi) {}
            else if (layer == layer_joueur) {
                if (fireTimer >= fireInterval) {
                TankFunctions.Shoot(missilePrefab, firePoint);
                fireTimer = 0;
                }

            }
        }
        else {
            //agent.velocity = new Vector3(0,0,0);
            TankFunctions.MovePerpendicularPlayer(transform, playerTank, baseCurrentSpeed, tankSmoothness, rb, angleThreshold, agent,target);
        }
    }
    

    private void OnCollisionEnter(Collision collision)   // Destruction du tank lors de la collision avec un Missile
    {
        if (collision.gameObject.CompareTag("Missile") | collision.gameObject.CompareTag("MissilePlayer"))
        {
            Destroy(gameObject);
        }
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
