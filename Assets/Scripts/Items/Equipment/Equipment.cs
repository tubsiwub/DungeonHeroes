using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Equipment", menuName = "Scriptable/Equipment")]
public class Equipment : ScriptableObject 
{
	[Header("Universal")]
	public Sprite spriteImage;
	[Tooltip("Heavier items put a burden on your character.")]
	public int weight;			// heaviest item in the game would weight 50 with most of the heaviest pieces weighing about 40 each
	[Tooltip("Directly affects combat interval.")]
	public float encumbrance;	// +/- to combat interval timer, with negatives making combat faster

	[Header("Attack")]
	[Tooltip("Adds to the base physical damage the character does to other characters in combat.")]
	public int weaponAttack;
	[Tooltip("Adds to the base magical damage the character does to other characters in combat.")]
	public int magicAttack;

	[Header("Defense")]
	[Tooltip("Directly affects physical damage reduction.")]
	public int armorDefense;
	[Tooltip("Directly affects magical damage reduction.")]
	public int armorMagicResistance;

	void Start () 
	{
		
	}
	
	void Update () 
	{
		
	}
}
