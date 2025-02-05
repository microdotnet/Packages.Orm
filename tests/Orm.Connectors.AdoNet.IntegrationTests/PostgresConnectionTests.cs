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
    public async Task WhenScalarRetrievalIsExecutedThenCorrectResultIsReturned()
    {
        var connection = new AdoNetDatabaseConnection<NpgsqlConnection>(this.fixture.ConnectionString);
        var result = await connection.ExecuteScalarAsync(
            "SELECT * FROM ExecuteScalar(@Increase)",
            CommandType.Text,
            [new ParameterInformation("Increase", 3, DbType.Int32)],
            CancellationToken.None);
        result.ShouldNotBeNull();
        result.ShouldBeOfType<long>();
        ((long)result).ShouldBe(7L);
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