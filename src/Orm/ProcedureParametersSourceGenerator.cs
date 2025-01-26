using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MicroDotNet.Packages.Orm
{
    [Generator]
    public class ProcedureParametersSourceGenerator : ClassExtensionGeneratorBase<ProcedureParametersAttribute>
    {
        protected override void Execute(
            Compilation compilation,
            ImmutableArray<(ClassDeclarationSyntax, AttributeData)> classes,
            SourceProductionContext context)
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
                var propertiesExtraction = new StringBuilder();
                var props = x.Item1.Members
                    .AsEnumerable()
                    .Where(o => o.IsKind(SyntaxKind.PropertyDeclaration))
                    .OfType<PropertyDeclarationSyntax>()
                    .ToList();
                foreach (var prop in props)
                {
                    propertiesExtraction.AppendLine(
                        $@"            result.Add(""{prop.Identifier.Text}"", this.{prop.Identifier.Text});");
                }

                var generatedFileContents = string.Format(
                    CultureInfo.InvariantCulture,
                    ProcedureParametersSourceGeneratorResources.FileContentsTemplate,
                    namespaceName,
                    className,
                    propertiesExtraction.ToString());

                context.AddSource(
                    $"{x.Item1.Identifier.Text}_ExtractParameters.g.cs",
                    generatedFileContents);
            }
        }
    }
}