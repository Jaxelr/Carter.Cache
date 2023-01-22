using System;
using System.Text;

namespace Carter.Cache.Tests.Fakes;

public class FakeCachedResponse : CachedResponse
{
    public FakeCachedResponse()
    {
        byte[] datum = Encoding.UTF8.GetBytes($"Hello world with custom id: {Guid.NewGuid()}");

        ContentType = "text/plain";
        Body = datum;
        ContentLength = datum.Length;
        StatusCode = 200;
    }
}
