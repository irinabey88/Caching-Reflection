# Caching Reflection
# Homework
1. Generic equality comparer based on reflection. Must compare two objects and return true or false. Here are some requirements:
-	Generic
-	Can have availability to compare properties of non-primitive types (custom classes, arrays)
-	Full error handling (null reference, types equality)
2. Caching and reflection
Implement caching system using Redis cache (for example StackExchange.Redis nuget package). Classes which will be cached must be covered by special attribute which is allow to setup expiration time for objects of such classes. All objects must be cached in some text format (json or xml or csv). Serialization and deserialization of objects must be written on reflection and it must be generic. 
