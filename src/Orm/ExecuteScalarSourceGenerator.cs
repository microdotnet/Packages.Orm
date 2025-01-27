using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using MicroDotNet.Packages.Orm.Tools;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MicroDotNet.Packages.Orm
{
    [Generator]
    public class ExecuteScalarSourceGenerator : ClassExtensionGeneratorBase<ReadScalarAttribute>
    {
        protected override void Execute(Compilation compilation, ImmutableArray<(ClassDeclarationSyntax, AttributeData)> classes, SourceProductionContext context)
        {
            foreach (var (x, i) in classes.Select((x, i) => (x, i)))
            {
                var namespaceNode = x.Item1.FindParentNode<BaseNamespaceDeclarationSyntax>();
                var namespaceName = string.Empty;
                if (namespaceNode != null)
                {
                    namespaceName = namespaceNode.Name.GetText().ToString().Trim();
                }

                var className = x.Item1.Identifier.Text;

                var generatedFileContents = string.Format(
                    CultureInfo.InvariantCulture,
                    ExecuteScalarSourceGeneratorResources.FileContentsTemplate,
                    namespaceName,
                    className);

                context.AddSource(
                    $"{x.Item1.Identifier.Text}_ExecuteScalar.g.cs",
                    generatedFileContents);
            }
        }
    }
}