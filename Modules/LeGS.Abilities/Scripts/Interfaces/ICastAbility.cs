using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LEGS.Abilities
{
    /// <summary>
    /// Ability that has been casted
    /// </summary>
    public interface ICastAbility : IEntity
    {
        IEntity Caster { get; }
        IAbility Ability { get; }
    }
}
