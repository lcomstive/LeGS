namespace LEGS
{
	/// <summary>
	/// An <see cref="IEntity"/> that can have <see cref="IStatusEffect"/>s added and removed
	/// </summary>
	public interface IStatusEffectReceiver : IEntity
	{
		/// <summary>
		/// Adds an <see cref="IStatusEffect"/> to this <see cref="IEntity"/>, from <paramref name="sender"/>
		/// </summary>
		public void AddStatusEffect(IStatusEffect effect, IEntity sender);

		/// <summary>
		/// Removes an <see cref="IStatusEffect"/> from this <see cref="IEntity"/>
		/// </summary>
		public void RemoveStatusEffect(IStatusEffect effect);

		/// <summary>
		/// Retrieves an <see cref="IStatusEffect"/> from this <see cref="IEntity"/>
		/// </summary>
		/// <typeparam name="T">Type derived from <see cref="IStatusEffect"/> to fetch</typeparam>
		/// <param name="effectName">Related <see cref="IStatusEffect.Name"/></param>
		/// <returns>The found <see cref="IStatusEffect"/> if exists with same <see cref="IStatusEffect.Name"/>, otherwise null</returns>
		public T GetStatusEffect<T>(string effectName) where T : class, IStatusEffect; // `class` so a null-value can be returned
	}
}
