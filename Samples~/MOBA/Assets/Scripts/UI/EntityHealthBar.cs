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

	public class EntityHealthBar : MonoBehaviour
	{
		[SerializeField] private Slider m_HealthBar;
		[SerializeField] private TMP_Text m_HealthText;
		[SerializeField] private HealthTextStyle m_HealthTextStyle = HealthTextStyle.Percentage;
		[SerializeField] private int m_HealthStyleDecimalPlaces = 2;

		[SerializeField, Tooltip("Entity object. Must contain at least one component with an IDamageable-inherited class")]
		private GameObject m_Entity;

		private IDamageable m_Target;

		private void Start()
		{
			if (!m_Entity || !m_Entity.TryGetComponent(out m_Target))
			{
				Debug.LogWarning($"No IDamageable script found on '{m_Entity.name}'");
				Destroy(this);
				return;
			}

			// Set initial values
			UpdateUI();

			// Subscribe to health change event for all entities
			EventManager.Subscribe<EntityHealthChangeEventArgs>(EntityHealthChangeEventArgs.EventName, OnEntityHealthChanged, true);
		}

		private void OnEntityHealthChanged(EntityHealthChangeEventArgs args)
		{
			// Check if entity is target
			if (args.Entity == m_Target)
				UpdateUI();
		}

		private void UpdateUI()
		{
			m_HealthBar.maxValue = m_Target.MaxHealth;
			m_HealthBar.value = m_Target.Health;

			if (!m_HealthText)
				return;
			switch (m_HealthTextStyle)
			{
				default:
				case HealthTextStyle.Hidden:
					m_HealthText.text = string.Empty;
					break;
				case HealthTextStyle.Percentage:
					m_HealthText.text = ((m_Target.Health / m_Target.MaxHealth) * 100.0f).ToString($"F{m_HealthStyleDecimalPlaces}") + "%";
					break;
				case HealthTextStyle.Value:
					m_HealthText.text = m_Target.Health.ToString($"F{m_HealthStyleDecimalPlaces}") + " / " + m_Target.MaxHealth.ToString($"F{m_HealthStyleDecimalPlaces}");
					break;
			}
		}
	}
}