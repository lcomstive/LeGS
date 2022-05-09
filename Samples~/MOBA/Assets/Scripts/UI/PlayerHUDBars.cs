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

	public class PlayerHUDBars : MonoBehaviour
	{
		[SerializeField] private int m_TextStyleDecimalPlaces = 2;
		[SerializeField] private HealthTextStyle m_TextStyle = HealthTextStyle.Percentage;
		[SerializeField] private float m_ValueChangeSpeed = 10.0f;

		[SerializeField, Tooltip("Entity object. Must contain at least one component with an IDamageable-inherited class")]
		private GameObject m_Entity;

		[Header("Health Bar")]
		[SerializeField] private Slider   m_HealthBar;
		[SerializeField] private TMP_Text m_HealthText;

		[Header("Mana Bar")]
		[SerializeField] private Slider   m_ManaBar;
		[SerializeField] private TMP_Text m_ManaText;
		
		[Header("Experience Bar")]
		[SerializeField] private TMP_Text m_LevelText;
		[SerializeField] private Slider   m_ExperienceBar;

		private bool m_LockExperience = false; // When experience is at max, don't move slider value
		private MOBACharacter m_Target;

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

			m_ManaBar.value = m_Target.Mana;
			m_HealthBar.value = m_Target.Health;
			m_ExperienceBar.value = m_Target.Experience.Experience;

			// Subscribe to health change event for all entities
			EventManager.Subscribe<EntityHealthChangeEventArgs>(EntityHealthChangeEventArgs.EventName, OnEntityHealthChanged, true);
			EventManager.Subscribe<CharacterChangedEventArgs>(CharacterChangedEventArgs.EventName, OnCharacterChanged, true);
		}

		private void OnEntityHealthChanged(EntityHealthChangeEventArgs args)
		{
			// Check if entity is target
			if (args.Entity == (IEntity)m_Target)
				UpdateUI();
		}

		private void OnCharacterChanged(CharacterChangedEventArgs args)
		{
			// Check if entity is target
			if (args.Entity == (IEntity)m_Target)
				UpdateUI();
		}

		private void UpdateSliderValue(Slider slider, float desiredValue)
		{
			float speed = Time.deltaTime * m_ValueChangeSpeed;
			float valueDifference = desiredValue - slider.value;
			if (Mathf.Abs(valueDifference) > speed) slider.value += Mathf.Sign(valueDifference) * speed;
		}


		private void Update()
		{
			UpdateSliderValue(m_ManaBar, m_Target.Mana);
			UpdateSliderValue(m_HealthBar, m_Target.Health);

			if(!m_LockExperience)
				UpdateSliderValue(m_ExperienceBar, m_Target.Experience.Experience);
		}

		private void UpdateSlider(Slider slider, TMP_Text text, float value, float maxValue)
		{
			slider.maxValue = maxValue;
			// slider.value = value;

			if (!text)
				return;
			switch (m_TextStyle)
			{
				default:
				case HealthTextStyle.Hidden:
					text.text = string.Empty;
					break;
				case HealthTextStyle.Percentage:
					text.text = ((value / maxValue) * 100.0f).ToString($"F{m_TextStyleDecimalPlaces}") + "%";
					break;
				case HealthTextStyle.Value:
					text.text = value.ToString($"F{m_TextStyleDecimalPlaces}") + " / " + maxValue.ToString($"F{m_TextStyleDecimalPlaces}");
					break;
			}
		}

		private void UpdateUI()
		{
			UpdateSlider(m_ManaBar, m_ManaText, m_Target.Mana, m_Target.MaxMana);
			UpdateSlider(m_HealthBar, m_HealthText, m_Target.Health, m_Target.MaxHealth);

			int currentLevel = m_Target.Experience.Level;
			float experienceThisLevel = m_Target.Experience.ExperienceForLevel(currentLevel);
			float experienceNextLevel = m_Target.Experience.ExperienceForLevel(currentLevel + 1);

			m_LevelText.text = (currentLevel + 1).ToString();
			m_ExperienceBar.minValue = experienceThisLevel;
			m_ExperienceBar.maxValue = experienceNextLevel;

			m_LockExperience = experienceNextLevel == experienceThisLevel;
			if (m_LockExperience) // Fill bar if final level
				m_ExperienceBar.value = m_ExperienceBar.maxValue = experienceNextLevel + 0.1f;
		}
	}
}