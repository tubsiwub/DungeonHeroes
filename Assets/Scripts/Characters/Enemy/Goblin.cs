using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Enemy 
{
	// ...

	public new void Start () 
	{
		base.Start ();

		PresetStats ();

		SetAllegiances ();
	}

	public new void Update () 
	{
		base.Update ();


	}

	void PresetStats()
	{
		health = 35;
		maxHealth = health;
		mana = 40;
		maxMana = mana;
		gold = 0;
		karma = -15;

		physicalDamage = 7;
		magicalDamage = 4;
		physicalDefense = 6;
		magicalDefense = 3;

		// 40 baseline - out of 80 / 100 with special stat boosts (max)
		// total stat spread: 440
		strengthStat = 22;
		agilityStat = 54;
		staminaStat = 26;
		intelligenceStat = 21;
		wisdomStat = 12;
		dexterityStat = 54;
		poiseStat = 8;
		charismaStat = 67;
		perceptionStat = 40;
		luckStat = 72;
		speedStat = 64;
	}

	void SetAllegiances()
	{
		currentFaction = Factions.monster;

		alliedFactions.Add (Factions.monster);
		alliedFactions.Add (Factions.terror);

		enemyFactions.Add (Factions.undead);
		enemyFactions.Add (Factions.rival);
		enemyFactions.Add (Factions.player);
		enemyFactions.Add (Factions.alliance);
		enemyFactions.Add (Factions.elves);
		enemyFactions.Add (Factions.dwarves);
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
