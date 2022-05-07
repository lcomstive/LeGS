using UnityEngine;
using System.Collections.Generic;

namespace LEGS.Characters
{
	/// <summary>
	/// Simple class for an <see cref="ICharacter"/> & <see cref="IAttributeHolder"/>
	/// implementation.
	/// </summary>
	public class Character : Entity, ICharacter, IAttributeHolder
	{
		/// <summary>
		/// When true, limits <see cref="Health"/> to a maximum of <see cref="MaxHealth"/>
		/// </summary>
		[SerializeField, Tooltip("Limits Health to MaxHealth")]
		protected bool CapHealth = true;

		/// <summary>
		/// Current <see cref="IDamageable.Health"/> value
		/// </summary>
		[field: SerializeField]
		public float Health { get; protected set; }

		/// <summary>
		/// <see cref="IDamageable.MaxHealth"/> value
		/// </summary>
		[field: SerializeField]
		public float MaxHealth { get; protected set; }

		/// <summary>
		/// List of <see cref="Attribute"/>s in <see cref="IAttributeHolder"/>
		/// </summary>
		[SerializeField, Tooltip("Shown in inspector for debugging purposes. Not intended to be modified manually during runtime")]
		protected List<Attribute> Attributes = new List<Attribute>();

		/// <summary>
		/// Dictionary of currently applied <see cref="IStatusEffect"/>s,
		/// with key value as case-sensitive <see cref="IStatusEffect.Name"/>
		/// </summary>
		protected Dictionary<string, List<IStatusEffect>> AppliedStatusEffects = new Dictionary<string, List<IStatusEffect>>();

		private ushort m_EntityHealthChangeEventID;
		private ushort m_StatusEffectChangeEventID;

		#region IStatusEffectReceiver
		/// <summary>
		/// Try adding <paramref name="effect"/> to this <see cref="IStatusEffectReceiver"/>
		/// </summary>
		/// <param name="sender"><see cref="IEntity"/> that added <paramref name="effect"/></param>
		public virtual void AddStatusEffect(IStatusEffect effect, IEntity sender)
		{
			string effectName = effect.Name;

			// If effect isn't already in dictionary, create new list
			if (!AppliedStatusEffects.ContainsKey(effectName))
				AppliedStatusEffects.Add(effectName, new List<IStatusEffect>());

			// Check if adding this effect will exceed max stack size
			if (effect.MaxStackSize > 0 && AppliedStatusEffects[effectName].Count >= effect.MaxStackSize - 1)
				RemoveStatusEffect(AppliedStatusEffects[effectName][0]); // Remove first effect, effectively replacing with most recent

			// Add effect 
			AppliedStatusEffects[effectName].Add(effect);

			// Inform listeners
			effect.OnAdded(sender, this);
			EventManager.Publish(m_StatusEffectChangeEventID, new StatusEffectChangeArgs(sender, this, effect, true));
		}

		/// <summary>
		/// Attempts to remove <paramref name="effect"/> from this <see cref="IStatusEffectReceiver"/>
		/// </summary>
		/// <param name="effect"></param>
		public virtual void RemoveStatusEffect(IStatusEffect effect)
		{
			if(!AppliedStatusEffects.ContainsKey(effect.Name))
				return; // Not applied to this object

			int effectIndex = AppliedStatusEffects[effect.Name].FindIndex(x => x == effect);
			if(effectIndex < 0)
				return; // Not applied to this object

			// Inform listeners
			effect.OnRemoved();
			EventManager.Publish(m_StatusEffectChangeEventID, new StatusEffectChangeArgs(null, this, effect, false));

			// Remove from list
			AppliedStatusEffects[effect.Name].RemoveAt(effectIndex);

			// Check for and remove empty list
			if(AppliedStatusEffects[effect.Name].Count == 0)
				AppliedStatusEffects.Remove(effect.Name);
		}

		/// <summary>
		/// Tries to retrieve <see cref="IStatusEffect"/> from this <see cref="IStatusEffectReceiver"/>
		/// </summary>
		/// <typeparam name="T">Type of <see cref="IStatusEffect"/> to get</typeparam>
		/// <param name="name">Case-sensitive name, matching <see cref="IStatusEffect.Name"/></param>
		/// <returns>First instance of <typeparamref name="T"/> if found, otherwise null</returns>
		public virtual T GetStatusEffect<T>(string name) where T : class, IStatusEffect =>
			AppliedStatusEffects.ContainsKey(name) && AppliedStatusEffects[name].Count > 0 ? (T)AppliedStatusEffects[name][0] : null;

		/// <summary>
		/// Tries to retrieve all <see cref="IStatusEffect"/>s from this <see cref="IStatusEffectReceiver"/>
		/// matching <paramref name="name"/> (<i>case-sensitive</i>)
		/// </summary>
		/// <param name="name">Case-sensitive name, matching <see cref="IStatusEffect.Name"/></param>
		/// <returns>All instances of <see cref="IStatusEffect"/> if found, otherwise null</returns>
		public virtual List<IStatusEffect> GetStatusEffects(string name) =>
			AppliedStatusEffects.ContainsKey(name) ? AppliedStatusEffects[name] : null;
		#endregion

		#region IDamageable
		/// <summary>
		/// Reduces <see cref="Health"/> by <paramref name="amount"/> on this <see cref="IDamageable"/>
		///
		/// <para>
		///		<see cref="Health"/> may be limited to <see cref="MaxHealth"/> if
		///		<see cref="CapHealth"/> is true
		/// </para>
		/// </summary>
		/// <param name="amount">Value to reduce <see cref="Health"/> by</param>
		/// <param name="sender"><see cref="IEntity"/> that applied damage</param>
		public virtual void ApplyDamage(float amount, IEntity sender)
		{			
			// If health is capped, ensure (health - amount) does not surpass MaxHealth
			if(CapHealth && (Health - amount) >= MaxHealth)
				amount -= MaxHealth - (Health - amount); // Subtract difference

			Health -= amount;

			// Inform listeners
			EventManager.Publish(m_EntityHealthChangeEventID, new EntityHealthChangeEventArgs(this, sender, amount));
		}
		#endregion

		#region IAttributeHolder
		/// <summary>
		/// Adds an <see cref="Attribute"/> to this <see cref="IAttributeHolder"/>
		/// </summary>
		public virtual void AddAttribute(Attribute attribute) => Attributes.Add(attribute);

		/// <summary>
		/// Removes desired <see cref="Attribute"/> from this <see cref="IAttributeHolder"/>
		/// </summary>
		public virtual void RemoveAttribute(Attribute attribute) => Attributes.Remove(attribute);

		/// <summary>
		/// Checks if this <see cref="IAttributeHolder"/> contains an <see cref="Attribute"/>
		/// with <see cref="Attribute.Name"/> of <paramref name="name"/> (case-sensitive)
		/// </summary>
		/// <param name="name">Case-sensitive name to find matching <see cref="Attribute.Name"/></param>
		/// <returns>True if <see cref="Attribute"/> found with matching <see cref="Attribute.Name"/></returns>
		public virtual bool HasAttribute(string name)
		{
			foreach(Attribute attribute in Attributes)
				if(attribute.Name.Equals(name))
					return true;
			return false;
		}

		/// <summary>
		/// Tries to get an <see cref="Attribute"/> from this <see cref="IAttributeHolder"/>
		/// </summary>
		/// <param name="name">Case-sensitive name to find matching <see cref="Attribute.Name"/></param>
		/// <returns>Instance of <see cref="Attribute"/> if found, otherwise null</returns>
		public Attribute GetAttribute(string name)
		{
			foreach(Attribute attribute in Attributes)
				if(attribute.Name.Equals(name))
					return attribute;
			return null;
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