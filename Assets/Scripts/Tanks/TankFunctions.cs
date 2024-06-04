using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class TankFunctions : MonoBehaviour
{

    void Start()
    {
        
    }

    void Update()
    {

    }


    public static void Patrol(Transform transform, bool reversed, float speed)
    {
        // Logique de patrouille simple : tourner sur place
        if (reversed) {transform.Rotate(0, - speed * Time.deltaTime, 0);}
        else {transform.Rotate(0, speed * Time.deltaTime, 0);}
    }

    public static void MoveTowardPlayer(Transform transform, Transform playerTank, float baseCurrentSpeed, float tankSmoothness, Rigidbody rb, 
    float angleThreshold, NavMeshAgent agent)
    {
        agent.stoppingDistance = 10;
        agent.SetDestination(playerTank.position); //Utilise NavMesh pour se déplacer vers le joueur
    }

    public static void MovePerpendicularPlayer(Transform transform, Transform playerTank, float baseCurrentSpeed, float tankSmoothness, Rigidbody rb, 
    float angleThreshold, NavMeshAgent agent)
    {
        // Calcule la direction vers le joueur
        Vector3 direction = (playerTank.position - transform.position).normalized;

        // Calcule l'angle de rotation nécessaire pour faire face à la direction
        float targetAngle = (Mathf.Atan2(direction.x, direction.z) + 90)* Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref baseCurrentSpeed, tankSmoothness);
        
        Vector3 target = new Vector3(playerTank.position.x*Mathf.Cos(90), playerTank.position.y*Mathf.Sin(90), playerTank.position.z);

        agent.stoppingDistance = 0;
        agent.SetDestination(target); //Utilise NavMesh pour se déplacer vers le joueur
    }



    public static void TurretMovementTowardPlayer(Transform playerTank, Transform turret, float turretCurrentSpeed, float turretSmoothness)
    {
        // Calcule la direction vers le joueur
        Vector3 direction = turret.position - playerTank.position;

        // Vérifie si la direction est suffisante pour enclencher le mouvement
        if (direction.magnitude >= 0.1f)
        {
            // Calcule l'angle cible basé sur la direction vers le joueur
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            // Obtient l'angle actuel de la tourelle et le lisse pour suivre la cible
            float turretAngle = Mathf.SmoothDampAngle(turret.eulerAngles.y, targetAngle, ref turretCurrentSpeed, turretSmoothness);

            // Applique la rotation vers l'angle cible
            turret.rotation = Quaternion.Euler(0f, turretAngle, 0f);
        }
    }


    public static void Shoot(GameObject missilePrefab, Transform firePoint)
    {
        // Instantiate un missile et le tire
        GameObject missile = Instantiate(missilePrefab, firePoint.position, firePoint.rotation);
    }
}
