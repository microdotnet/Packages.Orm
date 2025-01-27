namespace MicroDotNet.Packages.Orm.IntegrationTests;

[ReadScalar("ScalarRetriever")]
public partial class ExecuteScalarContainer
{
    public ExecuteScalarContainer(
        SimpleParametersContainer parameters)
    {
        this.Parameters = parameters;
    }

    public SimpleParametersContainer Parameters { get; }
}