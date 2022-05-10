using UnityEngine;
using UnityEngine.InputSystem;

public class LookRotate : MonoBehaviour
{
	[SerializeField] private InputActionReference m_LookAction;
	[SerializeField] private bool m_RotateX = true;
	[SerializeField] private bool m_RotateY = true;

	[field: SerializeField] public float Sensitivity { get; set; } = 1.0f;
	[field: SerializeField] public bool InvertXInput { get; set; } = false;
	[field: SerializeField] public bool InvertYInput { get; set; } = true;

	[Header("Clamping")]
	[SerializeField] private bool m_Clamp = false;
	[SerializeField] private Vector2 m_MinMax = new Vector2(-60, 60);

	private Vector3 m_Euler;

	private void Update()
	{
		Vector2 input = m_LookAction.action.ReadValue<Vector2>() * Sensitivity * Time.deltaTime;

		if(input.sqrMagnitude <= 0.01f)
			return; // No input

		if(m_RotateY) m_Euler.y = m_Euler.y + input.x * (InvertXInput ? -1.0f : 1.0f);
		if(m_RotateX) m_Euler.x = ClampAngle(m_Euler.x + input.y * (InvertYInput ? -1.0f : 1.0f));

		transform.localRotation = Quaternion.Euler(m_Euler);
	}

	private float ClampAngle(float angle)
	{
		if (angle < -360f) angle += 360f;
		if (angle >  360f) angle -= 360f;
		return m_Clamp ? Mathf.Clamp(angle, m_MinMax.x, m_MinMax.y) : angle;
	}
}
