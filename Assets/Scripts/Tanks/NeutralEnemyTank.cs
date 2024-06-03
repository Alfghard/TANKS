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
    }

    void Update()
    {
        if (playerTank == null) return; // S'assure que playerTank est assigné avant d'exécuter le reste du code

        bool paused = Pause.isGamePaused();    // Récupère la valeur de paused
        if (!paused)
        {
            
            TankFunctions.TurretMovementTowardPlayer(playerTank, turret, turretCurrentSpeed, turretSmoothness);

            fireTimer += Time.deltaTime;
            behaviourTimer -= Time.deltaTime;

            if (behaviourTimer <= 0) {
                focusTowardPlayer = Random.value > 0.5;
                print(focusTowardPlayer);
                behaviourTimer = Random.Range(3f, 5f);
            }

            if (focusTowardPlayer) {
                TankFunctions.MoveTowardPlayer(transform, playerTank, baseCurrentSpeed, tankSmoothness, rb, angleThreshold, agent); 
            }
            else {
                TankFunctions.MovePerpendicularPlayer(transform, playerTank, baseCurrentSpeed, tankSmoothness, rb, angleThreshold, agent); 
            }

            if (fireTimer >= fireInterval)
            {
                TankFunctions.Shoot(missilePrefab, firePoint);
                fireTimer = 0;
            }
        }
        else {
            //agent.velocity = new Vector3(0,0,0);
            TankFunctions.MovePerpendicularPlayer(transform, playerTank, baseCurrentSpeed, tankSmoothness, rb, angleThreshold, agent);
        }
    }
    

    private void OnCollisionEnter(Collision collision)   // Destruction du tank lors de la collision avec un Missile
    {
        if (collision.gameObject.CompareTag("Missile") | collision.gameObject.CompareTag("MissilePlayer"))
        {
            Destroy(gameObject);
        }
    }

}
