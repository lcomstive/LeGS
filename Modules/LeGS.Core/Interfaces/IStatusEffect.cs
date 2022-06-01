namespace LEGS
{
	/// <summary>
	/// An object that can be added and removed from an <see cref="IStatusEffectReceiver"/>
	/// </summary>
	public interface IStatusEffect
	{
		/// <summary>
		/// Readable name suitable for debugging and/or game UI
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Maximum amount of this status effect to be applied to an entity.
		/// Value of 0 = infinite.
		/// </summary>
		uint MaxStackSize { get; }

		/// <summary>
		/// This status effect has been added to <paramref name="receiver"/>, from <paramref name="sender"/>
		/// </summary>
		void OnAdded(IEntity sender, IStatusEffectReceiver receiver);

		/// <summary>
		/// This effect has been removed from the receiving entity
		/// </summary>
		void OnRemoved();
	}
}