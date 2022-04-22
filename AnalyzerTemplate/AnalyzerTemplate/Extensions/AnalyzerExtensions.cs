using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace AnalyzerTemplate.Extensions
{
    public static class AnalyzerExtensions
    {
        private static readonly List<string> CollectionsList = new() {"IEnumerable", "IList", "ICollection", "IReadOnlyCollection", "List", "IReadOnlyList", "SortedList"};

        public static bool IfTypeIsArrayOrCollection(TypeSyntax returnType)
        {
            if (returnType is null) return false;

            if (returnType.IsKind(SyntaxKind.ArrayType)) return true;

            var genericName = returnType as GenericNameSyntax;

            return CollectionsList.Any(n => n.Equals(genericName?.Identifier.Text));

        }

        public static bool IfTypeIsArray(string type) => type.Contains("[]");
        public static bool IfTypeIsList(string type) => type.Contains("List<") && type.Contains(">");

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

        public static bool IfTypeOverridesOperator(ITypeSymbol typeInfo, string operatorName)
        {
            var typeMethods = typeInfo.GetMembers();

            if (typeMethods.IsDefaultOrEmpty) return false;

            var neededOperator = typeMethods.SingleOrDefault(m => m.Name == operatorName) as IMethodSymbol;

            return neededOperator != null;
        }

        public static bool IfTypeOverridesOperatorInBaseType(ITypeSymbol typeInfo, string operatorName)
        {
            if (IfTypeOverridesOperator(typeInfo, operatorName)) return true;

            var baseType = typeInfo.BaseType;

            while (baseType != null && baseType.Name != nameof(Object))
            {
                if (IfTypeOverridesOperator(baseType, operatorName)) return true;

                baseType = baseType.BaseType;
            }

            return false;
        }

        public static bool IfTypeOverridesMethod(TypeInfo typeInfo, string methodName)
        {
            var typeMethods = typeInfo.Type.GetMembers();

            if (typeMethods.IsDefaultOrEmpty) return false;

            var equalsMethod = typeMethods.SingleOrDefault(m => m.Name == methodName) as IMethodSymbol;

            return !(equalsMethod?.OverriddenMethod is null);
        }
    }
}