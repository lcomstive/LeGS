namespace LEGS
{
	/// <summary>
	/// Event for changing of an <see cref="IStatusEffect"/> in an <see cref="IEntity"/>
	/// </summary>
	public class StatusEffectChangeArgs : LEGEventArgs
	{
		/// <summary>
		/// Name of event in <see cref="EventManager"/>
		/// </summary>
		public static string EventName => "EntityStatusEffectChange";

		/// <summary>
		/// True if <see cref="Effect"/> was added during this event
		/// </summary>
		public bool Added { get; private set; }

		/// <summary>
		/// The <see cref="IStatusEffect"/> being altered
		/// </summary>
		public IStatusEffect Effect { get; private set; }

		/// <summary>
		/// Receiver of the status effect change
		/// </summary>
		public IStatusEffectReceiver Receiver { get; private set; }

		/// <param name="sender">The <see cref="IEntity"/> that caused the effect change</param>
		/// <param name="receiver"><see cref="IStatusEffectReceiver"/> getting the status effect change</param>
		/// <param name="effect">An <see cref="IStatusEffect"/> being altered</param>
		/// <param name="added">True if <see cref="Effect"/> is being added to <paramref name="receiver"/></param>
		public StatusEffectChangeArgs(IEntity sender, IStatusEffectReceiver receiver, IStatusEffect effect, bool added = true) : base(sender)
		{
			Added = added;
			Effect = effect;
			Receiver = receiver;
		}
	}
}
