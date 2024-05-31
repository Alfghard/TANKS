using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VSCodeEditor;

public class Change : MonoBehaviour
{
    [SerializeField] private MonoBehaviour Script1;
    [SerializeField] private MonoBehaviour Script2;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))    {
            Script1.GetComponent<MonoBehaviour>().enabled = false;
        }
    }
}
