using LEGS;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MOBAExample
{
	[CreateAssetMenu(fileName = "Status Effect", menuName = "MOBA/Status Effects/Health Altering")]
	public class HealthAlterStatusEffect : TimedStatusEffect
	{
		[Tooltip("Total damage, spread out over time")]
		public float TotalDamage = 10.0f;

		[Tooltip("Category of damage to apply. Can be used for resistances & buffs")]
		public DamageTypes DamageType = DamageTypes.Physical;

		private IDamageable m_Damageable;

		public override void OnAdded(IEntity sender, IStatusEffectReceiver receiver)
		{
			base.OnAdded(sender, receiver);
			m_Damageable = receiver as IDamageable;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();

			if (m_Damageable != null)
				m_Damageable.ApplyDamage((TotalDamage / Duration) * Time.deltaTime, Sender);
		}
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(HealthAlterStatusEffect))]
	public class HealthAlterStatusEffectEditor : Editor
	{
		private HealthAlterStatusEffect m_Effect;

		private void Awake() => m_Effect = (HealthAlterStatusEffect)target;

		public override void OnInspectorGUI()
		{
			m_Effect.Duration = Mathf.Clamp(m_Effect.Duration, 0, float.MaxValue);

			base.OnInspectorGUI();

			EditorGUILayout.Space(20);

			float amountPerSecond = m_Effect.Duration != 0.0f ? (m_Effect.TotalDamage / m_Effect.Duration) : m_Effect.TotalDamage;

			if (m_Effect.TotalDamage >= 0)
				EditorGUILayout.LabelField("Damage Per Second:  " + amountPerSecond.ToString("F2"));
			else
				EditorGUILayout.LabelField("Healing Per Second: " + (-amountPerSecond).ToString("F2"));
		}
	}
#endif
}