using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LEGS
{
	public class DestroyOnCollision : MonoBehaviour
	{
		[SerializeField, Tooltip("If not empty, only destroys if collision object contains tag")]
		private string[] m_CollisionTags;

		private void OnCollisionEnter(Collision collision)
		{
			if(m_CollisionTags.Length == 0 || collision.CompareTags(m_CollisionTags))
				Destroy(gameObject);
		}
	}
}
