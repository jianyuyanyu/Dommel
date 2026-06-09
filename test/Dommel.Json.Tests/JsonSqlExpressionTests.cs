using Xunit;

namespace Dommel.Json.Tests;

public class JsonSqlExpressionTests
{
    [Fact]
    public void GeneratesMySqlJsonValue()
    {
        // Arrange
        var sql = new JsonSqlExpression<Lead>(new MySqlSqlBuilder(), new DommelJsonOptions());

        // Act
        var str = sql.Where(p => p.Data.LastName == "Foo").ToSql(out var parameters);

        // Assert
        Assert.Equal(" where `Leads`.`Data`->'$.LastName' = @p1", str);
        Assert.Equal("p1", Assert.Single(parameters.ParameterNames));
    }

    [Fact]
    public void GeneratesSqlServerJsonValue()
    {
        // Arrange
        var sql = new JsonSqlExpression<Lead>(new SqlServerSqlBuilder(), new DommelJsonOptions());

        // Act
        var str = sql.Where(p => p.Data.LastName == "Foo").ToSql(out var parameters);

        // Assert
        Assert.Equal(" where JSON_VALUE([Leads].[Data], '$.LastName') = @p1", str);
        Assert.Equal("p1", Assert.Single(parameters.ParameterNames));
    }

    [Fact]
    public void GeneratesSqliteJsonValue()
    {
        // Arrange
        var sql = new JsonSqlExpression<Lead>(new SqliteSqlBuilder(), new DommelJsonOptions());

        // Act
        var str = sql.Where(p => p.Data.LastName == "Foo").ToSql(out var parameters);

        // Assert
        Assert.Equal(" where JSON_EXTRACT(\"Leads\".\"Data\", '$.LastName') = @p1", str);
        Assert.Equal("p1", Assert.Single(parameters.ParameterNames));
    }

    [Fact]
    public void GeneratesSqlServerCeJsonValue()
    {
        // Arrange
        var sql = new JsonSqlExpression<Lead>(new SqlServerCeSqlBuilder(), new DommelJsonOptions());

        // Act
        var str = sql.Where(p => p.Data.LastName == "Foo").ToSql(out var parameters);

        // Assert
        Assert.Equal(" where JSON_VALUE([Leads].[Data], '$.LastName') = @p1", str);
        Assert.Equal("p1", Assert.Single(parameters.ParameterNames));
    }

    [Fact]
    public void GeneratesPostgresJsonValue()
    {
        // Arrange
        var sql = new JsonSqlExpression<Lead>(new PostgresSqlBuilder(), new DommelJsonOptions());

        // Act
        var str = sql.Where(p => p.Data.LastName == "Foo").ToSql(out var parameters);

        // Assert
        Assert.Equal(" where \"Leads\".\"Data\"->>'LastName' = @p1", str);
        Assert.Equal("p1", Assert.Single(parameters.ParameterNames));
    }

    [Theory]
    [InlineData("Last'Name", "`Data`->'$.Last''Name'")]
    public void EscapesMySqlJsonPath(string path, string expected)
    {
        var builder = new MySqlSqlBuilder();

        var str = builder.JsonValue("`Data`", path);

        Assert.Equal(expected, str);
    }

    [Theory]
    [InlineData("Last'Name", "JSON_VALUE([Data], '$.Last''Name')")]
    public void EscapesSqlServerJsonPath(string path, string expected)
    {
        var builder = new SqlServerSqlBuilder();

        var str = builder.JsonValue("[Data]", path);

        Assert.Equal(expected, str);
    }

    [Theory]
    [InlineData("Last'Name", "JSON_EXTRACT(\"Data\", '$.Last''Name')")]
    public void EscapesSqliteJsonPath(string path, string expected)
    {
        var builder = new SqliteSqlBuilder();

        var str = builder.JsonValue("\"Data\"", path);

        Assert.Equal(expected, str);
    }

    [Theory]
    [InlineData("Last'Name", "JSON_VALUE([Data], '$.Last''Name')")]
    public void EscapesSqlServerCeJsonPath(string path, string expected)
    {
        var builder = new SqlServerCeSqlBuilder();

        var str = builder.JsonValue("[Data]", path);

        Assert.Equal(expected, str);
    }

    [Theory]
    [InlineData("Last'Name", "\"Data\"->>'Last''Name'")]
    public void EscapesPostgresJsonPath(string path, string expected)
    {
        var builder = new PostgresSqlBuilder();

        var str = builder.JsonValue("\"Data\"", path);

        Assert.Equal(expected, str);
    }
}
