using System;
using Sample.Carter.Cache.Redis.Application.Repository;

namespace Carter.Cache.Sample.Redis.Tests.Mocks;

public class MockHelloRepository : IHelloRepository
{
    public string SayHello(string name) => $"Hello world, your name is {name} the hour cached is {DateTime.Now}";
}
