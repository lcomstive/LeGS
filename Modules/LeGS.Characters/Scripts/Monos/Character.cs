using UnityEngine;
using System.Collections.Generic;

namespace LEGS.Characters
{
	public class Character : Entity, ICharacter
	{
		[SerializeField, Tooltip("Can health go above max health?")]
		protected bool CapHealth = true;

		[field: SerializeField]
		public float Health { get; protected set; }

		[field: SerializeField]
		public float MaxHealth { get; protected set; }

		/// <summary>
		/// 
		/// </summary>
		protected List<IDamageModifier> DamageModifiers = new List<IDamageModifier>();
		protected Dictionary<string, List<IStatusEffect>> AppliedStatusEffects = new Dictionary<string, List<IStatusEffect>>();

		private ushort m_EntityHealthChangeEventID;
		private ushort m_StatusEffectChangeEventID;

		#region ICharacter
		public virtual T AddDamageModifier<T>() where T : IDamageModifier, new()
		{
			T modifier = new T();
			DamageModifiers.Add(modifier);
			return modifier;
		}

		public virtual void RemoveDamageModifier(IDamageModifier modifier) => DamageModifiers.Remove(modifier);
		#endregion

		#region IStatusEffectReceiver
		public virtual void AddStatusEffect(IStatusEffect effect, IEntity sender)
		{
			string effectName = effect.Name;
			if (!AppliedStatusEffects.ContainsKey(effectName))
				AppliedStatusEffects.Add(effectName, new List<IStatusEffect>());

			if (effect.MaxStackSize > 0 && AppliedStatusEffects[effectName].Count >= effect.MaxStackSize)
				RemoveStatusEffect(AppliedStatusEffects[effectName][0]); // Remove first effect, replace with most recent

			AppliedStatusEffects[effectName].Add(effect);
			effect.OnAdded(sender, this);
			EventManager.Publish(m_StatusEffectChangeEventID, new StatusEffectChangeArgs(sender, this, effect, true));
		}

		public virtual void RemoveStatusEffect(IStatusEffect effect)
		{
			effect.OnRemoved();
			EventManager.Publish(m_StatusEffectChangeEventID, new StatusEffectChangeArgs(null, this, effect, false));

			AppliedStatusEffects[effect.Name].Remove(effect);
		}

		public virtual T GetStatusEffect<T>(string name) where T : class, IStatusEffect =>
			AppliedStatusEffects.ContainsKey(name) && AppliedStatusEffects[name].Count > 0 ? (T)AppliedStatusEffects[name][0] : null;

		#endregion

		#region IDamageable
		public virtual void ApplyDamage(float amount, IEntity sender)
		{
			foreach (IDamageModifier modifier in DamageModifiers)
				amount = modifier.CalculateDamage(amount, sender, this);

			float finalValue = Health - amount;
			
			// If health is capped, ensure health-amount does not surpass maxhealth
			if(CapHealth && Health - amount >= MaxHealth)
				amount -= MaxHealth - (Health - amount); // Subtract difference

			Health -= amount;

			EventManager.Publish(m_EntityHealthChangeEventID, new EntityHealthChangeEventArgs(this, sender, amount));
		}
		#endregion

		protected override void Awake()
		{
			base.Awake();

			Health = MaxHealth;

			m_StatusEffectChangeEventID = EventManager.RegisterEvent<StatusEffectChangeArgs>(StatusEffectChangeArgs.EventName);
			m_EntityHealthChangeEventID = EventManager.RegisterEvent<EntityHealthChangeEventArgs>(EntityHealthChangeEventArgs.EventName);
		}

		private void Update()
		{
			foreach (string effectName in AppliedStatusEffects.Keys)
			{
				// Reverse loop because timed effects may remove themselves in OnUpdate()
				for(int i = AppliedStatusEffects[effectName].Count - 1; i >= 0; i--)
				{
					TimedStatusEffect timedEffect = AppliedStatusEffects[effectName][i] as TimedStatusEffect;
					if (timedEffect)
						timedEffect.OnUpdate();
				}
			}
		}
	}
}