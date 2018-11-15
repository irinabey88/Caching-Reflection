namespace JsonSerializer.Atributes
{
    /// <summary>
    /// Represents a CacheAttribute.
    /// </summary>
    public class CachedAttribute : System.Attribute
    {
        public int ExpirationTimeMiliSeconds { get; }

        // <summary>
        /// Initializes a new instance of the <see cref="CachedAttribute"/>
        /// </summary>
        /// <param name="expirationTimeMiliSeconds"></param>
        public CachedAttribute(int expirationTimeMiliSeconds)
        {
            ExpirationTimeMiliSeconds = expirationTimeMiliSeconds;
        }
    }
}