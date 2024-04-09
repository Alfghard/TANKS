using UnityEngine;
using System.Collections;

public class Pause : MonoBehaviour 
{
	static bool paused = false;

	void Start () 
	{
		Update ();
	}
	
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			changePauseState ();
		}
	}

	public static void changePauseState ()
	{
		paused = !paused;
	}

	public static bool getPauseState ()
	{
		return paused;
	}
}