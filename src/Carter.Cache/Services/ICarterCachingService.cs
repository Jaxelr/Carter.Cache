using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Carter.Cache
{
    public interface ICarterCachingService
    {
        Task<bool> CheckCache(HttpContext context, CachingOptions options);

        Task SetCache(HttpContext context, CachedResponse response, CachingOptions options);
    }
}
