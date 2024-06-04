using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton instance

    public Text scoreText;    // Référence au texte du score
    public Text timerText;    // Référence au texte du chronomètre
    private int score = 0;    // Variable pour stocker le score
    private float timer = 0f; // Variable pour stocker le temps  écoulé 
    private static int nbEnnemiesAvant ; // variable pour stocker le nombre d'ennemis  précédents
    private static GameObject[] EnnTab;

    

    void Awake()
    {
        // Si une instance existe déjà  et n'est pas celle-ci, détruisez cette instance
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    void Start()
    {
        UpdateScoreText();
        UpdateTimerText();
        nbEnnemiesAvant = UpdateNbEnnemies();
    }

    void Update()
    {
        bool paused = Pause.isGamePaused();    //Récupère la valeur de paused
        if (!paused) {
            // Met   jour le chronom tre
            timer += Time.deltaTime;
            UpdateTimerText();
            UpdateNbEnnemies();
            int ecart = TestUpdateScore(); //Vérifie si un Tank ennemi a été tué
            if (ecart>0){
                AddScore(ecart*10);
            }
        }
    }

    static int TestUpdateScore()
    {
        int current = UpdateNbEnnemies();
        int ecart = 0;
        if (nbEnnemiesAvant != current)
        {   
            ecart = nbEnnemiesAvant - current;
            nbEnnemiesAvant = nbEnnemiesAvant - ecart; //devrait être égal à current
        }
        return ecart;

    }

    public static int UpdateNbEnnemies()
    {
        EnnTab = GameObject.FindGameObjectsWithTag("Tank");
        int nbEnnemies = 0;
        foreach(GameObject Enn in EnnTab) {
            nbEnnemies += 1;
        }
        return nbEnnemies;
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
            print(score);
        }
    }

    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            timerText.text = "Time: " + Mathf.FloorToInt(timer).ToString() + "s";
        }
    }

    public void SetUIReferences(Text score, Text timer)
    {
        scoreText = score;
        timerText = timer;
        UpdateScoreText();
        UpdateTimerText();
    }
}
