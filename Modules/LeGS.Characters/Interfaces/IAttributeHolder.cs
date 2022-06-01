namespace LEGS.Characters
{
    /// <summary>
	/// Object that can have <see cref="Attribute"/>s added & removed
	/// </summary>
    public interface IAttributeHolder
    {
        /// <summary>
		/// Adds <paramref name="attribute"/> to this object
		/// </summary>
        void AddAttribute(Attribute attribute);

        /// <summary>
		/// Removes <paramref name="attribute"/> to this object
		/// </summary>
        void RemoveAttribute(Attribute attribute);

        /// <summary>
		/// Retrieves an <see cref="Attribute"/> from this object
		/// with matching case-sensitive <see cref="Attribute.Name"/>
		/// </summary>
        Attribute GetAttribute(string name);
    }
}
