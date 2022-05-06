using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLock : MonoBehaviour
{
	[SerializeField] private bool m_LockOnStartup = true;
	[SerializeField] private InputActionReference m_ToggleLockInput;

	[Space()]

	[SerializeField] private MonoBehaviour[] m_EnableWithMouse;
	[SerializeField] private MonoBehaviour[] m_DisableWithMouse;

	private void Start()
	{
		m_ToggleLockInput.action.started += ToggleLockPressed;

		if(m_LockOnStartup)
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
	}

	private void OnDestroy() => m_ToggleLockInput.action.started -= ToggleLockPressed;

	private void ToggleLockPressed(InputAction.CallbackContext _)
	{
		bool unlocked = Cursor.visible;
		Cursor.visible = !unlocked;
		Cursor.lockState = unlocked ? CursorLockMode.Locked : CursorLockMode.None;

		foreach(MonoBehaviour c in m_EnableWithMouse)
			c.enabled = unlocked;
		foreach(MonoBehaviour c in m_DisableWithMouse)
			c.enabled = !unlocked;
	}
}
