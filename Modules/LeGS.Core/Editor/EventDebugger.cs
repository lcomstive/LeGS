using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace LEGS
{
    public class EventDebugger : EditorWindow
    {
		private struct EventLogEntry
		{
			public string Name;
			public ushort ID;
			public object Args;
		}

		private GUIStyle m_ScrollStyle;
		private List<EventLogEntry> m_Events = new List<EventLogEntry>();
		private Dictionary<ushort, int> m_Counters = new Dictionary<ushort, int>();

		[MenuItem("Window/LeGS/Event Debugger")]
		public static void ShowWindow() => GetWindow<EventDebugger>().Show();

		private void OnEnable()
		{
			m_Events.Clear();
			m_Counters.Clear();

			m_ScrollStyle = new GUIStyle();
			m_ScrollStyle.margin = new RectOffset(10, 10, 10, 10);

			EventManager.EventPublished += OnEventPublished;
		}

		private void OnDisable() => EventManager.EventPublished -= OnEventPublished;

		private void OnEventPublished(ushort id, object args)
		{
			if(!m_Counters.ContainsKey(id))
				m_Counters.Add(id, 0);
			m_Counters[id]++;
		}

		private static Vector2 m_CounterScroll = Vector2.zero;

		private void OnGUI()
		{
			Vector2 size = position.size;

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.LabelField("Counters", GUILayout.Width(size.x - 60));
				if(GUILayout.Button("Clear"))
					m_Counters.Clear();
			}
			EditorGUILayout.EndHorizontal();
			m_CounterScroll = EditorGUILayout.BeginScrollView(
				m_CounterScroll,
				m_ScrollStyle
			);
			{
				foreach(var pair in m_Counters)
				{
					EditorGUILayout.BeginHorizontal(GUILayout.Width(size.x - 20));
					EditorGUILayout.LabelField(EventManager.GetName(pair.Key), GUILayout.Width(size.x - 60));
					EditorGUILayout.LabelField(pair.Value.ToString(), GUILayout.Width(40));
					EditorGUILayout.EndHorizontal();
				}
			}
			EditorGUILayout.EndScrollView();

			Repaint();
		}
	}
}
