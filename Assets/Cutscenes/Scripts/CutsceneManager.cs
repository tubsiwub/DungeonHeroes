using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour 
{
	public bool inProgress = false;

	public static CutsceneManager instance;
	void Start () 
	{
		#region Singleton
		if(instance == null)
			instance = this;
		if(instance != this)
			Destroy(instance);
		#endregion

		// ...
	}
	
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.K))
		{
			PlayCutscene ("CutsceneTest_1");
		}
	}

	public static void PlayCutscene(string name)
	{
		if (CutsceneManager.instance.inProgress)	// don't play a cutscene while one is already active
			return;

		var cutsceneObj = Resources.Load ("Cutscenes/" + name + "/" + name);
		Instantiate (cutsceneObj);

		CutsceneManager.instance.inProgress = true;
	}
}
