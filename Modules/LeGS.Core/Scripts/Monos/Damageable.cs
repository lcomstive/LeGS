using System;
using UnityEngine;

namespace LEGS
{
	[RequireComponent(typeof(IEntity))]
    public class Damageable : MonoBehaviour, IDamageable
    {
		[SerializeField] private float m_Health;
		[SerializeField] private float m_MaxHealth;
		[SerializeField] private bool m_DestroyOnDeath = true;

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

		private IEntity m_Entity;
		private ushort m_EntityHealthChangeEventID, m_EntityDeathEventID;

		private void Start()
		{
			m_Entity = GetComponent<IEntity>();
			m_EntityDeathEventID = EventManager.RegisterEvent<EntityDeathEventArgs>(EntityDeathEventArgs.EventName);
			m_EntityHealthChangeEventID = EventManager.RegisterEvent<EntityHealthChangeEventArgs>(EntityHealthChangeEventArgs.EventName);

			Health = MaxHealth;
		}

		public void ApplyDamage(float amount, IEntity sender)
		{
			Health = Mathf.Clamp(Health - amount, 0.0f, MaxHealth);

			if(Health == 0)
			{
				EventManager.Publish(m_EntityDeathEventID, new EntityDeathEventArgs(m_Entity, sender));
				OnDied?.Invoke();

				if(m_DestroyOnDeath)
					Destroy(gameObject);
			}
		}

		public event Action OnDied;
    }
}
