using System;
using System.Collections.Generic;

namespace LEGS
{
	internal class EventQueue<T>
	{
		/// <summary>
		/// Type of event, valid types derive from <see cref="LEGEventArgs"/>
		/// </summary>
		public Type EventType { get; private set; }

		/// <summary>
		/// All active subscriptions
		/// </summary>
		private List<Action<T>> m_Subscriptions;

		public EventQueue()
		{
			EventType = typeof(T);
			m_Subscriptions = new List<Action<T>>();
		}

		/// <summary>
		/// Adds a subscriber to listeners
		/// </summary>
		public void Add(Action<T> callback) => m_Subscriptions.Add(callback);

		/// <summary>
		/// Removes a subscriber to listeners
		/// </summary>
		public void Remove(Action<T> callback) => m_Subscriptions.Remove(callback);

		/// <summary>
		/// Removes all subscriptions
		/// </summary>
		public void Clear() => m_Subscriptions.Clear();

		/// <summary>
		/// Raise this event and inform all listeners
		/// </summary>
		/// <param name="arg">Arguments to pass to listeners. Must be derived from <see cref="LEGEventArgs"/></param>
		public void Invoke(T arg)
		{
			for (int i = 0; i < m_Subscriptions.Count; i++)
				m_Subscriptions[i]?.Invoke(arg);
		}
	}
}