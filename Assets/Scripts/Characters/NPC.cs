using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPC", menuName = "Scriptable/NPC")]
public class NPC : ScriptableObject 
{
	public Sprite portraitSprite;

	[Header("Experience")]
	public int startingLevel;
	public int startingExperience;

	[Header("Personality")]
	public Character.Personality personality;

	[Header("Inventory")]
	public Character.Inventory inventory;

	[Header("Equipped Items")]
	public Character.EquippedItems equipped;

	[Header("Factions")]
	public List<Character.Factions> alliedFactions;
	public List<Character.Factions> enemyFactions;
	[Space(16.0f)]
	public Character.Factions startingFaction;

	[Header("Resources")]
	public int health;
	public int mana;
	public int gold;
	public int karma;

	[Header("Combat")]
	public int physicalDamage;
	public int magicalDamage;
	public int physicalDefense;
	public int magicalDefense;

	// 40 baseline - out of 80 / 100 with special stat boosts (max)
	// total stat spread: 440
	[Header("Starting Attributes")]
	public int strengthStat;
	public int agilityStat;
	public int staminaStat;
	public int intelligenceStat;
	public int wisdomStat;
	public int dexterityStat;
	public int poiseStat;
	public int charismaStat;
	public int perceptionStat;
	public int luckStat;
	public int speedStat;

	[Header("Max Attributes")]
	public int strengthMax;
	public int agilityMax;
	public int staminaMax;
	public int intelligenceMax;
	public int wisdomMax;
	public int dexterityMax;
	public int poiseMax;
	public int charismaMax;
	public int perceptionMax;
	public int luckMax;
	public int speedMax;

	[Header("Level Up Variation")]
	public int strengthGain;
	public int agilityGain;
	public int staminaGain;
	public int intelligenceGain;
	public int wisdomGain;
	public int dexterityGain;
	public int poiseGain;
	public int charismaGain;
	public int perceptionGain;
	public int luckGain;
	public int speedGain;

	[Header("Rewards")]
	public int experienceDrop;
	public Character.Inventory lootItems;
	public Character.EquippedItems lootEquipment;

	void Start () 
	{
		
	}
	
	void Update () 
	{
		
	}
}
