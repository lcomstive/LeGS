﻿using System.Linq;
using System.Collections.Generic;

namespace LEGS.Quests
{
	public enum QuestState : byte
	{
		Pending,
		InProgress,
		Completed,
		Incomplete
	}

	public class QuestParameter
	{
		/// <summary>
		/// Current value of the paramter
		/// </summary>
		public int Value;

		/// <summary>
		/// Maximum value of parameter. (e.g. 1 for boolean)
		/// </summary>
		public int MaxValue;

		/// <summary>
		/// Is this an optional parameter?
		/// </summary>
		public bool Optional;

		/// <summary>
		/// Description to display in-game
		/// </summary>
		public string Description;

		/// <summary>
		/// True if <see cref="Value"/> has reached <see cref="MaxValue"/>
		/// </summary>
		public bool IsCompleted => Value >= MaxValue;

		public QuestParameter(string description, int maxValue)
		{
			Value = 0;
			Optional = false;
			MaxValue = maxValue;
			Description = description;
		}
	}

	/// <summary>
	/// Event for when a change occurs in a <see cref="Quest"/>
	/// </summary>
	public class QuestEventArgs : LEGEventArgs
	{
		/// <summary>
		/// Quest being changed
		/// </summary>
		public Quest Quest { get; private set; }

		public QuestEventArgs(IEntity entity, Quest quest) : base(entity) => Quest = quest;
	}

	/// <summary>
	/// Base quest type
	/// </summary>
	[System.Serializable]
	public abstract class Quest
	{
		/// <summary>
		/// Display name
		/// </summary>
		public string Name;

		/// <summary>
		/// Description to display in-game
		/// </summary>
		public string Description;

		/// <summary>
		/// Identifier generated by <see cref="QuestManager"/>
		/// </summary>
		public uint ID { get; internal set; }

		/// <summary>
		/// <see cref="IEntity"/> that created this quest
		/// </summary>
		public IEntity Entity { get; internal set; }

		/// <summary>
		/// Current state
		/// </summary>
		public QuestState State { get; private set; }

		/// <summary>
		/// All parameters, with key as unique name
		/// </summary>
		protected Dictionary<string, QuestParameter> Parameters;

		public static ushort QuestEndEventID			 { get; private set; } = ushort.MaxValue;
		public static ushort QuestBeginEventID			 { get; private set; } = ushort.MaxValue;
		public static ushort QuestStatusChangeEventID	 { get; private set; } = ushort.MaxValue;
		public static ushort QuestParameterUpdateEventID { get; private set; } = ushort.MaxValue;

		public Quest()
		{
			ID = 0;
			Name = "Quest";
			Description = string.Empty;
			State = QuestState.Pending;
			Parameters = new Dictionary<string, QuestParameter>();

			if(QuestEndEventID == ushort.MaxValue)
			{
				QuestEndEventID = EventManager.Register<QuestEventArgs>("QuestEnd");
				QuestBeginEventID = EventManager.Register<QuestEventArgs>("QuestBegin");
				QuestStatusChangeEventID = EventManager.Register<QuestEventArgs>("QuestStatusChange");
				QuestParameterUpdateEventID = EventManager.Register<QuestEventArgs>("QuestParameterUpdate");
			}
		}

		public QuestParameter GetParameter(string name) =>
			Parameters.TryGetValue(name, out QuestParameter param) ? param : null;

		public void UpdateParameter(string name, int value)
		{
			if(!Parameters.ContainsKey(name))
				return;
			Parameters[name].Value = value;

			ParameterChanged?.Invoke(this, name, value);
			EventManager.Publish(QuestParameterUpdateEventID, new QuestEventArgs(null, this));
		}

		public (string[], QuestParameter[]) GetParameters() => (Parameters.Keys.ToArray(), Parameters.Values.ToArray());

		public void ChangeState(QuestState state)
		{
			if (State == state)
				return; // No change
			QuestState oldState = State;
			State = state;

			// Check if quest is beginning
			if (State == QuestState.InProgress)
				EventManager.Publish(QuestBeginEventID, new QuestEventArgs(null, this));

			// Check if quest is going into a state representing finished
			if ((byte)State > (byte)QuestState.InProgress)
				EventManager.Publish(QuestEndEventID, new QuestEventArgs(null, this));

			StateChanged?.Invoke(this);
			EventManager.Publish(QuestStatusChangeEventID, new QuestEventArgs(null, this));
		}

		/// <summary>
		/// Checks all parameters for completeness
		/// </summary>
		protected void ValidateParameters()
		{
			if (State != QuestState.InProgress)
				return; // No reason to check completeness

			foreach (QuestParameter param in Parameters.Values)
			{
				if (!param.IsCompleted && !param.Optional)
					return;
			}

			ChangeState(QuestState.Completed);
		}

		public delegate void OnStateChanged(Quest quest);
		public event OnStateChanged StateChanged;

		public delegate void OnParameterChanged(Quest quest, string parameterName, int value);
		public event OnParameterChanged ParameterChanged;
	}
}