using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character 
{
	public new void Start () 
	{
		base.Start ();
	}

	public new void Update () 
	{
		base.Update ();
	}

	public override void PlaySound(string type, float volume)
	{
		AudioClip sound = null;
		int channel = 4;

		switch (type.ToLower())
		{

		case "injured":
			sound = References.instance.injuredSounds [2];
			channel = 3;
			break;

		}

		// Play sound!
		References.instance.playerControl.audioChannel[channel].PlayOneShot(sound, volume);
	}
}
