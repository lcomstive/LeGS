namespace LEGS
{
    /// <summary>
    /// An object that can be serialized & deserialized using a <see cref="DataStream"/>
    /// </summary>
    public interface ISerializable
    {
        void Serialize(ref DataStream stream);

        static ISerializable Deserialize(DataStream stream) => null;
    }
}
