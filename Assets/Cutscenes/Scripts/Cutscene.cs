using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene : MonoBehaviour 
{
	public List<Camera> cutsceneCameras;
	public List<float> transitionLengths;

	int currentCamIndex = 0;

	Vector3 firstPosition;
	Quaternion firstRotation;

	void Start () 
	{
		// init
		firstPosition = cutsceneCameras[0].transform.position;
		firstRotation = cutsceneCameras [0].transform.rotation;

		// Check camera list
		if (cutsceneCameras == null || cutsceneCameras.Count <= 0)
			Debug.LogError ("List of Cutscene Cameras is either empty or NULL upon Awake.", this);

		// Check transition list
		if (transitionLengths == null || transitionLengths.Count <= 0)
			Debug.LogError ("List of Transition Lengths is either empty or NULL upon Awake.", this);

		foreach (Camera cam in cutsceneCameras)
			cam.enabled = false;
		
		// Start cutscene
		StartCoroutine("StartCutscene");
	}

	IEnumerator StartCutscene ()
	{
		// Turn on the first camera
		cutsceneCameras[currentCamIndex].enabled = true;
		cutsceneCameras[currentCamIndex].GetComponent<Animator>().enabled = false;

		// Position camera to match the main camera
		cutsceneCameras[currentCamIndex].transform.position = Camera.main.transform.position;
		cutsceneCameras[currentCamIndex].transform.rotation = Camera.main.transform.rotation;

		// Lerp to the position within its first animation while rotating
		while (Vector3.Distance (cutsceneCameras[currentCamIndex].transform.position, firstPosition) > 0.01f)
		{
			cutsceneCameras[currentCamIndex].transform.position = Vector3.MoveTowards (cutsceneCameras[currentCamIndex].transform.position, firstPosition, 4f * Time.deltaTime);
			cutsceneCameras[currentCamIndex].transform.rotation = Quaternion.Lerp (cutsceneCameras[currentCamIndex].transform.rotation, firstRotation, 1f * Time.deltaTime);
			yield return new WaitForEndOfFrame ();
		}
		cutsceneCameras[currentCamIndex].transform.position = firstPosition;
		cutsceneCameras [currentCamIndex].transform.rotation = firstRotation;
		cutsceneCameras[currentCamIndex].GetComponent<Animator>().enabled = true;

		// Now play cutscene
		cutsceneCameras[currentCamIndex].GetComponent<Animator> ().SetTrigger ("Play");
	}

	public void CheckCutscene()
	{
		// Called from an animation event within the current camera.

		StartCoroutine ("Transition");
	}

	IEnumerator Transition()
	{
		// Wait for the transition time specified, then move on to the next camera.

		float counter = transitionLengths [currentCamIndex];

		while (counter > 0)
		{
			counter -= Time.deltaTime;
			yield return new WaitForEndOfFrame ();
		}

		ProgressCutscene ();
	}

	public void ProgressCutscene()
	{
		if (currentCamIndex + 1 >= cutsceneCameras.Count)
		{
			// If we reach the end...
			CutsceneManager.instance.inProgress = false;
			Destroy(this.gameObject);
		}
		else
		{
			// Otherwise, progress the cutscene.

			// Knock out the current camera
			cutsceneCameras[currentCamIndex].enabled = false;

			// Then turn on the next camera
			currentCamIndex += 1;
			cutsceneCameras[currentCamIndex].enabled = true;

			cutsceneCameras[currentCamIndex].GetComponent<Animator> ().SetTrigger ("Play");
		}
	}
}
