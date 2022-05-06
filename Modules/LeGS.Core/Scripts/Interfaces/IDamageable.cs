namespace LEGS
{
	/// <summary>
	/// Represents an <see cref="IEntity"/> that has health and can receive damage
	/// </summary>
	public interface IDamageable : IEntity
	{
		float Health { get; }
		float MaxHealth { get;}

		/// <summary>
		/// Decreases <see cref="Health"/> by <paramref name="amount"/>. Healing is negative damage.
		/// 
		/// <para><see cref="Health"/> may not be decreased by <paramref name="amount"/> exactly due to attached <see cref="IDamageModifier"/>s</para>
		/// </summary>
		void ApplyDamage(float amount, IEntity sender);

		/// <summary>
		/// Adds a modifier for altering damage amount before being applied
		/// </summary>
		/// <typeparam name="T">Modifier to create. Must derive from <see cref="IDamageModifier"/></typeparam>
		/// <returns>Created instance of damage modifier</returns>
		T AddDamageModifier<T>() where T : IDamageModifier, new();

		/// <summary>
		/// Removes damage modifier
		/// </summary>
		void RemoveDamageModifier(IDamageModifier modifier);
	}
}