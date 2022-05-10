using System.Collections.Immutable;
using AnalyzerTemplate.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AnalyzerTemplate
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EqualsExpressionAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "UnsafeEqualsExpression";
        private const string Category = "Unsafe equals expression";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitleForUnsafeEqualityOperation), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormatForUnsafeEqualityOperation), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescriptionForUnsafeEqualityOperation), Resources.ResourceManager, typeof(Resources));
        private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

            context.RegisterSyntaxNodeAction(
                AnalyzeEqualsExpression,
                SyntaxKind.EqualsExpression);
        }

        public static void AnalyzeEqualsExpression(SyntaxNodeAnalysisContext context)
        {
            var semanticModel = context.SemanticModel;

            if (context.Node is not BinaryExpressionSyntax equalsBinaryExpressionSyntax) return;

            var (leftExpression, rightExpression) = (equalsBinaryExpressionSyntax.Left, equalsBinaryExpressionSyntax.Right);
            var (leftType, rightType) = (semanticModel.GetTypeInfo(leftExpression).Type, semanticModel.GetTypeInfo(rightExpression).Type);
            var (leftTypeKind, rightTypeKind) = (leftType.TypeKind, rightType.TypeKind);

            if (leftTypeKind != rightTypeKind) return;
            if (leftTypeKind == TypeKind.Interface || rightTypeKind == TypeKind.Interface) return;
            
            var ifLeftOverridesEquality = AnalyzerExtensions.IfTypeOverrides(leftType, "op_Equality", AnalyzerExtensions.IfOverridesOperator);
            var ifRightOverridesEquality = AnalyzerExtensions.IfTypeOverrides(rightType, "op_Equality", AnalyzerExtensions.IfOverridesOperator);

            var ifLeftOverridesEqualityInBase = AnalyzerExtensions.IfTypeOverridesInBaseType(leftType, "op_Equality", AnalyzerExtensions.IfOverridesOperator);
            var ifRightOverridesEqualityInBase = AnalyzerExtensions.IfTypeOverridesInBaseType(rightType, "op_Equality", AnalyzerExtensions.IfOverridesOperator);

            var ifLeftOverridesEquals = AnalyzerExtensions.IfTypeOverrides(leftType, nameof(Equals), AnalyzerExtensions.IfOverridesMethod);
            var ifRightOverridesEquals = AnalyzerExtensions.IfTypeOverrides(rightType, nameof(Equals), AnalyzerExtensions.IfOverridesMethod);

            var condition = leftTypeKind == TypeKind.Class && rightTypeKind == TypeKind.Class &&
                            (!ifLeftOverridesEqualityInBase && !ifRightOverridesEqualityInBase ||
                             !ifLeftOverridesEqualityInBase && ifRightOverridesEqualityInBase);

            if (!condition) return;

            context.ReportDiagnostic(Diagnostic
                .Create(Rule, equalsBinaryExpressionSyntax.GetLocation(), context.Node.ToString()));
        }
    }
}