using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace MicroDotNet.Packages.Orm.DatabaseAbstraction
{
    public interface IDatabaseConnection
    {
        string ConnectionString { get; }
        
        Task<object> ExecuteScalarAsync(
            string procedureName,
            CommandType commandType,
            IReadOnlyCollection<ParameterInformation> inputParameters,
            CancellationToken cancellationToken);
    }
}