# Tutorial 0 - Getting Familiar {#Tutorial0}

*LeGS* is built on a base of interface classes, with some simple implementations to get you started, and a handful of higher level classes
to showcase how these systems can work together.

## Interfaces

[IEntity](@ref LEGS.IEntity)s represent *LeGS* objects in the game world.

> A basic implementation is provided in [Entity](@ref LEGS.Entity)

<br>

[IStatusEffect](@ref LEGS.IStatusEffect)s can be added and removed from an [IStatusEffectReceiver](@ref LEGS.IStatusEffectReceiver).

> Basic implementations are provided in [StatusEffect](@ref LEGS.StatusEffect) and [TimedStatusEffect](@ref LEGS.TimedStatusEffect)

<br>

[IStatusEffectReceiver](@ref LEGS.IStatusEffectReceiver) is an [IEntity](@ref LEGS.Entity) that can have [IStatusEffect](@ref LEGS.IStatusEffect)s added and removed

> Basic implementation provided as part of [Character](LEGS.Characters.Character)

<br>

[IDamageable](@ref LEGS.IDamageable) objects have health and can receive damage.

> Basic implementation is provided in [Damageable](@ref LEGS.Damageable) and part of [Character](@ref LEGS.Characters.Character)

<br>

[ICharacter](@ref LEGS.Characters.ICharacter) inherits from [IEntity](@ref LEGS.IEntity), [IDamageable](@ref LEGS.IDamageable) & [IStatusEffectReceiver](@ref LEGS.IStatusEffectReceiver).

> Basic implementation is provided in [Character](@ref LEGS.Characters.Character).

## Tutorials
 - [Abilities](@ref Tutorial1)
 - [Characters](@ref Tutorial2)