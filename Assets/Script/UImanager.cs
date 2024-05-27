using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text scoreText; // R�f�rence au texte du score dans cette sc�ne
    public Text timerText; // R�f�rence au texte du chronom�tre dans cette sc�ne

    void Start()
    {
        GameManager.instance.SetUIReferences(scoreText, timerText);
    }
}
