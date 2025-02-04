using System.Data;

namespace MicroDotNet.Packages.Orm
{
    public class ReadOneAttribute : ReadDataAttributeBase
    {
        public ReadOneAttribute()
            : base(CallTypes.SelectOne)
        {
        }
    }
}