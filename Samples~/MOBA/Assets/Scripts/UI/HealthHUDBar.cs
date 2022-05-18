using LEGS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MOBAExample
{
	public enum HealthTextStyle
	{
		Hidden,
		Percentage,
		Value
	}

	public class HealthHUDBar : MonoBehaviour
	{
		[SerializeField] private int m_TextStyleDecimalPlaces = 2;
		[SerializeField] private HealthTextStyle m_TextStyle = HealthTextStyle.Percentage;
		[SerializeField] private float m_ValueChangeSpeed = 10.0f;

		[SerializeField, Tooltip("Entity object. Must contain at least one component with an IDamageable-inherited class")]
		private GameObject m_TargetGO;

		[Header("Health Bar")]
		[SerializeField] private Slider   m_HealthBar;
		[SerializeField] private TMP_Text m_HealthText;

		private IEntity m_Entity;
		private IDamageable m_Target;

		private void Start()
		{
			if(!m_TargetGO)
				m_TargetGO = gameObject;

			if(!m_TargetGO.TryGetComponent(out m_Target))
			{
				Debug.LogWarning($"No IDamageable script found on '{m_TargetGO.name}'");
				Destroy(this);
				return;
			}
			
			if (!m_TargetGO.TryGetComponent(out m_Entity))
			{
				Debug.LogWarning($"No IEntity script found on '{m_TargetGO.name}'");
				Destroy(this);
				return;
			}

			// Set initial values
			m_HealthBar.maxValue = m_Target.MaxHealth;
			m_HealthBar.value = m_Target.MaxHealth;
			UpdateUI();

			// Subscribe to health change event for all entities
			EventManager.Subscribe<EntityHealthChangeEventArgs>(EntityHealthChangeEventArgs.EventName, OnEntityHealthChanged, true);
		}

		private void OnEntityHealthChanged(EntityHealthChangeEventArgs args)
		{
			// Check if entity is target
			if (args.Entity == m_Entity)
				UpdateUI();
		}

		private void Update()
		{
			float speed = Time.deltaTime * m_ValueChangeSpeed;
			float valueDifference = m_Target.Health - m_HealthBar.value;
			if (Mathf.Abs(valueDifference) > speed) m_HealthBar.value += Mathf.Sign(valueDifference) * speed;
		}

		private void UpdateUI()
		{
			m_HealthBar.maxValue = m_Target.MaxHealth;

			if (!m_HealthText)
				return;
			switch (m_TextStyle)
			{
				default:
				case HealthTextStyle.Hidden:
					m_HealthText.text = string.Empty;
					break;
				case HealthTextStyle.Percentage:
					m_HealthText.text = ((m_Target.Health / m_Target.MaxHealth) * 100.0f).ToString($"F{m_TextStyleDecimalPlaces}") + "%";
					break;
				case HealthTextStyle.Value:
					m_HealthText.text = m_Target.Health.ToString($"F{m_TextStyleDecimalPlaces}") + " / " + m_Target.MaxHealth.ToString($"F{m_TextStyleDecimalPlaces}");
					break;
			}
		}
	}
}