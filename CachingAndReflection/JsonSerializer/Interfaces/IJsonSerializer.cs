using JsonSerializer.Models;

namespace JsonSerializer.Interfaces
{
    public interface IJsonSerializer<T> where T : class
    {
        string Serialize (T source);

        DeserializedObject<T> Deserialize(string jsonObject);
    }
}