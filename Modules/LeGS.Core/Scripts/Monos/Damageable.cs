using System;
using UnityEngine;

namespace LEGS
{
	/// <summary>
	/// Basic implementation of <see cref="IDamageable"/>.
	/// Recommended to have an <see cref="IEntity"/> attached to the same GameObject or one of it's children
	/// for event (<see cref="EntityHealthChangeEventArgs.Sender"/>) parameter.
	/// </summary>
    public class Damageable : MonoBehaviour, IDamageable
    {
		[SerializeField] private float m_Health;
		[SerializeField] private float m_MaxHealth;
		[SerializeField] private bool m_DestroyOnDeath = true;

		/// <summary>
		/// Health points available
		/// </summary>
		public float Health
		{
			get => m_Health;
			set
			{
				if(m_Health == value)
					return; // No change
				float delta = value - m_Health;
				m_Health = value;
				EventManager.Publish(m_EntityHealthChangeEventID, new EntityHealthChangeEventArgs(this, m_Entity, delta));
			}
		}

		/// <summary>
		/// Maximum health points
		/// </summary>
		public float MaxHealth
		{
			get => m_MaxHealth;
			set
			{
				if (m_MaxHealth == value)
					return; // No change
				float delta = value - m_MaxHealth;
				m_MaxHealth = value;
				EventManager.Publish(m_EntityHealthChangeEventID, new EntityHealthChangeEventArgs(this, m_Entity, delta));
			}
		}

		/// <summary>
		/// <see cref="IEntity"/> attached to GameObject or one of it's children
		/// </summary>
		private IEntity m_Entity;

		// Cache event IDs
		private ushort m_EntityHealthChangeEventID, m_EntityDeathEventID;

		private void Awake()
		{
			// Find related entity
			if(!TryGetComponent(out m_Entity))
			{
				m_Entity = GetComponentInChildren<IEntity>();
				if(m_Entity == null)
					Debug.LogWarning($"No IEntity found on '{gameObject.name}' or children.\nThe script will function, but events sent from it will have a null sender");
			}

			m_EntityDeathEventID = EventManager.RegisterEvent<EntityDeathEventArgs>(EntityDeathEventArgs.EventName);
			m_EntityHealthChangeEventID = EventManager.RegisterEvent<EntityHealthChangeEventArgs>(EntityHealthChangeEventArgs.EventName);

			Health = MaxHealth;
		}

		public void ApplyDamage(float amount, IEntity sender)
		{
			Health = Mathf.Clamp(Health - amount, 0.0f, MaxHealth);
			EventManager.Publish(m_EntityHealthChangeEventID, new EntityHealthChangeEventArgs(m_Entity, this, sender, amount));

			if (Health == 0)
			{
				EventManager.Publish(m_EntityDeathEventID, new EntityDeathEventArgs(m_Entity, sender));
				OnDied?.Invoke();

				if(m_DestroyOnDeath)
					Destroy(gameObject);
			}
		}

		/// <summary>
		/// <see cref="Health"/> has been depleted. (<i>Called after <see cref="EntityDeathEventArgs"/></i>)
		/// </summary>
		public event Action OnDied;
    }
}
