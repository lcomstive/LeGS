using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LEGS
{
    public class TimedStatusEffect : StatusEffect
    {
		[SerializeField, Tooltip("Time in seconds to apply this status effect")]
		public float Duration = 1.0f;

		public float TimeRemaining { get; private set; }

		public override void OnAdded(IEntity sender, IStatusEffectReceiver receiver)
		{
			TimeRemaining = Duration;
			base.OnAdded(sender, receiver);
		}

		public virtual void OnUpdate()
		{
			TimeRemaining -= Time.deltaTime;

			if(TimeRemaining <= 0.0f)
				((IStatusEffectReceiver)Receiver).RemoveStatusEffect(this);
		}
	}
}
