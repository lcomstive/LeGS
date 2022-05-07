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

		private void Start()
		{
			Entity = GetComponent<IEntity>();

#if ENABLE_INPUT_SYSTEM
			m_CastAction.action.performed += OnCastInput;
#endif
		}

#if ENABLE_INPUT_SYSTEM
		private void OnDestroy() => m_CastAction.action.performed -= OnCastInput;

		private void OnCastInput(InputAction.CallbackContext obj) => Cast();
#endif

		private void Update()
		{
			CooldownRemaining = Mathf.Clamp(CooldownRemaining - Time.deltaTime, 0.0f, float.MaxValue);

#if !ENABLE_INPUT_SYSTEM
			if(Input.GetKeyDown(m_CastKey))
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
