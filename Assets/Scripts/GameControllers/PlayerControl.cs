using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour 
{
	public Character selectedCharacter;

	public GameObject HUDSelector;

	public AudioSource[] audioChannel;	// 0 - music, 1 - ambience, 2 - talking, 3 - combat sounds, 4 - jingle, 

	Vector3 mousePosition;
	Ray mouseRay;

	// MouseClick variables
	float doubleClickTimer = 0.2f;
	float doubleClickCounter = 0;
	float mouseOverCooldown = 0.2f;

	void Start () 
	{
		
	}

	void Update () 
	{
		mousePosition = Input.mousePosition;
		mouseRay = Camera.main.ScreenPointToRay (mousePosition);

		bool doubleClick;

		Transform hoverTarget = MouseOver ();
		Transform clickTarget = MouseClick (out doubleClick);

		MouseOverAction (hoverTarget);

		if(!doubleClick)
			SingleClick (clickTarget);
		else
			DoubleClick (clickTarget);

		HUDSelector.GetComponent<Image> ().enabled = selectedCharacter;

		if (selectedCharacter)
		{
			Vector3 characterScreenPos = Camera.main.WorldToScreenPoint (selectedCharacter.transform.position);
			if (HUDSelector) HUDSelector.transform.position = characterScreenPos;
		}
	}

	void MouseOverAction(Transform hoverTarget)
	{
		if (hoverTarget == null)
			return;
		
		if (hoverTarget.tag.ToLower () == "character" && mouseOverCooldown <= 0)
		{
			if(!hoverTarget.GetComponent<Character> ().showingHealthbar)
				hoverTarget.GetComponent<Character> ().ShowHealthbar ();

			mouseOverCooldown = 0.2f;
		}
		mouseOverCooldown -= Time.deltaTime;
	}

	void SingleClick(Transform clickTarget)
	{
		if (clickTarget == null)
			return;

		// When we click a unit, display healthbar and select tit
		if (clickTarget.tag.ToLower () == "character")
		{
			if (clickTarget.GetComponent<Hero> ())
				selectedCharacter = clickTarget.GetComponent<Hero> ();

			if (!clickTarget.GetComponent<Character> ().showingHealthbar)
				clickTarget.GetComponent<Character> ().ShowHealthbar ();
		}
	}

	void DoubleClick(Transform clickTarget)
	{
		if (clickTarget == null)
			return;
		
		if (clickTarget.tag.ToLower () == "character")
		{
			if (clickTarget.GetComponent<Hero> ())
				selectedCharacter = clickTarget.GetComponent<Hero> ();

			if (!clickTarget.GetComponent<Character> ().showingHealthbar)
				clickTarget.GetComponent<Character> ().ShowHealthbar ();
		}
		else
		{
			SetTargetPosition ();
		}
	}


	public float radiusOfWireSphere;

	void OnDrawGizmos()
	{
		if (!Application.isPlaying || !selectedCharacter)
			return;

		Gizmos.color = Color.red;
		foreach (Vector3 v in selectedCharacter.posList)
		{
			Gizmos.DrawWireSphere (v, radiusOfWireSphere);
		}
	}

	Transform MouseOver()
	{
		Transform hoverTarget = null;

		RaycastHit hit;
		if (Physics.Raycast (mouseRay, out hit))
		{
			hoverTarget = hit.transform;
		}

		return hoverTarget;
	}

	Transform MouseClick(out bool doubleClick)
	{
		Transform clickTarget = null;
		doubleClick = false;

		if (doubleClickTimer > 0)
			doubleClickTimer -= Time.deltaTime;

		if (doubleClickTimer <= 0)
			doubleClickCounter = 0;

		if (Input.GetMouseButtonDown (0))
		{
			doubleClickTimer = 0.2f;

			// Single Action
			RaycastHit hit;
			if (Physics.Raycast (mouseRay, out hit))
			{
				clickTarget = hit.transform;

				if(selectedCharacter)
				if(selectedCharacter.posList.Count > 0)
				if (Vector3.Distance (hit.point, selectedCharacter.posList [0]) < 0.4f)
					ClearTargetPosition ();
			}

			// Double Action
			if (doubleClickCounter > 0)
			{
				doubleClick = true;
				clickTarget = DoubleClick (mouseRay);
			}
			else
				doubleClickCounter += 1;
		}

		if (Input.GetMouseButtonUp (0))
		{
			if (doubleClickTimer <= 0)
				DeselectCharacter ();
		}

		return clickTarget;	// returns single or double click
	}

	Transform DoubleClick(Ray mouseRay)
	{
		Transform clickTarget = null;

		doubleClickCounter = 0;

		RaycastHit hit;
		if (Physics.Raycast (mouseRay, out hit))
		{
			clickTarget = hit.transform;
		}

		return clickTarget;
	}

	void ClearTargetPosition()
	{
		selectedCharacter.posList.Clear ();
	}

	void DeselectCharacter()
	{
		selectedCharacter = null;
	}

	void SetTargetPosition()
	{
		RaycastHit hit;
		if (Physics.Raycast (mouseRay, out hit))
		{
			Vector3 worldPos = hit.point;

			if(selectedCharacter)
				selectedCharacter.posList.Add (worldPos);
		}
	}
}
