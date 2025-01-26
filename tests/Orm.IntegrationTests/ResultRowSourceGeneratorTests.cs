using MicroDotNet.Packages.Orm.Stories;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MicroDotNet.Packages.Orm.IntegrationTests;

public class ResultRowSourceGeneratorTests
{
    private const string CodeToProcess = 
"""
namespace SomeNamespace
{
    using MicroDotNet.Packages.Orm;

    [ResultRow]
    public class SimpleResultContainer
    {
        public SimpleResultContainer(SimpleResultContainer column1, int column2)
        {
            this.Column1 = column1;
            this.Column2 = column2;
        }

        public SimpleResultContainer Column1 { get; }
        
        public int Column2 { get; }
    }
}
""";

    private ResultRowSourceGenerator generator = default!;

    private string sourceCode = default!;
    
    private GeneratorDriverRunResult compilationResult = default!;

    [Fact]
    public void WhenGeneratorIsRunThenResultRowCreationIsGenerated()
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
        this.generator = new ResultRowSourceGenerator();
    }

    private void CodeIsCompiled()
    {
        var compilation = CSharpCompilation.Create("CSharpCodeGen.GenerateAssembly")
            .AddSyntaxTrees(CSharpSyntaxTree.ParseText(this.sourceCode))
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddReferences(MetadataReference.CreateFromFile(typeof(ResultRowAttribute).Assembly.Location))
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var driver = CSharpGeneratorDriver.Create(generator)
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