using UnityEngine;
using LEGS;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MOBAExample
{
	[RequireComponent(typeof(MOBACharacter))]
	public class CharacterExperience : MonoBehaviour
	{
		public const int MaxLevel = 18;

		public int Level { get; private set; }
		public float Experience { get; private set; }

		private MOBACharacter m_Character;
		private ushort m_CharacterChangedEventID;
		private AnimationCurve m_InverseExperienceCurve;

		private void Start()
		{
			m_Character = GetComponent<MOBACharacter>();
			EventManager.Subscribe<EntityDeathEventArgs>(EntityDeathEventArgs.EventName, OnEntityDeath, true);

			m_CharacterChangedEventID = EventManager.RegisterEvent<CharacterChangedEventArgs>(CharacterChangedEventArgs.EventName);

			CreateInvertedExperienceCurve();
		}

		private void CreateInvertedExperienceCurve()
		{
			m_InverseExperienceCurve = new AnimationCurve();
			AnimationCurve curve = m_Character.Info.ExperienceLevels;
			float totalTime = curve.keys[^1].time;
			float sampleX = 0; // Point to sample
			float deltaX = 0.005f; // Sample delta
			float lastValue = curve.Evaluate(sampleX);
			while (sampleX < totalTime)
			{
				float value = curve.Evaluate(sampleX);
				float deltaY = lastValue - value;
				float tangent = deltaX / deltaY;
				m_InverseExperienceCurve.AddKey(new Keyframe(value, sampleX, tangent, tangent));

				sampleX += deltaX;
				lastValue = value;
			}

			/*
			for (int i = 0; i < m_InverseExperienceCurve.length; i++)
				m_InverseExperienceCurve.SmoothTangents(i, 0.1f);
			*/
		}

		private void OnDestroy() => EventManager.Unsubscribe<EntityDeathEventArgs>(EntityDeathEventArgs.EventName, OnEntityDeath);

		private void OnEntityDeath(EntityDeathEventArgs args)
		{
			if (args.Entity == (IEntity)m_Character)
				return; // We died.. rip

			MOBACharacter character = args.Entity as MOBACharacter;
			if (!character)
				return; // Invalid/incompatible cast to MOBACharacter

			AddExperience(character.Info.KillRewardExperience);

			// TODO: Gain coin equal to character.m_Info.KillReward
		}

		public void AddExperience(float experience)
		{
			int previousLevel = Level;
			Experience += experience;
			Level = m_Character.Info?.GetCurrentLevel(Experience) ?? 0;

			EventManager.Publish(m_CharacterChangedEventID, new CharacterChangedEventArgs(m_Character));

			if (previousLevel == Level) // Did not level up
				return;

			// Levelled up
			Debug.Log($"'{m_Character.DisplayName}' reached level {Level + 1}");

			LevelUp?.Invoke(Level);
		}

		public float ExperienceForLevel(int level) => m_InverseExperienceCurve?.Evaluate(level) ?? 0;

		public event System.Action<int> LevelUp;

#if UNITY_EDITOR
		[CustomEditor(typeof(CharacterExperience))]
		public class CharacterExperienceEditor : Editor
		{
			CharacterExperience m_Target;

			private void OnEnable() => m_Target = (CharacterExperience)target;

			public override void OnInspectorGUI()
			{
				base.OnInspectorGUI();

				EditorGUILayout.Space();

				EditorGUILayout.LabelField($"Level: {m_Target.Level + 1}");
				EditorGUILayout.LabelField($"Experience: {m_Target.Experience}");

				/*
				if (GUILayout.Button("Update Experience Cache"))
					m_Target.CreateInvertedExperienceCurve();
				*/

				if (GUILayout.Button("+50 Experience"))
					m_Target.AddExperience(50);
			}
		}
#endif
	}
}