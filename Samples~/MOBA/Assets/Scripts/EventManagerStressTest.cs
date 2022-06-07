using LEGS;
using System;
using UnityEngine;

public class EventManagerStressTest : MonoBehaviour
{
	public enum StressType { None, String, StringNoSubscribe, UShort, UShortNoSubscribe }

	private const string TestEventArgsName = "EventManagerStressTestEvent";

	[SerializeField] private StressType m_Type = StressType.None;
	[SerializeField] private int m_StressIterations = 100;

	private ushort m_TestEventID;

	private void Start() => m_TestEventID = EventManager.Register(TestEventArgsName);
	private void OnDestroy() => EventManager.Deregister(TestEventArgsName);

	private void Update()
	{
		if(m_Type == StressType.None)
			return;

		for(int i = 0; i < m_StressIterations; i++)
		{
			if(m_Type == StressType.String)
				StressIterationString();
			else
				StressIterationUShort();
		}
	}

	private void StressIterationString()
	{
		if (!EventManager.Exists(TestEventArgsName))
			throw new Exception($"{TestEventArgsName} is not registered");

		ushort eventID = EventManager.GetID(TestEventArgsName);
		EventManager.GetEventType(eventID);

		if(m_Type != StressType.StringNoSubscribe) EventManager.Subscribe<LEGEventArgs>(TestEventArgsName, OnTestEventArgs);
		EventManager.Publish(TestEventArgsName);
		if (m_Type != StressType.StringNoSubscribe) EventManager.Unsubscribe<LEGEventArgs>(TestEventArgsName, OnTestEventArgs);
	}

	private void StressIterationUShort()
	{
		if (!EventManager.Exists(m_TestEventID))
			throw new Exception($"{TestEventArgsName} is not registered");

		EventManager.GetEventType(m_TestEventID);

		if (m_Type != StressType.UShortNoSubscribe) EventManager.Subscribe<LEGEventArgs>(m_TestEventID, OnTestEventArgs);
		EventManager.Publish(m_TestEventID);
		if (m_Type != StressType.UShortNoSubscribe) EventManager.Unsubscribe<LEGEventArgs>(m_TestEventID, OnTestEventArgs);
	}

	private void OnTestEventArgs(LEGEventArgs _) { }
}
