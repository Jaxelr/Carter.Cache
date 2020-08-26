using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Carter.Cache.Keys
{
    public class DefaultKeyGenerator : ICacheKey
    {
        public string Get(HttpRequest request)
        {
            if (request == null)
            {
                return string.Empty;
            }

            var parameters = new Dictionary<string, string>();

            foreach (var queryKey in request.Query)
            {
                parameters[queryKey.Key] = queryKey.Value;
            }

            if (request.HasFormContentType)
            {
                foreach (var formKey in request.Form)
                {
                    parameters[formKey.Key] = formKey.Value;
                }
            }

            if (request.Headers.ContainsKey("Accept"))
            {
                parameters.Add("Accept", request.Headers["Accept"]);
            }

            var url = new Url
            {
                BasePath = request.PathBase,
                HostName = request.Host.Host,
                Path = request.Path,
                Port = request.Host.Port,
                Query = string.Concat((parameters.Count > 0 ? "?" : string.Empty), string.Join("&", parameters.Select(a => string.Join("=", a.Key, a.Value)))),
                Scheme = request.Scheme
            };

            return url.ToString();
        }
    }
}
