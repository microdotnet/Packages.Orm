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

    public async Task<ExecutionResult<TResult>> ExecuteProcedureAsync<TResult>(
        string procedureName,
        CommandType commandType,
        IReadOnlyCollection<ParameterInfo> inputParameters,
        IReadOnlyCollection<string> outputParameters,
        Func<IDataReader, TResult> mapper,
        CancellationToken cancellationToken)
        where TResult : class
    {
        var command = await this.PrepareCommandAsync(
            procedureName,
            commandType,
            inputParameters,
            outputParameters,
            cancellationToken)
            .ConfigureAwait(false);
        var reader = await command.ExecuteReaderAsync(cancellationToken)
            .ConfigureAwait(false);
        var rows = new List<TResult>();
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            rows.Add(mapper(reader));
        }

        var outputParametersValues = new Dictionary<string, object>();
        foreach (var outputParameter in outputParameters)
        {
            var param = command.Parameters[outputParameter];
            outputParametersValues.Add(outputParameter, param.Value!);
        }
        
        var returnValueParameter = command.Parameters[ReturnValueParameterName];
        var returnValue = (int)(returnValueParameter.Value ?? 0);
        if (command.Connection is not null && Transaction.Current is not null)
        {
            await command.Connection.CloseAsync()
                .ConfigureAwait(false);
        }

        return new ExecutionResult<TResult>(
            rows,
            outputParametersValues,
            returnValue);
    }

    private async Task<DbCommand> PrepareCommandAsync(
        string procedureName,
        CommandType commandType,
        IReadOnlyCollection<ParameterInfo> inputParameters,
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