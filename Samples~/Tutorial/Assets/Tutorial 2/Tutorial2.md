# Tutorial 2 - Characters {#Tutorial2}

This tutorial will take you through creating a character that can receive damage and status effects.

> You can follow the *LeGS* [installation guide](@ref installation) if the package is not already in your project.
> 
> Sample project files can be imported with the package, but are not necessary to follow this tutorial.

## Adding a character & attributes
Add a [Character](@ref LEGS.Characters.Character) component to a GameObject in your scene,
the same object that has the [Simple Ability Caster](@ref LEGS.Abilities.SimpleAbilityCaster).

This behaviour implements the interfaces [IEntity](@ref LEGS.IEntity), [IDamageable](@ref LEGS.IDamageable),
[IStatusEffectReceiver](@ref LEGS.IStatusEffectReceiver) & [IAttributeHolder](@ref LEGS.Characters.IAttributeHolder).

To test out the attributes, create a new one and give it a name (e.g. *Strength*), and fill out the values as desired.

> `Modifier Multiply` multiplies the end result.
> A value of zero will result in zero while an output of one will return `BaseValue + ModifierAdd`

![Inspector of character class](https://media.githubusercontent.com/media/lcomstive/LeGS/gh-pages/Media/Tutorial%202/CharacterInspector.png)

## Modifying ability to use the attribute
To use the newly added attribute, we'll be modifying the `AbilityExplode` class from the previous tutorial.

Inside the `Activate` function, sometime before the `foreach` loop add some code to
calculate a damage value based on an attribute on the casting character.
```cs
float damage = 0; // Damage to apply to hit objects

if(gameObject.TryGetComponent(out Character character) &&
	character.HasAttribute("Strength")) // Check that character has attribute
{
	// CurrentValue is the calculated value, equal to
	//	(BaseValue + ModifierAdd) * ModifierMultiply
	damage = character.GetAttribute("Strength").CurrentValue;
}

// Iterate over all found colliders
foreach (Collider collider in colliders)
...
```

In the `foreach` loop, check each collider for an [IDamageable](@ref LEGS.IDamageable) and apply the calculated damage.

```cs
foreach(Collider collider in colliders)
{
	...

	if(collider.TryGetComponent(out IDamageable damageable))
		damageable.ApplyDamage(damage, caster);
	
	...
}
```

## Testing the attribute
When you use the explosion ability and hit a GameObject that has a collider and class that inherits from [IDamageable](@ref LEGS.IDamageable),
health will be reduced on that hit object based on the casting character's attribute.

To test this, add either a [Character](@ref LEGS.Characters.Character) or [Damageable](@ref LEGS.Damageable) component
to some objects in the scene and watch their health decrease when hit with your ability!

<br>
<b><i>Next: [Tutorial - Events](@ref Tutorial3)</i></b>