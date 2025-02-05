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
        const string createExecuteScalarResultSetTypeCommandText =
            """
            CREATE TYPE ExecuteScalarRow AS (Result BIGINT)
            """;
        await ExecuteCommand(connection, createExecuteScalarResultSetTypeCommandText);
        const string createExecuteScalarFunctionCommandText =
            """
            CREATE FUNCTION ExecuteScalar
            (
                Increase INT
            )
                RETURNS SETOF ExecuteScalarRow
            LANGUAGE plpgsql
            AS $$
            BEGIN
                RETURN QUERY
                    SELECT
                        COUNT(*) + Increase Result
                    FROM
                        test_table;
            END;
            $$
            """;
        await ExecuteCommand(connection, createExecuteScalarFunctionCommandText);
    }
}