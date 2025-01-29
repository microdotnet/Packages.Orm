using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MicroDotNet.Packages.Orm.DatabaseAbstraction
{
    public class StoredProcedureCallContext
    {
        public StoredProcedureCallContext(
            string procedureName,
            CallTypes callType,
            IEnumerable<ParameterInfo> parameters)
        {
            this.ProcedureName = procedureName;
            this.CallType = callType;
            this.Parameters = new ReadOnlyCollection<ParameterInfo>(parameters.ToList());
        }

        public string ProcedureName { get; }

        public CallTypes CallType { get; }

        public ReadOnlyCollection<ParameterInfo> Parameters { get; }
    }
}