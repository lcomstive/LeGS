using LEGS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MOBAExample
{
	[CreateAssetMenu(menuName = "MOBA/Abilities/Add Status Effect", fileName = "Add Effect")]
	public class MOBAAbilityAddStatusEffect : MOBAAbility
	{
		[SerializeField] private StatusEffect m_Effect;

		public override void Activate(IEntity caster, GameObject gameObject)
		{
			IStatusEffectReceiver receiver = gameObject.GetComponent<IStatusEffectReceiver>();
			if(receiver != null)
				receiver.AddStatusEffect(m_Effect, caster);
		}
	}
}