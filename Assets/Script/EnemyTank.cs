using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTank : MonoBehaviour
{
    [SerializeField] private Transform turret;
    float turretAngle = 0; 

    void Start()
    {
        
    }

    void Update()
    {
        bool paused = Pause.getPauseState();    //Récupère la valeur de paused
        if (!paused) {
            TurretMovement();
        }
    }

    public void OnCollisionEnter(Collision collision)   //Destruction du tank lors de la collision avec un Missile
    {
        if (collision.gameObject.tag == "Missile")
        {
            Destroy(gameObject);
        }
    }

    private void TurretMovement()   //effectue un mouvement de rotation constant du canon
    {
        turretAngle++;
        turret.rotation = Quaternion.Euler(0f, turretAngle, 0f);
    }
}
