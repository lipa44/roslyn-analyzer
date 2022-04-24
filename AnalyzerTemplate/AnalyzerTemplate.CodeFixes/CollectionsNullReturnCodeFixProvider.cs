using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AnalyzerTemplate.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

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
                    title: CodeFixResources.NullCollectionsReturnTitle,
                    createChangedDocument: c => AddEmptyCollectionAsync(context.Document, diagnostic, root),
                    equivalenceKey: nameof(CodeFixResources.NullCollectionsReturnTitle)),
                diagnostic);
        }

        private async Task<Document> AddEmptyCollectionAsync(Document document, Diagnostic diagnostic, SyntaxNode root)
        {
            var returnExpression = root.FindNode(diagnostic.Location.SourceSpan)
                .FirstAncestorOrSelf<LiteralExpressionSyntax>();

            var method = returnExpression.FirstAncestorOrSelf<MethodDeclarationSyntax>();

            ExpressionSyntax newReturnExpression;
            if (method.ReturnType.IsKind(SyntaxKind.ArrayType))
            {
                newReturnExpression = CodeFixProviderExtensions.CreateExpressionForArray(method.ReturnType as ArrayTypeSyntax);
            }
            else if (method.ReturnType is GenericNameSyntax genericName && genericName.Identifier.Text == "List")
            {
                newReturnExpression = CodeFixProviderExtensions.CreateExpressionForList(genericName);
            }
            else if (returnExpression.IfContainsAncestor<ReturnStatementSyntax>())
            {
                genericName = method.ReturnType as GenericNameSyntax;

                var returnTypeWithoutList = genericName?.TypeArgumentList.Arguments.ToString();

                newReturnExpression = CodeFixProviderExtensions.CreateExpressionForUndefined(returnTypeWithoutList);
            }
            else if (returnExpression.IfContainsAncestor<YieldStatementSyntax>())
            {
                newReturnExpression = CreateExpressionForUndefined(method.ReturnType as GenericNameSyntax);
            }
            else
            {
                throw new Exception("BAD ANALYZER!!!!");
            }

            var newRoot = root.ReplaceNode(returnExpression, newReturnExpression);

            return await Task.FromResult(document.WithSyntaxRoot(newRoot));
        }

        private static ExpressionSyntax CreateExpressionForUndefined(GenericNameSyntax genericReturnType)
        {
            var type = genericReturnType.TypeArgumentList.Arguments.ToString();


            if (AnalyzerExtensions.IfTypeIsArray(type))
            {
                var matches = new Regex(@"((?<ArrayType>(\S*))\[])").Matches(type);

                var arrayType = ArrayType(ParseTypeName(matches[0].Groups["ArrayType"].Value));
                return CodeFixProviderExtensions.CreateExpressionForArray(arrayType);
            }

            if (AnalyzerExtensions.IfTypeIsList(type))
            {
                var matches = new Regex(@"(List<(?<ListType>(\S*))>)").Matches(type);

                var returnType = GenericName("List")
                    .WithTypeArgumentList(
                        TypeArgumentList(
                            SingletonSeparatedList<TypeSyntax>(
                                IdentifierName(matches[0].Groups["ListType"].Value))));

                return CodeFixProviderExtensions.CreateExpressionForList(returnType);
            }

            return CodeFixProviderExtensions
                .CreateExpressionForUndefined(new Regex(@"(([^\<]*)<(?<GenericType>(\S*))>)").Matches(type)[0].Groups["GenericType"].Value);
        }
    }
}