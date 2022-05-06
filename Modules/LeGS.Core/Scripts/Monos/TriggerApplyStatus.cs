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

	[RequireComponent(typeof(Collider))]
	public class TriggerApplyStatus : MonoBehaviour
	{
		public StatusEffect Effect;
		public TriggerApplyType TriggerType = TriggerApplyType.Enter;

		private IEntity m_Entity = null;

		private void Start()
		{
			GetComponent<Collider>().isTrigger = true;

			m_Entity = GetComponent<IEntity>();
		}

		private void ApplyTo(Collider collider)
		{
			if(!Effect)
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