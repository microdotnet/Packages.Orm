using System;

namespace MicroDotNet.Packages.Orm
{
    public abstract class StoredProcedureCallAttributeBase : Attribute
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

        public enum CallTypes
        {
            SelectMultiple = 1,
            SelectOne = 2,
            ExecuteScalar = 4,
        }
    }
}