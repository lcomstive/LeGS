using UnityEngine;

namespace LEGS.Tutorial
{
	[RequireComponent(typeof(CharacterController))]
	public class PlayerMovement : MonoBehaviour
	{
		[SerializeField] private float m_WalkSpeed		= 10.0f;
		[SerializeField] private float m_SprintSpeed	= 22.0f;
		[SerializeField] private float m_JumpForce		= 7.5f;
		[SerializeField] private float m_AirMoveSpeed	= 5.0f;

		[Header("Gravity")]
		public bool UseGravity		 = true;
		public float GravityModifier = 1.0f;

		private bool m_IsGrounded	= false;
		private Vector3 m_Velocity	= Vector3.zero;
		private CharacterController m_Controller;

		// Get the attached CharacterController
		private void Start() => m_Controller = GetComponent<CharacterController>();

		private void Update()
		{
			// Movement input
			Vector3 movement = new Vector3(
				-(Input.GetKey(KeyCode.A) ? 1 : 0) + (Input.GetKey(KeyCode.D) ? 1 : 0),
				0, // Y axis
				-(Input.GetKey(KeyCode.S) ? 1 : 0) + (Input.GetKey(KeyCode.W) ? 1 : 0)
			);
			// Make movement relative to transform forward
			movement = transform.TransformDirection(movement);

			// Calculate velocity based on grounded state
			if (m_IsGrounded)
			{
				// Multiply by appropriate speed (walking vs sprinting)
				movement *= Input.GetKey(KeyCode.LeftShift) ? m_SprintSpeed : m_WalkSpeed;

				m_Velocity = Vector3.zero;
				if (Input.GetKeyDown(KeyCode.Space))
				{
					m_Velocity = movement + Vector3.up * m_JumpForce; // Apply jump force, store movement to retain momentum in-air
					m_IsGrounded = false;
				}
			}
			else
			{
				movement *= m_AirMoveSpeed;

				if (UseGravity) // Apply gravity
					m_Velocity += Physics.gravity * Time.deltaTime * GravityModifier;
			}

			// Move the controller
			m_Controller.Move((movement + m_Velocity) * Time.deltaTime);
			m_IsGrounded = m_IsGrounded || m_Controller.isGrounded;
		}
	}
}