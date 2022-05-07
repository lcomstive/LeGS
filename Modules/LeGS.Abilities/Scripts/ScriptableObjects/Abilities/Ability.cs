using UnityEngine;

namespace LEGS.Abilities
{
	/// <summary>
	/// <see cref="ScriptableObject"/> version of an <see cref="IAbility"/> for convenience
	/// </summary>
	public abstract class Ability : ScriptableObject, IAbility
	{
		/// <summary>
		/// Name for debugging and/or UI
		/// </summary>
		[field: SerializeField] public string DisplayName { get; private set; } = "Ability";

		/// <summary>
		/// Description to show to players
		/// </summary>
		[field: SerializeField] public string Description { get; private set; } = "Ability description";

		/// <summary>
		/// Minimum time, in seconds, between activations
		/// </summary>
		[Tooltip("Minimum time, in seconds, between activations")]
		[field: SerializeField] public float Cooldown { get; private set; } = 2.5f;

		/// <summary>
		/// Activates this ability on the desired <see cref="IEntity"/>,
		/// with reference to it's associated <see cref="GameObject"/> for convenience
		/// </summary>
		public abstract void Activate(IEntity caster, GameObject gameObject);
	}
}
