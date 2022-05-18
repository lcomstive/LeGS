#if ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace LEGS
{
	[System.Serializable]
	public class DeviceChangeEvent
	{
		public string SchemeName = "Gamepad";
		public UnityEvent Event;
	}

	public class DeviceChangeEventArgs : LEGEventArgs
	{
		public static string Name => "OnInputDeviceChanged";

		public string NewControlScheme => Input.currentControlScheme;
		public PlayerInput Input { get; private set; }

		public DeviceChangeEventArgs(PlayerInput input, IEntity sender = null)
		{
			Input = input;
			Entity = sender;
		}
	}

	public class DeviceChangeListener : MonoBehaviour
	{
		[SerializeField]
		protected List<DeviceChangeEvent> m_Events = new List<DeviceChangeEvent>();

		private IEntity m_Entity;
		private ushort m_DeviceChangeEventID;

		private void Start()
		{
			m_DeviceChangeEventID = EventManager.RegisterEvent<DeviceChangeEventArgs>(DeviceChangeEventArgs.Name);

			PlayerInput playerInput;
			if(!TryGetComponent(out playerInput))
				// PlayerInput component is not on this gameObject, check the scene
				playerInput = FindObjectOfType<PlayerInput>();

			if(playerInput)
			{
				playerInput.onControlsChanged += OnControlsChanged;
				playerInput.controlsChangedEvent.AddListener(OnControlsChanged);
				OnControlsChanged(playerInput);
			}

			m_Entity = GetComponentInChildren<IEntity>();
			if(m_Entity == null)
				m_Entity = GetComponentInParent<IEntity>();
		}

		public void OnControlsChanged(PlayerInput playerInput)
		{
			Debug.Log($"Scheme switched to {playerInput.currentControlScheme} for Player {playerInput.playerIndex + 1}");

			foreach(DeviceChangeEvent e in m_Events)
				if(e.SchemeName.Equals(playerInput.currentControlScheme))
					e.Event.Invoke();

			EventManager.Publish(m_DeviceChangeEventID, new DeviceChangeEventArgs(playerInput, m_Entity));
		}
	}
}
#endif