using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LEGS
{
	public class StatusEffect : ScriptableObject, IStatusEffect
	{
		[SerializeField] private string m_DisplayName = "Status Effect";
		[SerializeField] private uint m_MaxStackSize = 1;

		public string Name => m_DisplayName;
		public uint MaxStackSize => m_MaxStackSize;

		public IEntity Sender { get; private set; }
		public IEntity Receiver { get; private set; }

		public virtual void OnAdded(IEntity sender, IStatusEffectReceiver receiver)
		{
			Sender = sender;
			Receiver = receiver;

			EventManager.Publish(StatusEffectChangeArgs.EventName, new StatusEffectChangeArgs(Sender, Receiver, this, true));
		}

		public virtual void OnRemoved() => EventManager.Publish(StatusEffectChangeArgs.EventName, new StatusEffectChangeArgs(Sender, Receiver, this, false));
	}
}
