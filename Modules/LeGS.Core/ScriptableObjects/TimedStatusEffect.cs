using UnityEngine;

namespace LEGS
{
	/// <summary>
	/// <see cref="StatusEffect"/> that removes itself from <see cref="StatusEffect.Receiver"/>
	/// after <see cref="Duration"/> seconds has passed
	/// </summary>
    public class TimedStatusEffect : StatusEffect
    {
		/// <summary>
		/// Time in seconds to apply this status effect. <0 = no expirt
		/// </summary>
		[SerializeField, Tooltip("Time in seconds to apply this status effect. <0 = no expiry")]
		public float Duration = 1.0f;

		/// <summary>
		/// Time before removing from <see cref="StatusEffect.Receiver"/>
		/// </summary>
		public float TimeRemaining { get; private set; }

		public override void OnAdded(IEntity sender, IStatusEffectReceiver receiver)
		{
			TimeRemaining = Duration;
			base.OnAdded(sender, receiver);
		}

		/// <summary>
		/// Called once per frame on the main game thread
		/// </summary>
		public virtual void OnUpdate()
		{
			if(TimeRemaining < 0)
				return; // Less than zero before removing time, ignore. Most likely set duration <0

			TimeRemaining -= Time.deltaTime;

			if(TimeRemaining <= 0.0f)
				((IStatusEffectReceiver)Receiver).RemoveStatusEffect(this);
		}
	}
}
