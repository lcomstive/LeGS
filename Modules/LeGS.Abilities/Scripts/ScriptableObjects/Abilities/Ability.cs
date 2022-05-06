using UnityEngine;

namespace LEGS.Abilities
{
	public abstract class Ability : ScriptableObject, IAbility
	{
		[field: SerializeField] public string DisplayName { get; private set; } = "Ability";
		[field: SerializeField] public string Description { get; private set; } = "Ability description";
		[Tooltip("Minimum time, in seconds, between activations")]
		[field: SerializeField] public float Cooldown { get; private set; } = 2.5f;

		public abstract void Activate(IEntity caster, GameObject gameObject);
	}
}
