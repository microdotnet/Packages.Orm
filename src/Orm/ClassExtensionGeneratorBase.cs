using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MicroDotNet.Packages.Orm
{
    public abstract class ClassExtensionGeneratorBase<TAttribute> : IIncrementalGenerator
        where TAttribute : Attribute
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var fullyQualifiedMetadataName = typeof(TAttribute).FullName;
            if (string.IsNullOrEmpty(fullyQualifiedMetadataName))
            {
                return;
            }

            var classes = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    fullyQualifiedMetadataName,
                    ExtractionHelpers.FilterClassDeclarations,
                    ExtractionHelpers.TransformForClassExtractions<TAttribute>)
                .Where(m => m.Item1 != null && m.Item2 != null);
            var compilationAndClasses = context.CompilationProvider.Combine(classes.Collect());

            context.RegisterSourceOutput(
                compilationAndClasses,
                (spc, source) => Execute(source.Item1, source.Item2, spc));
        }

        protected abstract void Execute(
            Compilation compilation,
            ImmutableArray<(ClassDeclarationSyntax, AttributeData)> classes,
            SourceProductionContext context);
    }
}