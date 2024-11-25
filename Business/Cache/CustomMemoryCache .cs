using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;

namespace Business.Helpers
{
    // Custom Memory Cache implementation
    public class CustomMemoryCache : IMemoryCache
    {
        private readonly Dictionary<object, object> _cache;
        private readonly Dictionary<object, DateTime> _expirationTimes;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   Teo, 25-Nov-24. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public CustomMemoryCache()
        {
            _cache = new Dictionary<object, object>();
            _expirationTimes = new Dictionary<object, DateTime>();
        }

        // Create an entry in the cache
        public ICacheEntry CreateEntry(object key)
        {
            return new CustomCacheEntry(key); // Return the custom cache entry
        }

        public void Dispose()
        {
            // Dispose of resources (if needed)
            _cache.Clear();
            _expirationTimes.Clear();
        }

        // Try to get a value from the cache.
        public bool TryGetValue(object key, out object value)
        {
            // Check if the item exists and has not expired
            if (_cache.ContainsKey(key) && _expirationTimes.ContainsKey(key) && _expirationTimes[key] > DateTime.Now)
            {
                value = _cache[key];
                return true;
            }

            value = null;
            return false;
        }

        // Remove an item from the cache
        public void Remove(object key)
        {
            _cache.Remove(key);
            _expirationTimes.Remove(key);
        }

        // Set a value in the cache with expiration time
        public void Set(object key, object value, TimeSpan absoluteExpirationRelativeToNow)
        {
            _cache[key] = value;
            _expirationTimes[key] = DateTime.Now.Add(absoluteExpirationRelativeToNow);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Custom cache entry class implementing ICacheEntry. </summary>
    ///
    /// <remarks>   Teo, 25-Nov-24. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class CustomCacheEntry : ICacheEntry
    {
        public object Key { get; }
        public object Value { get; set; }
        public DateTimeOffset? AbsoluteExpiration { get; set; }
        public TimeSpan? SlidingExpiration { get; set; }  // This is the property we added, with the correct type TimeSpan?
        public long Size { get; set; }
        public TimeSpan? AbsoluteExpirationRelativeToNow { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IList<IChangeToken> ExpirationTokens => throw new NotImplementedException();

        public IList<PostEvictionCallbackRegistration> PostEvictionCallbacks => throw new NotImplementedException();

        public CacheItemPriority Priority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        long? ICacheEntry.Size { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public CustomCacheEntry(object key)
        {
            Key = key;
        }

        public void Dispose()
        {
            // Dispose logic if needed
        }
    }
}
