namespace LEGS.Characters
{
	/// <summary>
	/// <see cref="LEGEventArgs"/> when an <see cref="IAttributeHolder"/>'s
	/// attribute gets added, removed or modified
	/// </summary>
	public class AttributeEventArgs : LEGEventArgs
	{
		/// <summary>
		/// Target <see cref="Attribute"/>
		/// </summary>
		public Attribute Attribute { get; private set; }

		/// <param name="entity"><see cref="IEntity"/> causing change to <paramref name="attribute"/></param>
		public AttributeEventArgs(IEntity entity, Attribute attribute) : base(entity) => Attribute = attribute;
	}
}