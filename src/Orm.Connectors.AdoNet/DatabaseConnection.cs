using System.Collections.ObjectModel;
using System.Data;
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

    public async Task<ReadOnlyCollection<TResult>> ReadDataAsync<TResult>(
        string procedureName,
        ICollection<ParameterInfo> parameters,
        Func<IDataRecord, TResult> mapper,
        CancellationToken cancellationToken)
        where TResult : class
    {
        var connection = new TConnection();
        connection.ConnectionString = this.ConnectionString;
        var command = connection.CreateCommand();
        command.CommandText = procedureName;
        command.CommandType = CommandType.StoredProcedure;
        foreach (var param in parameters)
        {
            var p = command.CreateParameter();
            p.ParameterName = param.Name;
            p.DbType = param.DbType;
            p.Value = param.Value;
            command.Parameters.Add(p);
        }

        await connection.OpenAsync(cancellationToken)
            .ConfigureAwait(false);
        var reader = await command.ExecuteReaderAsync(cancellationToken)
            .ConfigureAwait(false);
        var results = new List<TResult>();
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            results.Add(mapper(reader));
        }

        if (Transaction.Current is not null)
        {
            await connection.CloseAsync()
                .ConfigureAwait(false);
        }

        return new ReadOnlyCollection<TResult>(results);
    }
}