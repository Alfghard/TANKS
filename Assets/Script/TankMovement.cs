using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed = 5f; // Vitesse de d�placement
    [SerializeField] private float turnSpeed = 180f; // Vitesse de rotation

    void Start()
    {
        Physics.gravity = new Vector3(0, -1000, 0);
    }

    void Update()
    {
        // Mouvement d'avant en arri�re
        float moveAmount = Input.GetAxis("Vertical") * speed;
        Vector3 move = transform.forward * moveAmount;
        rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);

        // Rotation
        float turnAmount = Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime;
        Quaternion turn = Quaternion.Euler(0f, turnAmount, 0f);
        rb.MoveRotation(rb.rotation * turn);

        // Saut (si n�cessaire)
        if (Input.GetMouseButtonDown(0))
        {
            rb.AddForce(Vector3.up * 10000);
        }
    }
}