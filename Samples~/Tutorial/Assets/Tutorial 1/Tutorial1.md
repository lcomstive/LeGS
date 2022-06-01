# Tutorial 1 - Abilities {#Tutorial1}

This tutorial will take you through creating an ability that spawns an object,
followed by steps for creating a custom ability that applies an outwards force to nearby rigidbodies.

> You can follow the *LeGS* [installation guide](@ref Installation) if the package is not already in your project.
> 
> Sample project files can be imported with the package, but are not necessary to follow this tutorial.

## Spawn objects ability
### Creating an ability
By using the asset menu (*often this is a right click in your assets window*), you can see example abilities under `Create > LeGS > Abilities`. For this tutorial we will select `Spawn Object`.

![Unity create ability using context menu](https://media.githubusercontent.com/media/lcomstive/LeGS/gh-pages/Media/Tutorial%201/CreateAbility.png)

Feel free to edit the ability's properties, such as name and description. There are tooltips on each parameter to explain their usage.

For the tutorial a cube will be used as the spawned prefab, but feel free to use any prefab available.
The spawned cube has a [Rigidbody](https://docs.unity3d.com/ScriptReference/Rigidbody.html) attached,
so the `Force` parameter will apply that amount of force upon creating the cube.

![Inspector of spawn ability](https://media.githubusercontent.com/media/lcomstive/LeGS/gh-pages/Media/Tutorial%201/SpawnObjectAbility.png)

### Using the ability
Attach a [SimpleAbilityCaster](@ref LEGS.Abilities.SimpleAbilityCaster) script to any GameObject in the active scene,
place your newly created ability in the `Ability` slot and assign a casting key.

Press play and activate your new ability!

![Spawning cubes](https://media.githubusercontent.com/media/lcomstive/LeGS/gh-pages/Media/Tutorial%201/SpawnCubes.gif)

## Creating a custom ability
Create a new script, below is the recommended code to start this new ability.

```cs
using LEGS;
using UnityEngine;
using LEGS.Abilities;

// Adds our ability to the asset context menu
[CreateAssetMenu(fileName = "Explode", menuName = "LeGS/Abilities/Explode")]
// Our class, which inherits from LEGS.Abilities.Ability
//
// Ability that detects Rigidbodies within a radius of caster and applies an
// instant force to them, in direction away from caster
public class AbilityExplode : Ability
{
	// Required
	// Called whenever the ability is to be cast 
	public override void Activate(IEntity caster, GameObject gameObject) { }
}
```

We will add three parameters in the class that will be used in the calculations during ability activation
```cs
// Radius around caster to check for rigidbodies
public float Radius = 10.0f;

// Instant force to apply to rigidbodies in area
public float Force = 12.5f;

// Explosion force to add in an upwards direction
public float UpwardsForce = 2.5f;
```

Next is to fill out the `Activate` function to execute the explosion

```cs
// Store the source of the explosion
Vector3 center = gameObject.transform.position;

// Get the collider on the casting object, used to avoid affecting itself
Collider casterCollider = gameObject.GetComponent<Collider>();

// Get all colliders within radius of gameObject (this includes colliders attached to the caster gameobject)
Collider[] colliders = Physics.OverlapSphere(center, Radius);

// Iterate over all found colliders
foreach(Collider collider in colliders)
{
	// Make sure the caster doesn't explode itself
	if(collider == casterCollider)
		continue; // Ignore this collider, keep looping

	// Check for rigidbody on collider
	if(collider.TryGetComponent(out Rigidbody rigidbody))
		// Apply explosive force
		rigidbody.AddExplosionForce(Force, center, Radius, UpwardsForce, ForceMode.Impulse);
}
```

Now you can follow the first part of the tutorial but with your own custom ability!

![Explosions!](https://media.githubusercontent.com/media/lcomstive/LeGS/gh-pages/Media/Tutorial%201/Explosion.gif)

<br>
<b><i>Next: [Tutorial - Characters](@ref Tutorial2)</i></b>