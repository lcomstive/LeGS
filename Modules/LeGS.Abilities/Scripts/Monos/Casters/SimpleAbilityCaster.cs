using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace LEGS.Abilities
{
	/// <summary>
	/// Basic class that activates an ability on keypress,
	/// with cooldown functionality
	/// </summary>
	[RequireComponent(typeof(IEntity))]
	public class SimpleAbilityCaster : MonoBehaviour
	{
		[field: SerializeField] public Ability Ability { get; private set; }
		
		/// <summary>
		/// Casting entity
		/// </summary>
		public IEntity Entity { get; private set; }

		/// <summary>
		/// Visible name of attached entity
		/// </summary>
		public string DisplayName => Entity?.DisplayName ?? string.Empty;

		#if ENABLE_INPUT_SYSTEM
		[SerializeField] private InputActionReference m_CastAction;
		#else
		[SerializeField] private KeyCode m_CastKey = KeyCode.F;
		#endif

		public bool CanCast => CooldownRemaining <= 0;
		public float CooldownRemaining { get; private set; } = 0;

		private void Start() => Entity = GetComponent<IEntity>();

		private void Update()
		{
			CooldownRemaining = Mathf.Clamp(CooldownRemaining - Time.deltaTime, 0.0f, float.MaxValue);

#if ENABLE_INPUT_SYSTEM
			if(m_CastAction.action.IsPressed() && CanCast)
				Cast();
#else
			if(Input.GetKeyDown(m_CastKey) && CanCast)
				Cast();
#endif
		}

		public void Cast()
		{
			if(!CanCast || !Ability)
				return;
			Ability.Activate(Entity, gameObject);
			CooldownRemaining = Ability.Cooldown;
		}
	}
}
