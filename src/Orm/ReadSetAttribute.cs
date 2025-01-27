namespace MicroDotNet.Packages.Orm
{
    public class ReadSetAttribute : ReadDataAttributeBase
    {
        public ReadSetAttribute(string procedureName)
            : base(procedureName, CallTypes.SelectMultiple)
        {
        }
    }
}