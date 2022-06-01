using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace LEGS
{
	public static class EventManager
	{
		/// <summary>
		/// Event queues, mapped by ID of event
		/// 
		/// <para>Key: ID of event </para>
		/// <para>Value: <see cref="EventQueue{T}"/></para>
		/// </summary>
		private static Dictionary<ushort, object> m_Queues = new Dictionary<ushort, object>();

		/// <summary>
		/// Map of event names -> IDs
		/// </summary>
		private static Dictionary<string, ushort> m_IDs = new Dictionary<string, ushort>();

		/// <summary>
		/// Map of event IDs -> names
		/// </summary>
		private static Dictionary<ushort, string> m_Names = new Dictionary<ushort, string>();

		/// <returns>Event ID associated with <paramref name="name"/>, or <see cref="ushort.MaxValue"/> if not found</returns>
		public static ushort GetID(string name) =>
			m_IDs.TryGetValue(name, out ushort id) ? id : ushort.MaxValue;

		/// <returns>Event name associated with <paramref name="eventID"/>, or <see cref="string.Empty"/> if not found</returns>
		public static string GetName(ushort eventID) =>
			m_Names.TryGetValue(eventID, out string name) ? name : string.Empty;

		/// <summary>
		/// Registers an event by name & type, allowing the event to be published
		/// </summary>
		/// <typeparam name="T">Type of event args. Must derive from <see cref="LEGEventArgs"/></typeparam>
		/// <param name="name">Readable name of event</param>
		/// <returns>Event ID of new or existing event by name</returns>
		public static ushort Register<T>(string name) where T : LEGEventArgs
		{
			// Check for existing event by name
			if(m_IDs.ContainsKey(name))
				return m_IDs[name];

			// Assign ID
			ushort eventID = 0;
			while (m_Names.ContainsKey(eventID))
				eventID++;

			// Add to queues
			m_IDs.Add(name, eventID);
			m_Names.Add(eventID, name);
			m_Queues.Add(eventID, new EventQueue<T>());

			EventRegistered?.Invoke(eventID, typeof(T));

			return eventID;
		}

		/// <summary>
		/// Registers an event by name, allowing the event to be published. Uses default <see cref="LEGEventArgs"/> as event type.
		/// </summary>
		public static ushort Register(string name) => Register<LEGEventArgs>(name);

		/// <summary>
		/// Removes an event from the registry
		/// </summary>
		public static void Deregister(string name) => Deregister(GetID(name));

		/// <summary>
		/// Removes an event from the registry
		/// </summary>
		public static void Deregister(ushort id)
		{
			if(m_Queues.ContainsKey(id))
			{
				EventDeregistered(id, m_Queues[id].GetType());
				m_Queues.Remove(id);
			}
		}

		/// <summary>
		/// Clears all registered events
		/// </summary>
		public static void Clear()
		{
			m_Queues.Clear();
			m_IDs.Clear();
			m_Names.Clear();
		}

		/// <returns>True if event is registered</returns>
		public static bool Exists(ushort eventID) => m_Queues.ContainsKey(eventID);

		/// <returns>True if event is registered</returns>
		public static bool Exists(string name) => m_Queues.ContainsKey(GetID(name));

		/// <summary>
		/// Listens to an event
		/// </summary>
		/// <typeparam name="T">Type of event args. Must derive from <see cref="LEGEventArgs"/></typeparam>
		/// <returns>Success if subscribed. Unsuccessful if event does not exist, or event type is incompatable with desired event</returns>
		public static bool Subscribe<T>(ushort eventID, Action<T> callback) where T : LEGEventArgs
		{
			if (!Exists(eventID))
				return false;

			EventQueue<T> queue = m_Queues[eventID] as EventQueue<T>;
			if (queue == null)
				return false; // Wrong event type

			queue.Add(callback);
			return true;
		}

		/// <summary>
		/// Listens to an event
		/// </summary>
		/// <typeparam name="T">Type of event args. Must derive from <see cref="LEGEventArgs"/></typeparam>
		/// <param name="createIfNotExist">If event does not exist, creates it with <paramref name="eventName"/> and event type <typeparamref name="T"/> before subscribing</param>
		/// <returns>Success if subscribed. Unsuccessful if event does not exist (see <paramref name="createIfNotExist"/>), or event type <typeparamref name="T"/> is incompatable with desired event</returns>
		public static bool Subscribe<T>(string eventName, Action<T> callback, bool createIfNotExist = false) where T : LEGEventArgs
		{
			ushort eventID = Exists(eventName) ? GetID(eventName) : (createIfNotExist ? Register<T>(eventName) : ushort.MaxValue);
			if(eventID == ushort.MaxValue)
				return false;

			return Subscribe(eventID, callback);
		}

		/// <summary>
		/// Stops listening to event
		/// </summary>
		/// <typeparam name="T">Type of event args. Must derive from <see cref="LEGEventArgs"/></typeparam>
		/// <returns>Success if unsubscribed. Unsuccessful if event does not exist, or event type <typeparamref name="T"/> is incompatible with desired event</returns>
		public static bool Unsubscribe<T>(ushort eventID, Action<T> callback) where T : LEGEventArgs
		{
			if (!Exists(eventID))
				return false;

			EventQueue<T> queue = m_Queues[eventID] as EventQueue<T>;
			if (queue == null)
				return false; // wrong event type

			queue.Remove(callback);
			return true;
		}

		/// <summary>
		/// Stops listening to event
		/// </summary>
		/// <typeparam name="T">Type of event args. Must derive from <see cref="LEGEventArgs"/></typeparam>
		/// <returns>Success if unsubscribed. Unsuccessful if event does not exist, or event type <typeparamref name="T"/> is incompatible with desired event</returns>
		public static bool Unsubscribe<T>(string eventName, Action<T> callback) where T : LEGEventArgs => Unsubscribe(GetID(eventName), callback);

		/// <summary>
		/// Raises an event and informs listeners
		/// </summary>
		/// <typeparam name="T">Type of event args. Must derive from <see cref="LEGEventArgs"/></typeparam>
		/// <returns>Success state. Unsuccessful if event does not exist, or event type <typeparamref name="T"/> is incompatible with desired event</returns>
		public static bool Publish<T>(ushort eventID, T args) where T : LEGEventArgs
		{
			if (!Exists(eventID))
			{
				Debug.LogWarning($"Failed publishing event {eventID} - event not found");
				return false;
			}

			args.EventID = eventID;

			EventQueue<T> queue = m_Queues[eventID] as EventQueue<T>;
			if (queue == null)
			{ 
				Debug.LogWarning($"Failed publishing event {GetName(eventID)} - args type '{typeof(T).Name}' was invalid");
				return false; // Wrong event type
			}

			queue.Invoke(args);

			try { EventPublished?.Invoke(eventID, args); }
			catch(Exception e) { Debug.LogException(e); }

			return true;
		}

		/// <summary>
		/// Raises an event and informs listeners. Uses default <see cref="LEGEventArgs"/> as event type.
		/// </summary>
		public static bool Publish(ushort eventID) => Publish(eventID, new LEGEventArgs(null));

		/// <summary>
		/// Raises an event and informs listeners
		/// </summary>
		/// <typeparam name="T">Type of event args. Must derive from <see cref="LEGEventArgs"/></typeparam>
		public static bool Publish<T>(string eventName, T args) where T : LEGEventArgs => Publish(GetID(eventName), args);

		/// <summary>
		/// Raises an event and informs listeners. Uses default <see cref="LEGEventArgs"/> as event type.
		/// </summary>
		public static bool Publish(string eventName) => Publish<LEGEventArgs>(GetID(eventName), null);

		/// <summary>
		/// Gets all registered event IDs
		/// </summary>
		/// <returns></returns>
		public static List<ushort> GetEventIDs() => m_IDs.Values.ToList();

		public static Type GetEventType(ushort id) => m_Queues.ContainsKey(id) ? m_Queues[id].GetType() : null;

		public delegate void OnEventChanged(ushort id, Type eventType);
		public delegate void OnEventPublished(ushort id, object args);

		public static event OnEventChanged EventRegistered;
		public static event OnEventChanged EventDeregistered;
		public static event OnEventPublished EventPublished;
	}
}