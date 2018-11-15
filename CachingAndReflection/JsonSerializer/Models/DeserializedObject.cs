using System;

namespace JsonSerializer.Models
{
    /// <summary>
    /// Represents a DeserializedObject{T}.
    /// </summary>
    /// <typeparam name="T">Type of deserializedObject.</typeparam>
    public class DeserializedObject<T> where T : class 
    {
        public T Object { get; set; }

        public int ExprTime { get; set; }

        public DateTime CreateTime { get; set; }
    }
}