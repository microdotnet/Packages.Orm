using System.Data;
using MicroDotNet.Packages.Orm.DatabaseAbstraction;
using Npgsql;

namespace MicroDotNet.Packages.Orm.Connectors.AdoNet.IntegrationTests;

public class PostgresConnectionTests : IClassFixture<PostgresServerFixture>
{
    private readonly PostgresServerFixture fixture;

    public PostgresConnectionTests(PostgresServerFixture fixture)
    {
        this.fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
    }

    [Fact]
    public async Task WhenDataRetrievalIsExecutedThenCorrectResultIsReturned()
    {
        var connection = new AdoNetDatabaseConnection<NpgsqlConnection>(this.fixture.ConnectionString);
        var result = await connection.ExecuteProcedureAsync(
            "SELECT * FROM ExtractTestData(@Id)",
            CommandType.Text,
            [new ParameterInformation("Id", 1, DbType.Int32)],
            [],
            dr => new Row(dr.GetInt32(0), dr.GetString(1)),
            CancellationToken.None);
        result.ShouldNotBeNull();
        result.Rows.Count.ShouldBe(1);
        result.Rows[0].Id.ShouldBe(1);
        result.Rows[0].Text.ShouldBe("Value1");
    }

    [Fact]
    public async Task WhenScalarRetrievalIsExecutedThenCorrectResultIsReturned()
    {
        var connection = new AdoNetDatabaseConnection<NpgsqlConnection>(this.fixture.ConnectionString);
        var result = await connection.ExecuteProcedureAsync(
            "SELECT CountTestData(@Increase)",
            CommandType.Text,
            [new ParameterInformation("Increase", 3, DbType.Int32)],
            [],
            dr => new CountResult(dr.GetInt32(0)),
            CancellationToken.None);
        result.ShouldNotBeNull();
        result.Rows.Count.ShouldBe(1);
        result.Rows[0].Value.ShouldBe(7);
    }

    private class Row
    {
        public Row(int id, string text)
        {
            this.Id = id;
            this.Text = text;
        }

        public int Id { get; }

        public string Text { get; }
    }

    private class CountResult
    {
        public CountResult(
            int value)
        {
            this.Value = value;
        }

        public int Value { get; }
    }
}