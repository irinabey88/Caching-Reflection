using System;
using JsonSerializer.Interfaces;
using JsonSerializer.Models;
using JsonSerializer.Service;

namespace CachingAndReflection
{
    class Program
    {
        static void Main(string[] args)
        {
            ICacheService service = new CachedService();

            service.SaveData(new TestObject() {
                Id = 1, Name = "Test",
                InnerObject = new InnerObject
                {
                    Amount = 2,
                    Customer = "Customer"
                }});

            service.SaveData(new TestObject()
            {
                Id = 2,
                Name = "Test2",
                InnerObject = new InnerObject
                {
                    Amount = 3,
                    Customer = "Customer2"
                }
            });

            service.SaveData(new TestObject()
            {
                Id = 3,
                Name = "Test3",
                InnerObject = new InnerObject
                {
                    Amount = 3,
                    Customer = "Customer3"
                }
            });

            var getData = service.GetData(1);
            Console.WriteLine($"{getData.Id}-{getData.Name}");
            var getData2 = service.GetData(2);
            Console.WriteLine($"{getData2.Id}-{getData2.Name}");
            var getData3 = service.GetData(1);
            Console.WriteLine($"{getData3.Id}-{getData3.Name}");

            Console.ReadLine();

        }
    }
}
