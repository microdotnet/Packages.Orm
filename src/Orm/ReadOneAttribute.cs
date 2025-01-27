namespace MicroDotNet.Packages.Orm
{
    public class ReadOneAttribute : ReadDataAttributeBase
    {
        public ReadOneAttribute(string procedureName)
            : base(procedureName, CallTypes.SelectOne)
        {
        }
    }
}