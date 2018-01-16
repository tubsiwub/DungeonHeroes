using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// actions called from within animation events

public class CutsceneCamera : MonoBehaviour 
{
	public Cutscene cutscene;

	public void CheckCutscene()
	{
		cutscene.CheckCutscene ();
	}

	// separate values by spaces - object name, animation name
	public void AnimateObject(string animName)
	{
		string[] names = animName.Split (' ');
		print (names [0] + " : " + names [1]);
		GameObject.Find(names[0]).GetComponent<Animator> ().Play (names[1]);
	}
}
