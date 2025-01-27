using MicroDotNet.Packages.Orm.Stories;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MicroDotNet.Packages.Orm.IntegrationTests;

public class ExecuteScalarSourceGeneratorTests
{
    
    private const string CodeToProcess = 
"""
namespace SomeNamespace
{
    using MicroDotNet.Packages.Orm;

    [ReadScalar("ScalarRetriever")]
    public partial class ExecuteScalarContainer
    {
        public ExecuteScalarContainer(
            SimpleParametersContainer parameters)
        {
            this.Parameters = parameters;
        }

        public SimpleParametersContainer Parameters { get; }
    }
}
""";

    private ExecuteScalarSourceGenerator generator = default!;

    private string sourceCode = default!;
    
    private GeneratorDriverRunResult compilationResult = default!;

    [Fact]
    public void WhenGeneratorIsRunThenParameterExtractionIsGenerated()
    {
        this.Given(t => t.SourceCodeIs(CodeToProcess))
            .And(t => t.GeneratorIsCreated())
            .When(t => t.CodeIsCompiled())
            .Then(t => t.CompilationResultIsValid())
            .BDDfy<Issue1DesignApi>();
    }
    
    private void SourceCodeIs(string value) => this.sourceCode = value;

    private void GeneratorIsCreated()
    {
        this.generator = new ExecuteScalarSourceGenerator();
    }

    private void CodeIsCompiled()
    {
        var compilation = CSharpCompilation.Create("CSharpCodeGen.GenerateAssembly")
            .AddSyntaxTrees(CSharpSyntaxTree.ParseText(this.sourceCode))
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddReferences(MetadataReference.CreateFromFile(typeof(ProcedureParametersAttribute).Assembly.Location))
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var driver = CSharpGeneratorDriver.Create(this.generator)
            .RunGeneratorsAndUpdateCompilation(compilation, out _, out var _);

        // Verify the generated code
        this.compilationResult = driver.GetRunResult();
    }

    private void CompilationResultIsValid()
    {
        this.compilationResult.Should().NotBeNull();
        this.compilationResult.GeneratedTrees.Should().HaveCount(1);
    }
}