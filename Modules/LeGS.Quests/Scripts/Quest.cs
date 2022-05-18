using System.Linq;
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
		public int Value;
		public int MaxValue;
		public bool Optional;
		public string Description;

		public bool IsCompleted => Value >= MaxValue;

		public QuestParameter(string description, int maxValue)
		{
			Value = 0;
			Optional = false;
			MaxValue = maxValue;
			Description = description;
		}
	}

	public class QuestEventArgs : LEGEventArgs
	{
		public Quest Quest { get; private set; }

		public QuestEventArgs(IEntity entity, Quest quest) : base(entity) => Quest = quest;
	}

	[System.Serializable]
	public abstract class Quest
	{
		public string Name;
		public string Description;

		public uint ID { get; internal set; }
		public IEntity Entity { get; internal set; }
		public QuestState State { get; private set; }

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
				QuestEndEventID = EventManager.RegisterEvent<QuestEventArgs>("QuestEnd");
				QuestBeginEventID = EventManager.RegisterEvent<QuestEventArgs>("QuestBegin");
				QuestStatusChangeEventID = EventManager.RegisterEvent<QuestEventArgs>("QuestStatusChange");
				QuestParameterUpdateEventID = EventManager.RegisterEvent<QuestEventArgs>("QuestParameterUpdate");
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
