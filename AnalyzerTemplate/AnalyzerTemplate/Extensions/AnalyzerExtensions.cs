using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AnalyzerTemplate.Extensions
{
    public static class AnalyzerExtensions
    {
        private static readonly List<string> CollectionsList = new() {"IEnumerable", "IList", "ICollection", "IReadOnlyCollection", "List", "IReadOnlyList", "SortedList"};

        public static bool IfTypeIsArrayOrCollection(TypeSyntax returnType)
        {
            if (returnType is null) return false;

            return returnType.IsKind(SyntaxKind.ArrayType) ||
                   CollectionsList.Any(n => n.Equals((returnType as GenericNameSyntax)?.Identifier.Text));
        }

        public static bool IfTypeIsArray(string type) => type.Contains("[]");
        public static bool IfTypeIsList(string type) => type.Contains("List<") && type.Contains(">");

        public static bool IfTypeOverrides(ITypeSymbol typeInfo, string overrideName,
            Func<IEnumerable<IMethodSymbol>, bool> comparer)
        {
            var typeMethods = typeInfo.GetMembers();

            if (typeMethods.IsDefaultOrEmpty) return false;

            var neededOperator = typeMethods
                .Where(m => m.Name == overrideName)
                .Select(m => m as IMethodSymbol);

            return comparer(neededOperator);
        }

        public static bool IfTypeOverridesInBaseType(ITypeSymbol typeInfo, string operatorName,
            Func<IEnumerable<IMethodSymbol>, bool> comparer)
        {
            if (IfTypeOverrides(typeInfo, operatorName, comparer)) return true;

            var baseType = typeInfo.BaseType;

            while (baseType != null && baseType.Name != nameof(Object))
            {
                if (IfTypeOverrides(baseType, operatorName, comparer)) return true;

                baseType = baseType.BaseType;
            }

            return false;
        }

        public static bool Overrides(MethodInfo baseMethod, Type type)
        {
            if (baseMethod == null) return false;
            if (type == null) return false;
            if (!type.IsSubclassOf(baseMethod.ReflectedType)) return false;

            while (type != baseMethod.ReflectedType)
            {
                var methods = type.GetMethods(BindingFlags.Instance |
                                              BindingFlags.DeclaredOnly |
                                              BindingFlags.Public |
                                              BindingFlags.NonPublic);

                if (methods.Any(m => m.GetBaseDefinition() == baseMethod))
                    return true;

                type = type.BaseType;
            }

            return false;
        }

        public static readonly Func<IEnumerable<IMethodSymbol>, bool> IfOverridesMethod =
            methodSymbols => methodSymbols.Any(m => m.OverriddenMethod is not null);

        public static readonly Func<IEnumerable<IMethodSymbol>, bool> IfOverridesOperator =
            methodSymbols => methodSymbols.Any();
    }
}