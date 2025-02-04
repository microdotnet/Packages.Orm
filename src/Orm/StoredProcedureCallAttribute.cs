using System;
using System.Data;

namespace MicroDotNet.Packages.Orm
{
    public abstract partial class DatabaseCallAttributeBase : Attribute
    {
        protected DatabaseCallAttributeBase(
            CallTypes callType)
        {
            this.CallType = callType;
        }

        public CallTypes CallType { get; }
    }
}