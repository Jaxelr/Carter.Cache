﻿using System;
using Sample.Carter.Cache.Memcached.Application.Repository;

namespace Carter.Cache.Sample.Memcached.Tests.Mocks;

public class MockHelloRepository : IHelloRepository
{
    public string SayHello(string name) => $"Hello world, your name is {name} the hour cached is {DateTime.Now}";
}
