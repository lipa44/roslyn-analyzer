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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CollectionsNullYieldReturnCodeFixProvider)), Shared]
    public class CollectionsNullYieldReturnCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(CollectionsNullYieldReturnAnalyzer.DiagnosticId);

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

        private async Task<Document> AddEmptyCollectionAsync(Document document, Diagnostic diagnostic, SyntaxNode root)
        {
            var returnExpression = root.FindNode(diagnostic.Location.SourceSpan)
                .FirstAncestorOrSelf<LiteralExpressionSyntax>();

            var method = returnExpression.FirstAncestorOrSelf<MethodDeclarationSyntax>();
            var returnType = method.ReturnType;

            ExpressionSyntax newReturnExpression;
            if (returnType.IsKind(SyntaxKind.ArrayType))
            {
                var arrayType = method.ReturnType as ArrayTypeSyntax;

                newReturnExpression = AnalyzerExtensions.CreateExpressionForArray(arrayType);
            }
            else if (returnType is GenericNameSyntax genericName && genericName.Identifier.Text == "List")
            {
                newReturnExpression = AnalyzerExtensions.CreateExpressionForList(genericName);
            }
            else
            {
                genericName = method.ReturnType as GenericNameSyntax;

                newReturnExpression = CreateExpressionForUndefined(genericName);
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
                return AnalyzerExtensions.CreateExpressionForArray(arrayType);
            }

            if (AnalyzerExtensions.IfTypeIsList(type))
            {
                var matches = new Regex(@"(List<(?<ListType>(\S*))>)").Matches(type);

                var returnType = GenericName("List")
                    .WithTypeArgumentList(
                        TypeArgumentList(
                            SingletonSeparatedList<TypeSyntax>(
                                IdentifierName(matches[0].Groups["ListType"].Value))));

                return AnalyzerExtensions.CreateExpressionForList(returnType);
            }

            return AnalyzerExtensions
                .CreateExpressionForUndefined(new Regex(@"(([^\<]*)<(?<GenericType>(\S*))>)").Matches(type)[0].Groups["GenericType"].Value);
        }
    }
}