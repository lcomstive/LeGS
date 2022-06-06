#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

// Multiheader columns based off accepted answer 
//		https://gamedev.stackexchange.com/questions/188771/creating-a-custom-editor-window-using-a-multi-column-header

namespace LEGS
{
    public class EventDebugger : EditorWindow
    {
		private class EventLogEntry
		{
			public string Name;
			public uint TotalCount;
			public uint CountThisSecond;
			public uint CountLastSecond;

			public EventLogEntry(string name)
			{
				Name = name;
				TotalCount = CountThisSecond = CountLastSecond = 0;
			}

			public static EventLogEntry operator ++(EventLogEntry entry)
			{
				entry.TotalCount++;
				entry.CountThisSecond++;
				return entry;
			}
		}

		private MultiColumnHeader m_Header;
		private MultiColumnHeaderState m_HeaderState;
		private MultiColumnHeaderState.Column[] m_HeaderColumns;

		private Color m_RowColourDark  = Color.white * 0.1f;
		private Color m_RowColourLight = Color.white * 0.3f;

		private bool m_EnableCounter = false;
		private Dictionary<ushort, EventLogEntry> m_Counters = new Dictionary<ushort, EventLogEntry>();

		[MenuItem("Window/LeGS/Event Debugger")]
		public static void ShowWindow() => GetWindow<EventDebugger>().Show();

		private void OnEnable()
		{
			m_Counters.Clear();

			EventManager.EventPublished += OnEventPublished;

			m_EnableCounter = true;
			SecondCounterLoop();

			// Setup multi header column
			m_HeaderColumns = new MultiColumnHeaderState.Column[]
			{
				new MultiColumnHeaderState.Column()
				{
					allowToggleVisibility = false,
					autoResize = true,
					minWidth = 100.0f,
					canSort = true,
					sortingArrowAlignment = TextAlignment.Right,
					headerContent = new GUIContent("Name"),
					headerTextAlignment = TextAlignment.Left
				},
				new MultiColumnHeaderState.Column()
				{
					allowToggleVisibility = true,
					autoResize = true,
					minWidth = 35.0f,
					canSort = true,
					sortingArrowAlignment = TextAlignment.Right,
					headerContent = new GUIContent("Per Second", "Events per second"),
					headerTextAlignment = TextAlignment.Center
				},
				new MultiColumnHeaderState.Column()
				{
					allowToggleVisibility = true,
					autoResize = true,
					minWidth = 35.0f,
					canSort = true,
					sortingArrowAlignment = TextAlignment.Right,
					headerContent = new GUIContent("Total Count", "Total events that have occurred"),
					headerTextAlignment = TextAlignment.Center
				}
			};

			m_HeaderState = new MultiColumnHeaderState(m_HeaderColumns);
			m_Header = new MultiColumnHeader(m_HeaderState);
			m_Header.visibleColumnsChanged += (header) => header.ResizeToFit();
			m_Header.ResizeToFit();
		}

		private void OnDisable()
		{
			m_EnableCounter = false;
			EventManager.EventPublished -= OnEventPublished;
		}

		private async void SecondCounterLoop()
		{
			await Task.Delay(1000);
			foreach(var pair in m_Counters)
			{
				pair.Value.CountLastSecond = pair.Value.CountThisSecond;
				pair.Value.CountThisSecond = 0;
			}

			if(m_EnableCounter)
				SecondCounterLoop();
		}

		private void OnEventPublished(ushort id, object args)
		{
			if(!m_Counters.ContainsKey(id))
				m_Counters.Add(id, new EventLogEntry(EventManager.GetName(id)));
			m_Counters[id]++;
		}

		private static Vector2 m_CounterScroll = Vector2.zero;

		private void OnGUI()
		{
			GUILayout.FlexibleSpace();
			Rect windowRect = GUILayoutUtility.GetLastRect();
			windowRect.width = position.width;
			windowRect.height = position.height;

			float columnHeight = EditorGUIUtility.singleLineHeight;

			Rect viewRect = new Rect(windowRect)
			{
				xMax = m_HeaderColumns.Sum(x => x.width)
			};

			Rect rowRect = new Rect(windowRect);
			rowRect.height = columnHeight;

			Rect scrollRect = GUILayoutUtility.GetRect(0, float.MaxValue, 0, float.MaxValue);

			m_CounterScroll = GUI.BeginScrollView(scrollRect, m_CounterScroll, viewRect, false, false);
			m_Header.OnGUI(rowRect, 0.0f);

			ushort[] keys = m_Counters.Keys.ToArray();
			for(int i = 0; i < keys.Length; i++)
			{
				rowRect.y += columnHeight;
				EditorGUI.DrawRect(rowRect, i % 2 == 0 ? m_RowColourDark : m_RowColourLight);

				EventLogEntry entry = m_Counters[keys[i]];

				// Name
				if(m_Header.IsColumnVisible(0))
				{
					Rect columnRect = m_Header.GetColumnRect(m_Header.GetVisibleColumnIndex(0));

					columnRect.y = rowRect.y;

					EditorGUI.LabelField(
						m_Header.GetCellRect(0, columnRect),
						entry.Name
					);
				}

				// Events per second
				if(m_Header.IsColumnVisible(1))
				{
					Rect columnRect = m_Header.GetColumnRect(m_Header.GetVisibleColumnIndex(1));

					columnRect.y = rowRect.y;

					EditorGUI.LabelField(
						m_Header.GetCellRect(1, columnRect),
						entry.CountLastSecond.ToString()
					);
				}

				// Total events
				if(m_Header.IsColumnVisible(2))
				{
					Rect columnRect = m_Header.GetColumnRect(m_Header.GetVisibleColumnIndex(2));

					columnRect.y = rowRect.y;

					EditorGUI.LabelField(
						m_Header.GetCellRect(2, columnRect),
						entry.TotalCount.ToString()
					);
				}
			}

			GUI.EndScrollView(true);

			Repaint();
		}
	}
}
#endif