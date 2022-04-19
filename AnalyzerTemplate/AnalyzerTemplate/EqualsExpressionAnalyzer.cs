using System.Collections.Immutable;
using System.Linq;
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
/*            var semanticModel = context.SemanticModel;*/

            if (!(context.Node is BinaryExpressionSyntax equalsBinaryExpressionSyntax)) return;

            var left = equalsBinaryExpressionSyntax.Left;
            var right = equalsBinaryExpressionSyntax.Right;

/*            var leftTypeInfo = semanticModel.GetTypeInfo(left);
            var rightTypeInfo = semanticModel.GetTypeInfo(right);

            var leftType = leftTypeInfo.Type.TypeKind;
            var rightType = rightTypeInfo.Type.TypeKind;

            if (leftType != rightType) return;*/

            var equalsOperation = context.SemanticModel.GetOperation(equalsBinaryExpressionSyntax);

            var leftSymbolInfo = context.SemanticModel.GetPreprocessingSymbolInfo(left);
            var rightSymbolInfo = context.SemanticModel.GetPreprocessingSymbolInfo(right);

            context.ReportDiagnostic(Diagnostic
                .Create(Rule, equalsBinaryExpressionSyntax.GetLocation(), Category));
        }
    }
}
