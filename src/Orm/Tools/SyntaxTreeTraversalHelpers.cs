using Microsoft.CodeAnalysis;

namespace MicroDotNet.Packages.Orm.Tools
{
    public static class SyntaxTreeTraversalHelpers
    {
        public static TNode FindParentNode<TNode>(this SyntaxNode current)
            where TNode : SyntaxNode
        {
            if (current == null)
            {
                return null;
            }

            var parent = current.Parent;
            if (parent is TNode node)
            {
                return node;
            }

            return parent.FindParentNode<TNode>();
        }
    }
}