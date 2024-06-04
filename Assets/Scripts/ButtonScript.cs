using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
	public void ChangeSceneButton(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
		bool paused = Pause.isGamePaused();
        if (paused) { Pause.changePauseState(); }
	}
	public void Exit()
	{
		Application.Quit();
	}
}