using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using JsonSerializer.Atributes;
using JsonSerializer.Interfaces;
using JsonSerializer.Models;
using Newtonsoft.Json.Linq;

namespace JsonSerializer
{
    public class JsonSerializer<T> : IJsonSerializer<T> where T : class 
    {
        public DeserializedObject<T> Deserialize(string jsonObject)
        {
            if (string.IsNullOrEmpty(jsonObject))
            {
                throw new ArgumentNullException(nameof(jsonObject));
            }

            var instance = Activator.CreateInstance(typeof(T));
            var properties = typeof(T).GetProperties().ToList(); ;

            JObject json = JObject.Parse(jsonObject);

            var dateTag = json.SelectToken($"CreationDate");
            var dateValue = dateTag.Value<string>();

            var attribute = typeof(T).GetCustomAttributes(typeof(CachedAttribute), true);
            var resultDeserialization = new DeserializedObject<T>();

            if (attribute == null || attribute.Length == 0)
            {
                throw new ArgumentException($"Object is not cached");
            }
            var attr = attribute[0] as CachedAttribute;
            var exprTime = attr.ExpirationTimeMiliSeconds;

            DateTime dt =
                DateTime.ParseExact(dateValue, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            resultDeserialization.CreateTime = dt;
            resultDeserialization.ExprTime = exprTime;

            if (DateTime.UtcNow.Millisecond - dt.Millisecond > exprTime)
            {
                resultDeserialization.Object = null;
                return resultDeserialization;
            }


            foreach (var property in properties)
            {
                var location = json.SelectToken($"Object.{property.Name}");

                if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                {
                    Type propertyType = property.PropertyType;
                    var propertyValue = location.Value<string>();
                    TypeConverter converter = TypeDescriptor.GetConverter(propertyType);
                    var value = converter.ConvertFromString(propertyValue);

                    property.SetValue(instance, value);
                }
                else if(property.PropertyType.IsArray)
                {
                    if (property.PropertyType.GetElementType().IsValueType ||
                        property.PropertyType.GetElementType().IsValueType is string)
                    {
                        Type propertyType = property.PropertyType;

                        if (location.HasValues)
                        {
                            var values = location?.Values<string>();
                            if (values != null)
                            {
                                var typedArray =
                                    Array.CreateInstance(property.PropertyType.GetElementType(), values.ToList().Count);
                                TypeConverter converter = TypeDescriptor.GetConverter(propertyType.GetElementType());

                                int i = 0;
                                foreach (var element in values)
                                {
                                    var value = converter.ConvertFromString(element);
                                    typedArray.SetValue(value, i);
                                }

                                property.SetValue(instance, typedArray);
                            }
                        }
                    }
                    else
                    {
                        //TODO Not implements for innner not value type
                    }
                }
                else
                {
                    var desObj = DeserializeObject(property, json, $"Object.{property.Name}");                  
                    property.SetValue(instance, desObj);
                }
            }

            resultDeserialization.Object = (T) instance;
            return resultDeserialization;
        }
        
        public string Serialize(T source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            StringBuilder jsonString = new StringBuilder($"{{ \"CreationDate\" : \"{DateTime.UtcNow.ToString($@"dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}\",");

            var innerJson = SerializeObject(source);

            jsonString.Append($"\"Object\" : {innerJson}");

            jsonString.Append($"}}");
            return jsonString.ToString();
        }

        public string SerializeObject(T source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            StringBuilder jsonString = new StringBuilder($"{{");
            var properties = typeof(T).GetProperties().ToList(); ;
            foreach (var property in properties)
            {
                var propertyValue = GetPropertyValueT(property.Name, source);
                jsonString.Append($"\"{property.Name}\" :");

                if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                {
                    jsonString.Append($"\"{propertyValue}\",");
                }
                else if (property.PropertyType.IsArray)
                {
                    if (propertyValue == null)
                    {
                        jsonString.Append($"null");
                    }
                    else
                    {
                        Array array = propertyValue as Array;
                        var arrayLength = array?.Length;

                        jsonString.Append($@"[");
                        for (var i = 0; i < arrayLength; i++)
                        {
                            var value = array.GetValue(i);
                            if (value.GetType().IsValueType || value is string)
                            {
                                jsonString.Append($"\"{value}\"");
                            }
                            else
                            {
                                var innerJson = SerializeObject(value.GetType(), value);
                                jsonString.Append($"{innerJson}");
                            }
                            if (i < arrayLength - 1)
                            {
                                jsonString.Append($",");
                            }
                        }

                        jsonString.Append($@"],");
                    }
                }
                else
                {
                    var innerJson = SerializeObject(propertyValue.GetType(), propertyValue);
                    jsonString.Append($"{innerJson},");
                }
            }

            if (jsonString.ToString().EndsWith(","))
            {
                jsonString.Remove(jsonString.Length - 1, 1);
            }
            jsonString.Append($"}}");
            return jsonString.ToString();
        }

        public object DeserializeObject(PropertyInfo propertyInfo, JObject jsonObj, string innerProperty)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }
            if (jsonObj == null)
            {
                throw new ArgumentNullException(nameof(jsonObj));
            }
            if (string.IsNullOrEmpty(innerProperty))
            {
                throw new ArgumentNullException(innerProperty);
            }

            object instance = Activator.CreateInstance(propertyInfo.PropertyType);
            var properties = propertyInfo.PropertyType.GetProperties().ToList();

            foreach (var property in properties)
            {
                var location = jsonObj.SelectToken($"{innerProperty}.{property.Name}");

                if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                {
                    Type propertyType = property.PropertyType;
                    var propertyValue = location.Value<string>();
                    TypeConverter converter = TypeDescriptor.GetConverter(propertyType);
                    var value = converter.ConvertFromString(propertyValue);

                    property.SetValue(instance, value);
                }
                else if (property.PropertyType.IsArray)
                {
                    if (property.PropertyType.GetElementType().IsValueType ||
                        property.PropertyType.GetElementType().IsValueType is string)
                    {
                        Type propertyType = property.PropertyType;

                        var values = location.Values<string>();
                        var typedArray =
                            Array.CreateInstance(property.PropertyType.GetElementType(), values.ToList().Count);
                        TypeConverter converter = TypeDescriptor.GetConverter(propertyType.GetElementType());

                        int i = 0;
                        foreach (var element in values)
                        {
                            var value = converter.ConvertFromString(element);
                            typedArray.SetValue(value, i);
                        }

                        property.SetValue(instance, typedArray);
                    }
                    else
                    {
                        //TODO Not implements for innner not value type
                    }
                }
                else
                {
                    Type objectType = typeof(JsonSerializer<>);
                    Type[] typeArgs = { property.PropertyType };
                    var genericType = objectType.MakeGenericType(typeArgs);
                    var activatorInstance = Activator.CreateInstance(genericType);
                    MethodInfo method = genericType.GetMethod("DeserializeObject");
                    var json = method?.Invoke(activatorInstance, new object[] { property, jsonObj, $"{innerProperty}.{property.Name}" });

                    return (string)json;
                }
            }

            return instance;
        }

        private object GetPropertyValueT(string propertyName, T source)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException(propertyName);
            }
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source.GetType().GetProperty(propertyName)?.GetValue(source, null);
        }

        private string SerializeObject(Type type, object source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            Type objectType = typeof(JsonSerializer<>);
            Type[] typeArgs = { type };
            var genericType = objectType.MakeGenericType(typeArgs);
            var instance = Activator.CreateInstance(genericType);
            MethodInfo method = genericType.GetMethod("SerializeObject");
            var json = method?.Invoke(instance, new[] { source });

            return (string)json;
        }
    }
}
