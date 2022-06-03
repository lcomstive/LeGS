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

		/// <summary>
		/// Clears all quests
		/// </summary>
		public static void Clear() => m_Quests.Clear();

		/// <summary>
		/// Creates a new quest
		/// </summary>
		/// <param name="entity"><see cref="IEntity"/> creating quest</param>
		/// <returns>Newly created <see cref="Quest"/></returns>
		public static Quest Create(IEntity entity)
		{
			uint id = 0;
			while(m_Quests.ContainsKey(id))
				id++;

			Quest quest = new Quest(id, entity);
			m_Quests.Add(id, quest);
			return quest;
		}

		/// <returns><see cref="Quest"/> matching <paramref name="id"/>, or null if not found</returns>
		public static Quest Get(uint id) =>
			m_Quests.TryGetValue(id, out Quest value) ? value : null;

		/// <summary>
		/// Removes <paramref name="quest"/> from global <see cref="Quest"/> list
		/// </summary>
		public static void Remove(Quest quest) => Remove(quest.ID);

		/// <summary>
		/// Removes <see cref="Quest"/> with <paramref name="ID"/> from global <see cref="Quest"/> list
		/// </summary>
		public static void Remove(uint ID)
		{
			if(!m_Quests.ContainsKey(ID))
				m_Quests.Remove(ID);
		}

		/// <returns>All <see cref="Quest"/>s with <see cref="Quest.State"/> matching <paramref name="state"/></returns>
		public static List<Quest> GetAllQuestsWithState(QuestState state)
		{
			List<Quest> quests = new List<Quest>();
			foreach(var pair in m_Quests)
				if(pair.Value.State == state)
					quests.Add(pair.Value);
			return quests;
		}
	}
}
