using UnityEngine;

namespace LEGS
{
	public class Entity : MonoBehaviour, IEntity
	{
		[Tooltip("Readable name suitable for debugging and/or UI")]
		[field: SerializeField] public string DisplayName { get; private set; } = "Entity";

		private ushort m_SpawnEventID;

		protected virtual void Awake()
		{
			m_SpawnEventID = EventManager.RegisterEvent<EntitySpawnEventArgs>(EntitySpawnEventArgs.EventName);

			EventManager.Publish(m_SpawnEventID, new EntitySpawnEventArgs(this));
		}
	}
}