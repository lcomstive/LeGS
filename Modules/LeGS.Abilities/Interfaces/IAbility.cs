using UnityEngine;

namespace LEGS.Abilities
{
    /// <summary>
	/// An activateable script that modifies a character or the world in some form
	/// </summary>
    public interface IAbility
    {
        /// <summary>
        /// Minimum time, in seconds, between activations
        /// </summary>
        float Cooldown { get; }

        /// <summary>
        /// Name of ability
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Activate and use this ablity
        /// </summary>
        /// <param name="caster"><see cref="IEntity"/> that activated this ability</param>
        /// <param name="gameObject"><see cref="GameObject"/> containing <paramref name="caster"/></param>
        void Activate(IEntity caster, GameObject gameObject);
    }
}
