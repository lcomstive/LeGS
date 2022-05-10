using UnityEngine;

namespace LEGS.Abilities
{
	/// <summary>
	/// <see cref="Ability"/> that spawns an object
	/// </summary>
	[CreateAssetMenu(menuName = "LeGS/Abilities/Spawn Object", fileName = "Spawn Object")]
	public class AbilitySpawnObject : Ability
	{
		[SerializeField] private GameObject m_Prefab;
		[SerializeField] private Vector3 m_SpawnOffset;
		[SerializeField] private bool m_ChildOfSpawner = false;

		[SerializeField, Tooltip("Force to apply if spawned object has a Rigidbody")]
		private Vector3 m_Force;

		public override void Activate(IEntity caster, GameObject gameObject)
		{
			if(!m_Prefab)
				return; // Nothing to spawn

			Vector3 spawnPos = gameObject.transform.position + gameObject.transform.TransformDirection(m_SpawnOffset);
			Quaternion spawnRot = gameObject.transform.rotation * m_Prefab.transform.rotation;
			GameObject spawned = Instantiate(m_Prefab, spawnPos, spawnRot);

			if(m_ChildOfSpawner)
				spawned.transform.parent = gameObject.transform;

			if(spawned.TryGetComponent(out Rigidbody rigidbody))
				rigidbody.AddForce(gameObject.transform.TransformDirection(m_Force), ForceMode.Impulse);
		}
	}
}
