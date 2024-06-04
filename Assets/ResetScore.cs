using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetScore : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Resetter();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Resetter(){
        GameManager GM = GameManager.FindFirstObjectByType<GameManager>();
        GM.ResetScore();
        GM.ResetTimer();
    }
}
