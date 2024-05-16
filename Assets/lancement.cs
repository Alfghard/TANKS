using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lancement : MonoBehaviour
{
    [SerializeField] MapGenerator generator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) {
            generator.enabled = true;
        };
    }
}
