using System;
using LEGS.Shop;
using UnityEngine;
using LEGS.Abilities;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MOBAExample
{
	[CreateAssetMenu(fileName = "Character", menuName = "MOBA/Character")]
	public class MOBACharacterInfo : ScriptableObject
	{
		public Currency KillReward => m_KillReward;
		public float KillRewardExperience => m_KillRewardEXP;

		public MOBAAbility[] Abilities = new MOBAAbility[4];
		[SerializeField] private float m_KillRewardEXP = 10.0f;
		[SerializeField] private Currency m_KillReward = new Currency(100);

		[Tooltip("Horizontal = experience. Vertical = level")]
		public AnimationCurve ExperienceLevels = new AnimationCurve(new Keyframe(0, 1), new Keyframe(100, 18));

		/// <summary>
		/// TRAIT KEYFRAMES
		/// HORIZONTAL: CHARACTER LEVEL
		/// VERTICAL:	TRAIT VALUE
		/// </summary>

		[SerializeField, HideInInspector]
		private AnimationCurve[] m_BaseTraits = new AnimationCurve[Enum.GetValues(typeof(CharacterTrait)).Length];

		public int GetCurrentLevel(float experience) => (int)Mathf.Floor(ExperienceLevels.Evaluate(experience));

		public float this[CharacterTrait trait, int level]
		{
			get => m_BaseTraits[(int)trait].Evaluate(level);
		}

#if UNITY_EDITOR
		[CustomEditor(typeof(MOBACharacterInfo))]
		public class MOBACharacterInfoEditor : Editor
		{
			private SerializedProperty m_Property;

			private static readonly CharacterTrait[] AllTraits = (CharacterTrait[])Enum.GetValues(typeof(CharacterTrait));

			private void OnEnable() => m_Property = serializedObject.FindProperty("m_BaseTraits");

			public override void OnInspectorGUI()
			{
				base.OnInspectorGUI();

				serializedObject.Update();

				EditorGUILayout.LabelField("Traits");

				EditorGUILayout.LabelField("Horizontal: Character Level");
				EditorGUILayout.LabelField("Vertical:   Stat Value");

				EditorGUILayout.Space();

				for(int i = 0; i < AllTraits.Length; i++)
				{
					AnimationCurve curve = m_Property.GetArrayElementAtIndex(i).animationCurveValue;
					curve = EditorGUILayout.CurveField(AllTraits[i].ToString(), curve);
					m_Property.GetArrayElementAtIndex(i).animationCurveValue = curve;
				}

				serializedObject.ApplyModifiedProperties();
			}
		}
#endif
	}
}
