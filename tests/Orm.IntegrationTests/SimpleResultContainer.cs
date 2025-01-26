using System.Data;

namespace MicroDotNet.Packages.Orm.IntegrationTests;

[ResultRow]
public partial class SimpleResultContainer
{
    public SimpleResultContainer(string column1, int column2)
    {
        this.Column1 = column1;
        this.Column2 = column2;
    }

    public string Column1 { get; }
    
    public int Column2 { get; }
    
    public static SimpleResultContainer ReadFromDataRecord1(IDataRecord dataRecord)
    {
        if (Ordinals.Count == 0)
        {
            lock (OrdinalsLock)
            {
                if (Ordinals.Count == 0)
                {
                    var column1_ordinal = dataRecord.GetOrdinal("column1");
                    var column2_ordinal = dataRecord.GetOrdinal("column2");
    Ordinals.AddOrUpdate("column1", column1_ordinal, (k, ov) => column1_ordinal);
                }
            }
        }
        var column1_value = dataRecord.GetString(Ordinals["column1"]);
        var column2_value = dataRecord.GetInt32(Ordinals["column2"]);
        return new SimpleResultContainer(
            column1_value,
            column2_value);
    }
}