using System;
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
            behaviourTimer -= Time.deltaTime;
            fireTimer += Time.deltaTime;

            if (behaviourTimer <= 0) {
                randomTurretDir = UnityEngine.Random.value > 0.5;
                print(randomTurretSpeed);
                behaviourTimer = UnityEngine.Random.Range(1f, 3f);
            }

            TankFunctions.Patrol(turret, randomTurretDir, 60f);

            if (layer == layer_wall) {}
            else if (layer == layer_ennemi) {}
            else if (layer == layer_joueur) {
                if (fireTimer >= fireInterval) {
                TankFunctions.Shoot(missilePrefab, firePoint);
                fireTimer = 0;
                }

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
