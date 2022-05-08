using UnityEngine;

namespace LEGS.Abilities
{
	/// <summary>
	/// Adds instant force to attached <see cref="Rigidbody"/>
	/// </summary>
	[CreateAssetMenu(menuName = "LeGS/Abilities/Apply Force", fileName = "Apply Force")]
    public class AbilityApplyForce : Ability
    {
		[SerializeField] private Vector3 m_Force;
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
			rigidbody.AddRelativeForce(
				m_Force,
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
