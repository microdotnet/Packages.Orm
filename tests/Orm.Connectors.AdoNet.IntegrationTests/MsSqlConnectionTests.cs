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
    public async Task WhenDataRetrievalIsExecutedThenCorrectResultIsReturned()
    {
        var connection = new AdoNetDatabaseConnection<SqlConnection>(this.fixture.ConnectionString);
        var result = await connection.ExecuteProcedureAsync(
            "[dbo].[ExtractTestData]",
            CommandType.StoredProcedure,
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
        var connection = new AdoNetDatabaseConnection<SqlConnection>(this.fixture.ConnectionString);
        var result = await connection.ExecuteProcedureAsync(
            "[dbo].[CountTestData]",
            CommandType.StoredProcedure,
            [new ParameterInformation("Increase", 3, DbType.Int32)],
            [],
            _ => new NoRowsResult(),
            CancellationToken.None);
        result.ShouldNotBeNull();
        result.ReturnValue.ShouldBe(7);
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