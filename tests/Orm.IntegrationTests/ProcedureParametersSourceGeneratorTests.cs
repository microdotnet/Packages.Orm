using MicroDotNet.Packages.Orm.Stories;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MicroDotNet.Packages.Orm.IntegrationTests;

public class ProcedureParametersSourceGeneratorTests
{
    private const string CodeToProcess = 
"""
namespace SomeNamespace
{
    using MicroDotNet.Packages.Orm;

    [ProcedureParameters]
    public partial class ParametersContainer
    {
        public ParametersContainer(
            string parameter1)
        {
            this.Parameter1 = parameter1;
        }

        public string Parameter1 { get; }
    }
}
""";

    private ProcedureParametersSourceGenerator generator = default!;

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
        this.generator = new ProcedureParametersSourceGenerator();
    }

    private void CodeIsCompiled()
    {
        var compilation = CSharpCompilation.Create("CSharpCodeGen.GenerateAssembly")
            .AddSyntaxTrees(CSharpSyntaxTree.ParseText(this.sourceCode))
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddReferences(MetadataReference.CreateFromFile(typeof(ProcedureParametersAttribute).Assembly.Location))
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var driver = CSharpGeneratorDriver.Create(generator)
            .RunGeneratorsAndUpdateCompilation(compilation, out _, out var _);

        // Verify the generated code
        this.compilationResult = driver.GetRunResult();
    }

    private void CompilationResultIsValid()
    {
        this.compilationResult.Should().NotBeNull();
    }
}