using UnityEngine;

namespace LEGS.Abilities
{
	public interface IAbilityTargetSelector
	{
		IEntity GetTarget(GameObject gameObject, IAbility ability, IEntity caster);
	}
}
