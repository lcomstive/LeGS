using UnityEngine;

namespace LEGS
{
	/// <summary>
	/// Applies force to a <see cref="Rigidbody"/> upon collision
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public class ApplyForceOnCollision : MonoBehaviour
	{
		public Vector3 Force;
		public bool IgnoreMass = false;

		private void OnCollisionEnter(Collision collision)
		{
			if(collision.gameObject.TryGetComponent(out Rigidbody rigidbody))
				rigidbody.AddForce(Force, IgnoreMass ? ForceMode.VelocityChange : ForceMode.Impulse);
		}
	}
}
