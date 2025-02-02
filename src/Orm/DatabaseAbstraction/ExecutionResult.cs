using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MicroDotNet.Packages.Orm.DatabaseAbstraction
{
    public class ExecutionResult<TResultRow>
        where TResultRow : class
    {
        public ExecutionResult(
            IEnumerable<TResultRow> rows,
            IDictionary<string, object> outputParameters,
            int returnValue)
        {
            this.Rows = new ReadOnlyCollection<TResultRow>(rows.ToList());
            this.OutputParameters = new ReadOnlyDictionary<string, object>(outputParameters);
            this.ReturnValue = returnValue;
        }

        public ReadOnlyCollection<TResultRow> Rows { get; }

        public ReadOnlyDictionary<string, object> OutputParameters { get; }

        public int ReturnValue { get; }
    }
}