using System.Data;
using MicroDotNet.Packages.Orm.DatabaseAbstraction;
using Microsoft.Data.SqlClient;

namespace MicroDotNet.Packages.Orm.Connectors.AdoNet.IntegrationTests;

public class SqlDatabaseConnectionTests
    : IClassFixture<SqlServerFixture>
{
    private readonly SqlServerFixture sqlServer;

    public SqlDatabaseConnectionTests(SqlServerFixture sqlServer)
    {
        this.sqlServer = sqlServer ?? throw new ArgumentNullException(nameof(sqlServer));
    }

    [Fact]
    public async Task WhenCommandIsExecutedThenCorrectResultIsReturned()
    {
        var connection = new DatabaseConnection<SqlConnection>(this.sqlServer.ConnectionString);
        var result = await connection.ReadDataAsync(
            "[dbo].[ExtractTestData]",
            [new ParameterInfo("Id", 1, DbType.Int32)],
            dr => new Row(dr.GetInt32(0), dr.GetString(1)),
            CancellationToken.None);
        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(1);
        result[0].Id.ShouldBe(1);
        result[0].Text.ShouldBe("Value1");
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