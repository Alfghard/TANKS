//import
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] float speed ;


    // Start is called before the first frame update once
    void Start()
    {
        Physics.gravity = new Vector3(0, -1000, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical")).normalized;
        rb.velocity = direction*speed;
        if(Input.GetMouseButtonDown(0)){
            rb.AddForce(Vector3.up*10000);
        }
        
    }
}
