# LeGS Basics

An [entity](interface_l_e_g_s_1_1_i_entity.html) represents an object in the LeGS system.

All entities have a name, intended for debugging and/or showing in-game.

## Interfaces
The most basic form of an entity is represented by the interface [IEntity](interface_l_e_g_s_1_1_i_entity.html); you can inherit from this interface for finer control and game-specific implementations.

All basic LeGS module types are interfaces, intended to be acquired through the `GetComponent` function. Using interfaces in this way provides flexibility and ensures systems work together regardless of implementation.

## Events
Events are used heavily within the LeGS package, specifically through the [EventManager](class_l_e_g_s_1_1_event_manager.html) class.

Registered events have an associated `ushort` ID, linked to their `string` ID - either can be passed through to most [EventManager](class_l_e_g_s_1_1_event_manager.html) functions, but the `ushort` version is preferred for faster operations.

### Creating Events
All valid events derive from the `LEGEventArgs` class, which contain two properties: the ID (`ushort`) of the source event, and the `IEntity` that sent the event (*can be null*).

Events must be registered with the [`EventManager`](class_l_e_g_s_1_1_event_manager.html) using a unique string identifier,
this function returns a `ushort` for the generated, or existing, event ID.

To remove an event, the `EventManager.Deregister` function can be used, either with a `ushort` or `string` ID.

#### Example
```cs
using LEGS;

public class CustomEventArgs : LEGEventArgs
{
	// Example property
	public string Details { get; private set; }

	public CustomEventArgs(string details, IEntity sender) : base(sender) // Call constructor of LEGEventArgs
	{
		Details = details;
	}
}

// Example custom MonoBehaviour
public class ExampleEventSender : MonoBehaviour
{
	public IEntity Entity = null; // Entity sending the event, can be set from another script

	// Cache the ID of the event from EventManager
	private ushort CustomEventID;

	// Register event
	void Start()
	{
		customEventID = EventManager.RegisterEvent<CustomEventArgs>("GameCustomEvent");
	}

	// Remove event
	void OnDestroy()
	{
		EventManager.DeregisterEvent(customEventID);
	}

	void Update()
	{
		if(Input.IsKeyDown(KeyCode.F)) // If 'F' is pressed, send new event
			EventManager.Publish<CustomEventArgs>(customEventID, new CustomEventArgs("F was pressed", Entity));
	}
}
```

### Listening to Events
The `Subscribe` & `Unsubscribe` functions are used to add and remove a listening function. These two functions use generics to identify the type of event expected.

Callback functions take exactly one parameter, matching the type passed as the generic for `Subscribe`.

#### Example
```cs
using LEGS;

public class ExampleEventListener : MonoBehaviour
{
	private ushort CustomEventID;

	void Start()
	{
		// Start listening to event
		EventManager.Subscribe<CustomEventArgs>("GameCustomEvent", OnCustomEvent);

		/*
		A ushort can be passed instead of the event name, for example from EventManager.GetEventID(..)

		If passing a string for the event name, as oppposed to the ushort version, a third boolean parameter
		can be passed to indicate that the event should be created & registered if it does not exist.
		Future registering of the event with the same name will retrieve the newly registered event.
		*/
	}

	void OnDestroy()
	{
		// Stop listening
		EventManager.Unsubscribe<CustomEventArgs>("GameCustomEvent", OnCustomEvent);
	}

	// One argument, same as used with Subscribe
	private void OnCustomEvent(CustomEventArgs args)
	{
		Debug.Log(args.Details);
	}
}
```

#### Built-In Events
| Event Name | Type | Description |
|-----------:|:----:|:------------|
| `EntityHealthChange` | `EntityHealthChangeEventArgs` | When an [`IDamageable`](interface_l_e_g_s_1_1_i_damageable.html)'s health changes |
| `EntitySpawn` | `LEGEventArgs` | When an [IEntity](interface_l_e_g_s_1_1_i_entity.html) gets created |
| `EntityDeath` | `EntityDeathEventArgs` | When an [IEntity](interface_l_e_g_s_1_1_i_entity.html) gets destroyed |
| `EntityStatusEffectChange` | `StatusEffectChangeArgs` | Event for changing of an [IStatusEffect](interface_l_e_g_s_1_1_i_status_effect.html) in an [IEntity](interfaceLEGS_1_1IEntity.html) |