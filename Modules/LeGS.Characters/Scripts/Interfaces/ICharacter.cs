namespace LEGS.Characters
{
	/// <summary>
	/// For characters that can receive damage & status effects.
	/// 
	/// <para>Derives from <see cref="IEntity"/>, <see cref="IDamageable"/> & <see cref="IStatusEffectReceiver"/></para>
	/// </summary>
	public interface ICharacter : IEntity, IDamageable, IStatusEffectReceiver
	{

	}
}