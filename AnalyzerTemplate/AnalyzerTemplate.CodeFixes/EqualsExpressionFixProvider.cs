using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Document = Microsoft.CodeAnalysis.Document;

namespace AnalyzerTemplate
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EqualsExpressionFixProvider)), Shared]
    public class EqualsExpressionFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(EqualsExpressionAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.UnsafeEqualityOperationTitle,
                    createChangedDocument: async c => await FixEqualsExpression(context.Document, diagnostic, root),
                    equivalenceKey: nameof(CodeFixResources.UnsafeEqualityOperationTitle)),
                diagnostic);
        }

        private async Task<Document> FixEqualsExpression(Document document, Diagnostic diagnostic, SyntaxNode root)
        {
            var semanticModel = await document.GetSemanticModelAsync().ConfigureAwait(false);

            var equalsBinaryExpressionSyntax = root.FindNode(diagnostic.Location.SourceSpan)
                .FirstAncestorOrSelf<BinaryExpressionSyntax>();

            var (leftOperand, rightOperand) = (equalsBinaryExpressionSyntax.Left, equalsBinaryExpressionSyntax.Right);
            var (leftOperandInfo, rightOperandInfo) = (semanticModel.GetSymbolInfo(leftOperand), semanticModel.GetSymbolInfo(rightOperand));

            var newExpression = InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(leftOperandInfo.Symbol.Name),
                        IdentifierName(nameof(Equals))))
                .WithArgumentList(
                    ArgumentList(
                        SingletonSeparatedList(
                            Argument(
                                IdentifierName(rightOperandInfo.Symbol.Name)))));

            var newRoot = root.ReplaceNode(equalsBinaryExpressionSyntax, newExpression);

            return await Task.FromResult(document.WithSyntaxRoot(newRoot));
        }
    }
}