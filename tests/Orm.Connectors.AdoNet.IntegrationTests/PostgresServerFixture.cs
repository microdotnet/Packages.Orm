using System.Data;
using Npgsql;
using Testcontainers.PostgreSql;

namespace MicroDotNet.Packages.Orm.Connectors.AdoNet.IntegrationTests;

public class PostgresServerFixture: IAsyncLifetime
{
    private readonly PostgreSqlContainer container;

    public PostgresServerFixture()
    {
        this.container = new PostgreSqlBuilder()
            .WithImage("postgres:17.2")
            .Build();
    }

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

    private static async Task ExecuteCommand(NpgsqlConnection connection, string commandText)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = commandText;
        command.CommandType = CommandType.Text;
        await command.ExecuteNonQueryAsync();
    }

    private async Task CreateTestDatabase()
    {
        await using var connection = new NpgsqlConnection(this.ConnectionString);
        await connection.OpenAsync();
        const string createTestTableCommandText =
            """
            CREATE TABLE test_table
            (
                id SERIAL PRIMARY KEY,
                text VARCHAR(50) NOT NULL
            )
            """;
        await ExecuteCommand(connection, createTestTableCommandText);
        const string fillTestTableWithDataCommandText =
            """
            INSERT INTO test_table (Text) VALUES ('Value1');
            INSERT INTO test_table (Text) VALUES ('Value2');
            INSERT INTO test_table (Text) VALUES ('Value3');
            INSERT INTO test_table (Text) VALUES ('Value4');
            """;
        await ExecuteCommand(connection, fillTestTableWithDataCommandText);
        const string createResultSetTypeCommandText =
            """
            CREATE TYPE TestDataRow AS (Id INT, Text VARCHAR(50))
            """;
        await ExecuteCommand(connection, createResultSetTypeCommandText);
        const string createRetrievalProcedureCommandText =
            """
            CREATE FUNCTION ExtractTestData
            (
                Id INT
            )
                RETURNS SETOF TestDataRow
            LANGUAGE plpgsql
            AS $$
            BEGIN
                RETURN QUERY
                    SELECT
                        t.Id,
                        t.Text
                    FROM
                        test_table t
                    WHERE
                        t.Id = ExtractTestData.Id;
            END;
            $$
            """;
        await ExecuteCommand(connection, createRetrievalProcedureCommandText);
        const string createCounterProcedureCommandText =
            """
            CREATE FUNCTION CountTestData
            (
                Increase INT
            )
                RETURNS INT
            LANGUAGE plpgsql
            AS $$
            DECLARE
                Result INT;
            BEGIN
                SELECT
                    COUNT(*) + Increase INTO Result
                FROM
                    test_table;

                RETURN Result;
            END;
            $$
            """;
        await ExecuteCommand(connection, createCounterProcedureCommandText);
    }
}