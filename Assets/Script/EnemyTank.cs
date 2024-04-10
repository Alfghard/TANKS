using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTank : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform turret;
    float turretAngle = 0; 
    float baseAngle = 0;

    void Start()
    {
        
    }

    void Update()
    {
        bool paused = Pause.isGamePaused();    //Récupère la valeur de paused
        if (!paused) {
            TurretMovement();
            BaseMovement();
        }
    }

    public void OnCollisionEnter(Collision collision)   //Destruction du tank lors de la collision avec un Missile
    {
        if (collision.gameObject.tag == "Missile")
        {
            Destroy(gameObject);
        }
    }

    private void BaseMovement()
    {
        baseAngle = (baseAngle + 1) % 360;
        rb.rotation = Quaternion.Euler(0f, baseAngle, 0f);
    }

    private void TurretMovement()   //effectue un mouvement de rotation constant du canon
    {
        turretAngle = (turretAngle - 1) % 360;
        turret.rotation = Quaternion.Euler(0f, turretAngle, 0f);
    }
}
