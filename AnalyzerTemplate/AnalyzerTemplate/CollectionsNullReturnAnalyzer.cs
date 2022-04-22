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

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Unsafe return expression";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

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
            if (!(context.Node is MethodDeclarationSyntax methodDeclaration)) return;

            if (!AnalyzerExtensions.IfTypeIsArrayOrCollection(methodDeclaration.ReturnType))
                return;

            var methodReturnStatements = methodDeclaration.Body?.DescendantNodes()
                .OfType<ReturnStatementSyntax>()
                .ToList();

            if (methodReturnStatements is null) return;

            var nullLiteralExpressions = methodReturnStatements
                .Where(s => s.Expression.IsKind(SyntaxKind.NullLiteralExpression))
                .ToList();

            foreach (var nullLiteralExpr in nullLiteralExpressions)
            {
                context.ReportDiagnostic(Diagnostic
                    .Create(Rule, nullLiteralExpr.Expression.GetLocation(), "Returning null instead of empty collection"));
            }
        }
    }
}
