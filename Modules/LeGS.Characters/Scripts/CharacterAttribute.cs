using System;
using UnityEngine;

namespace LEGS.Characters
{
	/// <summary>
	/// An attribute on a character with modifiers.
	///
	/// <para>Can be used or instance as character's health, attack speed, or strength values.</para>
	/// <para>Final value is calculated as <code>(BaseValue + ModifierAdd) * ModifierMultiply</code></para>
	/// </summary>
	[Serializable]
	public class Attribute
	{
		/// <summary>
		/// Display name
		/// </summary>
		[field: SerializeField] public string Name { get; set; }

		[SerializeField, Tooltip("Base value for calculations")]
		private float m_BaseValue = 10.0f;

		/// <summary>
		/// Base value for calculations
		/// </summary>
		public float BaseValue
		{
			get => m_BaseValue;
			set
			{
				if(m_BaseValue == value)
					return;
				m_IsDirty = true;
				m_BaseValue = value;
				BaseValueChanged?.Invoke(this);
				CalculateCurrentValue();
			}
		}

		[SerializeField, Tooltip("Value to add to base value, before multiplication")]
		private float m_ModifierAdd = 0.0f;

		/// <summary>
		/// Value to add to base value, before multiplication
		/// </summary>
		public float ModifierAdd
		{
			get => m_ModifierAdd;
			set
			{
				if(m_ModifierAdd == value)
					return;
				m_IsDirty = true;
				m_ModifierAdd = value;
				FireModifierChangedEvent();
				CalculateCurrentValue();
			}
		}

		[SerializeField, Tooltip("Value to multiply by (BaseValue + ModifierAdd)")]
		private float m_ModifierMultiply = 1.0f;

		/// <summary>
		/// Value to multiply by <code>BaseValue + ModifierAdd</code>
		/// </summary>
		public float ModifierMultiplier
		{
			get => m_ModifierMultiply;
			set
			{
				if(m_ModifierMultiply == value)
					return;
				m_IsDirty = true;
				m_ModifierMultiply = value;
				FireModifierChangedEvent();
				CalculateCurrentValue();
			}
		}

		private float m_CurrentValue = 0.0f;

		/// <summary>
		/// Final calculated value. This is equal to
		/// <code>(BaseValue + ModifierAdd) * ModifierMultiply</code>
		/// </summary>
		public float CurrentValue
		{
			get
			{
				if(m_IsDirty)
					CalculateCurrentValue();
				return m_CurrentValue;
			}
		}

		/// <summary>
		/// True if <see cref="CalculateCurrentValue"/> needs to be called
		/// when next appropriate
		/// </summary>
		protected bool m_IsDirty = true;

		public Attribute(string name = "Attribute", float baseValue = 0.0f)
		{
			Name = name;
			m_BaseValue = baseValue;
		}

		/// <summary>
		/// Sets <see cref="CurrentValue"/> using <see cref="BaseValue"/>,
		/// <see cref="ModifierAdd"/> and <see cref="ModifierMultiplier"/>.
		///
		/// <para>Sets <see cref="m_IsDirty"/> to false.</para>
		/// </summary>
		protected virtual void CalculateCurrentValue()
		{
			if(!m_IsDirty)
				return; // No changes need to be made

			m_IsDirty = false;

			// Calculate new current value
			m_CurrentValue = (BaseValue + ModifierAdd) * m_ModifierMultiply;

			// Inform listeners
			CurrentValueChanged?.Invoke(this);
		}

		/// <summary>
		/// Informs listeners that a modifier has been changed
		/// </summary>
		protected void FireModifierChangedEvent() => ModifierChanged?.Invoke(this);

		public delegate void OnAttributeChange(Attribute attribute);

		/// <summary>
		/// Informs when <see cref="BaseValue"/> has changed
		/// </summary>
		public event OnAttributeChange BaseValueChanged;

		/// <summary>
		/// Informs when either <see cref="ModifierAdd"/> or <see cref="ModifierMultiplier"/> has been changed
		/// </summary>
		public event OnAttributeChange ModifierChanged;

		/// <summary>
		/// Informs when the <see cref="CurrentValue"/> has been changed
		/// </summary>
		public event OnAttributeChange CurrentValueChanged;
	}
}
