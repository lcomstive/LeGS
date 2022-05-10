using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LEGS;

namespace MOBAExample
{
	public class DamagePopup : MonoBehaviour
	{
		[SerializeField] private AnimationCurve m_ScaleCurve;
		[SerializeField] private Vector3 m_MovementPerSecond;
		[SerializeField] private bool m_DestroyAfterScaleCurve = false;

		private float m_Time = 0;
		private float m_MaxTime = 0;

		private void Start() => m_MaxTime = m_ScaleCurve.keys[^1].time;

		private void Update()
		{
			if(m_Time > m_MaxTime)
				return; // No need to update

			transform.localScale = Vector3.one * m_ScaleCurve.Evaluate(m_Time);
			transform.position += m_MovementPerSecond * Time.deltaTime;

			m_Time += Time.deltaTime;

			if(m_Time >= m_MaxTime && m_DestroyAfterScaleCurve)
				Destroy(gameObject);
		}
	}
}