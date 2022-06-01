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

		private void Start()
		{
			if(!m_Camera)
				m_Camera = Camera.main;
		}

		private void Update()
		{
			Quaternion rotation = Quaternion.LookRotation(-m_Camera.transform.forward);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * m_RotateSpeed);
		}
	}
}
