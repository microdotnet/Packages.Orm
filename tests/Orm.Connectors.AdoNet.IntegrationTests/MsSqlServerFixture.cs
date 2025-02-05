using System.Data;
using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;

namespace MicroDotNet.Packages.Orm.Connectors.AdoNet.IntegrationTests;

public class MsSqlServerFixture : IAsyncLifetime
{
    private const string AdminPassword = @"T3stC0NT/\|NERs";
    
    // private static readonly Lazy<SqlServerFixture> LazyInstance = new(() => new SqlServerFixture());

    private readonly MsSqlContainer container;

    public MsSqlServerFixture()
    {
        var builder = new MsSqlBuilder()
            .WithPassword(AdminPassword);
        this.container = builder.Build();
    }
    
    // public static SqlServerFixture Instance => LazyInstance.Value;

    public string ConnectionString => this.container.GetConnectionString();
    
    public async Task InitializeAsync()
    {
        await this.container.StartAsync();
        await this.CreateTestDatabase();
    }

    public async Task DisposeAsync()
    {
        await this.container.DisposeAsync();
    }

    private static async Task ExecuteCommand(SqlConnection connection, string commandText)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = commandText;
        command.CommandType = CommandType.Text;
        await command.ExecuteNonQueryAsync();
    }

    private async Task CreateTestDatabase()
    {
        await using var connection = new SqlConnection(this.ConnectionString);
        await connection.OpenAsync();
        const string createTestTableCommandText =
            """
            CREATE TABLE [dbo].[TestTable]
            (
                [Id] INT NOT NULL IDENTITY(1, 1) PRIMARY KEY,
                [Text] NVARCHAR(50) NOT NULL
            )
            """;
        await ExecuteCommand(connection, createTestTableCommandText);
        const string fillTestTableWithDataCommandText =
            """
            INSERT INTO [dbo].[TestTable] ([Text]) VALUES ('Value1');
            INSERT INTO [dbo].[TestTable] ([Text]) VALUES ('Value2');
            INSERT INTO [dbo].[TestTable] ([Text]) VALUES ('Value3');
            INSERT INTO [dbo].[TestTable] ([Text]) VALUES ('Value4');
            """;
        await ExecuteCommand(connection, fillTestTableWithDataCommandText);
        const string createExecuteScalarProcedure =
            """
            CREATE PROCEDURE [dbo].[ExecuteScalar]
            (
                @Increase INT
            )
            AS
            BEGIN
                DECLARE @Result INT
                SELECT
                    COUNT(*) + @Increase [Result]
                FROM
                    [dbo].[TestTable]
            END
            """;
        await ExecuteCommand(connection, createExecuteScalarProcedure);
    }
}