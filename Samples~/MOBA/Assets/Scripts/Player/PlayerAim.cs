using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MOBAExample
{
	public class PlayerAim : MonoBehaviour
	{
		[SerializeField] private float m_MaxRaycastDistance = 10.0f;
		[SerializeField] private LayerMask m_RayLayers;
		[field: SerializeField] public Transform AimOrigin { get; private set; }

		public GameObject AimTarget { get; private set; }

		public Vector3 AimDirection { get; private set; }

		/// <summary>
		/// How far forward from the transform is the aim direction targeting
		/// </summary>
		public float AimDistance { get; private set; }

		private void Update()
		{
			Vector3 target = Vector3.zero;
			Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
			if (Physics.Raycast(ray, out RaycastHit hit, m_MaxRaycastDistance, m_RayLayers, QueryTriggerInteraction.Ignore))
			{
				target = hit.point;
				AimDistance = hit.distance;
			}
			else
			{
				target = ray.origin + ray.direction * m_MaxRaycastDistance;
				AimDistance = m_MaxRaycastDistance;
			}

			AimDirection = (target - AimOrigin.position).normalized;
		}

		private void OnDrawGizmos()
		{
			if(!Application.isPlaying)
				return;

			Gizmos.color = Color.blue;

			Vector3 start = AimOrigin.position;
			Vector3 end = start + AimDirection * AimDistance;
			Gizmos.DrawLine(start, end);
		}
	}
}