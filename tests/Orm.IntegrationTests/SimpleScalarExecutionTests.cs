using System.Data;
using MicroDotNet.Packages.Orm.DatabaseAbstraction;
using MicroDotNet.Packages.Orm.Stories;

namespace MicroDotNet.Packages.Orm.IntegrationTests;

public class SimpleScalarExecutionTests
{
    private readonly Mock<IDatabaseConnection> connectionMock = new();
    
    private SimpleParametersContainer parameters = default!;

    private SimpleScalarExecutionContainer container = default!;
    
    private object result = default!;

    [Fact]
    public void WhenNotGenericScalarCallIsExecutedThenValueIsReturned()
    {
        var expectedResult = new ExecutionResult<object>(
            ["TEST"],
            new Dictionary<string, object>(),
            0);
        this.Given(t => t.ParametersInstanceIsCreated())
            .And(t => t.ExecutionContainerIsCreated())
            .And(t => t.ConnectionIsMocked(expectedResult))
            .When(t => t.CommandIsExecuted())
            .Then(t => t.ResultIsNotNull())
            .BDDfy<Issue1DesignApi>();
    }

    [Fact]
    public void WhenNotGenericScalarCallIsExecutedThenDataIsRetrievedFromConnection()
    {
        var expectedResult = new ExecutionResult<object>(
            ["TEST"],
            new Dictionary<string, object>(),
            0);
        this.Given(t => t.ParametersInstanceIsCreated())
            .And(t => t.ExecutionContainerIsCreated())
            .And(t => t.ConnectionIsMocked(expectedResult))
            .When(t => t.CommandIsExecuted())
            .Then(t => t.DatabaseConnectionIsCalled())
            .BDDfy<Issue1DesignApi>();
    }

    [Fact]
    public void WhenNotGenericScalarCallIsExecutedThenExpectedResultIsReturned()
    {
        var expectedResult = "TEST";
        this.Given(t => t.ParametersInstanceIsCreated())
            .And(t => t.ExecutionContainerIsCreated())
            .And(t => t.ConnectionIsMocked(expectedResult))
            .When(t => t.CommandIsExecuted())
            .Then(t => t.ResultFromDatabaseIsReturned("TEST"))
            .BDDfy<Issue1DesignApi>();
    }

    private void ParametersInstanceIsCreated()
    {
        this.parameters = new("paramvalue1", 2);
    }

    private void ExecutionContainerIsCreated()
    {
        this.container = new(this.parameters);
    }

    private void ConnectionIsMocked(object expectedResult)
    {
        this.connectionMock
            .Setup(conn => conn.ExecuteScalarAsync(
                It.IsAny<string>(),
                It.IsAny<CommandType>(),
                It.IsAny<IReadOnlyCollection<ParameterInformation>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
    }

    private async Task CommandIsExecuted()
    {
        this.result = await this.container.ExecuteScalar(this.connectionMock.Object, CancellationToken.None);
    }

    private void ResultIsNotNull()
    {
        this.result.ShouldNotBeNull();
    }

    private void DatabaseConnectionIsCalled()
    {
        this.connectionMock
            .Verify(conn => conn.ExecuteScalarAsync(
                SimpleScalarExecutionContainer.CommandText,
                SimpleScalarExecutionContainer.CommandType,
                It.Is<IReadOnlyCollection<ParameterInformation>>(
                    c => c.Count == 2 && (string)c.ElementAt(0).Value == this.parameters.Parameter1 && (int)c.ElementAt(1).Value == this.parameters.Parameter2),
                It.IsAny<CancellationToken>()),
                Times.Once());
    }

    private void ResultFromDatabaseIsReturned(string expectedValue)
    {
        this.result.ShouldBe(expectedValue);
    }
}