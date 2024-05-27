using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuCanvas : MonoBehaviour
{
    [SerializeField] private GameObject canvas;

    // Start is called before the first frame update
    void Start()
    {
        canvas.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        canvas.SetActive(Pause.isGamePaused());
    }
}
