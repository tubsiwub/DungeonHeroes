using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emotion : MonoBehaviour 
{
	public Character targetCharacter;

	Animator anim;

	void Start()
	{
		anim = GetComponent<Animator> ();
		anim.Play ("DisplayEmotion");
	}

	void Update () 
	{
		//Vector3 characterScreenPos = Camera.main.WorldToScreenPoint (targetCharacter.transform.position + Camera.main.transform.right + (Vector3.up*2));
		//transform.position = characterScreenPos;

		//Rect newRect = MathFunctions.GUIRectWithObject (targetCharacter.gameObject);
		//float newSize = (newRect.size.x + newRect.size.y) / 2;
		//GetComponent<RectTransform> ().position = new Vector3(newRect.position.x + newSize, Screen.height - newRect.position.y, 0);
		//GetComponent<RectTransform> ().sizeDelta = new Vector2(newSize, newSize);

		if (targetCharacter == null)
			return;

		Rect newRect = MathFunctions.GUIRectWithObject (targetCharacter.gameObject);
		float newSize = 25;
		GetComponent<RectTransform> ().position = new Vector3(newRect.position.x + newSize, Screen.height - newRect.position.y, 0);
		GetComponent<RectTransform> ().sizeDelta = new Vector2(newSize, newSize);
	}

	public void Destroy()
	{
		Destroy (this.gameObject);
	}
}
