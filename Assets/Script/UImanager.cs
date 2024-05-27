using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text scoreText; // Référence au texte du score dans cette scène
    public Text timerText; // Référence au texte du chronomètre dans cette scène

    void Start()
    {
        GameManager.instance.SetUIReferences(scoreText, timerText);
    }
}
