using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LEGS;

namespace MOBAExample
{
	[RequireComponent(typeof(IEntity))]
	public class GiveExperienceOnDeath : MonoBehaviour
	{
		[SerializeField] private float m_Experience = 30.0f;

		private IEntity m_Entity;

		private void Start()
		{
			m_Entity = GetComponent<IEntity>();
			EventManager.Subscribe<EntityDeathEventArgs>(EntityDeathEventArgs.EventName, OnEntityDeath, true);
		}

		private void OnEntityDeath(EntityDeathEventArgs args)
		{
			if(args.Entity != m_Entity)
				return;
			MOBACharacter character = args.Killer as MOBACharacter;
			if(character)
				character.Experience.AddExperience(m_Experience);
		}
	}
}