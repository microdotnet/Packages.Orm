using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using MicroDotNet.Packages.Orm.Connectors.AdoNet;
using MicroDotNet.Packages.Orm.DatabaseAbstraction;
using MicroDotNet.Packages.Orm.Stories;
using Moq.Protected;

namespace MicroDotNet.Packages.Orm.IntegrationTests.DbCommandExtensionsTests;

public class FillParametersTests
{
    private readonly Mock<DbCommand> command = new();
    
    private readonly Mock<DbParameterCollection> dbParameters = new();
    
    private readonly Collection<ParameterInfo> parameters = new();

    [Fact]
    public void WhenParametersCollectionIsFilledThenParametersAreAdded()
    {
        this.Given(t => t.CommandMockIsInitialized())
            .And(t => t.ParameterIsAdded("Param1", "Param1Value", DbType.String))
            .And(t => t.ParameterIsAdded("Param2", 321, DbType.Int32))
            .When(t => t.ParametersCollectionIsFilled())
            .Then(t => t.ParametersCollectionContains("Param1", "Param1Value", DbType.String))
            .Then(t => t.ParametersCollectionContains("Param2", 321, DbType.Int32))
            .BDDfy<Issue1DesignApi>();
    }

    private void CommandMockIsInitialized()
    {
        this.command
            .Protected()
            .Setup<DbParameter>("CreateDbParameter")
            .Returns(() => new ParameterMock());
        this.command
            .Protected()
            .SetupGet<DbParameterCollection>("DbParameterCollection")
            .Returns(dbParameters.Object);
    }

    private void ParameterIsAdded(string parameterName, object value, DbType dbType)
    {
        this.parameters.Add(new ParameterInfo(parameterName, value, dbType));
    }

    private void ParametersCollectionIsFilled()
    {
        this.command.Object.FillParameters(
            new StoredProcedureCallContext(
                string.Empty,
                CallTypes.ExecuteScalar,
                this.parameters.AsReadOnly()));
    }

    private void ParametersCollectionContains(
        string parameterName,
        object value,
        DbType dbType)
    {
        this.dbParameters
            .Verify(
                c => c.Add(It.Is<DbParameter>(p => p.ParameterName == parameterName && value.Equals(p.Value) && p.DbType == dbType)),
                Times.Once(),
                $"Missing parameter '{parameterName}'");
    }

    private class ParameterMock : DbParameter
    {
        public override void ResetDbType()
        {
            throw new NotImplementedException();
        }

        public override DbType DbType { get; set; }
        public override ParameterDirection Direction { get; set; }
        public override bool IsNullable { get; set; }

        [AllowNull]
        public override string ParameterName { get; set; }

        [AllowNull]
        public override string SourceColumn { get; set; }

        public override object? Value { get; set; }

        public override bool SourceColumnNullMapping { get; set; }

        public override int Size { get; set; }
    }
}