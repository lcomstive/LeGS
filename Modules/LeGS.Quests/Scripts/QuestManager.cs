using LEGS;
using System.Collections.Generic;

namespace LEGS.Quests
{
	public static class QuestManager
	{
		/// <summary>
		/// Key: Quest ID
		/// Value: Quest
		/// </summary>
		private static Dictionary<uint, Quest> m_Quests = new Dictionary<uint, Quest>();

		public static void Clear() => m_Quests.Clear();

		public static T Create<T>(IEntity entity, params object[] args) where T : Quest
		{
			uint id = 0;
			while(m_Quests.ContainsKey(id))
				id++;

			Quest quest = (T)System.Activator.CreateInstance(typeof(T), args);
			quest.ID = id;
			quest.Entity = entity;
			m_Quests.Add(id, quest);

			return (T)quest;
		}

		public static Quest Get<T>(uint id) where T : Quest =>
			m_Quests.TryGetValue(id, out Quest value) ? value : null;

		public static void Remove(Quest quest)
		{
			if(!m_Quests.ContainsKey(quest.ID))
				return;

			m_Quests.Remove(quest.ID);
		}

		public static List<Quest> GetAllQuestsWithState(QuestState state)
		{
			List<Quest> quests = new List<Quest>();
			foreach(var pair in m_Quests)
			{
				if(pair.Value.State == state)
					quests.Add(pair.Value);
			}
			return quests;
		}
	}
}
