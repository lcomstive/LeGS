namespace LEGS
{
	/// <summary>
	/// Represents an object that has health and can receive damage
	/// </summary>
	public interface IDamageable
	{
		/// <summary>
		/// How much damage an object can take before dying
		/// </summary>
		float Health	{ get; }

		float MaxHealth { get;}

		/// <summary>
		/// Decreases <see cref="Health"/> by <paramref name="amount"/>. Healing is negative damage.
		/// 
		/// <para><see cref="Health"/> may not be decreased by <paramref name="amount"/> exactly due to attached <see cref="IDamageModifier"/>s</para>
		/// </summary>
		void ApplyDamage(float amount, IEntity sender);
	}
}