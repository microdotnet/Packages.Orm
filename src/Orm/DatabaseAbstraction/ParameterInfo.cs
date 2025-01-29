using System.Data;

namespace MicroDotNet.Packages.Orm.DatabaseAbstraction
{
    public class ParameterInfo
    {
        public ParameterInfo(string name, object value, DbType dbType)
        {
            this.Name = name;
            this.Value = value;
            this.DbType = dbType;
        }

        public string Name { get; }

        public object Value { get; }

        public DbType DbType { get; }
    }
}