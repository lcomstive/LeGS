using UnityEngine;
using UnityEngine.InputSystem;

public class FollowTarget : MonoBehaviour
{
	[SerializeField] private Transform m_Target;
	[SerializeField] private Vector3 m_Offset;

	[SerializeField] private float m_FollowSpeed = 2.0f;
	[SerializeField] private float m_RotationSpeed = 1.0f;

	[SerializeField] private bool m_FollowPosition = true;
	[SerializeField] private bool m_FollowRotationX = true;
	[SerializeField] private bool m_FollowRotationY = true;

	private void Update()
	{
		if(!m_Target)
		{
			enabled = false;
			return;
		}

		// Position
		if(m_FollowPosition)
		{
			Vector3 targetPos = m_Target.position + m_Offset;
			transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * m_FollowSpeed);
		}

		// Rotation
		Vector3 eulerAngles = transform.eulerAngles;
		eulerAngles.z = 0;
		if(m_FollowRotationX) eulerAngles.x = m_Target.transform.eulerAngles.x;
		if(m_FollowRotationY) eulerAngles.y = m_Target.transform.eulerAngles.y;
		if(m_FollowRotationX || m_FollowRotationY)
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(eulerAngles), Time.deltaTime * m_RotationSpeed);
	}
}
