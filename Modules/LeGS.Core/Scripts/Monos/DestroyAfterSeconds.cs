using UnityEngine;

namespace LEGS
{
	/// <summary>
	/// Destroys attached <see cref="GameObject"/> after <see cref="m_Seconds"/>
	/// </summary>
	public class DestroyAfterSeconds : MonoBehaviour
	{
		[SerializeField, Tooltip("Time, in seconds, before destroying this object")]
		private float m_Seconds = 10.0f;

		private void Start() => Destroy(gameObject, m_Seconds);
	}
}
