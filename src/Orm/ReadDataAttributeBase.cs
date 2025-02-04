using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;

namespace MicroDotNet.Packages.Orm
{
    public abstract class ReadDataAttributeBase : DatabaseCallAttributeBase
    {
        private static readonly ReadOnlyCollection<CallTypes> AllowedCallTypes = new ReadOnlyCollection<CallTypes>(
            new[]
            {
                CallTypes.SelectOne,
                CallTypes.SelectMultiple,
            });
        
        protected ReadDataAttributeBase(
            CallTypes callType)
            : base(callType)
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