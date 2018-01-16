using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class FitIntoMask : MonoBehaviour 
{
	RectTransform parentTransform;

	void OnEnable () 
	{
		Rescale ();
	}
	
	public void Rescale () 
	{
		parentTransform = transform.parent.GetComponent<RectTransform> ();

		Rect rect = GetComponent<RectTransform>().rect;
		rect.width = parentTransform.rect.width;
		rect.height = parentTransform.rect.height;
		GetComponent<RectTransform> ().sizeDelta = rect.size;
	}
}
