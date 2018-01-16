using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Singleton class containing references to assets that need to be found by multiple objects / classes at runtime from the Resources folder(s).

public class MapPreferences : MonoBehaviour 
{
	public float mapRadius;
	public float mapFarRadius;

	public static MapPreferences instance = null;

	void Awake()
	{
		#region Singleton
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
		#endregion
	}

	void Start()
	{
		
	}
}
