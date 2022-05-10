using LEGS;
using UnityEngine;
using LEGS.Abilities;
using LEGS.Characters;
using System.Collections;
using System.Collections.Generic;

namespace MOBAExample
{
	public class MOBACharacter : Character
	{
		[field: SerializeField] public MOBACharacterInfo Info { get; private set; }

		public static readonly CharacterTrait[] AllTraits = (CharacterTrait[])System.Enum.GetValues(typeof(CharacterTrait));
		
		public float MaxMana { get; private set; }
		public CharacterExperience Experience { get; private set; }

		private float m_Mana;
		public float Mana
		{
			get => m_Mana;
			set
			{
				value = Mathf.Clamp(value, 0, MaxMana);
				if(m_Mana == value)
					return;

				m_Mana = value;
				EventManager.Publish(m_CharacterChangedEventID, new CharacterChangedEventArgs(this));
			}
		}


		private ushort m_HealthChangedEventID, m_CharacterChangedEventID;
		private Coroutine m_RegenCoroutine;
		private Dictionary<CharacterTrait, Attribute> m_AttributeTraits = new Dictionary<CharacterTrait, Attribute>();

		protected override void Awake()
		{
			base.Awake();

			Experience = GetComponent<CharacterExperience>();

			// Set attributes
			Attributes = new List<Attribute>();
			foreach (CharacterTrait trait in AllTraits)
			{
				Attribute attribute = new Attribute(trait.ToString(), Info[trait, Experience.Level]);
				Attributes.Add(attribute);
				m_AttributeTraits.Add(trait, attribute);
			}

			m_RegenCoroutine = StartCoroutine(RegenLoop());

			m_Mana = MaxMana = m_AttributeTraits[CharacterTrait.Mana].CurrentValue;
			Health = MaxHealth = m_AttributeTraits[CharacterTrait.Health].CurrentValue;

			// Register events
			m_HealthChangedEventID = EventManager.RegisterEvent<EntityHealthChangeEventArgs>(EntityHealthChangeEventArgs.EventName);
			m_CharacterChangedEventID = EventManager.RegisterEvent<CharacterChangedEventArgs>(CharacterChangedEventArgs.EventName);

			// Listen to relevant events
			m_AttributeTraits[CharacterTrait.Mana].CurrentValueChanged += OnAttributeManaChanged;
			m_AttributeTraits[CharacterTrait.Health].CurrentValueChanged += OnAttributeHealthChanged;

			Experience.LevelUp += OnCharacterLevelUp;
		}

		private void OnCharacterLevelUp(int level)
		{
			// Update all trait base values
			foreach (CharacterTrait trait in AllTraits)
				GetAttribute(trait).BaseValue = Info[trait, level];
		}

		private void OnAttributeHealthChanged(Attribute attribute)
		{
			bool healthFollow = Health == MaxHealth;
			MaxHealth = attribute.CurrentValue;
			if (healthFollow)
				Health = MaxHealth;

			EventManager.Publish(m_HealthChangedEventID, new EntityHealthChangeEventArgs(this, this, 0));
		}

		private void OnAttributeManaChanged(Attribute attribute)
		{
			bool manaFollow = Mana == MaxMana;
			MaxMana = attribute.CurrentValue;
			if (manaFollow)
				Mana = MaxMana;
		}

		public Attribute GetAttribute(CharacterTrait trait) => m_AttributeTraits[trait];

		private void OnDestroy()
		{
			if (m_RegenCoroutine != null)
				StopCoroutine(m_RegenCoroutine);
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
			if (ability.ReceiverEffect != null)
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

			if (resistance > 0) // Apply resistance
				amount *= 1.0f - (resistance / (resistance + 100.0f));

			base.ApplyDamage(amount, sender);
		}

		private IEnumerator RegenLoop()
		{
			yield return new WaitForSeconds(1.0f);

			Mana = Mathf.Clamp(Mana + this[CharacterTrait.ManaRegen].CurrentValue, 0, MaxMana);
			ApplyDamage(-this[CharacterTrait.HealthRegen].CurrentValue, this);

			m_RegenCoroutine = StartCoroutine(RegenLoop());
		}

		public virtual bool CanCast(MOBAAbility ability)
		{
			if (ability.CostType == AbilityCost.Mana &&
				Mana <= ability.Cost)
				return false; // Not enough mana

			if (ability.CostType == AbilityCost.Health &&
				Health <= ability.Cost)
				return false; // Not enough mana

			return true;
		}

		public Attribute this[CharacterTrait trait] => GetAttribute(trait);
	}
}