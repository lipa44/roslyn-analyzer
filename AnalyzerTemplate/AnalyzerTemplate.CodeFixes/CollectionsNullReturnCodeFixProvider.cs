using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using AnalyzerTemplate.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AnalyzerTemplate
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CollectionsNullReturnCodeFixProvider)), Shared]
    public class CollectionsNullReturnCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(CollectionsNullReturnAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.CodeFixTitle,
                    createChangedDocument: c => AddEmptyCollectionAsync(context.Document, diagnostic, root),
                    equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
                diagnostic);
        }

        private Task<Document> AddEmptyCollectionAsync(Document document, Diagnostic diagnostic, SyntaxNode root)
        {
            var returnExpression = root.FindNode(diagnostic.Location.SourceSpan)
                .FirstAncestorOrSelf<LiteralExpressionSyntax>();

            var method = returnExpression.FirstAncestorOrSelf<MethodDeclarationSyntax>();

            if (method.ReturnType.IsKind(SyntaxKind.ArrayType))
            {
                var returnType = method.ReturnType as ArrayTypeSyntax;

                var newReturnExpression = AnalyzerExtensions.CreateExpressionForArray(returnType);

                var newRoot = root.ReplaceNode(returnExpression, newReturnExpression);

                return Task.FromResult(document.WithSyntaxRoot(newRoot));
            }

            if (method.ReturnType is GenericNameSyntax genericName && genericName.Identifier.Text == "List")
            {
                var newReturnExpression = AnalyzerExtensions.CreateExpressionForList(genericName);

                var newRoot = root.ReplaceNode(returnExpression, newReturnExpression);

                return Task.FromResult(document.WithSyntaxRoot(newRoot));
            }
            else
            {
                genericName = method.ReturnType as GenericNameSyntax;

                var returnTypeWithoutList = genericName?.TypeArgumentList.Arguments.ToString();

                var newReturnExpression = AnalyzerExtensions.CreateExpressionForUndefined(returnTypeWithoutList);

                var newRoot = root.ReplaceNode(returnExpression, newReturnExpression);

                return Task.FromResult(document.WithSyntaxRoot(newRoot));
            }
        }
    }
}