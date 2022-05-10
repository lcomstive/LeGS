using UnityEngine;

namespace LEGS.Characters
{
	/// <summary>
	/// A <see cref="TimedStatusEffect"/> that modifies an <see cref="Attribute"/>
	/// for it's duration.
	/// </summary>
	[CreateAssetMenu(menuName = "LeGS/Status Effects/Modify Attribute")]
	public class StatusEffectModifyAttribute : TimedStatusEffect
	{
		/// <summary>
		/// Case-sensitive name of attribute to modify
		/// </summary>
		[SerializeField, Tooltip("Case-sensitive name of attribute to modify")]
		private string m_AttributeName;

		/// <summary>
		/// Value to add to <see cref="Attribute.ModifierAdd"/>
		/// </summary>
		[SerializeField, Tooltip("Value to add to modifier")]
		private float m_AddModifier;

		/// <summary>
		/// Value to add to <see cref="Attribute.ModifierMultiplier"/>.
		/// Final modifier value is <code>1.0 + <see cref="Attribute.ModifierMultiplier"/></code>
		/// </summary>
		[SerializeField, Tooltip("Value to add to multiplier modifier. Final multiplier value is (1.0+MultiplyModifier)")]
		private float m_MultiplyModifier;

		private Attribute m_Attribute;

		public override void OnAdded(IEntity sender, IStatusEffectReceiver receiver)
		{
			base.OnAdded(sender, receiver);

			IAttributeHolder attributeHolder = Receiver as IAttributeHolder;
			if(attributeHolder == null)
				return; // Not an attribute holder

			m_Attribute = attributeHolder.GetAttribute(m_AttributeName);
			if(m_Attribute == null)
				return; // Attribute not on receiver

			// Add modifiers
			m_Attribute.ModifierAdd			+= m_AddModifier;
			m_Attribute.ModifierMultiplier	+= m_MultiplyModifier;
		}

		public override void OnRemoved()
		{
			base.OnRemoved();

			if(m_Attribute == null)
				return; // Attribute was not found on receiver

			// Remove modifiers
			m_Attribute.ModifierAdd			-= m_AddModifier;
			m_Attribute.ModifierMultiplier	-= m_MultiplyModifier;
		}
	}
}
