using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour 
{
	[Header("Portrait")]
	public Image characterPortrait;

	[Header("Tabs")]
	public GameObject StatsTab;
	public GameObject EquipmentTab;
	public GameObject PersonalityTab;

	[Header("Inventory")]
	public Image[] inventoryItem;

	[Header("Stats")]
	public TextMeshProUGUI HealthMana;
	public TextMeshProUGUI GoldKarma;
	public TextMeshProUGUI PhysicalMagical;
	public TextMeshProUGUI PhysicalMagical2;
	public TextMeshProUGUI Attributes;
	public TextMeshProUGUI Attributes2;

	InventoryItem[] slots;
	Character currentCharacter;

	void Start () 
	{
		HideUI ();
	}
	
	void Update () 
	{
		if (currentCharacter != References.instance.playerControl.selectedCharacter)
		{
			ShowUI ();
		}

		currentCharacter = References.instance.playerControl.selectedCharacter;

		if (currentCharacter == null)
		{
			HideUI ();
			return;
		}


		UpdateUI();
	}

	void HideUI()
	{
		GetComponent<RectTransform> ().localScale = Vector3.zero;
		StatsTab.GetComponent<RectTransform> ().localScale = Vector3.zero;
		//EquipmentTab.GetComponent<RectTransform> ().localScale = Vector3.zero;
		//PersonalityTab.GetComponent<RectTransform> ().localScale = Vector3.zero;
	}

	void ShowUI()
	{
		GetComponent<RectTransform> ().localScale = Vector3.one;
		StatsTab.GetComponent<RectTransform> ().localScale = Vector3.one;
		//EquipmentTab.GetComponent<RectTransform> ().localScale = Vector3.one;
		//PersonalityTab.GetComponent<RectTransform> ().localScale = Vector3.one;
	}

	void UpdateUI()
	{
		slots = new InventoryItem[8];
		slots [0] = currentCharacter.npc.inventory.slot1;
		slots [1] = currentCharacter.npc.inventory.slot2;
		slots [2] = currentCharacter.npc.inventory.slot3;
		slots [3] = currentCharacter.npc.inventory.slot4;
		slots [4] = currentCharacter.npc.inventory.slot5;
		slots [5] = currentCharacter.npc.inventory.slot6;
		slots [6] = currentCharacter.npc.inventory.slot7;
		slots [7] = currentCharacter.npc.inventory.slot8;

		currentCharacter = References.instance.playerControl.selectedCharacter;

		// Portrait
		characterPortrait.sprite = currentCharacter.npc.portraitSprite;

		// Inventory
		for(int i = 0; i < 8; i++)
		{
			if (slots [i] != null)
				inventoryItem [i].sprite = slots [i].itemPortrait;
			else
				inventoryItem [i].sprite = new Sprite ();
		}

		// Stats
		HealthMana.text = "Health: " + currentCharacter.health + "\nMana: " + currentCharacter.mana;
		GoldKarma.text = "Gold: " + currentCharacter.gold + "\nKarma: " + currentCharacter.karma;

		PhysicalMagical.text = "Physical: " + currentCharacter.physicalDamage + "\nKarma: " + currentCharacter.magicalDamage;
		PhysicalMagical2.text = "Physical: " + currentCharacter.physicalDefense + "\nKarma: " + currentCharacter.magicalDefense;

		Attributes.text = "Strength: " + currentCharacter.strengthStat + "\nAgility: " + currentCharacter.agilityStat;
		Attributes.text += "\nStamina: " + currentCharacter.staminaStat + "\nIntelligence: " + currentCharacter.intelligenceStat;
		Attributes.text += "\nWisdom: " + currentCharacter.wisdomStat + "\nDexterity: " + currentCharacter.dexterityStat;

		Attributes2.text = "Poise: " + currentCharacter.poiseStat + "\nCharisma " + currentCharacter.charismaStat;
		Attributes2.text += "\nPerception: " + currentCharacter.perceptionStat + "\nLuck " + currentCharacter.luckStat;
		Attributes2.text += "\nSpeed: " + currentCharacter.speedStat;
	}
}
