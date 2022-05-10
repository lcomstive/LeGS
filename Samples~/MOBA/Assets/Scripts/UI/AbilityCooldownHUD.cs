using LEGS;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace MOBAExample
{
	[Serializable]
	public class AbilityCooldownUI
	{
		public Image Icon;
		public Image CooldownOverlay;
	}

	public class AbilityCooldownHUD : MonoBehaviour
	{
		[SerializeField] private AbilityCooldownUI[] m_AbilityUI;

		private void Start()
		{
			EventManager.Subscribe<AbilityCastInfoChangedEventArgs>(
				AbilityCastInfoChangedEventArgs.EventName,
				OnAbilityCastInfoChanged,
				true
			);
		}

		private void OnDestroy()
		{
			EventManager.Unsubscribe<AbilityCastInfoChangedEventArgs>(
				AbilityCastInfoChangedEventArgs.EventName,
				OnAbilityCastInfoChanged
			);
		}

		private void OnAbilityCastInfoChanged(AbilityCastInfoChangedEventArgs args)
		{
			if(m_AbilityUI.Length <= args.Info.AbilityIndex)
				return; // Not enough UI for ability count

			AbilityCooldownUI ui = m_AbilityUI[args.Info.AbilityIndex];
			ui.CooldownOverlay.fillAmount = args.Info.CooldownRemainingPercent;

			MOBAAbility mobaAbility = args.Info.Ability as MOBAAbility;
			if(args.Info.Ability is MOBAAbility)
				ui.Icon.sprite = mobaAbility.Icon;
		}
	}
}