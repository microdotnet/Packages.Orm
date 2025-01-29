using System.Data;
using System.Data.Common;
using MicroDotNet.Packages.Orm.DatabaseAbstraction;

namespace MicroDotNet.Packages.Orm.Connectors.AdoNet;

public static class DbCommandExtensions
{
    public static DbCommand CreateCommand(
        this DbConnection connection,
        StoredProcedureCallContext context)
    {
        var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = context.ProcedureName;
        command.FillParameters(context);
        return command;
    }

    public static void FillParameters(
        this DbCommand command,
        StoredProcedureCallContext context)
    {
        foreach (var input in context.Parameters)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = input.Name;
            parameter.Value = input.Value;
            parameter.DbType = input.DbType;
            parameter.Direction = ParameterDirection.Input;
            command.Parameters.Add(parameter);
        }
    }
}