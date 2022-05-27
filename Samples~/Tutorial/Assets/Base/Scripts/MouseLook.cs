using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LEGS.Tutorial
{
	public class MouseLook : MonoBehaviour
	{
		[SerializeField] private float m_Sensitivity = 1;
		[SerializeField, Tooltip("Rotates based on mouse X axis")] private bool m_RotateX = true;
		[SerializeField, Tooltip("Rotates based on mouse Y axis")] private bool m_RotateY = false;

		private Vector2 m_Rotation = Vector2.zero;

		private void Update()
		{
			if(m_RotateX) m_Rotation.y += Input.GetAxis("Mouse X");
			if(m_RotateY) m_Rotation.x -= Input.GetAxis("Mouse Y");

			transform.localEulerAngles = m_Rotation * m_Sensitivity;
		}
	}
}