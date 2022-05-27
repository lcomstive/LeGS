using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LEGS
{
	/// <summary>
	/// Spawns a prefab during this GameObject's destruction
	/// </summary>
    public class SpawnOnDestroy : MonoBehaviour
    {
		[SerializeField, Tooltip("Effects to spawn in-place on destruction")]
		private GameObject m_Effects;

		[SerializeField] private bool m_InheritRotation = false;
		[SerializeField] private bool m_InheritParent = false;
		[SerializeField] private bool m_InheritScale = false;

		private void OnDestroy()
		{
			if (!m_Effects)
				return;
			Quaternion rotation = m_InheritRotation ? transform.rotation : m_Effects.transform.rotation;
			Transform parent = m_InheritParent ? transform.parent : null;
			GameObject created = Instantiate(m_Effects, transform.position, rotation, parent);

			if(m_InheritScale)
				created.transform.localScale = transform.localScale;
		}
	}
}
