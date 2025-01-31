using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using MicroDotNet.Packages.Orm.DatabaseAbstraction;
using MicroDotNet.Packages.Orm.Stories;
using Moq.Protected;

namespace MicroDotNet.Packages.Orm.Connectors.AdoNet.UnitTests.DbCommandExtensionsTests;

public class CreateCommandTests
{
    private readonly Mock<DbConnection> connectionMock = new();

    private readonly Mock<DbCommand> commandMock = new();

    private readonly Collection<ParameterInfo> parameters = new();

    private string procedureName = default!;

    private CallTypes callType;

    private StoredProcedureCallContext context = default!;

    private DbCommand createdCommand = default!;

    [Fact]
    public void WhenCommandIsCreatedThenCommandTextAndTypeAreCorrect()
    {
        this.Given(t => t.CommandCreationIsMocked())
            .And(t => t.StoredProcedureNameIs("PROCEDURE_NAME"))
            .And(t => t.CallTypeIs(CallTypes.SelectMultiple))
            .And(t => t.CallContextIsCreated())
            .When(t => t.CommandIsCreated())
            .Then(t => t.CommandTextIs(this.procedureName))
            .And(t => t.CommandTypeIs(CommandType.StoredProcedure))
            .BDDfy<Issue1DesignApi>();
    }

    private void CommandCreationIsMocked()
    {
        this.connectionMock
            .Protected()
            .Setup<DbCommand>("CreateDbCommand")
            .Returns(this.commandMock.Object);
    }

    private void StoredProcedureNameIs(string value)
    {
        this.procedureName = value;
    }

    private void CallTypeIs(CallTypes value)
    {
        this.callType = value;
    }

    private void WithParameter(string name, object value, DbType type)
    {
        var param = new ParameterInfo(name, value, type);
        this.parameters.Add(param);
    }

    private void CallContextIsCreated()
    {
        this.context = new(
            this.procedureName,
            this.callType,
            this.parameters);
    }

    private void CommandIsCreated()
    {
        this.createdCommand = this.connectionMock.Object.CreateCommand(
            this.context);
    }

    private void MockedCommandIsReturned()
    {
        this.createdCommand
            .ShouldBeSameAs(this.commandMock.Object);
    }

    private void CommandTextIs(string value)
    {
        this.commandMock
            .VerifySet(
                command => command.CommandText = value,
                Times.Once());
    }

    private void CommandTypeIs(CommandType value)
    {
        this.commandMock
            .VerifySet(
                command => command.CommandType = value);
    }
}