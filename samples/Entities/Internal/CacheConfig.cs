namespace Sample.Carter.Cache.Application.Entities
{
    public class CacheConfig
    {
        public bool CacheEnabled { get; set; }
        public int CacheTimespan { get; set; }
        public int CacheMaxSize { get; set; }
    }
}
