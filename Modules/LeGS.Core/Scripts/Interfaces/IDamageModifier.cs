namespace LEGS
{
	/// <summary>
	/// Alters incoming damage to an <see cref="IDamageable"/>
	/// </summary>
	public interface IDamageModifier
	{
		/// <summary>
		/// Calculate damage being applied to <paramref name="receiver"/>.
		/// 
		/// <para>The final damage applied to <paramref name="receiver"/> is the accumulation of multiple <see cref="IDamageModifier"/>s that may be attached</para>
		/// </summary>
		/// <param name="input">Incoming damage</param>
		/// <param name="sender"><see cref="IEntity"/> that sent incoming damage</param>
		/// <returns>Damage to apply to <paramref name="receiver"/></returns>
		float CalculateDamage(float input, IEntity sender, IDamageable receiver);
	}
}