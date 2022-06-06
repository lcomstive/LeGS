﻿using System.Linq;
using System.Collections.Generic;

namespace LEGS.Quests
{
	public enum QuestState : byte
	{
		Pending,
		InProgress,
		Complete,
		Incomplete
	}

	public class QuestParameter
	{
		/// <summary>
		/// Current value of the paramter
		/// </summary>
		public int Value = 0;

		/// <summary>
		/// Maximum value of parameter. (e.g. 1 for boolean)
		/// </summary>
		public int MaxValue = 1;

		/// <summary>
		/// Is this an optional parameter?
		/// </summary>
		public bool Optional = false;

		/// <summary>
		/// Description to display in-game
		/// </summary>
		public string Description = "";

		/// <summary>
		/// True if <see cref="Value"/> has reached <see cref="MaxValue"/>
		/// </summary>
		public bool IsCompleted => Value >= MaxValue;

		public override bool Equals(object obj)
		{
			if(obj is not QuestParameter)
				return false;

			QuestParameter other = (QuestParameter)obj;
			return Value == other.Value &&
				MaxValue == other.MaxValue &&
				Optional == other.Optional &&
				Description.Equals(other.Description);
		}

		public override int GetHashCode() => base.GetHashCode();
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
	public class Quest : ISerializable
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
		public uint ID { get; private set; }

		/// <summary>
		/// <see cref="IEntity"/> that created this quest
		/// </summary>
		public IEntity Entity { get; private set; }

		/// <summary>
		/// Current state
		/// </summary>
		public QuestState State
		{
			get => m_State;
			set => ChangeState(value);
		}

		/// <summary>
		/// All parameters, with key as unique name
		/// </summary>
		protected Dictionary<string, QuestParameter> Parameters;

		/// <summary>
		/// How many parameters this quest has
		/// </summary>
		public int ParameterCount => Parameters.Count;

		public static ushort QuestEndEventID			 { get; private set; } = ushort.MaxValue;
		public static ushort QuestBeginEventID			 { get; private set; } = ushort.MaxValue;
		public static ushort QuestStatusChangeEventID	 { get; private set; } = ushort.MaxValue;
		public static ushort QuestParameterUpdateEventID { get; private set; } = ushort.MaxValue;

		private QuestState m_State = QuestState.Pending;

		public Quest(uint id, IEntity creatingEntity)
		{
			ID = id;
			Entity = creatingEntity;
			Name = "Quest";
			Description = string.Empty;
			m_State = QuestState.Pending;
			Parameters = new Dictionary<string, QuestParameter>();

			if(QuestEndEventID == ushort.MaxValue)
			{
				QuestEndEventID = EventManager.Register<QuestEventArgs>("QuestEnd");
				QuestBeginEventID = EventManager.Register<QuestEventArgs>("QuestBegin");
				QuestStatusChangeEventID = EventManager.Register<QuestEventArgs>("QuestStatusChange");
				QuestParameterUpdateEventID = EventManager.Register<QuestEventArgs>("QuestParameterUpdate");
			}
		}

		public QuestParameter AddParameter(string name)
		{
			if(Parameters.ContainsKey(name))
				return Parameters[name];
			QuestParameter parameter = new QuestParameter();
			Parameters.Add(name, parameter);
			return parameter;
		}

		public void AddParameter(string name, QuestParameter parameter)
		{
			if(!Parameters.ContainsKey(name))
				Parameters.Add(name, parameter);
		}

		public void RemoveParameter(string name)
		{
			if(Parameters.ContainsKey(name))
				Parameters.Remove(name);
		}

		public QuestParameter GetParameter(string name) =>
			Parameters.TryGetValue(name, out QuestParameter param) ? param : null;

		public void SetParameter(string name, int value)
		{
			if(!Parameters.ContainsKey(name))
				return;
			Parameters[name].Value = value;

			ParameterChanged?.Invoke(this, name, value);
			EventManager.Publish(QuestParameterUpdateEventID, new QuestEventArgs(Entity, this));
		}

		public void SetParameterName(string oldName, string newName)
		{
			if(!Parameters.ContainsKey(oldName))
				return;

			Parameters[newName] = Parameters[oldName];
			Parameters.Remove(oldName);
			EventManager.Publish(QuestParameterUpdateEventID, new QuestEventArgs(Entity, this));
		}

		public void SetParameter(string name, QuestParameter parameter)
		{
			if(Parameters.TryGetValue(name, out QuestParameter oldValue) &&
				oldValue.Equals(parameter))
				return; // No change

			Parameters[name] = parameter;

			ParameterChanged?.Invoke(this, name, parameter.Value);
			EventManager.Publish(QuestParameterUpdateEventID, new QuestEventArgs(Entity, this));
		}

		public (string[], QuestParameter[]) GetParameters() => (Parameters.Keys.ToArray(), Parameters.Values.ToArray());

		public bool HasParameter(string name) => Parameters.ContainsKey(name);

		public void ChangeState(QuestState state)
		{
			if (State == state)
				return; // No change
			QuestState oldState = m_State;
			m_State = state;

			// Check if quest is beginning
			if (State == QuestState.InProgress)
				EventManager.Publish(QuestBeginEventID, new QuestEventArgs(Entity, this));

			// Check if quest is going into a state representing finished
			if ((byte)State > (byte)QuestState.InProgress)
				EventManager.Publish(QuestEndEventID, new QuestEventArgs(Entity, this));

			StateChanged?.Invoke(this);
			EventManager.Publish(QuestStatusChangeEventID, new QuestEventArgs(Entity, this));
		}

		/// <summary>
		/// Checks all parameters for completeness
		/// </summary>
		protected void ValidateParameters()
		{
			if (State != QuestState.InProgress)
				return; // No reason to check completeness

			foreach (QuestParameter param in Parameters.Values)
				if (!param.IsCompleted && !param.Optional)
					return;

			ChangeState(QuestState.Complete);
		}

		private void OnSerialized(ref DataStream stream)
		{
			stream.Serialize(ref Name);
			stream.Serialize(ref Description);

			byte state = (byte)m_State;
			stream.Serialize(ref state);
			m_State = (QuestState)state;

			if(stream.isWriting)
			{
				stream.Write(Parameters.Count);
				foreach(var key in Parameters.Keys)
					stream.Write(key);
			}
			else
			{
				int parameterCount = stream.Read<int>();
				for(int i = 0; i < parameterCount; i++)
					Parameters.Add(stream.Read<string>(), new QuestParameter());
			}

			foreach(var pair in Parameters)
			{
				stream.Serialize(ref pair.Value.Value);
				stream.Serialize(ref pair.Value.MaxValue);
				stream.Serialize(ref pair.Value.Optional);
				stream.Serialize(ref pair.Value.Description);
			}
		}

		public void Serialize(ref DataStream stream) => OnSerialized(ref stream);

		public static ISerializable Deserialize(DataStream stream)
		{
			Quest quest = QuestManager.Create(null);
			quest.OnSerialized(ref stream);
			return quest;
		}

		public QuestParameter this[string parameterName]
		{
			get => GetParameter(parameterName);
			set => SetParameter(parameterName, value);
		}

		public delegate void OnStateChanged(Quest quest);
		public event OnStateChanged StateChanged;

		public delegate void OnParameterChanged(Quest quest, string parameterName, int value);
		public event OnParameterChanged ParameterChanged;
	}
}
