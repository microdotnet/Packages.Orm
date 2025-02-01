using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace MicroDotNet.Packages.Orm.DatabaseAbstraction
{
    public interface IDatabaseConnection
    {
        string ConnectionString { get; }
        
        Task<ReadOnlyCollection<TResult>> ReadDataAsync<TResult>(
            string procedureName,
            ICollection<ParameterInfo> parameters,
            Func<IDataRecord, TResult> mapper,
            CancellationToken cancellationToken)
            where TResult : class;
    }
}