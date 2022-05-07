using UnityEngine;

namespace LEGS.Abilities
{
	/// <summary>
	/// Intended to be added to GameObjects that have been cast from abilities.
	/// Helpful for projectile-based abilities and identifying who cast them.
	/// </summary>
	public class AbilityInfo : MonoBehaviour
	{
		[field: SerializeField] public IEntity Caster { get; private set; }
		[field: SerializeField] public IAbility Ability { get; private set; }

		public void Initialise(IEntity caster, IAbility ability)
		{
			Caster = caster;
			Ability = ability;
		}
	}
}
