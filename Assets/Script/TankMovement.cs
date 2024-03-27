using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] float speed;

    void Start()
    {
        Physics.gravity = new Vector3(0, -1000, 0);
    }

    void Update()
    {
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        rb.velocity = direction * speed;
        if (Input.GetMouseButtonDown(0))
        {
            rb.AddForce(Vector3.up * 10000);
        }

    }
}