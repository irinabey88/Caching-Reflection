using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EqualityComparer
{
    public class GenericEqualityComparer<T> : IEqualityComparer<T> where T : class
    {
        public bool Equals(T x, T y)
        {
            return IsEquals(x, y);
        }

        public bool IsEquals(T x, T y)
        {
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
            {
                if (!ReferenceEquals(x, y))
                {
                    return false;
                }
            }
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            var properties = GetProperties<T>();
            foreach (var property in properties)
            {
                var valueX = GetPropertyValue<T>(property.Name, x);
                var valueY = GetPropertyValue<T>(property.Name, y);
                
                if(property.PropertyType.IsArray)
                {
                    if (ReferenceEquals(valueX, null) || ReferenceEquals(valueY, null))
                    {
                        if (valueY == null && valueX == null)
                        {                            
                        }
                        else
                        {
                            return false;
                        }
                    }

                    if (!(valueY == null && valueX == null))
                    {
                        Array arrX = valueX as Array;
                        var lenghtX = arrX.Length;

                        Array arrY = valueY as Array;
                        var lenghtY = arrY.Length;

                        if (lenghtY != lenghtX)
                        {
                            return false;
                        }

                        for (var i = 0; i < lenghtX; i++)
                        {
                            var valX = arrX.GetValue(i);
                            var valY = arrY.GetValue(i);

                            if (valX.GetType() != valY.GetType())
                            {
                                return false;
                            }

                            if (valX.GetType().IsValueType || valX is string)
                            {
                                if (!valX.Equals(valY))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                var compare = CompareWithCurrentClassIsEqual(valX.GetType(), valueX, valueY);

                                if (!compare)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
                else if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                {
                    if (!valueX.Equals(valueY))
                    {
                        return false;
                    }
                }
                else
                {
                    var compare = CompareWithCurrentClassIsEqual(property.PropertyType, valueX, valueY);

                    if (!compare)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }

        private bool CompareWithCurrentClassIsEqual(Type type, object valueX, object valueY)
        {
            Type objectType = typeof(GenericEqualityComparer<>);
            Type[] typeArgs = { type };
            var genericType = objectType.MakeGenericType(typeArgs);
            var instance = Activator.CreateInstance(genericType);
            MethodInfo method = genericType.GetMethod("IsEquals");
            var compare = method?.Invoke(instance, new[] { valueX, valueY });

            return compare != null && (bool)compare;
        }

        private IEnumerable<PropertyInfo> GetProperties<T>()
        {
            var propertyInfos = typeof(T).GetProperties().ToList();

            return propertyInfos;
        }

        private object GetPropertyValue<T>(string propertyName, T source)
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
    }
}
