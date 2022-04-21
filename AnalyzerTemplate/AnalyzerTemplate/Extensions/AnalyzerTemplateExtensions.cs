using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace AnalyzerTemplate.Extensions
{
    public static class AnalyzerTemplateExtensions
    {
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