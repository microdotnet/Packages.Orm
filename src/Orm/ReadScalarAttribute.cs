using System.Data;

namespace MicroDotNet.Packages.Orm
{
    public class ReadScalarAttribute : ReadDataAttributeBase
    {
        public ReadScalarAttribute()
            : base(CallTypes.ExecuteScalar)
        {
        }
    }
}