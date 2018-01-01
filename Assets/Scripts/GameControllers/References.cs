using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Singleton class containing references to assets that need to be found by multiple objects / classes at runtime from the Resources folder(s).

public class References : MonoBehaviour 
{
	public PlayerControl playerControl;

	// Images / Animations:

	// Emotion:
	public GameObject emotionObj;
	public Sprite alarmEmotion, angerEmotion, confusionEmotion, hatredEmotion, normalEmotion, praiseEmotion, sadnessEmotion;

	// Healthbar:
	public GameObject healthbarObj;

	// Sounds / Music:

	// Injured:
	public AudioClip[] injuredSounds;


	public static References instance = null;

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
		// Scene Objects
		playerControl = GameObject.Find("PlayerControl").GetComponent<PlayerControl>();

		// GameObjects / Prefabs
		emotionObj = Resources.Load<GameObject> ("Emotion");
		healthbarObj = Resources.Load<GameObject> ("Healthbar");

		// Sprites
		alarmEmotion = Resources.Load<Sprite> ("Emotion/alarmEmotionBubble");
		angerEmotion = Resources.Load<Sprite> ("Emotion/angerEmotionBubble");
		confusionEmotion = Resources.Load<Sprite> ("Emotion/confusionEmotionBubble");
		hatredEmotion = Resources.Load<Sprite> ("Emotion/hatredEmotionBubble");
		normalEmotion = Resources.Load<Sprite> ("Emotion/normalEmotionBubble");
		praiseEmotion = Resources.Load<Sprite> ("Emotion/praiseEmotionBubble");
		sadnessEmotion = Resources.Load<Sprite> ("Emotion/sadnessEmotionBubble");

		// Sounds
		injuredSounds = Resources.LoadAll<AudioClip>("Injured");
	}
}
