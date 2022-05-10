using LEGS;
using UnityEngine;

namespace MOBAExample
{
	/// <summary>
	/// <see cref="TimedStatusEffect"/> that restores <see cref="MOBACharacter.Mana"/> over time to <see cref="MOBACharacter"/>
	/// </summary>
	[CreateAssetMenu(fileName = "Mana Over Time", menuName = "MOBA/Status Effects/Mana Over Time")]
	public class StatusEffectChangeMana : TimedStatusEffect
	{
		/// <summary>
		/// Total <see cref="MOBACharacter.Mana"/> to restore, spread out over <see cref="TimedStatusEffect.Duration"/>
		/// </summary>
		[Tooltip("Total mana to restore, spread out over duration")]
		public float TotalMana = 10.0f;

		private MOBACharacter m_Character;

		public override void OnAdded(IEntity sender, IStatusEffectReceiver receiver)
		{
			base.OnAdded(sender, receiver);
			m_Character = receiver as MOBACharacter;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();

			// Apply damage to receiver
			if (m_Character)
				m_Character.Mana += (TotalMana / Duration) * Time.deltaTime;
		}
	}
}