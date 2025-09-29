using System.Text.Json;

namespace Carter.Cache.Tests.Fakes;

public static class FakeJsonSerializer
{
    public static readonly JsonSerializerOptions Options = new() { PropertyNameCaseInsensitive = true };
}
