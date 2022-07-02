using System.Collections.Immutable;
using System.Linq;
using AnalyzerTemplate.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AnalyzerTemplate
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CollectionsNullReturnAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CollectionsNullReturn";
        private const string Category = "Unsafe return expression";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitleForNullCollectionsReturn), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormatForNullCollectionsReturn), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescriptionForNullCollectionsReturn), Resources.ResourceManager, typeof(Resources));
        private static readonly DiagnosticDescriptor Rule = new (DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

            context.RegisterSyntaxNodeAction(
                AnalyzeMethodWithCollectionAsReturnType,
                SyntaxKind.MethodDeclaration);
        }

        public static void AnalyzeMethodWithCollectionAsReturnType(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not MethodDeclarationSyntax methodDeclaration) return;
            if (!AnalyzerExtensions.IfTypeIsArrayOrCollection(methodDeclaration.ReturnType)) return;
            if (methodDeclaration.Body is null) return;

            var returnStatements = methodDeclaration.Body.DescendantNodes()
                .OfType<ReturnStatementSyntax>();

            var yieldReturnStatements = methodDeclaration.Body.DescendantNodes()
                .OfType<YieldStatementSyntax>();

            foreach (var nullLiteralExpr in returnStatements
                         .Where(s => s.Expression.IsKind(SyntaxKind.NullLiteralExpression)))
            {
                context.ReportDiagnostic(Diagnostic
                    .Create(Rule, nullLiteralExpr.Expression.GetLocation(), nullLiteralExpr.ToString()));
            }

            foreach (var nullLiteralExpr in yieldReturnStatements
                         .Where(s => s.Expression.IsKind(SyntaxKind.NullLiteralExpression)))
            {
                context.ReportDiagnostic(Diagnostic
                    .Create(Rule, nullLiteralExpr.Expression.GetLocation(), nullLiteralExpr.ToString()));
            }
        }
    }
}