using LEGS;
using UnityEngine;
using LEGS.Abilities;

namespace MOBAExample
{
	public enum AbilityCost
	{
		/// <summary>
		/// No cost of ability
		/// </summary>
		None,

		/// <summary>
		/// Consumes mana upon use
		/// </summary>
		Mana,

		/// <summary>
		/// Consumes health upon use
		/// </summary>
		Health
	}


	public enum AbilityTarget
	{
		/// <summary>
		/// Cast ability on self
		/// </summary>
		Self,

		/// <summary>
		/// Target must be another entity
		/// </summary>
		Entity,

		/// <summary>
		/// Target must be entity on opposing team
		/// </summary>
		Enemy,

		/// <summary>
		/// Target must be entity on ally team
		/// </summary>
		Ally,

		/// <summary>
		/// Cast in character or cursor direction (depends on implementation)
		/// </summary>
		Free
	}

	[CreateAssetMenu(fileName = "Ability", menuName = "MOBA/Abilities/Empty")]
	public class MOBAAbility : ScriptableObject, IAbility
	{
		[field: SerializeField] public Sprite Icon					{ get; private set; }
		[field: SerializeField] public string DisplayName			{ get; private set; } = "Ability";
		[field: SerializeField] public float Range					{ get; private set; } = 5.0f;
		[field: SerializeField] public float Cost					{ get; private set; } = 10.0f;
		[field: SerializeField] public AbilityCost CostType			{ get; private set; } = AbilityCost.Mana;
		[field: SerializeField] public AbilityTarget Target			{ get; private set; } = AbilityTarget.Free;
		[Tooltip("In seconds")]
		[field: SerializeField] public float Cooldown				{ get; private set; } = 2.5f;
		[field: SerializeField] public DamageTypes DamageType		{ get; private set; } = DamageTypes.Magical;

		[Tooltip("Scaling of appropriate damage type, or exact damage to apply if true damage")]
		[field: SerializeField] public float AttackDamageScale		{ get; private set; } = 1.0f;
		[field: SerializeField] public IStatusEffect CasterEffect	{ get; private set; } = null;
		[field: SerializeField] public IStatusEffect ReceiverEffect	{ get; private set; } = null;

		public virtual void Activate(IEntity caster, GameObject gameObject)
		{
			Debug.Log($"'{caster.DisplayName}' casting ability '{DisplayName}'");
		}
	}
}