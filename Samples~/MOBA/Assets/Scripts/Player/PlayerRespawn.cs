using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(PlayerController))]
public class PlayerRespawn : MonoBehaviour
{
	[SerializeField, Tooltip("Offset of player to spawn point ground")]
	private Vector3 m_SpawnOffset = new Vector3(0, 1, 0);

	[SerializeField] private bool m_ResetVelocity = true;

	[SerializeField, Tooltip("How often to check for ground, in seconds")]
	private float m_GroundCheckInterval = 0.2f;

	[SerializeField, Tooltip("How many previous ground positions to hold in memory")]
	[Range(1, 6)]
	private uint m_GroundCheckAmount = 3;

	[SerializeField] private float m_MaxRayLength = 2.5f;

	[Header("Audio")]
	[SerializeField] private AudioClip[] m_Sounds;
	[SerializeField] private AudioSource m_AudioSource;

	private Coroutine m_CheckCoroutine;
	private Vector3 m_OriginalSpawnPoint;
    private PlayerController m_PlayerController;
	private Queue<Vector3> m_LastGroundedPoint = new Queue<Vector3>();

	private void Start()
	{
		m_OriginalSpawnPoint = transform.position;

		m_PlayerController = GetComponent<PlayerController>();
		m_CheckCoroutine = StartCoroutine(CheckForGround());
	}

	private void OnDestroy() => StopCoroutine(m_CheckCoroutine);

	private IEnumerator CheckForGround()
	{
		if(m_PlayerController.IsGrounded)
			m_LastGroundedPoint.Enqueue(transform.position); // Append element

		if(m_LastGroundedPoint.Count > m_GroundCheckAmount)
			m_LastGroundedPoint.Dequeue(); // Remove first element

		yield return new WaitForSeconds(m_GroundCheckInterval);

		m_CheckCoroutine = StartCoroutine(CheckForGround());
	}

	public void Respawn()
	{
		bool foundGround = false;
		float rayLength = m_MaxRayLength / 2.0f;
		Vector3[] groundPoints = m_LastGroundedPoint.ToArray();
		for(int i = groundPoints.Length - 1; i >= 0; i--)
		{
			if (!Physics.Raycast(groundPoints[i] + Vector3.up * rayLength, Vector3.down, out RaycastHit hit, rayLength))
				continue;

			transform.position = hit.point + m_SpawnOffset;
			foundGround = true;
			break;
		}

		if(!foundGround)
			transform.position = m_OriginalSpawnPoint;

		if (m_AudioSource)
			m_AudioSource.PlayOneShot(m_Sounds[Random.Range(0, m_Sounds.Length)]);

		if (m_ResetVelocity && TryGetComponent(out Rigidbody rb))
			rb.velocity = Vector3.zero;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		foreach(Vector3 position in m_LastGroundedPoint)
			Gizmos.DrawWireCube(position, new Vector3(.05f, .05f, .05f));
	}
}

#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(PlayerRespawn))]
public class PlayerRespawnEditor : Editor
{
	public override void OnInspectorGUI()
	{
		EditorStyles.label.wordWrap = true;
		EditorGUILayout.LabelField("This component checks if the player is grounded at a timed interval, " +
									"storing the ground location in a list.\nWhen the Respawn method is called " +
									"this transform is placed at the last ground point with a raycast hit + offset");
		EditorGUILayout.Space();
		base.OnInspectorGUI();
	}
}
#endif