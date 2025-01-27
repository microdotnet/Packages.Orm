using System.Collections.ObjectModel;

namespace MicroDotNet.Packages.Orm.DatabaseAbstraction
{
    public class StoredProcedureCallContext
    {
        public StoredProcedureCallContext(string procedureName, ReadOnlyDictionary<string, object> parameters)
        {
            this.ProcedureName = procedureName;
            this.Parameters = parameters;
        }

        public string ProcedureName { get; }
        
        public ReadOnlyDictionary<string, object> Parameters { get; }
    }
}