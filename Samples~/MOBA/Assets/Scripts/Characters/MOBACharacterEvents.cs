using LEGS;

namespace MOBAExample
{
	public class CharacterChangedEventArgs : LEGEventArgs
	{
		public static string EventName => "MOBACharacterChanged";

		public MOBACharacter Character { get; private set; }

		public CharacterChangedEventArgs(MOBACharacter character) => Entity = Character = character;
	}
}