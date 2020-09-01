using Carter.Cache.Keys;
using Carter.Cache.Stores;

namespace Carter.Cache
{
    public class CachingOption
    {
        public ICacheKey Key { get; set; }
        public ICacheStore Store { get; set; }

        public CachingOption() : this(new DefaultKeyGenerator(), new DefaultMemoryStore())
        {
        }

        public CachingOption(long maxSize) : this(new DefaultKeyGenerator(), new DefaultMemoryStore(maxSize))
        {
        }

        public CachingOption(ICacheKey key) : this(key, new DefaultMemoryStore())
        {
        }

        public CachingOption(ICacheStore store) : this(new DefaultKeyGenerator(), store)
        {
        }

        public CachingOption(ICacheKey key, ICacheStore store) => (Key, Store) = (key, store);
    }
}
