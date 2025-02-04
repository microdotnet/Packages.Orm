using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using MicroDotNet.Packages.Orm.Tools;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MicroDotNet.Packages.Orm
{
    [Generator]
    public class ExecuteScalarSourceGenerator : ClassExtensionGeneratorBase<ReadScalarAttribute>
    {
        protected override void Execute(Compilation compilation, ImmutableArray<(ClassDeclarationSyntax, AttributeData)> classes, SourceProductionContext context)
        {
            foreach (var classData in classes.Select(c => new { ClassData = c.Item1, AttributeData = c.Item2 }))
            {
                var namespaceNode = classData.ClassData.FindParentNode<BaseNamespaceDeclarationSyntax>();
                var namespaceName = string.Empty;
                if (namespaceNode != null)
                {
                    namespaceName = namespaceNode.Name.GetText().ToString().Trim();
                }

                var className = classData.ClassData.Identifier.Text;

                var generatedFileContents = string.Format(
                    CultureInfo.InvariantCulture,
                    ExecuteScalarSourceGeneratorResources.FileContentsTemplate,
                    namespaceName,
                    className,
                    "COMMAND_TEXT",
                    "TableDirect");

                context.AddSource(
                    $"{classData.ClassData.Identifier.Text}_ExecuteScalar.g.cs",
                    generatedFileContents);
            }
        }
    }
}