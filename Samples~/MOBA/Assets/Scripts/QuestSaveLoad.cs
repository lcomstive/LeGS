using UnityEngine;
using LEGS.Quests;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

public class QuestSaveLoad : MonoBehaviour
{
	[SerializeField] private string m_SaveLocation = "Quests.dat";
	[SerializeField] private List<Quest> m_Quests;

	public void Save()
	{

	}

	public void Load()
	{

	}

#if UNITY_EDITOR
	[CustomEditor(typeof(QuestSaveLoad))]
	public class QuestSaveLoadEditor : Editor
	{
		ReorderableList m_List;
		QuestSaveLoad m_Target;
		List<bool> m_QuestFoldouts = new List<bool>();

		private void OnEnable()
		{
			m_Target = (QuestSaveLoad)target;

			m_List = new ReorderableList(m_Target.m_Quests, typeof(Quest));
			m_List.drawElementCallback = OnDrawListElement;
			m_List.drawHeaderCallback = OnDrawHeader;

			m_List.elementHeightCallback = OnGetElementHeight;
			m_List.onAddCallback = OnElementAdd;
			m_List.onRemoveCallback = OnElementRemove;

			m_QuestFoldouts.Clear();
			for(int i = 0; i < m_Target.m_Quests.Count; i++)
				m_QuestFoldouts.Add(false);
		}

		public override void OnInspectorGUI()
		{
			m_Target.m_SaveLocation = EditorGUILayout.TextField("Save Location", m_Target.m_SaveLocation);

			m_List.DoLayoutList();
		}

		private void OnElementAdd(ReorderableList _)
		{
			m_Target.m_Quests.Add(QuestManager.Create(null));
			m_QuestFoldouts.Add(false);
		}

		void OnElementRemove(ReorderableList _)
		{
			var removeIndices = m_List.selectedIndices;
			for(int i = removeIndices.Count - 1; i >= 0; i--)
			{
				QuestManager.Remove(m_Target.m_Quests[i]);
				m_Target.m_Quests.RemoveAt(i);
				m_QuestFoldouts.RemoveAt(i);
			}
		}

		private void OnDrawHeader(Rect rect) => EditorGUI.LabelField(rect, "Quests");

		private void OnDrawListElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			Quest quest = m_Target.m_Quests[index];

			Rect drawRect = new Rect(rect.x + 10, rect.y, rect.width - 20, EditorGUIUtility.singleLineHeight);
			quest.Name = EditorGUI.TextField(drawRect, "Name", quest.Name);
			drawRect.y += EditorGUIUtility.singleLineHeight;
			quest.Description = EditorGUI.TextField(drawRect, "Description", quest.Description);
			drawRect.y += EditorGUIUtility.singleLineHeight;
			quest.State = (QuestState)EditorGUI.EnumPopup(drawRect, "State", quest.State);
			drawRect.y += EditorGUIUtility.singleLineHeight;
			// Foldout
			m_QuestFoldouts[index] = EditorGUI.Foldout(drawRect, m_QuestFoldouts[index], "Parameters");
			if(m_QuestFoldouts[index])
			{
				drawRect.x += 20;
				drawRect.width -= 20;
				drawRect.y += EditorGUIUtility.singleLineHeight;

				(string[] paramNames, QuestParameter[] parameters) = quest.GetParameters();
				for(int i = 0; i < parameters.Length; i++)
				{
					string paramName = EditorGUI.TextField(drawRect, "Name", paramNames[i]);
					if(!paramName.Equals(paramNames[i]))
						quest.SetParameterName(paramNames[i], paramName);

					drawRect.y += EditorGUIUtility.singleLineHeight;
				}

				if(GUI.Button(new Rect(rect.width - 60, drawRect.y, 20, 20), "+"))
					quest.AddParameter("Parameter " + parameters.Length);
				if(GUI.Button(new Rect(rect.width - 30, drawRect.y, 20, 20), "-"))
					quest.RemoveParameter(paramNames[^1]);
			}
		}

		private float OnGetElementHeight(int index)
		{
			float height = EditorGUIUtility.singleLineHeight * 4;
			if(index < 0 || index >= m_QuestFoldouts.Count || !m_QuestFoldouts[index])
				return height;
			height += EditorGUIUtility.singleLineHeight * (m_Target.m_Quests[index].ParameterCount * 3);
			height += EditorGUIUtility.singleLineHeight;
			return height;
		}
	}
#endif
}
