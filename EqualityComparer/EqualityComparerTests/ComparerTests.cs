using EqualityComparer;
using NUnit.Framework;

namespace EqualityComparerTests
{
    [TestFixture]
    public class ComparerTests
    {
        private GenericEqualityComparer<TestObject> _equalityComparer;

        public ComparerTests()
        {
            _equalityComparer = new GenericEqualityComparer<TestObject>();
        }

        [Test]
        public void Equals_SameProperties_ExpectTrue()
        {
            var t1 = new TestObject() { Id = 1, Name = "Test", InnerObject = new InnerObject { Amount = 2, Customer = "Customer" }, Array = new []{1, 2, 2}};
            var t2 = new TestObject() { Id = 1, Name = "Test", InnerObject = new InnerObject { Amount = 2, Customer = "Customer" }, Array = new[] { 1, 2, 2 } };

            var result = _equalityComparer.Equals(t1, t2);

            Assert.IsTrue(result);
        }
        [Test]
        public void Equals_DiffrentStringPropertiesInTestObject_ExpectFalse()
        {
            var t1 = new TestObject() { Id = 1, Name = "Test", InnerObject = new InnerObject { Amount = 2, Customer = "Customer" }, Array = new[] { 1, 2, 2 } };
            var t2 = new TestObject() { Id = 1, Name = "Test1", InnerObject = new InnerObject { Amount = 2, Customer = "Customer1" }, Array = new[] { 1, 2, 2 } };

            var result = _equalityComparer.Equals(t1, t2);

            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_DiffrentIntPropertiesInTestObject_ExpectFalse()
        {
            var t1 = new TestObject() { Id = 1, Name = "Test", InnerObject = new InnerObject { Amount = 2, Customer = "Customer" }, Array = new[] { 1, 2, 2 } };
            var t2 = new TestObject() { Id = 6, Name = "Test", InnerObject = new InnerObject { Amount = 2, Customer = "Customer1" }, Array = new[] { 1, 2, 2 } };

            var result = _equalityComparer.Equals(t1, t2);

            Assert.IsFalse(result);
        }


        [Test]
        public void Equals_DiffrentStringPropertiesInInnerOject_ExpectFalse()
        {
            var t1 = new TestObject() { Id = 1, Name = "Test", InnerObject = new InnerObject { Amount = 2, Customer = "Customer"}, Array = new[] { 1, 2, 2 } };
            var t2 = new TestObject() { Id = 1, Name = "Test", InnerObject = new InnerObject { Amount = 2, Customer = "Customer1" }, Array = new[] { 1, 2, 2 } };

            var result = _equalityComparer.Equals(t1, t2);

            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_DiffrentIntPropertiesInInnerOject_ExpectFalse()
        {
            var t1 = new TestObject() { Id = 1, Name = "Test", InnerObject = new InnerObject { Amount = 2, Customer = "Customer" }, Array = new[] { 1, 2, 2 } };
            var t2 = new TestObject() { Id = 1, Name = "Test", InnerObject = new InnerObject { Amount = 4, Customer = "Customer" }, Array = new[] { 1, 2, 2 } };

            var result = _equalityComparer.Equals(t1, t2);

            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_DiffrentInInnerOjectNull_ExpectFalse()
        {
            var t1 = new TestObject() { Id = 1, Name = "Test", InnerObject =  null, Array = new[] { 1, 2, 2 } };
            var t2 = new TestObject() { Id = 1, Name = "Test", InnerObject = new InnerObject { Amount = 4, Customer = "Customer" }, Array = new[] { 1, 2, 2 } };

            var result = _equalityComparer.Equals(t1, t2);

            Assert.IsFalse(result);
        }
        [Test]
        public void Equals_BothInInnerOjectNull_ExpectTrue()
        {
            var t1 = new TestObject() { Id = 1, Name = "Test", InnerObject = null, Array = new[] { 1, 2, 2 } };
            var t2 = new TestObject() { Id = 1, Name = "Test", InnerObject = null, Array = new[] { 1, 2, 2 } };

            var result = _equalityComparer.Equals(t1, t2);

            Assert.IsTrue(result);
        }

        [Test]
        public void Equals_DiffrenPropertiesArray_ExpectFalse()
        {
            var t1 = new TestObject() { Id = 1, Name = "Test", InnerObject = new InnerObject { Amount = 2, Customer = "Customer" }, Array = new[] { 1, 2, 2 } };
            var t2 = new TestObject() { Id = 1, Name = "Test", InnerObject = new InnerObject { Amount = 2, Customer = "Customer" }, Array = new[] { 1, 3, 2 } };

            var result = _equalityComparer.Equals(t1, t2);

            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_DiffrentPropertiesArrayLength_ExpectFalse()
        {
            var t1 = new TestObject() { Id = 1, Name = "Test", InnerObject = new InnerObject { Amount = 2, Customer = "Customer" }, Array = new[] { 1, 2, 2, 7 } };
            var t2 = new TestObject() { Id = 1, Name = "Test", InnerObject = new InnerObject { Amount = 2, Customer = "Customer" }, Array = new[] { 1, 2, 2 } };

            var result = _equalityComparer.Equals(t1, t2);

            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_DiffrentPropertiesArrayIsNull_ExpectFalse()
        {
            var t1 = new TestObject() { Id = 1, Name = "Test", InnerObject = new InnerObject { Amount = 2, Customer = "Customer" }, Array = null };
            var t2 = new TestObject() { Id = 1, Name = "Test", InnerObject = new InnerObject { Amount = 2, Customer = "Customer" }, Array = new[] { 1, 2, 2 } };

            var result = _equalityComparer.Equals(t1, t2);

            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_DiffrentPropertiesArrayIsNull_ExpectTrue()
        {
            var t1 = new TestObject() { Id = 1, Name = "Test", InnerObject = new InnerObject { Amount = 2, Customer = "Customer" }, Array = null };
            var t2 = new TestObject() { Id = 1, Name = "Test", InnerObject = new InnerObject { Amount = 2, Customer = "Customer" }, Array = null};

            var result = _equalityComparer.Equals(t1, t2);

            Assert.IsTrue(result);
        }

    }
}
