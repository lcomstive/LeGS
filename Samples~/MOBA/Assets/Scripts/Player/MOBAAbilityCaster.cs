using LEGS;
using UnityEngine;
using LEGS.Abilities;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace MOBAExample
{
	public class AbilityCastInfo
	{
		public MOBAAbility Ability { get; set; }
		public int AbilityIndex { get; private set; }
		public MOBAAbilityCaster Caster { get; private set; }

		private float m_CooldownRemaining;
		public float CooldownRemaining
		{
			get => m_CooldownRemaining;
			set
			{
				if(m_CooldownRemaining == value)
					return;

				m_CooldownRemaining = value;
				EventManager.Publish(s_AbilityCastInfoChangedEventID, new AbilityCastInfoChangedEventArgs(this));
			}
		}

		/// <summary>
		/// Value ranging 0.0-1.0 
		/// </summary>
		public float CooldownRemainingPercent => CooldownRemaining / Ability.Cooldown;

		public bool CanCast => CooldownRemaining <= 0;

		private static ushort s_AbilityCastInfoChangedEventID;

		public AbilityCastInfo(int abilityIndex, MOBAAbility ability, MOBAAbilityCaster caster)
		{
			AbilityIndex = abilityIndex;
			Caster = caster;
			Ability = ability;

			s_AbilityCastInfoChangedEventID = EventManager.RegisterEvent<AbilityCastInfoChangedEventArgs>(AbilityCastInfoChangedEventArgs.EventName);

			ResetCooldown();
		}

		public void ResetCooldown() => CooldownRemaining = Ability?.Cooldown ?? float.MaxValue;

		public void ReduceCooldown(float amount)
		{
			if(CooldownRemaining <= 0)
				return; // Nothing to reduce

			CooldownRemaining = Mathf.Clamp(CooldownRemaining - amount, 0, Ability?.Cooldown ?? float.MaxValue);
		}
	}

	public class MOBAAbilityCaster : MonoBehaviour
	{
		[field: SerializeField] public MOBACharacter Character { get; private set; }

		[Header("Input")]
#if ENABLE_INPUT_SYSTEM
		[SerializeField] private InputActionReference[] m_AbilityInputs;
#else
		[SerializeField] private KeyCode[] m_AbilityInputs;
#endif

		public AbilityCastInfo[] Abilities { get; private set; }

		private void Start()
		{
			Debug.Assert(Character != null, "The 'Character' field needs to be assigned to 'MOBAAbilityCaster'");

			Abilities = new AbilityCastInfo[Character.Info.Abilities.Length];
			for(int i = 0; i < Character.Info.Abilities.Length; i++)
				Abilities[i] = new AbilityCastInfo(i, Character.Info.Abilities[i], this);
		}

		private void Update()
		{
			if(Abilities.Length == 0 || m_AbilityInputs.Length == 0)
				return;

			foreach(AbilityCastInfo info in Abilities)
				info.ReduceCooldown(Time.deltaTime);

			for (int i = 0; i < m_AbilityInputs.Length; i++)
			{
#if ENABLE_INPUT_SYSTEM
					if(m_AbilityInputs[i].action.IsPressed())
#else
					if(Input.GetKeyDown(m_AbilityInputs[i]))
#endif
						CastAbility(i);
			}
		}

		public void CastAbility(int index)
		{
			if(index < 0 || index >= Abilities.Length)
				return;
			MOBAAbility ability = Abilities[index].Ability;
			if (!Abilities[index].CanCast || !Character.CanCast(ability))
				return;

			if (ability.CostType == AbilityCost.Mana)	Character.Mana -= ability.Cost;
			if (ability.CostType == AbilityCost.Health) Character.ApplyDamage(ability.Cost, Character, DamageTypes.True);

			ability.Activate(Character, Character.gameObject);
			Abilities[index].ResetCooldown();
		}
	}
}