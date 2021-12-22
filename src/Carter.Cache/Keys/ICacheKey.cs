using Microsoft.AspNetCore.Http;

namespace Carter.Cache.Keys;

public interface ICacheKey
{
    string Get(HttpRequest request);
}
