using System.Data;
using System.Linq.Expressions;
using MicroDotNet.Packages.Orm.Stories;

namespace MicroDotNet.Packages.Orm.IntegrationTests;

public class SimpleResultTests
{
    private readonly Mock<IDataRecord> dataRecord = new();

    private SimpleResultContainer? result = null;

    [Fact]
    public void WhenResultIsExtractedFromDataRecordThenItHasCorrectPropertyValues()
    {
        const string column1 = "VALUE 1";
        const int column2 = 1234;
        this.Given(t => t.ColumnsAreMocked(column1, column2))
            .When(t => t.ResultIsExtracted())
            .Then(t => t.ResultIsNotNull())
            .And(t => t.ResultPropertiesAre(column1, column2))
            .BDDfy<Issue1DesignApi>();
    }

    private void ColumnsAreMocked(string column1, int column2)
    {
        this.dataRecord
            .Setup(dr => dr.GetOrdinal(It.IsAny<string>()))
            .Returns((string cn) => cn switch
            {
                "column1" => 0,
                "column2" => 1,
                _ => throw new NotImplementedException()
            });
        this.dataRecord
            .Setup(dr => dr.GetString(0))
            .Returns(column1);
        this.dataRecord
            .Setup(dr => dr.GetInt32(1))
            .Returns(column2);
    }

    private void ResultIsExtracted()
    {
        this.result = SimpleResultContainer.ReadFromDataRecord(this.dataRecord.Object);
    }

    private void ResultIsNotNull()
    {
        this.result.ShouldNotBeNull();
    }

    private void ResultPropertiesAre(string value1, int value2)
    {
        this.result!.Column1.ShouldBe(value1);
        this.result.Column2.ShouldBe(value2);
    }
}