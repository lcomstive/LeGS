using LEGS;
using UnityEngine;
using LEGS.Abilities;
using LEGS.Characters;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MOBAExample
{
	public class MOBACharacter : Character
	{
		public const uint MaxLevel = 18;

		[SerializeField] private MOBACharacterInfo m_Info;

		public int Level => m_Info.GetCurrentLevel(m_Experience);
		public static readonly CharacterTrait[] AllTraits = (CharacterTrait[])System.Enum.GetValues(typeof(CharacterTrait));

		private float m_Experience;
		private Coroutine m_RegenCoroutine;
		private Dictionary<CharacterTrait, Attribute> m_AttributeTraits = new Dictionary<CharacterTrait, Attribute>();

		protected override void Awake()
		{
			base.Awake();
			m_Experience = 0;
			EventManager.Subscribe<EntityDeathEventArgs>(EntityDeathEventArgs.EventName, OnEntityDeath, true);

			Attributes = new List<Attribute>();
			foreach (CharacterTrait trait in AllTraits)
			{
				Attribute attribute = new Attribute(trait.ToString(), m_Info[trait, Level]);
				Attributes.Add(attribute);
				m_AttributeTraits.Add(trait, attribute);
			}

			m_RegenCoroutine = StartCoroutine(RegenLoop());

			m_AttributeTraits[CharacterTrait.Health].CurrentValueChanged += OnAttributeHealthChanged;

			float healthAttribute = m_AttributeTraits[CharacterTrait.Health].CurrentValue;
			Health = MaxHealth = healthAttribute;
			EventManager.Publish(EntityHealthChangeEventArgs.EventName, new EntityHealthChangeEventArgs(this, this, Health));
		}

		private void OnAttributeHealthChanged(Attribute attribute)
		{
			MaxHealth = attribute.CurrentValue;
			EventManager.Publish(EntityHealthChangeEventArgs.EventName, new EntityHealthChangeEventArgs(this, this, 0));
		}

		public Attribute GetAttribute(CharacterTrait trait) => m_AttributeTraits[trait];

		private void OnDestroy()
		{
			if (m_RegenCoroutine != null)
				StopCoroutine(m_RegenCoroutine);

			EventManager.Unsubscribe<EntityDeathEventArgs>(EntityDeathEventArgs.EventName, OnEntityDeath);
		}

		public override void ApplyDamage(float amount, IEntity sender)
		{
			if (sender is AbilityInfo)
			{
				AbilityInfo abilityInfo = (AbilityInfo)sender;
				if (ApplyAbilityDamage(abilityInfo.Ability as MOBAAbility, abilityInfo.Caster as MOBACharacter))
					return;
			}

			base.ApplyDamage(amount, sender);

			if (Health <= 0)
				EventManager.Publish(EntityDeathEventArgs.EventName, new EntityDeathEventArgs(this, sender));
		}

		public bool ApplyAbilityDamage(MOBAAbility ability, MOBACharacter sender)
		{
			if (!ability || !sender)
				return false;

			// Calculate damage to apply
			float damage = 0;
			switch (ability.DamageType)
			{
				default:
				case DamageTypes.Physical:
					damage = sender.GetAttribute(CharacterTrait.AttackDamage).CurrentValue * ability.AttackDamageScale;
					break;
				case DamageTypes.Magical:
					damage = sender.GetAttribute(CharacterTrait.AbilityPower).CurrentValue * ability.AttackDamageScale;
					break;
				case DamageTypes.True:
					damage = ability.AttackDamageScale;
					break;
			}
			
			// Apply damage
			ApplyDamage(damage, sender, ability.DamageType);

			// Apply status effect
			if(ability.ReceiverEffect != null)
				AddStatusEffect(ability.ReceiverEffect, sender);

			return true;
		}

		public void ApplyDamage(float amount, MOBACharacter sender, DamageTypes type)
		{
			float resistance;

			// Calculate resistance
			if (type == DamageTypes.Physical)
				resistance = Mathf.Max(
					GetAttribute(CharacterTrait.Armour).CurrentValue -
						sender.GetAttribute(CharacterTrait.ArmourPenetration).CurrentValue,
					0.0f);
			else
				resistance = Mathf.Max(
					GetAttribute(CharacterTrait.MagicResistance).CurrentValue -
						sender.GetAttribute(CharacterTrait.MagicPenetration).CurrentValue,
					0.0f);

			if(resistance > 0) // Apply resistance
				amount *= 1.0f - (resistance / (resistance + 100.0f));

			base.ApplyDamage(amount, sender);
		}

		private void OnEntityDeath(EntityDeathEventArgs args)
		{
			// Check if we performed the kill
			if (args.Killer != (IEntity)this)
				return;

			MOBACharacter character = args.Entity as MOBACharacter;
			if (!character)
				return; // Invalid/incompatible cast to MOBACharacter

			int previousLevel = Level;
			m_Experience += character.m_Info.KillRewardExperience;
			if (Level != previousLevel)
			{
				Debug.Log($"'{DisplayName}' reached level {Level + 1}");
				// TODO: level up event
			}

			// TODO: Gain coin equal to character.m_Info.KillReward
		}

		private IEnumerator RegenLoop()
		{
			yield return new WaitForSeconds(1.0f);
			ApplyDamage(-this[CharacterTrait.HealthRegen].CurrentValue, this);

			m_RegenCoroutine = StartCoroutine(RegenLoop());
		}

		public void AddExperience(float experience)
		{
			int level = Level;
			m_Experience += experience;
			if (level == Level) // Did not level up
				return;

			// Levelled up

			// Update base trait values
			foreach (CharacterTrait trait in AllTraits)
				GetAttribute(trait).BaseValue = m_Info[trait, Level];

			LevelUp?.Invoke(this);
		}

		public Attribute this[CharacterTrait trait] => GetAttribute(trait);

		public delegate void OnLevelUp(MOBACharacter character);
		public event OnLevelUp LevelUp;

#if UNITY_EDITOR
		[CustomEditor(typeof(MOBACharacter))]
		public class MOBACharacterEditor : Editor
		{
			MOBACharacter m_Target;

			private void OnEnable() => m_Target = (MOBACharacter)target;

			public override void OnInspectorGUI()
			{
				base.OnInspectorGUI();

				EditorGUILayout.Space();

				EditorGUILayout.LabelField($"Level: {m_Target.Level + 1}");
				EditorGUILayout.LabelField($"Experience: {m_Target.m_Experience}");
			}
		}
#endif
	}
}