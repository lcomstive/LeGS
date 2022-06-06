using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LEGS
{
	public class Billboard : MonoBehaviour
	{
		[SerializeField, Tooltip("If not set, uses the main camera")]
		private Camera m_Camera;

		[SerializeField] private float m_RotateSpeed = 10;
		[SerializeField] private bool m_InvertRotation = false;

		private void Start()
		{
			if(!m_Camera)
				m_Camera = Camera.main;
		}

		private void Update()
		{
			Quaternion rotation = Quaternion.LookRotation(m_Camera.transform.forward * (m_InvertRotation ? -1.0f : 1.0f));
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * m_RotateSpeed);
		}
	}
}
