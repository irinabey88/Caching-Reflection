using JsonSerializer;
using JsonSerializer.Models;
using NUnit.Framework;

namespace CachingAndReflectionTests
{
    [TestFixture]
    public class JsonSerializerTests
    {
        private readonly JsonSerializer<TestObject> _jsonSerializer;

        public JsonSerializerTests()
        {
            _jsonSerializer = new JsonSerializer<TestObject>();
        }

        [Test]
        public void Serialize_Object_ExpectNotEmpty()
        {
            var t1 = new TestObject() { Id = 1, Name = "Test", InnerObject = new InnerObject { Amount = 2, Customer = "Customer" },
                Array = new[] { 1, 2, 2 }               
            };
            
            var result = _jsonSerializer.Serialize(t1);

            Assert.IsNotEmpty(result);
        }

        [Test]
        public void Deserialize_Object_ExpectNotNul()
        {
            var t1 = new TestObject()
            {
                Id = 1,
                Name = "Test",
                InnerObject = new InnerObject { Amount = 2, Customer = "Customer" },
                Array = new[] { 1, 2, 2 },               
            };
            var str = _jsonSerializer.Serialize(t1);

            var result = _jsonSerializer.Deserialize(str);  
            
            Assert.IsNotNull(result.Object);
        }
    }
}
