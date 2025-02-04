using System.Data;

namespace MicroDotNet.Packages.Orm
{
    public class ReadSetAttribute : ReadDataAttributeBase
    {
        public ReadSetAttribute()
            : base(CallTypes.SelectMultiple)
        {
        }
    }
}