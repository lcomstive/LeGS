namespace LEGS
{
	public class LEGEventArgs
	{
		/// <summary>
		/// Identifier of the published event. Usable with <see cref="EventManager"/>
		/// </summary>
		public ushort EventID { get; internal set; }

		/// <summary>
		/// <see cref="IEntity"/> that sent this event
		/// </summary>
		public IEntity Entity { get; protected set; }
	}
}