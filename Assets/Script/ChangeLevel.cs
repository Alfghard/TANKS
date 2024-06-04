using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeLevel : MonoBehaviour
{
    private static int nbEnnemies = 0;
    private static GameObject[] EnnTab;
    [SerializeField] private string SceneName;

    private static float timer;
    
    void Start()
    {
        UpdateNbEnnemies();
    }

    static void TestChangeScene(string SceneName)
    {
        if (nbEnnemies == 0) 
        {
            bool paused = Pause.isGamePaused();    // Récupère la valeur de paused
            if (SceneName == "VictoryScreen") {
                Pause.changePauseState();
            }
            SceneManager.LoadScene(SceneName);
        }
        return;
    }

    public static void UpdateNbEnnemies()
    {
        EnnTab = GameObject.FindGameObjectsWithTag("Tank");
        nbEnnemies = 0;
        foreach(GameObject Enn in EnnTab) {
            nbEnnemies += 1;
        }
        return;
    }

    void Update()
    {

        UpdateNbEnnemies();
        TestChangeScene(SceneName);
    }
}
