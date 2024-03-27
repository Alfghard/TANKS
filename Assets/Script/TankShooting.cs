using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankShooting : MonoBehaviour
{

    [SerializeField] public Transform firePoint;
    [SerializeField] public GameObject missilePrefab;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject missile = Instantiate(missilePrefab, firePoint.position, firePoint.rotation);
            missile.transform.Rotate(90f, 0, 0);
        }
        
    }
}
