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
    public class ResultRowSourceGenerator : ClassExtensionGeneratorBase<ResultRowAttribute>
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

                var constructors = x.Item1.Members.OfType<ConstructorDeclarationSyntax>()
                    .ToList();
                if (constructors.Count == 0)
                {
                    continue;
                }

                if (constructors.Count > 1)
                {
                    continue;
                }
                
                var constructor = constructors.Single();
                var parameters = constructor.ParameterList.Parameters;
                var ordinalsCalculation = new StringBuilder();
                var resultCreation = new StringBuilder();
                foreach (var parameter in parameters)
                {
                    ordinalsCalculation.AppendLine($"                var {parameter.Identifier.Text}_ordinal = dataRecord.GetOrdinal(\"{parameter.Identifier.Text}\");");
                    ordinalsCalculation.AppendLine($"                Ordinals.AddOrUpdate(\"{parameter.Identifier.Text}\", {parameter.Identifier.Text}_ordinal, (k, ov) => {parameter.Identifier.Text}_ordinal);");
                    var parameterExtraction = TypeMappingProvider.CreateMapping(
                        parameter.Identifier.Text,
                        parameter.Type.ToString());
                    resultCreation.AppendLine($"            {parameterExtraction}");
                }

                ordinalsCalculation.Length--;
                resultCreation.AppendLine($"            return new {className}(");
                for (var paramIndex = 0; paramIndex < parameters.Count; paramIndex++)
                {
                    resultCreation.Append($"                {parameters[paramIndex].Identifier.Text}_value");
                    if (paramIndex == parameters.Count - 1)
                    {
                        resultCreation.Append(");");
                    }
                    else
                    {
                        resultCreation.AppendLine(",");
                    }
                }
                
                var generatedFileContents = string.Format(
                    CultureInfo.InvariantCulture,
                    ResultRowSourceGeneratorResources.FileContentsTemplate,
                    namespaceName,
                    className,
                    ordinalsCalculation.ToString(),
                    resultCreation.ToString());

                context.AddSource(
                    $"{x.Item1.Identifier.Text}_ReadFromDataRecord.g.cs",
                    generatedFileContents);
            }
        }
    }
}