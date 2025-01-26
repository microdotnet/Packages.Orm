using System;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MicroDotNet.Packages.Orm.Tools
{
    public static class ExtractionHelpers
    {
        public static bool FilterSyntaxNodes<TNode>(
            SyntaxNode node,
            CancellationToken cancellationToken)
            where TNode : SyntaxNode
        {
            return node is TNode;
        }

        public static bool FilterClassDeclarations(
            SyntaxNode node,
            CancellationToken cancellationToken)
        {
            return FilterSyntaxNodes<ClassDeclarationSyntax>(node, cancellationToken);
        }

        public static (ClassDeclarationSyntax, AttributeData) TransformForClassExtractions<TAttribute>(
            GeneratorAttributeSyntaxContext context,
            CancellationToken ct)
            where TAttribute : Attribute
        {
            if (!(context.TargetNode is ClassDeclarationSyntax))
            {
                return (null, null);
            }

            var attr = context.Attributes.FirstOrDefault(a =>
                a.AttributeClass != null && a.AttributeClass.Name == typeof(TAttribute).Name);
            return (context.TargetNode as ClassDeclarationSyntax, attr);
        }
    }
}