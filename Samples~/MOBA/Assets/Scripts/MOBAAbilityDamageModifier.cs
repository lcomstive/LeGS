using LEGS;
using UnityEngine;
using LEGS.Abilities;

namespace MOBAExample
{
	/*
	public class MOBAAbilityDamageModifier
	{
		public float CalculateDamage(float input, IEntity sender, IDamageable receiver)
		{
			AbilityInfo abilityInfo = sender as AbilityInfo;
			if(abilityInfo == null) return input;

			MOBAAbility ability = (MOBAAbility)abilityInfo.Ability;
			MOBACharacter receiverCharacter = (MOBACharacter)receiver;
			MOBACharacter senderCharacter = (MOBACharacter)abilityInfo.Caster;

			// Check resistance to attack damage type
			float resistance = 0;
			if (ability.DamageType == DamageTypes.Physical)
				resistance = Mathf.Max(0.0f, receiverCharacter[CharacterTrait.Armour] - senderCharacter[CharacterTrait.ArmourPenetration]);
			else if (ability.DamageType == DamageTypes.Magical)
				resistance = Mathf.Max(0.0f, receiverCharacter[CharacterTrait.MagicResistance] - senderCharacter[CharacterTrait.MagicPenetration]);

			// Apply resistance
			input *= 1.0f - (resistance / (resistance + 100.0f));

			// Add status effect to this character if one is attached
			IStatusEffectReceiver statusEffectReceiver = (IStatusEffectReceiver)receiver;
			if (ability.ReceiverEffect != null && statusEffectReceiver != null)
				statusEffectReceiver.AddStatusEffect(ability.ReceiverEffect, abilityInfo.Caster);

			return input;
		}
	}
	*/
}