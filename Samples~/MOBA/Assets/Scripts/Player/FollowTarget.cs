using UnityEngine;
using UnityEngine.InputSystem;

public class FollowTarget : MonoBehaviour
{
	[SerializeField] private Transform m_Target;
	[SerializeField] private Vector3 m_Offset;

	[SerializeField] private float m_FollowSpeed = 2.0f;
	[SerializeField] private float m_RotationSpeed = 1.0f;

	private void Update()
	{
		if(!m_Target)
		{
			enabled = false;
			return;
		}

		// Position
		Vector3 targetPos = m_Target.position + m_Offset;
		transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * m_FollowSpeed);

		// Rotation
		transform.rotation = Quaternion.Slerp(transform.rotation, m_Target.rotation, Time.deltaTime * m_RotationSpeed);
	}
}
