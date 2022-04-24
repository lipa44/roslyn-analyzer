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

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
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
            var (leftTypeInfo, rightTypeInfo) = (semanticModel.GetTypeInfo(leftExpression), semanticModel.GetTypeInfo(rightExpression));
            var (leftTypeKind, rightTypeKind) = (leftTypeInfo.Type.TypeKind, rightTypeInfo.Type.TypeKind);

            if (leftTypeKind != rightTypeKind) return;
            if (leftTypeKind == TypeKind.Interface || rightTypeKind == TypeKind.Interface) return;
            
            var ifLeftOverridesEquality = AnalyzerExtensions.IfTypeOverridesOperator(leftTypeInfo.Type, "op_Equality");
            var ifRightOverridesEquality = AnalyzerExtensions.IfTypeOverridesOperator(rightTypeInfo.Type, "op_Equality");

            var condition = ifLeftOverridesEquality && ifRightOverridesEquality
                            || !ifLeftOverridesEquality && !ifRightOverridesEquality;

            if (!condition) return;

            var equalsOperation = context.SemanticModel.GetOperation(equalsBinaryExpressionSyntax);

            context.ReportDiagnostic(Diagnostic
                .Create(Rule, equalsBinaryExpressionSyntax.GetLocation(), context.Node.ToString()));
        }
    }
}