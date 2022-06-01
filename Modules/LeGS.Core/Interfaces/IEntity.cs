namespace LEGS
{
	/// <summary>
	/// Represents a LeGS object. This is the base type for most LeGS types.
	/// </summary>
	public interface IEntity
	{
		/// <summary>
		/// Readable name suitable for debugging and/or game UI
		/// </summary>
		public string DisplayName { get; }
	}
}