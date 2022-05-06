using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LEGS
{
	/// <summary>
	/// Event for changing of a <see cref="IStatusEffect"/> in an <see cref="IEntity"/>
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
		/// The <see cref="IEntity"/> that caused the effect change
		/// </summary>
		public IEntity EffectSender { get; private set; }

		/// <summary>
		/// The <see cref="IStatusEffect"/> being altered
		/// </summary>
		public IStatusEffect Effect { get; private set; }

		/// <param name="sender">The <see cref="IEntity"/> that caused the effect change</param>
		/// <param name="receiver"><see cref="IEntity"/> receiving the status effect change. Preferably this is also an <see cref="IStatusEffectReceiver"/></param>
		/// <param name="effect">An <see cref="IStatusEffect"/> being altered</param>
		/// <param name="added">True if <see cref="Effect"/> is being added to <paramref name="receiver"/></param>
		public StatusEffectChangeArgs(IEntity sender, IEntity receiver, IStatusEffect effect, bool added = true)
		{
			Added = added;
			Effect = effect;
			Entity = receiver;
			EffectSender = sender;
		}
	}
}
