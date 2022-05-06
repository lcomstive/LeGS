using UnityEngine;

namespace LEGS.Abilities
{
	[CreateAssetMenu(menuName = "LeGS/Abilities/Add Status Effect", fileName = "Add Status Effect")]
	public class AbilityAddStatusEffect : Ability
	{
		/// <summary>
		/// Status effect to apply to casting entity
		/// </summary>
		[Tooltip("Status effect to apply to casting entity")]
		[field: SerializeField] public StatusEffect Effect { get; private set; }

		public override void Activate(IEntity caster, GameObject gameObject)
		{
			IStatusEffectReceiver receiver = gameObject.GetComponent<IStatusEffectReceiver>();
			if(receiver != null)
				receiver.AddStatusEffect(Effect, caster);
		}
	}
}
