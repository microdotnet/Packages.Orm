using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace MicroDotNet.Packages.Orm.IntegrationTests;

public class DbConnectionMock : DbConnection
{
    private readonly Func<DbCommand> commandFactory;

    public DbConnectionMock(Func<DbCommand> commandFactory)
    {
        this.commandFactory = commandFactory;
    }

    [AllowNull]
    public override string ConnectionString { get; set; }

    public override string Database => "DatabaseName";

    public override ConnectionState State => ConnectionState.Open;

    public override string DataSource => "server.com";
    
    public override string ServerVersion => "1.0.0.0";

    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
    {
        throw new NotImplementedException();
    }

    public override void ChangeDatabase(string databaseName)
    {
        throw new NotImplementedException();
    }

    public override void Close()
    {
    }

    public override void Open()
    {
    }

    protected override DbCommand CreateDbCommand()
    {
        return this.commandFactory();
    }
}