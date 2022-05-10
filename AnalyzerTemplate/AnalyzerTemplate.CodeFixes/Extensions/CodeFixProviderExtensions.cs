using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace AnalyzerTemplate.Extensions
{
    public static class CodeFixProviderExtensions
    {
        public static bool IfContainsAncestor<TNode>(this SyntaxNode node)
            where TNode : SyntaxNode
        {
            return node.FirstAncestorOrSelf<TNode>() is TNode;
        }

        public static ArrayCreationExpressionSyntax CreateExpressionForArray(ArrayTypeSyntax arrayType)
        {
            return ArrayCreationExpression(
                    arrayType
                        .WithRankSpecifiers(
                            SingletonList(
                                ArrayRankSpecifier(
                                    SingletonSeparatedList<ExpressionSyntax>(
                                        OmittedArraySizeExpression())))))
                .WithInitializer(
                    InitializerExpression(
                        SyntaxKind.ArrayInitializerExpression));
        }

        public static ObjectCreationExpressionSyntax CreateExpressionForList(GenericNameSyntax listType)
        {
            return ObjectCreationExpression(listType)
                .WithArgumentList(ArgumentList());
        }

        public static InvocationExpressionSyntax CreateExpressionForUndefined(string type)
        {
            return InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName("Array"),
                    GenericName(
                            Identifier("Empty"))
                        .WithTypeArgumentList(
                            TypeArgumentList(
                                SingletonSeparatedList<TypeSyntax>(
                                    IdentifierName(type))))));
        }
        
        // private static 
    }
}
