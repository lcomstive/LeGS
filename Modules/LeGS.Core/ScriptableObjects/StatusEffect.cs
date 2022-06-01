using UnityEngine;

namespace LEGS
{
	/// <summary>
	/// <see cref="ScriptableObject"/> derived <see cref="IStatusEffect"/>
	/// </summary>
	public class StatusEffect : ScriptableObject, IStatusEffect
	{
		[SerializeField, Tooltip("Readable name suitable for debugging and/or game UI")]
		private string m_DisplayName = "Status Effect";

		[SerializeField, Tooltip("Maximum amount of this status effect to be applied to an entity. Value of 0 = infinite.")]
		private uint m_MaxStackSize = 1;

		public string Name => m_DisplayName;
		public uint MaxStackSize => m_MaxStackSize;

		public IEntity Sender { get; private set; }
		public IStatusEffectReceiver Receiver { get; private set; }

		public virtual void OnAdded(IEntity sender, IStatusEffectReceiver receiver)
		{
			Sender = sender;
			Receiver = receiver;

			EventManager.Publish(StatusEffectChangeArgs.EventName, new StatusEffectChangeArgs(Sender, Receiver, this, true));
		}

		public virtual void OnRemoved() => EventManager.Publish(StatusEffectChangeArgs.EventName, new StatusEffectChangeArgs(Sender, Receiver, this, false));
	}
}
