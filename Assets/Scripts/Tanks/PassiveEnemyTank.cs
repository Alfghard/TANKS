using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class PassiveEnemyTank : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;                // RigidBody du tank
    [SerializeField] private Transform turret;            // Objet Tourelle
    [SerializeField] private Transform firePoint;         // Point de tir
    [SerializeField] private GameObject missilePrefab;    // Prefab du missile
    //[SerializeField] private float turretSmoothness = 0.5f; // Temps de rotation de la tourelle
    [SerializeField] private float fireInterval = 3f;     // Intervalle de tir

    private Transform playerTank;                         // Référence au tank du joueur
    private float fireTimer = 0;
    private float behaviourTimer = 0;
    private bool randomTurretDir;
    private float randomTurretSpeed;
    private float turretAngle;                            // État de l'angle de la tourelle
    //private float turretCurrentSpeed = 0f;                // État de la vitesse angulaire de la tourelle

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
            behaviourTimer -= Time.deltaTime;
            fireTimer += Time.deltaTime;

            if (behaviourTimer <= 0) {
                randomTurretDir = Random.value > 0.5;
                print(randomTurretSpeed);
                behaviourTimer = Random.Range(1f, 3f);
            }

            TankFunctions.Patrol(turret, randomTurretDir, 60f);

            if (fireTimer >= fireInterval)
            {
                TankFunctions.Shoot(missilePrefab, firePoint);
                fireTimer = 0;
            }
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
