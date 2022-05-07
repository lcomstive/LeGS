using UnityEngine;

namespace LEGS
{
	/// <summary>
	/// <see cref="MonoBehaviour"/> base implementation of an <see cref="IEntity"/>
	/// </summary>
	public class Entity : MonoBehaviour, IEntity
	{
		/// <summary>
		/// Readable name suitable for debugging and/or UI
		/// </summary>
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