namespace MicroDotNet.Packages.Orm
{
    public class ReadScalarAttribute : ReadDataAttributeBase
    {
        public ReadScalarAttribute(string procedureName)
            : base(procedureName, CallTypes.ExecuteScalar)
        {
        }
    }
}