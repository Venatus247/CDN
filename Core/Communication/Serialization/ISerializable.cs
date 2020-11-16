namespace Core.Communication.Serialization
{
    public interface ISerializable<T> where T: class, ISerializable<T>, new()
    {
    }
}