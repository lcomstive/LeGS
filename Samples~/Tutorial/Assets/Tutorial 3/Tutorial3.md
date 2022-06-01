# Tutorial 3 - Events {#Tutorial3}

Similar to [C# events](https://docs.microsoft.com/en-us/dotnet/standard/events/) and [UnityEvent](https://docs.unity3d.com/ScriptReference/Events.UnityEvent.html)s, [LEGS events](@ref LEGS.LEGEventArgs) can be [registered](@ref LEGS.EventManager#Register), [subscribed](@ref LEGS.EventManager#Subscribe<T>) and [published](@ref LEGS.EventManager#Publish<T>) using the [EventManager](@ref LEGS.EventManager).

> You can follow the *LeGS* [installation guide](@ref Installation) if the package is not already in your project.
> 
> Sample project files can be imported with the package, but are not necessary to follow this tutorial.

## Listening to events

A simple class that listens to entity death events might look like the following

```cs
using LEGS;
using UnityEngine;
using LEGS.Characters;

public class DeathEventListener : MonoBehaviour
{
	private ushort m_EventID;

	private void Start()
	{
		// Get the ID of the event. The reverse can be done using `EventManager.GetName(m_EventID)`
		m_EventID = EventManager.GetID(EntityDeathEventArgs.EventName);

		// m_EventID could be replaced with a name (string) instead.
		// EntityDeathEventArgs is derived from LEGEventArgs and is the
		//	single parameter passed to the callback (defined below)
		EventManager.Subscribe<EntityDeathEventArgs>(m_EventID, OnEntityDied);
	}

	private void OnDestroy()
	{
		EventManager.Unsubscribe<EntityDeathEventArgs>(m_EventID, OnEntityDied);
	}

	// Callback from EventManager
	private void OnEntityDied(EntityDeathEventArgs args)
	{
		Debug.Log($"'{args.Entity.DisplayName}' was killed by '{args.Killer.DisplayName}'");
	}
}
```

## Creating an event
The only requirements for creating an event is
 - the class has to derive from LEGS.LEGEventArgs, and
 - the constructor needs to call the base constructor with an [IEntity](LEGS.IEntity) representing the sender of the event

An example event definition
```cs
using LEGS;

public class DebugMessageEventArgs : LEGEventArgs
{
	public string DebugMessage { get; private set; }

	public DebugMessageEventArgs(IEntity sender, string debugMessage) : base(sender)
	{
		DebugMessage = debugMessage;
	}
}
```

Which can be used like 
```cs
// Registering the event informs the EventManager what type to expect and associatese a name with an ID
ushort eventID = EventManager.Register<DebugMessageEventArgs>("DebugMessage Event Name");

// Get the sender of the event, this can be null
IEntity sender = GetComponent<IEntity>();

// Publish can use either event IDs (ushort) or names (string)
EventManager.Publish<DebugMessageEventArgs>(eventID, new DebugMessageEventArgs("Test message", sender));
```