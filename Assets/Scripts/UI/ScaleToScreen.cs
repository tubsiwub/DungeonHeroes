using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScaleToScreen : MonoBehaviour 
{
	float screenWidth, screenHeight;
	float fullScreenWidth, fullScreenHeight;

	public bool useFullscreenForCalculations = false;

	[Tooltip("Percentage of the screen we're showing.")]
	[Header("Percentage Size")]
	public float left;
	public float right;
	public float top;
	public float bottom;

	void OnEnable () 
	{
		SetDimensions ();
	}

	void Update () 
	{
		if (fullScreenWidth != Screen.width || fullScreenHeight != Screen.height)
			SetDimensions ();
		
		if (screenWidth != transform.parent.GetComponent<RectTransform>().rect.width 
		|| screenHeight != transform.parent.GetComponent<RectTransform>().rect.height)
			SetDimensions ();

		if (GetComponent<RectTransform> ().offsetMin != new Vector2 (left * screenWidth, bottom * screenHeight)
		|| GetComponent<RectTransform> ().offsetMax != new Vector2 (-1 * (right * screenWidth), -1 * (top * screenHeight)))
			SetDimensions ();
	}

	void SetDimensions()
	{
		fullScreenWidth = Screen.width;
		fullScreenHeight = Screen.height;
		screenWidth = transform.parent.GetComponent<RectTransform> ().rect.width;
		screenHeight = transform.parent.GetComponent<RectTransform> ().rect.height;

		//print (screenWidth + " : " + screenHeight);

		// If it's above 1.0, we assume the input is actual percentage
		if(left > 1.0f)
			left *= 0.01f;
		if(right > 1.0f)
			right *= 0.01f;
		if(top > 1.0f)
			top *= 0.01f;
		if(bottom > 1.0f)
			bottom *= 0.01f;

		// Left, -Bottom
		GetComponent<RectTransform> ().offsetMin = new Vector2 (left * screenWidth, bottom * screenHeight);

		// -Right, Top
		GetComponent<RectTransform> ().offsetMax = new Vector2 (-1 * (right * screenWidth), -1 * (top * screenHeight));

		// Reset each child
		if(transform.childCount > 0)
			foreach (Transform child in transform)
			{
				if(child.GetComponent<FitIntoMask>())
					child.GetComponent<FitIntoMask> ().Rescale ();
			}
	}
}
