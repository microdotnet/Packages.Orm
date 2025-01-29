using System;

namespace MicroDotNet.Packages.Orm
{
    public abstract partial class StoredProcedureCallAttributeBase : Attribute
    {
        protected StoredProcedureCallAttributeBase(string procedureName, CallTypes callType)
        {
            if (string.IsNullOrWhiteSpace(procedureName))
            {
                throw new ArgumentNullException(
                    nameof(procedureName),
                    StoredProcedureCallAttributeBaseResources.StoredProcedureNameIsEmpty);
            }
            this.ProcedureName = procedureName;
            this.CallType = callType;
        }

        public string ProcedureName { get; }
        
        public CallTypes CallType { get; }
    }
}