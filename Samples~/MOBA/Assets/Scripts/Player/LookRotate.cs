using UnityEngine;
using UnityEngine.InputSystem;

public class LookRotate : MonoBehaviour
{
	[Header("Input")]
	[SerializeField] private InputActionReference m_LookAction;
	[SerializeField] private float m_RotationSpeed = 1.0f;

	private void Update()
	{
		Vector2 input = m_LookAction.action.ReadValue<Vector2>();
		transform.eulerAngles += new Vector3(0, input.x, 0) * m_RotationSpeed * Time.deltaTime;

		Quaternion targetRot = transform.rotation * Quaternion.Euler(0, input.x, 0);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * m_RotationSpeed);
	}
}
