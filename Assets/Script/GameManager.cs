using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text scoreText;    // R�f�rence au texte du score
    public Text timerText;    // R�f�rence au texte du chronom�tre
    private int score = 0;    // Variable pour stocker le score
    private float timer = 0f; // Variable pour stocker le temps �coul�

    void Start()
    {
        scoreText.text = "Score: 0";
        timerText.text = "Time: 0s";
    }

    void Update()
    {
        // Met � jour le chronom�tre
        timer += Time.deltaTime;
        timerText.text = "Time: " + Mathf.FloorToInt(timer).ToString() + "s";
    }

    public void AddScore(int points)
    {
        score += points;
        scoreText.text = "Score: " + score.ToString();
    }
}
