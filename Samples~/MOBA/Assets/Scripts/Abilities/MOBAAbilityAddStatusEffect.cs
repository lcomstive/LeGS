using LEGS;
using UnityEngine;

namespace MOBAExample
{
	/// <summary>
	/// Applies a status effect to the casting entity
	/// </summary>
	[CreateAssetMenu(menuName = "MOBA/Abilities/Add Status Effect", fileName = "Add Status Effect")]
	public class MOBAAbilityAddStatusEffect : MOBAAbility
	{
		/// <summary>
		/// Status effect to apply to casting <see cref="IEntity"/>
		/// </summary>
		[Tooltip("Status effect to apply to casting entity")]
		[field: SerializeField] public StatusEffect Effect { get; private set; }

		/// <summary>
		/// Adds <see cref="Effect"/> to <paramref name="caster"/>,
		/// if <paramref name="caster"/> derives from <see cref="IStatusEffectReceiver"/>
		/// </summary>
		public override void Activate(IEntity caster, GameObject gameObject)
		{
			if(gameObject.TryGetComponent<IStatusEffectReceiver>(out var receiver))
				receiver.AddStatusEffect(Effect, caster);
		}
	}
}
