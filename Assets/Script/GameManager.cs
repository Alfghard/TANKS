using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton instance

    public Text scoreText;    // R�f�rence au texte du score
    public Text timerText;    // R�f�rence au texte du chronom�tre
    private int score = 0;    // Variable pour stocker le score
    private float timer = 0f; // Variable pour stocker le temps �coul�

    void Awake()
    {
        // Si une instance existe d�j� et n'est pas celle-ci, d�truisez cette instance
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
    }

    void Update()
    {
        // Met � jour le chronom�tre
        timer += Time.deltaTime;
        UpdateTimerText();
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
