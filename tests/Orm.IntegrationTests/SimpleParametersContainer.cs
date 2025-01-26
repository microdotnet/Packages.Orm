namespace MicroDotNet.Packages.Orm.IntegrationTests;

[ProcedureParameters]
public partial class SimpleParametersContainer
{
    public SimpleParametersContainer(string parameter1, int parameter2)
    {
        this.Parameter1 = parameter1;
        this.Parameter2 = parameter2;
    }

    public string Parameter1 { get; }
    
    public int Parameter2 { get; }
}