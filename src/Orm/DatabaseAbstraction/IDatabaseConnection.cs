using System.Threading;
using System.Threading.Tasks;

namespace MicroDotNet.Packages.Orm.DatabaseAbstraction
{
    public interface IDatabaseConnection
    {
        string ConnectionString { get; }
        
        Task<StoredProcedureCallResult> ExecuteAsync(
            StoredProcedureCallContext context,
            CancellationToken cancellationToken);
    }
}