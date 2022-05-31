# Tutorial 0 - Getting Familiar {#Tutorial0}

*LeGS* is built on interfaces & events, with some simple implementations to get you started.

## Interfaces
 - [IEntity](@ref LEGS.IEntity)s represent *LeGS* objects in the game world.
 - [IStatusEffect](@ref LEGS.IStatusEffect)s can be added and removed from an [IStatusEffectReceiver](@ref LEGS.IStatusEffectReceiver)
 - [IStatusEffectReceiver](@ref LEGS.IStatusEffectReceiver) is an [IEntity](@ref LEGS.Entity) that can have [IStatusEffect](@ref LEGS.IStatusEffect)s added and removed
 - [IDamageable](@ref LEGS.IDamageable) objects have health and can receive damage.
 - [ICharacter](@ref LEGS.Characters.ICharacter) inherits from [IEntity](@ref LEGS.IEntity), [IDamageable](@ref LEGS.IDamageable) & [IStatusEffectReceiver](@ref LEGS.IStatusEffectReceiver).

## Events
The [EventManager](@ref LEGS.EventManager) handles LeGS events, all of which will be derived from [LEGEventArgs](@ref LEGS.LEGEventArgs).

Events can be [registered](@ref LEGS.EventManager#RegisterEvent), [subscribed](@ref LEGS.EventManager#Subscribe<T>) and [published](@ref LEGS.EventManager#Publish<T>).

| Name | Description |
|-----:|:------------|
| [EntityHealthChangeEventArgs](@ref LEGS.EntityHealthChangeEventArgs) | [Damageable](@ref LEGS.IDamageable)'s health change |
| [EntityDeathEventArgs](@ref LEGS.EntityDeathEventArgs) | [Damageable](@ref LEGS.IDamageable)'s health has reached zero |
| [StatusEffectChangeArgs](@ref LEGS.StatusEffectChangeArgs) | A [status effect](@ref LEGS.IStatusEffect) has been added or removed from a [receiver](@ref LEGS.IStatusEffectReceiver) |
| [AttributeEventArgs](@ref LEGS.Characters.AttributeEventArgs) | An [IAttributeHolder](@ref LEGS.Characters.IAttributeHolder)'s attribute has been added, removed or modified |
| [QuestEventArgs](@ref LEGS.Quests.QuestEventArgs) | Event for changes in a [quest](@ref LEGS.Quests.Quest) |

## Tutorials
 - [Abilities](@ref Tutorial1)
 - [Characters](@ref Tutorial2)