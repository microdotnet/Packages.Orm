namespace MicroDotNet.Packages.Orm.DatabaseAbstraction
{
    public interface IDatabaseConnectionFactory
    {
        IDatabaseConnection Create(string name);
    }
}