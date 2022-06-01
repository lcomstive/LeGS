using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LEGS
{
	/// <summary>
	/// <see cref="TimedStatusEffect"/> that applies damage over time to <see cref="StatusEffect.Receiver"/>
	/// </summary>
	[CreateAssetMenu(fileName = "Damage Over Time", menuName = "LeGS/Status Effects/Damage Over Time")]
	public class DamageOverTimeStatusEffect : TimedStatusEffect
	{
		/// <summary>
		/// Total damage to apply, spread out over <see cref="TimedStatusEffect.Duration"/>
		/// </summary>
		[Tooltip("Total damage to apply, spread out over duration")]
		public float TotalDamage = 10.0f;

		private IDamageable m_Damageable;

		public override void OnAdded(IEntity sender, IStatusEffectReceiver receiver)
		{
			base.OnAdded(sender, receiver);
			m_Damageable = receiver as IDamageable;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();

			// Apply damage to receiver
			if(m_Damageable != null)
				m_Damageable.ApplyDamage((TotalDamage / Duration) * Time.deltaTime, Sender);
		}
	}

#if UNITY_EDITOR
	/// <summary>
	/// Custom editor for <see cref="DamageOverTimeStatusEffect"/>,
	/// to display calculated damage per second below standard inspector
	/// </summary>
	[CustomEditor(typeof(DamageOverTimeStatusEffect))]
	public class DamageOverTimeStatusEffectEditor : Editor
	{
		private DamageOverTimeStatusEffect m_Effect;

		private void Awake() => m_Effect = (DamageOverTimeStatusEffect)target;

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUILayout.Space(20);

			float amountPerSecond = m_Effect.Duration != 0.0f ? (m_Effect.TotalDamage / m_Effect.Duration) : m_Effect.TotalDamage;

			if(m_Effect.TotalDamage >= 0)
				EditorGUILayout.LabelField("Damage Per Second:  " + amountPerSecond.ToString("F2"));
			else
				EditorGUILayout.LabelField("Healing Per Second: " + (-amountPerSecond).ToString("F2"));
		}
	}
#endif
}