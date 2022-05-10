using LEGS;
using UnityEngine;

namespace MOBAExample
{
	[CreateAssetMenu(menuName = "MOBA/Abilities/Spawn Object", fileName = "Spawn Object")]
	public class MOBAAbilitySpawnObject : MOBAAbility
	{
		[SerializeField] private GameObject m_Prefab;
		[SerializeField] private Vector3 m_SpawnOffset;
		[SerializeField] private bool m_ChildOfSpawner = false;

		/// <summary>
		/// Force to apply if spawned object has a Rigidbody attached
		/// </summary>
		[SerializeField, Tooltip("Force to apply if spawned object has a Rigidbody")]
		private float m_Force;

		public override void Activate(IEntity caster, GameObject gameObject)
		{
			if(!m_Prefab)
				return; // Nothing to spawn
			
			PlayerAim playerAim = gameObject.GetComponent<PlayerAim>();

			Transform spawnOrigin = playerAim?.AimOrigin ?? gameObject.transform;
			Quaternion spawnRot = gameObject.transform.rotation * m_Prefab.transform.rotation;
			Vector3 spawnPos = spawnOrigin.position + spawnOrigin.TransformDirection(m_SpawnOffset);

			GameObject spawned = Instantiate(
				m_Prefab,
				spawnPos,
				spawnRot,
				m_ChildOfSpawner ? gameObject.transform : null
			);

			Vector3 aimDirection = playerAim?.AimDirection ?? gameObject.transform.forward;
			aimDirection = aimDirection.normalized * m_Force;

			if(spawned.TryGetComponent(out Rigidbody rigidbody))
				rigidbody.AddForce(aimDirection, ForceMode.Impulse);
		}
	}
}
