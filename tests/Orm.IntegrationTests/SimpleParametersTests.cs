using MicroDotNet.Packages.Orm.Stories;

namespace MicroDotNet.Packages.Orm.IntegrationTests;

public class SimpleParametersTests
{
    private SimpleParametersContainer instance = default!;
    
    private Dictionary<string, object> extractedParameters = default!;
    
    [Fact]
    public void WhenSimpleParametersInstanceIsCreatedThenCorrectParametersCanBeExtracted()
    {
        const string param1 = "param1";
        const int param2 = 120;
        this.Given(t => t.InstanceIsCreated(param1, param2))
            .When(t => t.ParametersAreExtracted())
            .Then(t => t.ExtractedParametersCountIs(2))
            .And(t => t.ExtractedParametersContains("Parameter1", param1))
            .And(t => t.ExtractedParametersContains("Parameter2", param2))
            .BDDfy<Issue1DesignApi>();
    }

    private void InstanceIsCreated(string value1, int value2)
    {
        this.instance = new SimpleParametersContainer(value1, value2);
    }

    private void ParametersAreExtracted()
    {
        this.extractedParameters = this.instance.ExtractParameters();
    }

    private void ExtractedParametersCountIs(int value)
    {
        this.extractedParameters.Should().HaveCount(value);
    }

    private void ExtractedParametersContains(string key, object value)
    {
        this.extractedParameters.Should()
            .ContainKey(key);
        this.extractedParameters[key].Should()
            .Be(value);
    }
}