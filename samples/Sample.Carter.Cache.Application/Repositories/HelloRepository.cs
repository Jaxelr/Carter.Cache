namespace Sample.Carter.Cache.Application.Repository;

public class HelloRepository : IHelloRepository
{
    public string SayHello(string name) => $"Hello world, your name is {name} the hour cached is {System.DateTime.Now}";
}
