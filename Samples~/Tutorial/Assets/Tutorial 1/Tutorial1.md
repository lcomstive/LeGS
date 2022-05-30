# Tutorial 1 - Abilities {#Tutorial1}

## Helper Classes
There are a few helper scripts to get started with the ability system before needing to delve deeper into custom implementations.

### Simple Ability Caster
The [Simple Ability Caster](@ref LEGS.Abilities.SimpleAbilityCaster) component binds a single key input to an [ability](@ref LEGS.Abilities.Ability).

> This script is compatible with both the legacy and newer package input systems

### Ability Info
An [AbilityInfo](@ref LEGS.Abilities.AbilityInfo) component holds information about an
[ability](@ref LEGS.Abilities.IAbility) and the [entity](@ref LEGS.IEntity) that cast it.

### Ability
This abstract class is a [ScriptableObject](https://docs.unity3d.com/Manual/class-ScriptableObject.html) implementation of
the [IAbility](@ref LEGS.Abilities.IAbility) interface, with parameters for
a [display name](@ref LEGS.Abilities.Ability#DisplayName), [description](@ref LEGS.Abilities.Ability#Description),
and [cooldown](@ref LEGS.Abilities.Ability#Cooldown).

Some implementations have been provided, which can be accessed under the `LEGS/Abilities` asset context menu.

| Name | Description |
|-----:|:------------|
| [Add Status Effect](@ref LEGS.Abilities.AbilityAddStatusEffect) | Adds a [status effect](@ref LEGS.StatusEffect) onto the casting [entity](@ref LEGS.IEntity) |
| [Apply Force](@ref LEGS.Abilities.AbilityApplyForce) | Adds an instant force to casting [entity's](@ref LEGS.IEntity) attached [Rigidbody](https://docs.unity3d.com/ScriptReference/Rigidbody.html) |
| [Spawn Object](@ref LEGS.Abilities.AbilitySpawnObject) | Spawns a prefab at the casting [entity](@ref LEGS.IEntity), with an optional offset |


