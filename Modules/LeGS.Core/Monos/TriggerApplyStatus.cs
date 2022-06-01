using UnityEngine;

namespace LEGS
{
	[System.Flags]
	public enum TriggerApplyType
	{
		None = 0,
		Enter = 1,
		Exit = 2,
		Stay = 4
	}

	/// <summary>
	/// Applies a <see cref="StatusEffect"/> during trigger event(s)
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public class TriggerApplyStatus : MonoBehaviour
	{
		/// <summary>
		/// A <see cref="StatusEffect"/> to apply to entity
		/// </summary>
		public StatusEffect Effect;

		/// <summary>
		/// When to apply <see cref="Effect"/>
		/// </summary>
		public TriggerApplyType TriggerType = TriggerApplyType.Enter;

		[Tooltip("When not empty, only applies status effect to colliders with a matching tag")]
		public string[] FilterTags = new string[0];

		private IEntity m_Entity = null;

		private void Start()
		{
			GetComponent<Collider>().isTrigger = true;

			m_Entity = GetComponent<IEntity>();
		}

		private void ApplyTo(Collider collider)
		{
			if(!Effect || (FilterTags.Length != 0 && collider.CompareTags(FilterTags)))
				return;

			if (collider.TryGetComponent(out IStatusEffectReceiver receiver))
				receiver.AddStatusEffect(Effect, m_Entity);
		}

		#region Trigger Functions
		private void OnTriggerEnter(Collider other)
		{
			if (TriggerType.HasFlag(TriggerApplyType.Enter))
				ApplyTo(other);
		}

		private void OnTriggerExit(Collider other)
		{
			if (TriggerType.HasFlag(TriggerApplyType.Exit))
				ApplyTo(other);
		}

		private void OnTriggerStay(Collider other)
		{
			if (TriggerType.HasFlag(TriggerApplyType.Stay))
				ApplyTo(other);
		}
		#endregion
	}
}