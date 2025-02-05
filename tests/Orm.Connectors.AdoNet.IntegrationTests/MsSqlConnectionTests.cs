using System.Data;
using MicroDotNet.Packages.Orm.DatabaseAbstraction;
using Microsoft.Data.SqlClient;

namespace MicroDotNet.Packages.Orm.Connectors.AdoNet.IntegrationTests;

public class MsSqlConnectionTests : IClassFixture<MsSqlServerFixture>
{
    private readonly MsSqlServerFixture fixture;

    public MsSqlConnectionTests(MsSqlServerFixture fixture)
    {
        this.fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
    }

    [Fact]
    public async Task WhenScalarRetrievalIsExecutedThenCorrectResultIsReturned()
    {
        var connection = new AdoNetDatabaseConnection<SqlConnection>(this.fixture.ConnectionString);
        var result = await connection.ExecuteScalarAsync(
            "[dbo].[ExecuteScalar]",
            CommandType.StoredProcedure,
            [new ParameterInformation("Increase", 3, DbType.Int32)],
            CancellationToken.None);
        result.ShouldNotBeNull();
        result.ShouldBeOfType<int>();
        ((int)result).ShouldBe(7);
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
}