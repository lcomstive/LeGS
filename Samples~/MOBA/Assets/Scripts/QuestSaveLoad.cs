using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LEGS.Quests;

public class QuestSaveLoad : MonoBehaviour
{
	public static readonly string SaveLocation = $"{Application.dataPath}/Quests.dat";

	[SerializeField] private List<Quest> m_Quests;

	public void Save()
	{

	}

	public void Load()
	{

	}

	// TODO: Custom inspector to draw parameters
}
