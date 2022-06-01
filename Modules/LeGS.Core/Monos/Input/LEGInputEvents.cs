#if ENABLE_INPUT_SYSTEM
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LEGS
{
	[RequireComponent(typeof(PlayerInput))]
    public class LEGInputEvents : MonoBehaviour
    {
		[SerializeField] private Entity m_Entity;

		public int PlayerIndex => m_Input?.playerIndex ?? -1;
		public Action<LEGInputEventArgs> ActionTriggered;

        private PlayerInput m_Input;
		private ushort m_InputEventID;

		private void Start()
		{
			m_InputEventID = EventManager.RegisterEvent<LEGInputEventArgs>(LEGInputEventArgs.Name);
			m_Input = GetComponent<PlayerInput>();

			m_Input.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
			m_Input.onActionTriggered += OnActionTriggered;
		}

		private void OnDestroy() => m_Input.onActionTriggered -= OnActionTriggered;

		private void OnActionTriggered(InputAction.CallbackContext context)
		{
			if(context.phase == InputActionPhase.Performed)
				return; // Performed gets called once, at almost the same time as start, so just ignore it

			LEGInputEventArgs eventArgs = new LEGInputEventArgs(m_Entity, context, m_Input);
			EventManager.Publish(m_InputEventID, eventArgs);
			ActionTriggered?.Invoke(eventArgs);

			// Debug.Log($"'{m_Entity.DisplayName}' published action '{context.action.name}' {context.phase} (player #{m_Input.playerIndex})");
		}
	}
}
#endif