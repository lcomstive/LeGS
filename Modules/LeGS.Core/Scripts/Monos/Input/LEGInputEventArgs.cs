#if ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;

namespace LEGS
{
	public class LEGInputEventArgs : LEGEventArgs
	{
		public static string Name => "LEGInputEvent";

		public PlayerInput PlayerInput { get; private set; }
		public InputAction.CallbackContext Context { get; private set; }

		public LEGInputEventArgs(IEntity sender, InputAction.CallbackContext context, PlayerInput input)
		{
			Entity = sender;
			Context = context;
			PlayerInput = input;
		}
	}
}
#endif