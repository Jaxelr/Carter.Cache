using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Carter.Cache;

public interface ICarterCachingService
{
    Task<bool> CheckCache(HttpContext ctx, CachingOption options);

    Task SetCache(HttpContext ctx, CachedResponse response, CachingOption options);
}
