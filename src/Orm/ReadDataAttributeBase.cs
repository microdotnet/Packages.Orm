using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace MicroDotNet.Packages.Orm
{
    public abstract class ReadDataAttributeBase : StoredProcedureCallAttributeBase
    {
        private static readonly ReadOnlyCollection<CallTypes> AllowedCallTypes = new ReadOnlyCollection<CallTypes>(
            new[]
            {
                CallTypes.SelectOne,
                CallTypes.SelectMultiple,
            });
        
        protected ReadDataAttributeBase(string procedureName, CallTypes callType)
            : base(procedureName, callType)
        {
            if (!AllowedCallTypes.Contains(callType))
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    ReadDataAttributeBaseResources.CallTypeNotAllowed,
                    callType.ToString());
                throw new ArgumentException(message);
            }
        }
    }
}