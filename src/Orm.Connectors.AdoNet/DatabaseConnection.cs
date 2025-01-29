using System.Data.Common;
using System.Transactions;
using MicroDotNet.Packages.Orm.DatabaseAbstraction;

namespace MicroDotNet.Packages.Orm.Connectors.AdoNet;

public class DatabaseConnection<TConnection> : IDatabaseConnection
    where TConnection : DbConnection, new()
{
    public DatabaseConnection(string connectionString)
    {
        this.ConnectionString = connectionString;
    }

    public string ConnectionString { get; }

    public async Task<StoredProcedureCallResult> ExecuteAsync(
        StoredProcedureCallContext context,
        CancellationToken cancellationToken)
    {
        var connection = new TConnection();
        connection.ConnectionString = this.ConnectionString;
        var command = connection.CreateCommand(context);
        await connection.OpenAsync(cancellationToken)
            .ConfigureAwait(false);
        StoredProcedureCallResult result;
        if (context.CallType == CallTypes.ExecuteScalar)
        {
            result = await this.ReadFromDatabaseAsync(
                    command,
                    context,
                    cancellationToken)
                .ConfigureAwait(false);
        }
        else
        {
            result = new StoredProcedureCallResult();
        }

        if (Transaction.Current is not null)
        {
            await connection.CloseAsync()
                .ConfigureAwait(false);
        }

        return result;
    }

    private async Task<StoredProcedureCallResult> ReadFromDatabaseAsync(
        DbCommand command,
        StoredProcedureCallContext context,
        CancellationToken cancellationToken)
    {
        var reader = await command.ExecuteReaderAsync(cancellationToken)
            .ConfigureAwait(false);
        return new StoredProcedureCallResult();
    }
}