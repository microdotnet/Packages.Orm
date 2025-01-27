using System.Transactions;
using Microsoft.Data.SqlClient;
using MicroDotNet.Packages.Orm.DatabaseAbstraction;

namespace MicroDotNet.Packages.Orm.SqlServer
{
    public class SqlDatabaseConnection : IDatabaseConnection
    {
        public SqlDatabaseConnection(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public string ConnectionString { get; }

        public Task<StoredProcedureCallResult> ExecuteAsync(
            StoredProcedureCallContext context,
            CancellationToken cancellationToken)
        {
            var connection = new SqlConnection(this.ConnectionString);
            if (Transaction.Current is not null)
            {
                connection.Close();
            }
            return Task.FromResult(new StoredProcedureCallResult());
        }
    }
}