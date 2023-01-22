using System;
using Xunit;

namespace Carter.Cache.Tests.Unit.Models;

public class UrlFixtures
{
    private const string SecureLocalhost = "https://localhost:5001";
    private const string Localhost = "http://localhost:5000";

    [Fact]
    public void Build_url_secure()
    {
        //Arrange
        var local = new Url(SecureLocalhost);

        //Act
        string result = local.SiteBase;

        //Assert
        Assert.True(local.IsSecure);
        Assert.Equal(SecureLocalhost, local.ToString());
        Assert.Equal(SecureLocalhost, result);
    }

    [Fact]
    public void Build_url()
    {
        //Arrange
        var local = new Url(Localhost);

        //Act
        string result = local.SiteBase;

        //Assert
        Assert.False(local.IsSecure);
        Assert.Equal(Localhost, local.ToString());
        Assert.Equal(Localhost, result);
    }

    [Fact]
    public void Build_url_with_ip_address()
    {
        //Arrange
        const string localIp = "http://127.0.0.1:5000";
        var local = new Url(localIp);

        //Act
        string result = local.ToString();

        //Assert
        Assert.False(local.IsSecure);
        Assert.Equal(localIp, local.ToString());
        Assert.Equal(localIp, result);
    }

    [Fact]
    public void Build_url_empty()
    {
        //Arrange
        var local = new Url();

        //Act
        int? resultPort = local.Port;
        string resultQuery = local.Query;

        //Assert
        Assert.Null(resultPort);
        Assert.Empty(resultQuery);
    }

    [Fact]
    public void Build_url_with_query()
    {
        //Arrange
        const string query = "?abc=def";
        var local = new Url(string.Concat(SecureLocalhost, query));

        //Act
        string resultQuery = local.Query;

        //Assert
        Assert.NotEmpty(resultQuery);
        Assert.Equal(query, resultQuery);
    }

    [Fact]
    public void Build_url_with_null_query()
    {
        //Arrange
        var local = new Url(SecureLocalhost)
        {
            Query = null
        };

        //Act
        string query = local.Query;

        //Assert
        Assert.Empty(query);
    }

    [Fact]
    public void Build_url_clone()
    {
        //Arrange
        var local = new Url(SecureLocalhost);

        //Act
        var result = local.Clone();

        //Assert
        Assert.Equal(result.ToString(), local.ToString());
    }

    [Fact]
    public void Build_url_sitebase()
    {
        //Arrange
        var local = new Url(SecureLocalhost);

        //Act
        string result = local.SiteBase;

        //Assert
        Assert.Equal(result, local.ToString());
    }

    [Fact]
    public void Build_url_implicit_operator_poco()
    {
        //Arrange
        var local = new Url(SecureLocalhost);

        //Act
        string result = local;

        //Assert
        Assert.Equal(result, local.ToString());
    }

    [Fact]
    public void Build_url_implicit_operator_string()
    {
        //Arrange
        var local = new Url(SecureLocalhost);

        //Act
        string result = (Url) SecureLocalhost;

        //Assert
        Assert.Equal(result, local.ToString());
    }

    [Fact]
    public void Build_url_implicit_operator_uri()
    {
        //Arrange
        var local = new Url(SecureLocalhost);

        //Act
        var result = (Uri) local;

        //Assert
        Assert.Equal(result.ToString(), string.Concat(local.ToString(), "/"));
    }

    [Fact]
    public void Build_url_implicit_operator_from_uri()
    {
        //Arrange
        const string localUri = "localhost/child/goes/here";
        var local = new Uri(localUri, UriKind.Relative);

        //Act
        var result = (Url) local;

        //Assert
        Assert.Equal(result.ToString(), string.Concat("http://", local.ToString()));
    }
}
