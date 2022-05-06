using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LEGS.Abilities
{
	public class AbilityInfo : MonoBehaviour, ICastAbility
	{
		[field: SerializeField] public IEntity Caster { get; private set; }
		[field: SerializeField] public IAbility Ability { get; private set; }
		public string DisplayName => Ability.DisplayName;

		public void Initialise(IEntity caster, IAbility ability)
		{
			Caster = caster;
			Ability = ability;
		}
	}
}
