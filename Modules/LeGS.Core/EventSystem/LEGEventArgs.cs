namespace LEGS
{
	/// <summary>
	/// LeGS event arguments base, for all events from <see cref="EventManager"/>
	/// </summary>
	public class LEGEventArgs
	{
		/// <summary>
		/// Identifier of the published event. Usable with <see cref="EventManager"/>
		/// </summary>
		public ushort EventID { get; internal set; }

		/// <summary>
		/// <see cref="IEntity"/> that sent this event
		/// </summary>
		public IEntity Entity { get; private set; }

		public LEGEventArgs(IEntity sender) => Entity = sender;
	}
}