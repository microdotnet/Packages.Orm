using System.Data;

namespace MicroDotNet.Packages.Orm.IntegrationTests;

[ReadScalar]
public partial class SimpleScalarExecutionContainer
{
    public const string CommandText = "[dbo].[SimpleScalar]";
    
    public const CommandType CommandType = System.Data.CommandType.StoredProcedure;
    
    public SimpleScalarExecutionContainer(SimpleParametersContainer parameters)
    {
        this.Parameters = parameters;
    }

    public SimpleParametersContainer Parameters { get; }
}