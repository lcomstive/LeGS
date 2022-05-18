using LEGS;
using UnityEngine;

namespace MOBAExample
{
	/// <summary>
	/// Adds instant force to attached <see cref="Rigidbody"/>
	/// </summary>
	[CreateAssetMenu(menuName = "MOBA/Abilities/Apply Force", fileName = "Apply Force")]
    public class MOBAAbilityApplyForce : MOBAAbility
    {
		[SerializeField] private float m_Force;
		[SerializeField] private Vector3 m_Direction;

		[SerializeField, Tooltip("If enabled, direction is relative to entity forward")]
		private bool m_RelativeToPlayer = true;

		[SerializeField, Tooltip("If enabled, direction is overridden by PlayerAim.AimDirection")]
		private bool m_AimOverrideDirection;

		[SerializeField] private bool m_IgnoreMass = false;

		/// <summary>
		/// Prefab to spawn at location before applying force, can be used for special effects or playing audio for instance
		/// </summary>
		[Tooltip("Prefab to spawn at location before applying force, can be used for special effects or playing audio for instance")]
		[SerializeField] private GameObject m_EffectPrefab;

		[SerializeField] private Vector3 m_EffectSpawnOffset;

		public override void Activate(IEntity caster, GameObject gameObject)
		{
			if(!gameObject.TryGetComponent(out Rigidbody rigidbody))
				return;
			Vector3 force = m_Direction * m_Force;
			if(m_AimOverrideDirection && gameObject.TryGetComponent(out PlayerAim aim))
				force = aim.AimDirection * m_Force;
			else if(m_RelativeToPlayer)
				force = gameObject.transform.TransformDirection(m_Direction) * m_Force;

			rigidbody.AddForce(
				force,
				m_IgnoreMass ? ForceMode.VelocityChange : ForceMode.Impulse
			);

			if(m_EffectPrefab)
				Instantiate(
					m_EffectPrefab,
					gameObject.transform.position + gameObject.transform.TransformDirection(m_EffectSpawnOffset),
					gameObject.transform.rotation
				);
		}
	}
}
