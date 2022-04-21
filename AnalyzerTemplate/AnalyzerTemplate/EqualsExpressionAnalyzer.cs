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
        public const string DiagnosticId = "BadEqualsExpression";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Unsafe equals expression";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

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

            if (!(context.Node is BinaryExpressionSyntax equalsBinaryExpressionSyntax)) return;

            var left = equalsBinaryExpressionSyntax.Left;
            var right = equalsBinaryExpressionSyntax.Right;

            var leftTypeInfo = semanticModel.GetTypeInfo(left);
            var rightTypeInfo = semanticModel.GetTypeInfo(right);

            var leftTypeKind = leftTypeInfo.Type.TypeKind;
            var rightTypeKind = rightTypeInfo.Type.TypeKind;

            var ifLeftOverridesEquality = AnalyzerTemplateExtensions.IfTypeOverridesOperator(leftTypeInfo.Type, "op_Equality");
            var ifRightOverridesEquality = AnalyzerTemplateExtensions.IfTypeOverridesOperator(rightTypeInfo.Type, "op_Equality");

            if (leftTypeKind != rightTypeKind) return;
            if (leftTypeKind == TypeKind.Interface || rightTypeKind == TypeKind.Interface) return;

            var condition = (ifLeftOverridesEquality && ifRightOverridesEquality
                             || !ifLeftOverridesEquality && !ifRightOverridesEquality);

            if (!condition) return;

            var equalsOperation = context.SemanticModel.GetOperation(equalsBinaryExpressionSyntax);

            context.ReportDiagnostic(Diagnostic
                .Create(Rule, equalsBinaryExpressionSyntax.GetLocation(), context.Node.ToString()));
        }
    }
}
