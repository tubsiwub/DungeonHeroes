using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Pathfinding;

[Serializable][DisallowMultipleComponent]
public class Character : AIPath 
{
	// Events
	public delegate void CharacterDeath();
	public event CharacterDeath OnCharacterDeath;

	[Header("NPC")]
	public NPC npc;
	public float baseSpeed;

	// determines action variations in various situations
	public enum CharacterTrait { ___, greedy, brave, cowardly, empathetic, cautious, worried, caring, optimistic, 							// Notes *1
		rude, hateful, bashful, grateful, loving, resentful, annoying, pompous, forgetful, arrogant, selfish  }	

	// determines actions taken
	public enum Behavior { wander, idle, combat, flee, rest }																// Notes *5
	[Space(16.0f)]
	public Behavior currentBehavior;

	// emotions to display
	public enum Emotions { normal, sadness, anger, hatred, confusion, praise, alarm }										// Notes *3
	public Emotions displayedEmotion = Emotions.normal;

	[Serializable]
	public struct Personality																								// Notes *2
	{
		// Characters may have up to four defining personality traits
		//	with each scaling up to a max of 5.  The higher the traitValue,
		//	the more effect it has on their actions.

		public CharacterTrait trait1;
		public float traitValue1;

		public CharacterTrait trait2;
		public float traitValue2;

		public CharacterTrait trait3;
		public float traitValue3;

		public CharacterTrait trait4;
		public float traitValue4;
	}
	public Personality personality;

	[Serializable]
	public struct Inventory
	{
		public InventoryItem slot1;
		public InventoryItem slot2;
		public InventoryItem slot3;
		public InventoryItem slot4;
		public InventoryItem slot5;
		public InventoryItem slot6;
		public InventoryItem slot7;
		public InventoryItem slot8;
	}
	public Inventory inventory;

	[Serializable]
	public struct EquippedItems																								// Notes *7
	{
		// Equipment
		public Equipment mainHandSlot;
		public Equipment offHandSlot;																							
		public Equipment headSlot;																								
		public Equipment shoulderSlot;																							
		public Equipment chestSlot;																								
		public Equipment armSlot;																								
		public Equipment handSlot;																								
		public Equipment legSlot;																								
		public Equipment footSlot;																								
		public Equipment backSlot;																								
		public Equipment trinket1Slot;																								
		public Equipment trinket2Slot;																							
	}
	public EquippedItems equipment;

	public enum Factions { player, monster, terror, alliance, rival, elves, dwarves, undead }

	[HideInInspector]
	public List<Factions> alliedFactions, enemyFactions;
	public Factions currentFaction;

	// life values
	[HideInInspector] public int maxHealth;
	[HideInInspector] public int health;
	[HideInInspector] public int maxMana;
	[HideInInspector] public int mana;
	[HideInInspector] public int gold;
	[HideInInspector] public int karma;

	[HideInInspector] public int level;
	[HideInInspector] public int experience;

	// modifier variables
	[HideInInspector] public int physicalDamage;
	[HideInInspector] public int magicalDamage;
	[HideInInspector] public int physicalDefense;
	[HideInInspector] public int magicalDefense;

	// Stats																													// Notes *6
	[HideInInspector] public int strengthStat;
	[HideInInspector] public int agilityStat;	
	[HideInInspector] public int staminaStat;
	[HideInInspector] public int intelligenceStat;
	[HideInInspector] public int wisdomStat;
	[HideInInspector] public int dexterityStat;
	[HideInInspector] public int poiseStat;
	[HideInInspector] public int charismaStat;
	[HideInInspector] public int perceptionStat;
	[HideInInspector] public int luckStat;
	[HideInInspector] public int speedStat;

	[HideInInspector]
	public bool disableMovement = false;
	bool lastDisabled = false;

	[HideInInspector]
	public bool disableCombat = false;

	[HideInInspector]
	public float detectionRange;

	[HideInInspector]
	public float attackRange;

	[HideInInspector]
	public bool fatigued = false;
	float fatigueMeter = 100.0f;
	float fatigueMeterMax = 100.0f;

	public new void Start () 
	{
		base.Start ();

		posList = new List<Vector3> ();
		targettedByCharacters = new List<Character> ();

		// Random starting wander direction
		float randomRotation = UnityEngine.Random.Range(0,360);
		wanderDirection = new Vector3 (0, randomRotation, 0);

		SetDefaults ();
	}

	#region Defaults
	void SetDefaults()
	{
		combatActionIntervalDefault = combatActionInterval;

		level = 0;

		DefaultStats ();
		DefaultFactions ();
		DefaultInventory ();
		DefaultEquipment ();
		DefaultPersonality ();
	}

	void DefaultStats ()
	{
		// life values
		maxHealth = npc.health;
		health = npc.health;
		maxMana = npc.mana;
		mana = npc.mana;
		gold = npc.gold;
		karma = npc.karma;

		// modifier variables
		physicalDamage = npc.physicalDamage;
		magicalDamage = npc.magicalDamage;
		physicalDefense = npc.physicalDefense;
		magicalDefense = npc.magicalDefense;

		// Attributes
		strengthStat = npc.strengthStat;
		agilityStat = npc.agilityStat;
		staminaStat = npc.staminaStat;
		intelligenceStat = npc.intelligenceStat;
		wisdomStat = npc.wisdomStat;
		dexterityStat = npc.dexterityStat;
		poiseStat = npc.poiseStat;
		charismaStat = npc.charismaStat;
		perceptionStat = npc.perceptionStat;
		luckStat = npc.luckStat;
		speedStat = npc.speedStat;
	}
	void DefaultFactions ()
	{
		currentFaction = npc.startingFaction;

		foreach(Factions faction in npc.alliedFactions)
		{
			alliedFactions.Add (faction);
		}

		foreach(Factions faction in npc.enemyFactions)
		{
			enemyFactions.Add (faction);
		}
	}
	void DefaultInventory ()
	{
		inventory = npc.inventory;
	}
	void DefaultEquipment ()
	{
		equipment = npc.equipped;
	}
	void DefaultPersonality ()
	{
		personality = npc.personality;
	}
	#endregion
	
	public new void Update () 
	{
		base.Update ();

		CharacterPhases ();

		TestButtonPressed ();
	}

	void TestButtonPressed()
	{
		if (Input.GetKeyDown(KeyCode.K))
		{
			DisplayEmotion (Emotions.confusion);
		}
	}

	#region Character Phases
	void CharacterPhases()
	{
		MovementPhase ();	// Move

		LookPhase ();		// Check surroundings

		ActionPhase();		// Take action if possible
	}

	// Movement stops if a 'Current Action' is loaded.
	void MovementPhase()
	{
		DisabledCheck ();

		if (disableMovement)	// no movement phase for the disabled
			return;

		MoveToTarget ();		// move to target

		if(currentBehavior != Behavior.combat)
			WanderNearby(4.0f);
	}

	void DisabledCheck()
	{
		if (lastDisabled != disableMovement)
		{
			// The moment this character is disabled
			if (disableMovement)
				DisplayEmotion (Emotions.confusion);
		}
		lastDisabled = disableMovement;

		canMove = !disableMovement;
		canSearch = !disableMovement;
	}

	// Using stats to determine what can be seen,
	// then personality and character traits to decipher what is seen,
	// then order of importance to decide on which action takes priority.
	// Mark action as 'Current Action' and block the Look Phase until that action is completed.
	protected void LookPhase()
	{
		if (currentBehavior == Behavior.combat)	// ignore others while fighting
			return;

		Collider[] foundColliders = Physics.OverlapSphere (transform.position, detectionRange);

		foreach (Collider col in foundColliders)
		{
			// If seen, begin combat with that character - temporary code
			if (col.transform.tag.ToLower () == "character")
			{
				// Allied actions
				foreach(Factions faction in alliedFactions)
				{
					if(col.GetComponent<Character>().currentFaction == faction)	// if ally, skip to next check
						goto SkipCheck1;		// skip*1
				}

				// Enemy actions
				BeginCombatWithTarget (col.GetComponent<Character> ());
			}

			SkipCheck1:{}	// skip*1
		}
	}

	// Waits until 'Current Action' has an action loaded.
	// Carries on until the 'Current Action' is completed, then sets it to null.
	void ActionPhase()
	{
		if (currentBehavior == Behavior.combat && !disableCombat)
			CombatAction();
	}


	// Combat variables
	public float combatActionInterval;
	float combatActionIntervalDefault;

	void CombatAction()
	{
		if (combatActionInterval <= 0 && TargetWithinAttackRange(combatTarget.transform.position))
		{
			// Perform action
			combatTarget.ReceiveDamage(physicalDamage);

			combatActionInterval = SetCombatActionInterval();
		}
		combatActionInterval -= Time.deltaTime;
	}

	float SetCombatActionInterval()
	{
		float intervalModifier = combatActionIntervalDefault;

		// Equipment modifier
		int equipmentWeight = GetEquipmentWeight ();
		float equipmentEncumbrance = GetEquipmentEncumbrance ();
		intervalModifier += equipmentWeight / 103;	// 2.0 rough max addition
		intervalModifier += equipmentEncumbrance;	// Straight addition - some items increase this value while others decrease

		// Stat modifier
		intervalModifier -= npc.speedStat / 800;	// 0.1 max reduction

		// Fatigue modifier
		if (fatigued)
			intervalModifier *= 2;

		return intervalModifier;
	}

	bool TargetWithinAttackRange(Vector3 target)
	{
		if (Vector3.Distance (transform.position, target) < attackRange + 1)
			return true;

		return false;
	}
	#endregion

	#region Character Actions

	// MoveToTarget variables
	[HideInInspector] public List<Vector3> posList;
	public float distanceUntilRemoveSphere;

	void MoveToTarget()
	{
		if (posList.Count > 0)
		{
			disableCombat = true;
			target.position = posList [0];
		}
		else
			disableCombat = false;

		// Limit to # spots
		if (posList.Count > 1)
		{
			posList [0] = posList [1];
			posList.RemoveAt (1);
		}

		foreach (Vector3 v in posList)
		{
			if (Vector3.Distance (transform.position, v) < distanceUntilRemoveSphere && v == posList [0])
			{
				posList.Remove (v);
				break;
			}
		}
	}

	void HealTarget(Character targetCharacter)
	{
		// ... restore health to target's lifetotal - ignores stats
	}

	void AttackTarget(Character targetCharacter)
	{
		// ... deal damage to target's lifetotal - ignores stats
	}

	void AttackTargetWithEquippedWeapon(Character targetCharacter)
	{
		// ... deal damage to target taking into account equipment and stats
	}

	void ReceiveDamage(int value)
	{
		// ... remove specific amount from lifetotal directly

		health -= value;

		PlaySound ("injured", 1.0f);

		CheckIfDead ();
	}

	void ReceiveIncomingDamage(int value, int type)
	{
		// ... use stats and equipment to determine mitigation

		// types: physical = 0, magical = 1

		int finalValue = value;

		switch (type)
		{
		case 0:
			finalValue -= physicalDefense;
			finalValue -= GetEquipmentDefenseValue ();
			break;
		
		case 1:
			finalValue -= magicalDefense;
			finalValue -= GetEquipmentResistanceValue ();
			break;
		}

		ReceiveDamage (finalValue);
	}

	int GetEquipmentDefenseValue ()
	{
		int equipmentDefense = 0;

		if(equipment.armSlot)
		equipmentDefense += equipment.armSlot.armorDefense;
		if(equipment.backSlot)
		equipmentDefense += equipment.backSlot.armorDefense;
		if(equipment.chestSlot)
		equipmentDefense += equipment.chestSlot.armorDefense;
		if(equipment.footSlot)
		equipmentDefense += equipment.footSlot.armorDefense;
		if(equipment.handSlot)
		equipmentDefense += equipment.handSlot.armorDefense;
		if(equipment.headSlot)
		equipmentDefense += equipment.headSlot.armorDefense;
		if(equipment.legSlot)
		equipmentDefense += equipment.legSlot.armorDefense;
		if(equipment.shoulderSlot)
		equipmentDefense += equipment.shoulderSlot.armorDefense;
		if(equipment.offHandSlot)
		equipmentDefense += equipment.offHandSlot.armorDefense;
		if(equipment.mainHandSlot)
		equipmentDefense += equipment.mainHandSlot.armorDefense;
		if(equipment.trinket1Slot)
		equipmentDefense += equipment.trinket1Slot.armorDefense;
		if(equipment.trinket2Slot)
		equipmentDefense += equipment.trinket2Slot.armorDefense;
		
		return equipmentDefense;
	}

	int GetEquipmentResistanceValue ()
	{
		int equipmentResistance = 0;

		if(equipment.armSlot)
		equipmentResistance += equipment.armSlot.armorMagicResistance;
		if(equipment.backSlot)
		equipmentResistance += equipment.backSlot.armorMagicResistance;
		if(equipment.chestSlot)
		equipmentResistance += equipment.chestSlot.armorMagicResistance;
		if(equipment.footSlot)
		equipmentResistance += equipment.footSlot.armorMagicResistance;
		if(equipment.handSlot)
		equipmentResistance += equipment.handSlot.armorMagicResistance;
		if(equipment.headSlot)
		equipmentResistance += equipment.headSlot.armorMagicResistance;
		if(equipment.legSlot)
		equipmentResistance += equipment.legSlot.armorMagicResistance;
		if(equipment.shoulderSlot)
		equipmentResistance += equipment.shoulderSlot.armorMagicResistance;
		if(equipment.offHandSlot)
		equipmentResistance += equipment.offHandSlot.armorMagicResistance;
		if(equipment.mainHandSlot)
		equipmentResistance += equipment.mainHandSlot.armorMagicResistance;
		if(equipment.trinket1Slot)
		equipmentResistance += equipment.trinket1Slot.armorMagicResistance;
		if(equipment.trinket2Slot)
		equipmentResistance += equipment.trinket2Slot.armorMagicResistance;
		
		return equipmentResistance;
	}

	float GetEquipmentEncumbrance()
	{
		float equipmentEncumbrance = 0;

		if(equipment.armSlot)
		equipmentEncumbrance += equipment.armSlot.encumbrance;
		if(equipment.backSlot)
		equipmentEncumbrance += equipment.backSlot.encumbrance;
		if(equipment.chestSlot)
		equipmentEncumbrance += equipment.chestSlot.encumbrance;
		if(equipment.footSlot)
		equipmentEncumbrance += equipment.footSlot.encumbrance;
		if(equipment.handSlot)
		equipmentEncumbrance += equipment.handSlot.encumbrance;
		if(equipment.headSlot)
		equipmentEncumbrance += equipment.headSlot.encumbrance;
		if(equipment.legSlot)
		equipmentEncumbrance += equipment.legSlot.encumbrance;
		if(equipment.shoulderSlot)
		equipmentEncumbrance += equipment.shoulderSlot.encumbrance;
		if(equipment.offHandSlot)
		equipmentEncumbrance += equipment.offHandSlot.encumbrance;
		if(equipment.mainHandSlot)
		equipmentEncumbrance += equipment.mainHandSlot.encumbrance;
		if(equipment.trinket1Slot)
		equipmentEncumbrance += equipment.trinket1Slot.encumbrance;
		if(equipment.trinket2Slot)
		equipmentEncumbrance += equipment.trinket2Slot.encumbrance;
		
		return equipmentEncumbrance;
	}

	int GetEquipmentWeight()
	{
		int equipmentWeight = 0;

		if(equipment.armSlot)
		equipmentWeight += equipment.armSlot.weight;
		if(equipment.backSlot)
		equipmentWeight += equipment.backSlot.weight;
		if(equipment.chestSlot)
		equipmentWeight += equipment.chestSlot.weight;
		if(equipment.footSlot)
		equipmentWeight += equipment.footSlot.weight;
		if(equipment.handSlot)
		equipmentWeight += equipment.handSlot.weight;
		if(equipment.headSlot)
		equipmentWeight += equipment.headSlot.weight;
		if(equipment.legSlot)
		equipmentWeight += equipment.legSlot.weight;
		if(equipment.shoulderSlot)
		equipmentWeight += equipment.shoulderSlot.weight;
		if(equipment.offHandSlot)
		equipmentWeight += equipment.offHandSlot.weight;
		if(equipment.mainHandSlot)
		equipmentWeight += equipment.mainHandSlot.weight;
		if(equipment.trinket1Slot)
		equipmentWeight += equipment.trinket1Slot.weight;
		if(equipment.trinket2Slot)
		equipmentWeight += equipment.trinket2Slot.weight;
		
		return equipmentWeight;
	}

	void CheckIfDead()
	{
		if (health <= 0)
			Death ();
	}

	public virtual void PlaySound(string type, float volume)
	{
		// ... must override - check character specific classes
	}

	void BullyTarget(Character targetCharacter)
	{
		// ...
	}

	void PickpocketTarget(Character targetCharacter)
	{
		// ...
	}

	#region Wandering
	// Wander variables
	Vector3 wanderDirection;
	bool wanderPaused = false;

	// Normal wander movement
	float changeDirectionTimer;
	float changeDirectionInterval = 0.5f;	// time until we rotate slightly
	float maxChange = 8f;
	float minChange = 8f;

	// Wander only if set positions aren't stored
	void WanderNearby(float radius)
	{
		if (posList.Count > 0)
			return;

		Wandering (radius);
	}

	void Wandering(float radius)
	{
		// Will nudge direction slightly when complete
		changeDirectionTimer -= Time.deltaTime;
		if (changeDirectionTimer <= 0)
		{
			changeDirectionTimer = changeDirectionInterval;
			ChangeWanderDirection ();
		}

		// Sets direction
		transform.eulerAngles = wanderDirection;
		//target.transform.position = transform.position + transform.forward * radius;
		//target.transform.position = (Vector3)AstarPath.active.GetNearest (transform.position + transform.forward * radius).node.position;

		if (AstarPath.active.GetNearest (target.transform.position + transform.forward * radius).node != null)
		{
			target.transform.position = (Vector3)AstarPath.active.GetNearest (transform.position + transform.forward * radius).node.position;	// snap to grid
		}

		// Rotates the player to stay within the map range
		if (Vector3.Distance(Vector3.zero, transform.position - Vector3.up) > MapPreferences.instance.mapRadius 
			&& Vector3.Angle(transform.forward, transform.position - Vector3.zero) < 90)
		{
			wanderDirection += new Vector3 (0, UnityEngine.Random.Range (0, 360), 0);
		}
	}

	void ChangeWanderDirection()
	{
		float min = (wanderDirection.y - minChange) % 360;
		float max = (wanderDirection.y - maxChange) % 360;
		wanderDirection = new Vector3(0, UnityEngine.Random.Range (min, max), 0);
	}
	#endregion

	void LookForEnemy()
	{
		// ...
	}

	void LookForCorpse()
	{
		// ...
	}

	void LookForChest()
	{
		// ...
	}

	void LookForBountyFlag()
	{
		// ...
	}

	void CastSpell(Character targetCharacter, string spellName)
	{
		// ...
	}

	void CastSpell(string spellName)
	{
		// ...
	}
	#region Spells
	void SpellHeal(int value) // 'heal'
	{

	}

	void SpellFireball(int value) // 'fireball'
	{

	}
	#endregion

	void BeginRun()
	{
		// ...
	}

	#region Targetting
	Character combatTarget;

	protected void BeginCombatWithTarget(Character targetCharacter)
	{
		// ... given a target character, this character will lock it's target destination to always be attack-range distance away
		// every combat-interval the character will weigh odds to determine which item, ability or action it will take against it's target
		// it will then wait for that action to be completed before determining it's next action

		combatTarget = targetCharacter;
		targetCharacter.TargettedBy (this);
		DisplayEmotion (Emotions.anger);

		targetCharacter.OnCharacterDeath += TargetDeath;

		currentBehavior = Behavior.combat;
		StartCoroutine (TargetCharacter (targetCharacter));
	}

	IEnumerator TargetCharacter(Character targetCharacter)
	{
		while (combatTarget == targetCharacter && targetCharacter != null)
		{
			Vector3 directionFromTarget = (transform.position - targetCharacter.transform.position).normalized;
			target.transform.position = targetCharacter.transform.position + (directionFromTarget * attackRange);

			yield return new WaitForEndOfFrame ();
		}
	}

	// TargettedBy variables
	List<Character> targettedByCharacters;

	public void TargettedBy(Character otherCharacter)
	{
		// ... this is called when another character targets this character
		// we store their reference in a list so we can tell how many others are targetting this character

		if (!targettedByCharacters.Exists (u => otherCharacter))
		{
			targettedByCharacters.Add (otherCharacter);
		}
	}

	public void ReleaseAsTarget(Character otherCharacter)
	{
		// ... this is called when another character stops targetting this character

		if(targettedByCharacters.Exists(u => otherCharacter))
			targettedByCharacters.Remove (otherCharacter);
	}
	#endregion

	void GoToHome(Transform homeLocation)
	{
		// ...
	}

	void TargetDeath()
	{
		// ... when target dies, do something

		combatTarget = null;
		currentBehavior = Behavior.wander;
		DisplayEmotion (Emotions.normal);
	}

	public void CalculateExperience(int experienceGain, int enemyLevel)
	{
		float modifier = level - enemyLevel;

		if (modifier < -2)
			modifier = experience * 1.5f;
		else if (modifier < 0)
			modifier = experience * 1.2f;
		else if (modifier == 0)
			modifier = experience * 1.0f;
		else if (modifier > 2)
			modifier = experience * 0.0f;
		else if (modifier > 0)
			modifier = experience * 0.5f;
		else
			modifier = 0.0f;

		experience += Mathf.RoundToInt(modifier);

		if (experience >= 100)
		{
			experience -= 100;
			LevelUp (1);
		}
	}

	public void CalculateLootItems(Inventory itemsDropped)
	{
		//int modifier = level;
		//experience += target;
	}

	public void CalculateLootEquipment(EquippedItems equipmentDropped)
	{
		//int modifier = level;
		//experience += target;
	}

	void LevelUp(int numberOfLevelsGained)
	{
		// Gain stats according to specific max/min values randomly

		if (level >= 20)	// can't scale above level 20
			return;

		level += numberOfLevelsGained;

		// Remove fatigue because we're nice
		fatigued = false;
		fatigueMeter = fatigueMeterMax;

		AdjustAttributes ();
		AdjustCharacterValues ();
	}

	void AdjustAttributes()
	{
		strengthStat += UnityEngine.Random.Range (0, npc.strengthGain);
		if (strengthStat > npc.strengthMax)
			strengthStat = npc.strengthMax;

		agilityStat += UnityEngine.Random.Range (0, npc.agilityGain);
		if (agilityStat > npc.agilityMax)
			agilityStat = npc.agilityMax;

		staminaStat += UnityEngine.Random.Range (0, npc.staminaGain);
		if (staminaStat > npc.staminaMax)
			staminaStat = npc.staminaMax;

		intelligenceStat += UnityEngine.Random.Range (0, npc.intelligenceGain);
		if (intelligenceStat > npc.intelligenceMax)
			intelligenceStat = npc.intelligenceMax;

		wisdomStat += UnityEngine.Random.Range (0, npc.wisdomGain);
		if (wisdomStat > npc.wisdomMax)
			wisdomStat = npc.wisdomMax;

		dexterityStat += UnityEngine.Random.Range (0, npc.dexterityGain);
		if (dexterityStat > npc.dexterityMax)
			dexterityStat = npc.dexterityMax;

		poiseStat += UnityEngine.Random.Range (0, npc.poiseGain);
		if (poiseStat > npc.poiseMax)
			poiseStat = npc.poiseMax;

		charismaStat += UnityEngine.Random.Range (0, npc.charismaGain);
		if (charismaStat > npc.charismaMax)
			charismaStat = npc.charismaMax;

		perceptionStat += UnityEngine.Random.Range (0, npc.perceptionGain);
		if (perceptionStat > npc.perceptionMax)
			perceptionStat = npc.perceptionMax;

		luckStat += UnityEngine.Random.Range (0, npc.luckGain);
		if (luckStat > npc.luckMax)
			luckStat = npc.luckMax;

		speedStat += UnityEngine.Random.Range (0, npc.speedGain);
		if (speedStat > npc.speedMax)
			speedStat = npc.speedMax;
	}

	void AdjustCharacterValues()
	{
		SetMovementSpeed ();
	}

	void SetMovementSpeed()
	{
		speed = baseSpeed;
		speed -= GetEquipmentWeight() / 206;	// 1.0 rough max reduction
		speed += speedStat / 80;				// 1.0 max addition
	}

	void Death()
	{
		// ...

		if (OnCharacterDeath != null)
			OnCharacterDeath ();

		// Reward enemy for killing you!
		foreach (Character character in targettedByCharacters)
		{
			character.CalculateExperience (npc.experienceDrop, level);
			character.CalculateLootItems (npc.lootItems);
			character.CalculateLootEquipment (npc.lootEquipment);
		}

		targettedByCharacters = new List<Character>();

		{ // temp
			StopAllCoroutines ();
			GetComponent<MeshRenderer> ().enabled = false;
			GetComponent<Seeker> ().enabled = false;
			GetComponent<CharacterController> ().enabled = false;
			transform.position += Vector3.one * 9999;
			GetComponent<Character> ().enabled = false;
			Destroy (transform.parent.gameObject);
		}
	}


	public bool showingHealthbar = false;

	public void ShowHealthbar()
	{
		GameObject healthObj;
		healthObj = (GameObject)Instantiate (References.instance.healthbarObj, new Vector3(-99,-99,0), Quaternion.identity);
		healthObj.transform.SetParent (GameObject.FindWithTag ("MainPanel").transform);
		healthObj.GetComponent<Healthbar> ().targetCharacter = this;

		showingHealthbar = true;
	}

	void LootCorpse(Transform target)
	{
		// ...
	}

	void LootChest(Transform target)
	{
		// ...
	}

	void GoToShop(Transform shopLocation)
	{
		// ...
	}

	GameObject emotionObj = null;
	public virtual void DisplayEmotion(Emotions displayed)
	{
		if (emotionObj)
			Destroy (emotionObj);

		displayedEmotion = displayed;

		emotionObj = (GameObject)Instantiate (References.instance.emotionObj, new Vector3(-99,-99,0), Quaternion.identity);
		emotionObj.transform.SetParent (GameObject.FindWithTag ("MainPanel").transform);
		emotionObj.GetComponent<Emotion> ().targetCharacter = this;

		Sprite emotionSprite = null;
		if (displayed == Emotions.alarm)
			emotionSprite = References.instance.alarmEmotion;
		else if (displayed == Emotions.anger)
			emotionSprite = References.instance.angerEmotion;
		else if (displayed == Emotions.confusion)
			emotionSprite = References.instance.confusionEmotion;
		else if (displayed == Emotions.hatred)
			emotionSprite = References.instance.hatredEmotion;
		else if (displayed == Emotions.normal)
			emotionSprite = References.instance.normalEmotion;
		else if (displayed == Emotions.praise)
			emotionSprite = References.instance.praiseEmotion;
		else if (displayed == Emotions.sadness)
			emotionSprite = References.instance.sadnessEmotion;
		
		emotionObj.GetComponent<Image> ().sprite = emotionSprite;
	}

	void TauntEnemy(Character targetCharacter)
	{
		// ...
	}

	void TauntNearbyEnemies(float radius)
	{
		// ...
	}

	#endregion
}

// Notes:

// *1: Character Trait Guide - 
// o. Greedy - a greedy character will crack easier under the pressure of a gold offering.  
// 		They more readily accept bounty targets (taking their own health and the level of the target into less of an account).
//		These characters may attempt to steal gold from others, and sometimes items.
//		This isn't all bad though!  Greedy characters can loot corpses and graves of fallen heroes and enemies, recovering lost equipment, gold and perhaps treasures that have gone unnoticed!
//		-= 0 - will loot corpses of monsters for bonus gold.
//		-= 1 - chance to steal gold from monsters and monster lairs, regardless of combat
//		-= 2 - 
//		-= 3 -
//		-= 4 - has a small chance of attacking a friendly character out of nowhere if they are holding a rare item.
//
// ^. Arrogant - extreme version of Greedy.  While greedy characters may snap quickly at a gold-reward bounty and try to snag some coins from a passerby, an arrogant character can be seen
//		bullying friendly folk from time to time.  The bullying can go from simply wasting time to taking their items and gold by force...!  If a strong character has this trait, your town 
//		may become undefended quite fast.  Arrogance also allows the character to ignore orders from time to time, use helpful spells and items on themselves instead of others in need, and swipe treasure
//		from friendly characters that helped earn the rewards to begin with.
//		-= 0 - bully friendly characters stopping them from any action they were previously taking.	Strength check at 40%.  Chance to ignore characters in need.
//		-= 1 - will steal all treasure rewards from nearby chest and monster drops if unlooted in time.
//		-= 2 - bully friendly characters stopping them from any action they were previously taking.	Strength check at 100%.
//		-= 3 - bully friendly characters, chance of stealing items.	Strength check at 80%.
//		-= 4 - bully friendly characters, chance of stealing equipment and weapons.	Strength check at 20%.
//
// o. Selfish - extreme version of Arrogant.  Selfish characters will definitely take everything they can for themselves, refuse to use items or spells on others and will sometimes buyout
//		shop inventories so that others cannot purchase items if they have the cash available.  They even have a chance to attack a friendly character in order to claim a rare item for themselves.
//		-= 0 - will not use items or spells on friendly characters.
//		-= 1 - 
//		-= 2 - 
//		-= 3 - has a chance to buyout a shop's inventory if proper funds are available.
//		-= 4 - 
//
// o. Brave - a brave character is one who snaps to action at the sign of danger.  These characters don't require much coercion when it comes to a call to arms, and bounties don't need to be high to get
//		them to take action against a goblin camp or two.  Brave characters will rush in to defend others in need should their personality allow, taking hits in place of a friendly character regardless 
//		of who they are or what they've done.  Some may take advantage of this though...  Brave characters often don't know when to run, and can often times end up dying in a fight that cannot be won.
//		-= 0 - bounty requests for monsters and monster spawns are accepted at far lower values.
//		-= 1 - will jump in and rescure friendly characters in need if noticed.
//		-= 2 - 
//		-= 3 - will keep fighting until death with a small chance to realize and run away.
//		-= 4 - will run back to town if the town is under attack.
//
// ^. Hateful - extreme version of Brave.  While a brave hero may assault a monster for little more than to prove their worth, a hateful hero will attack monsters relentlessly until every last one of them
//		is dead or out of sight.  Once triffled with, a hateful character may attack the greedy thieves despite allegiances.  Depending on the forces that exist to stop them, they may track and kill
//		those who have wronged them in the past.  Hateful characters often cannot be stopped from attacking a target they've selected.
//		-= 0 - will target enemy camps without a bounty
//		-= 1 - will seek out enemies on the map that have attacked them
//		-= 2 - 
//		-= 3 - can attack and kill friendly heroes that have wronged them
//		-= 4 - cannot be stopped from attacking a target 
//
// o. Cowardly - with every attack received in combat, this character will have an increased chance to run away in fear.  When being bullied,
//		this character will succumb instantly with no chance of dampening the effects of the harassment (full item/gold loss).
//		-= 0 - combat bounties are half as effective
//		-= 1 - chance to run from each hit in combat, increasing per hit
//		-= 2 - chance to run scales faster
//		-= 3 - 
//		-= 4 - chance to freeze in fear at the sight of an enemy.  This causes the character to not be able to defend or retaliate in combat.
//
// o. Empathetic - 
//		-= 0 - 
//		-= 1 - has a chance to give a friendly character some gold when they display sadness.
//		-= 2 - 
//		-= 3 - 
//		-= 4 - 
//
// ^. Loving - extreme version of Empathetic.
//		-= 0 - 
//		-= 1 - will forgive all crimes and use items and spells on friendly characters regardless of actions taken against them or others.
//		-= 2 - 
//		-= 3 - 
//		-= 4 - 
//
// o. Cautious
//		-= 0 - 
//		-= 1 - can notice traps with a lower perception.
//		-= 2 - 
//		-= 3 - will always leave town with protective items and healing potions.
//		-= 4 - 
//
// ^. Worried - extreme version of Cautious.
//		-= 0 - 
//		-= 1 - will spend all of their money until their inventory is fully stocked.
//		-= 2 - 
//		-= 3 - automatically avoids avoidable traps.
//		-= 4 - will run to town if spotted by an enemy of higher level than the character.
//
// o. Optimistic
//		-= 0 - 
//		-= 1 - has a chance of taking on enemies of a much higher level
//		-= 2 - 
//		-= 3 - 
//		-= 4 - will engage a terror in battle if also brave
//
// o. Rude
//		-= 0 - 
//		-= 1 - may get thrown out of shops
//		-= 2 - 
//		-= 3 - 
//		-= 4 - 
//
// ^. Resentful - extreme version of Rude.
//		-= 0 - 
//		-= 1 - may start attacking a shop once thrown out of it; can be thrown out of shops.
//		-= 2 - 
//		-= 3 - 
//		-= 4 - 
//
// o. Bashful
//		-= 0 - 
//		-= 1 - may run to town when given items by other characters.
//		-= 2 - may run to town when having items used on them by other characters or by being healed.
//		-= 3 - 
//		-= 4 - may run to town when given praise by other characters.
//
// o. Grateful
//		-= 0 - may give a character some gold for having an item used on them.
//		-= 1 - may give a character an item for being healed or having an item used on them.
//		-= 2 - chances increase.
//		-= 3 - chances increase.
//		-= 4 - may follow and defend a target for being healed or having an item used on them.
//
// o. Annoying
//		-= 0 - 
//		-= 1 - adds a slight increase to taunt efficiency, automatically applies weak taunt to enemies near character
//		-= 2 - may get thrown out of shops
//		-= 3 - adds a slight increase to taunt efficiency, automatically applies medium taunt to enemies near character
//		-= 4 - may get banned from shops
//
// ^. Pompous - extreme version of Annoying.
//		-= 0 - may get banned from shops
//		-= 1 - 
//		-= 2 - 
//		-= 3 - may cause healers to ignore them at low health
//		-= 4 - high chance to taunt monsters onto them from a decent distance regardless of current action
//
// o. Forgetful - a forgetful character will 
//		-= 0 - may occasionally forget a path set for them
//		-= 1 - may forget to restock on potions when low and in town
//		-= 2 - 
//		-= 3 - may leave their home without their weapon 
//		-= 4 - may leave their home without their armor (first encounter causes them to fall into fear)
//
// o. Programming note here:  A character will have a list of actions and what they do that will have their own functions.  Each function will contain all relevant
//		possible interactions given the traits of the character and characters nearby.

// *2: Personality Guide - 
//	o. Each character can have up to four defining character traits.  Each trait can scale up to a max of 5, and no less than 1.
//	o. A trait will affect how the character reacts to situations, pressures and in combat against foes.
//	o. The higher the trait value for a specific trait, the greater the impact it has on the character with regards to the other traits present and the severity of those actions.
//		An example would be:  A trait value of 2 for 'greedy' may cause a Rogue to attempt to pickpocket some gold from friendly heroes and peasants in town,
//		while a trait value of 5 for 'greedy' may cause the Rogue to attempt to lift some of the valuable equipment from them as well...!  This would leave your
//		valuable defense force unarmed against threats!  Not every trait is beneficial at high or low scales.  Pay attention to how the characters act in the world!
//	o. Trait values and their effects are displayed above in Note *1.

// *3: Emotions Guide - 
//	o. All characters at points during gameplay can display emotions.
//	o. Currently, the emotions that can be displayed are:
//		Normal - ordinary state with no emotion bubble above the character's head.
//		Sadness - when something goes wrong near or to a character.
//		Anger - when something displeases a character.
//		Hatred - when something enrages a character.
//		Confusion - when something happens that a character isn't expecting.
//		Praise - when a nearby friendly character kills a higher level monster, they may get praised by others.
//		Alarm - when something alarms the character, such as being attacked by an unexpected foe, or being stolen from.
//	o. Emotions can be perceived by nearby characters, sometimes causing reactions and actions to be taken.

// *4: Factions Guide - 
//	o. Characters fall under one of a few factions:
//		Player - You!  These are your characters, villagers, etc.  
//			Can be attacked by Monster, Terror and Rival.
//		Monster - map monster enemies.
//			Can be attacked by Player, Terror, Alliance and Rival.
//		Terror - truly terrifying monsters.
//			Can be attacked by Player, Monster, Alliance and Rival.
//		Alliance - allied kingdom.
//			Can be attacked by Monster, Terror and Rival.
//		Rival - enemy kingdom.
//			Can be attacked by Player, Monster, Terror and Alliance.
//		Elves, Dwarves, Undead - factions, their allegiance depends on player actions or map settings.
//	o. Factions have alliances and enemies determining which units can attack which other units.
//	o. Taking offensive actions against allies can cause irreversible damage, and likewise taking friendly actions against enemies may cause peace treaties.
//  o. Despite there being no neutral status, it's possible to become neutrally alligned to some factions.
									
// *5: Behavior Guide - 
//	o. The current behavior states are:
//		Wander - The character performs movement actions and decisions in this state.
//		Idle - The character has turned off movement and actions taken, but can be acted upon.  Usually a character goes into idle while at a shop or performing a non-combat action.
//		Combat - No movement during this state.  The character deals damage to their target and takes damage from enemies.  The character can use items and cast spells in this state.
//		Flee - When in danger (or for other reasons) the character will enter 'flee' state in which they cannot be interacted with other than being attacked.  They will attempt to run back to their home to rest.
//		Rest - while at home, the character 'turns off' and cannot be interacted with.  Slowly recovers all mana and health and loses all buffs and debuffs applied once they re-enter the world.
//	o. Behavior determine what the character is doing at any given time.
//

// *6: Stat Guide - These affect your ability to perform actions
//
//	o. Strength - 
//		Increases physical damage done.
//		Increases inventory and equipment weight maximum
//
//	o. Agility - 
//		Dodge physical and magical attacks during combat (with magic being half as effective).
//		Critically strike enemies during combat.
//		Steal items.
//		Increased chance for multi-strike.
//
//	o. Stamina - 
//		Health total.
//		Duration of exploring before becoming tired.
//		Duration of combat before becoming fatigued.
//
//	o. Intelligence - 
//		Cast higher level spells.
//		Perform more complicated skills.
//		Act appropriately in combat, such as running away when a monster is too strong.
//
//	o. Wisdom - 
//		Hold onto more spells and skills.
//
//	o. Dexterity - 
//		Deal more damage with ranged weapons and spells.
//		Become less encumbered by equipment weight.
//
//	o. Poise - 
//		Lose less stamina when taking damage (half as effective when receiving magic damage).
//		Get stunned less often.
//		Chance to take normal damage from a critical strike.
//
//	o. Charisma - 
//		Discount prices at shops.
//		Higher chance for discounts at shops.
//	
//	o. Perception - 
//		Sight range increase.
//		Increased chance to see through disguises.
//
//	o. Luck - 
//		Increased chance for dodging attacks (small).
//		Increased chance for critically striking foes (small).
//		Increase chance for better loot off of monsters.
//		Better odds when gambling.
//
//	o. Speed - 
//		Increases movement speed.
//		Decreases combat action interval.

// *7: Equipment Guide
//
// o. Main Hand - 
//		Weight: Max weight of about 40 when using a two-hand weapon, and it being an axe or hammer
//				20 with it being a metal weapon.
//
// o. Off Hand - 
//		Weight: Max weight of about 40 when using a great shield
//
// o. Head - 
//		Weight: Max weight of about 20 when equipping a full-face metal helmet
//
// o. Shoulder - 
//		Weight: Max weight of about 20 when equipping separate metal shoulderpads
//
// o. Chest - 
//		Weight: Max weight of about 40 when equipping a full-chest plate armor set
//				25 with it being a chest piece that requires shoulders and arm pieces
//
// o. Arm - 
//		Weight: Max weight of about 20 when equpping separate metal arm guards
//
// o. Hand - 
//		Weight: Max weight of about 20 when equipping metal gauntlets 
//
// o. Leg - 
//		Weight: Max weight of about 30 when equpping a full plate leg armor set
//
// o. Foot - 
//		Weight: Max weight of about 20 when equipping metal boots 
//
// o. Back - 
//		Weight: Max weight of about 10 when equipping a studded cape 
//
// o. Trinket - 
//		Weight: Max weight of about 1 when equipping a solid material trinket made out of metal
//
//

// *8: Game Terms Guide
//
//	o. Critical Strike:  Dealing 2x the normal damage with one attack.  Calculated before reductions.
//
//	o. Devastating Strike:  Dealing 3x the normal damage with one attack.  Calculated before reductions.
//
//  o. Multi-Strike:  Chance of attacking again soon after the last attack by reducing the combat interval timer by 2/3 upon reset.
//
//	o. Steal:  Taking gold or items out of a Character's inventory or equipment menu and adding it to another Character.
//
//	o. Fatigue:  Character is exhausted, doubling the time between attacks in combat, slowing movement speed by half and negating all stat ability checks such as agility's dodge and perception's sight range.
//		Excludes Luck stat.  Cannot become fatigued by simple wandering - only combat actions can fatigue the player, however a low fatigue meter can cause instant fatigue upon entering combat.
//
//	o. Piercing:  Ignores half of the total armor value when attacking an enemy.