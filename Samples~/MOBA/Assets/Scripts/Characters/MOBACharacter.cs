using LEGS;
using UnityEngine;
using LEGS.Characters;
using System.Collections;

namespace MOBAExample
{
	public class MOBACharacter : Character
	{
		public const uint MaxLevel = 18;

		[SerializeField] private MOBACharacterInfo m_Info;

		public int Level => (int)Mathf.Floor(m_Info.ExperienceLevels.Evaluate(m_Experience));

		private float m_Experience;
		private Coroutine m_HealthRegenCoroutine;

		protected override void Awake()
		{
			base.Awake();
			AddDamageModifier<MOBAAbilityDamageModifier>();
			EventManager.Subscribe<EntityDeathEventArgs>(EntityDeathEventArgs.EventName, OnEntityDeath, true);

			m_HealthRegenCoroutine = StartCoroutine(HealthRegenLoop());
		}

		private void OnDestroy()
		{
			if(m_HealthRegenCoroutine != null)
				StopCoroutine(m_HealthRegenCoroutine);

			EventManager.Unsubscribe<EntityDeathEventArgs>(EntityDeathEventArgs.EventName, OnEntityDeath);
		}

		public override void ApplyDamage(float amount, IEntity sender)
		{
			base.ApplyDamage(amount, sender);

			if(Health <= 0)
				EventManager.Publish(EntityDeathEventArgs.EventName, new EntityDeathEventArgs(this, sender));
		}

		private void OnEntityDeath(EntityDeathEventArgs args)
		{
			// Check if we performed the kill
			if(args.Killer != (IEntity)this)
				return;

			MOBACharacter character = args.Entity as MOBACharacter;
			if(!character)
				return; // Invalid/incompatible cast to MOBACharacter

			int previousLevel = Level;
			m_Experience += character.m_Info.KillRewardExperience;
			if(Level != previousLevel)
			{
				Debug.Log($"'{DisplayName}' reached level {Level}");
				// TODO: Event
			}

			// TODO: Gain coin equal to character.m_Info.KillReward
		}

		private IEnumerator HealthRegenLoop()
		{
			yield return new WaitForSeconds(1.0f);
			ApplyDamage(-this[CharacterTrait.HealthRegen], this);

			m_HealthRegenCoroutine = StartCoroutine(HealthRegenLoop());
		}

		public float this[CharacterTrait trait]
		{
			get => m_Info[trait, Level];
		}
	}
}