using System.Data;
using System.Data.Common;
using System.Transactions;
using MicroDotNet.Packages.Orm.DatabaseAbstraction;

namespace MicroDotNet.Packages.Orm.Connectors.AdoNet;

public class AdoNetDatabaseConnection<TConnection> : IDatabaseConnection
    where TConnection : DbConnection, new()
{
    private const string ReturnValueParameterName = "ReturnValue";
    
    public AdoNetDatabaseConnection(string connectionString)
    {
        this.ConnectionString = connectionString;
    }

    public string ConnectionString { get; }

    public async Task<object?> ExecuteScalarAsync(
        string procedureName,
        CommandType commandType,
        IReadOnlyCollection<ParameterInformation> inputParameters,
        CancellationToken cancellationToken)
    {
        var command = await this.PrepareCommandAsync(
                procedureName,
                commandType,
                inputParameters,
                [],
                cancellationToken)
            .ConfigureAwait(false);
        return await command.ExecuteScalarAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task<DbCommand> PrepareCommandAsync(
        string procedureName,
        CommandType commandType,
        IReadOnlyCollection<ParameterInformation> inputParameters,
        IReadOnlyCollection<string> outputParameters,
        CancellationToken cancellationToken)
    {
        var connection = new TConnection();
        connection.ConnectionString = this.ConnectionString;
        var command = connection.CreateCommand();
        command.CommandText = procedureName;
        command.CommandType = commandType;
        foreach (var param in inputParameters)
        {
            var p = command.CreateParameter();
            p.ParameterName = param.Name;
            p.DbType = param.DbType;
            p.Value = param.Value ?? DBNull.Value;
            p.Direction = ParameterDirection.Input;
            command.Parameters.Add(p);
        }

        foreach (var param in outputParameters)
        {
            var p = command.CreateParameter();
            p.ParameterName = param;
            p.Direction = ParameterDirection.Output;
            command.Parameters.Add(p);
        }

        var returnValueParameter = command.CreateParameter();
        returnValueParameter.ParameterName = ReturnValueParameterName;
        returnValueParameter.DbType = DbType.Int32;
        returnValueParameter.Direction = ParameterDirection.ReturnValue;
        command.Parameters.Add(returnValueParameter);

        await connection.OpenAsync(cancellationToken)
            .ConfigureAwait(false);
        return command;
    }
}