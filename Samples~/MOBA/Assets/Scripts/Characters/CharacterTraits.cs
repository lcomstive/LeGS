using System;
using UnityEngine;

namespace MOBAExample
{
	// Based on League of Legends champion stats
	//		https://leagueoflegends.fandom.com/wiki/Champion_statistic

	public enum CharacterTrait : byte
	{
		//// OFFENSIVE ////

		/// <summary>
		/// <see cref="DamageTypes.Magical"/> damage scaling
		/// </summary>
		AbilityPower,

		/// <summary>
		/// Negates some of the target's <see cref="Armour"/> in damage calculations
		/// </summary>
		ArmourPenetration,

		/// <summary>
		/// <see cref="DamageTypes.Physical"/> damage scaling
		/// </summary>
		AttackDamage,

		/// <summary>
		/// Rate at which character can attack
		/// </summary>
		AttackSpeed,

		/// <summary>
		/// Percentage chance of character attacks being a critical strike
		/// </summary>
		CriticalStrikeChance,

		/// <summary>
		/// Percentage damage increase of critical strikes
		/// </summary>
		CriticalStrikeDamage,

		/// <summary>
		/// Percentage of damage dealt that is applied to attacker as healing
		/// </summary>
		LifeSteal,

		/// <summary>
		/// Negates some of the target's <see cref="MagicResistance"/> in damage calculations
		/// </summary>
		MagicPenetration,

		//// DEFENSIVE ////

		/// <summary>
		/// Used in calculations to reduce damage from <see cref="DamageTypes.Physical"/> damage
		/// </summary>
		Armour,

		/// <summary>
		/// Total damage that can be taken before character dies
		/// </summary>
		Health,

		/// <summary>
		/// <see cref="Health"/> that is passively restored every second
		/// </summary>
		HealthRegen,

		/// <summary>
		/// Used in calculations to reduce damage from <see cref="DamageTypes.Magical"/> damage
		/// </summary>
		MagicResistance,

		//// UTILITY ////

		/// <summary>
		/// Speed multiplier for movement
		/// </summary>
		MovementSpeed,

		/// <summary>
		/// Reduces cooldown times of abilities
		/// </summary>
		AbilityHaste,

		/// <summary>
		/// Maximum mana available for casting abilities
		/// </summary>
		Mana,

		/// <summary>
		/// Rate at which <see cref="Mana"/> is passively restored
		/// </summary>
		ManaRegen
	}
}