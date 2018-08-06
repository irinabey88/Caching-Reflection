using JsonSerializer.Models;

namespace JsonSerializer.Interfaces
{
    public interface ICacheService
    {
        void SaveData(TestObject testObject);

        TestObject GetData(int id);
    }
}