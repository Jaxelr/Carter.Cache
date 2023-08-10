using System.Collections.Generic;
using Carter.Cache.Keys;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Carter.Cache.Tests.Unit.Keys;

public class DefaultKeyGeneratorTests
{
    [Fact]
    public void Default_keyed_null()
    {
        //Arrange
        var keyGen = new DefaultKeyGenerator();

        //Act
        string key = keyGen.Get(null);

        //Assert
        Assert.Empty(key);
    }

    [Fact]
    public void Default_keyed_value()
    {
        //Arrange
        const string scheme = "http";
        const string host = "localhost";
        const int port = 80;
        const string path = "/hello";

        var request = A.Fake<HttpRequest>();

        A.CallTo(() => request.Scheme).Returns(scheme);
        A.CallTo(() => request.Host).Returns(new HostString(host, port));
        A.CallTo(() => request.Path).Returns(path);

        var keyGen = new DefaultKeyGenerator();

        //Act
        string key = keyGen.Get(request);

        //Assert
        Assert.Contains(scheme, key);
        Assert.Contains(host, key);
        Assert.Contains(port.ToString(), key);
        Assert.Contains(path, key);
    }

    [Fact]
    public void Default_keyed_value_with_query()
    {
        //Arrange
        const string scheme = "http";
        const string host = "localhost";
        const int port = 80;
        const string path = "/hello";
        const string queryId = "id";
        const string queryValue = "value";

        var request = A.Fake<HttpRequest>();

        var query = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
        {
            { queryId, new Microsoft.Extensions.Primitives.StringValues(queryValue) }
        };

        A.CallTo(() => request.Scheme).Returns(scheme);
        A.CallTo(() => request.Host).Returns(new HostString(host, port));
        A.CallTo(() => request.Path).Returns(path);
        A.CallTo(() => request.Query).Returns(new QueryCollection(query));

        var keyGen = new DefaultKeyGenerator();

        //Act
        string key = keyGen.Get(request);

        //Assert
        Assert.Contains(scheme, key);
        Assert.Contains(host, key);
        Assert.Contains(port.ToString(), key);
        Assert.Contains(path, key);
        Assert.Contains(queryId, key);
        Assert.Contains(queryValue, key);
    }

    [Fact]
    public void Default_keyed_value_with_accept_header()
    {
        //Arrange
        const string scheme = "http";
        const string host = "localhost";
        const int port = 80;
        const string path = "/hello";
        const string headerId = "Accept";
        const string headerValue = "application/json";

        var request = A.Fake<HttpRequest>();

        var headers = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
        {
            { headerId, new Microsoft.Extensions.Primitives.StringValues(headerValue) }
        };

        A.CallTo(() => request.Scheme).Returns(scheme);
        A.CallTo(() => request.Host).Returns(new HostString(host, port));
        A.CallTo(() => request.Path).Returns(path);
        A.CallTo(() => request.Headers).Returns(new HeaderDictionary(headers));

        var keyGen = new DefaultKeyGenerator();

        //Act
        string key = keyGen.Get(request);

        //Assert
        Assert.Contains(scheme, key);
        Assert.Contains(host, key);
        Assert.Contains(port.ToString(), key);
        Assert.Contains(path, key);
        Assert.Contains(headerId, key);
        Assert.Contains(headerValue, key);
    }

    [Fact]
    public void Default_keyed_value_with_form()
    {
        //Arrange
        const string scheme = "http";
        const string host = "localhost";
        const int port = 80;
        const string path = "/hello";
        const string formId = "formId";
        const string formValue = "formValue";

        var request = A.Fake<HttpRequest>();

        var forms = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
        {
            { formId, new Microsoft.Extensions.Primitives.StringValues(formValue) }
        };

        A.CallTo(() => request.Scheme).Returns(scheme);
        A.CallTo(() => request.Host).Returns(new HostString(host, port));
        A.CallTo(() => request.Path).Returns(path);
        A.CallTo(() => request.Form).Returns(new FormCollection(forms));
        A.CallTo(() => request.HasFormContentType).Returns(true);

        var keyGen = new DefaultKeyGenerator();

        //Act
        string key = keyGen.Get(request);

        //Assert
        Assert.Contains(scheme, key);
        Assert.Contains(host, key);
        Assert.Contains(port.ToString(), key);
        Assert.Contains(path, key);
        Assert.Contains(formId, key);
        Assert.Contains(formValue, key);
    }
}
