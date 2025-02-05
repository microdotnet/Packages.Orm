# Packages.Orm

Packages providing very simple (from usage standpoint) ORM functionality. Idea is to create code generators that will extend classes created by users with methods used for calling database.

## Example usage

There are few generators that are helping main generators perform operations:

### Parameters writer (`ProcedureParametersAttribute`)

Code writer that is used to generate method converting class properties to procedure's input parameters. To use the generator decorate your parameters class with `[ProcedureParameters]` attribute. This will generate `ExtractParameters` instance method that enumerates parameters of the instance.

Parameters will be created from public instance properties with names the same as property names. (In future there's a plan to introduce possiblity to customize parameter names, but not until basic logic is completed.)

```
[ProcedureParameters]
public partial class InputParameters
{
    public InputParameters(
        string parameter1,
        int parameter2)
    {
        this.Parameter1 = parameter1;
        this.Parameter2 = parameter2;
    }

    public string Parameter1 { get; }
    
    public int Parameter2 { get; }
}
```

### Result reader (`ResultRowAttribute`)

Code generator that is used to generate method reading instance of the class from `IDataRecord`. To use the generator decorate your result row class with `[ResultRow]` attribute. This will generate `ReadFromDataRecord` static method that reads instance from `IDataRecord`.

Mapping is done based on constructor parameters. `GetOrdinal` method of `IDataRecord` is used to find column to be mapped and the value is passed to constructor.

### Read scalar execution (`ReadScalarAttribute`)

Generates a method to execute database command and returns value from first column of first row of the result set.