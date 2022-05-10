using LEGS;

namespace MOBAExample
{
	/// <summary>
	/// When a <see cref="MOBACharacter"/> has been altered. Often <see cref="MOBACharacter.Mana"/> or <see cref="CharacterExperience.Experience"/>
	/// </summary>
	public class CharacterChangedEventArgs : LEGEventArgs
	{
		public static string EventName => "MOBACharacterChanged";

		public MOBACharacter Character { get; private set; }

		public CharacterChangedEventArgs(MOBACharacter character) => Entity = Character = character;
	}

	/// <summary>
	/// When an <see cref="AbilityCastInfo"/> has been changed on attached <see cref="MOBAAbilityCaster"/>.
	/// Often <see cref="AbilityCastInfo.CooldownRemaining"/> has changed.
	/// </summary>
	public class AbilityCastInfoChangedEventArgs : LEGEventArgs
	{
		public static string EventName => "MOBAAbilityCastInfoChanged";
		
		public AbilityCastInfo Info { get; private set; }

		public AbilityCastInfoChangedEventArgs(AbilityCastInfo info)
		{
			Info = info;
			Entity = info.Caster.Character;
		}
	}

}