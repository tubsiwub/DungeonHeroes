using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthbar : MonoBehaviour 
{
	public Character targetCharacter;
	public GameObject displayedHealth;

	float displayTime =  2.0f;

	Animator anim;

	void Start()
	{
		anim = GetComponent<Animator> ();
		anim.Play ("DisplayHealthbar");
	}

	void Update () 
	{
		if (targetCharacter == null)
			return;

		Rect newRect = MathFunctions.GUIRectWithObject (targetCharacter.gameObject);
		GetComponent<RectTransform> ().position = new Vector3(newRect.position.x + newRect.width/2, Screen.height - (newRect.position.y - newRect.height/2), 0);

		float displayedHealthSize = MathFunctions.ConvertNumberRanges(targetCharacter.health, targetCharacter.maxHealth, 0, 0, -50);
		displayedHealth.GetComponent<RectTransform> ().offsetMax = new Vector3 (displayedHealthSize, 0);

		if(displayTime <= 0 && References.instance.playerControl.selectedCharacter != targetCharacter)
			DetectMouse ();

		displayTime -= Time.deltaTime;
	}

	void DetectMouse()
	{
		Vector3 mousePosition = Input.mousePosition;
		Ray mouseRay = Camera.main.ScreenPointToRay (mousePosition);

		RaycastHit hit;
		if (Physics.Raycast (mouseRay, out hit))
		{
			if (hit.transform.gameObject != targetCharacter.gameObject)
			{
				targetCharacter.showingHealthbar = false;
				anim.Play ("RemoveHealthbar");
			}
		}
	}

	public void Destroy()
	{
		Destroy (this.gameObject);
	}
}
