using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Composition;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Operations;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Document = Microsoft.CodeAnalysis.Document;
using TypeInfo = Microsoft.CodeAnalysis.TypeInfo;

namespace AnalyzerTemplate
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EqualsExpressionFixProvider)), Shared]
    public class EqualsExpressionFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds =>
            ImmutableArray.Create(EqualsExpressionAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.CodeFixTitle,
                    createChangedDocument: c => FixEqualsExpression(context.Document, diagnostic, root),
                    equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
                diagnostic);
        }

        private async Task<Document> FixEqualsExpression(Document document, Diagnostic diagnostic, SyntaxNode root)
        {
            var semanticModel = await document.GetSemanticModelAsync().ConfigureAwait(false);

            var equalsBinaryExpressionSyntax = root.FindNode(diagnostic.Location.SourceSpan)
                .FirstAncestorOrSelf<BinaryExpressionSyntax>();

            var left = equalsBinaryExpressionSyntax.Left;
            var right = equalsBinaryExpressionSyntax.Right;

            var equalsOperation = semanticModel.GetOperation(equalsBinaryExpressionSyntax);

            var leftTypeInfo = semanticModel.GetTypeInfo(left);
            var rightTypeInfo = semanticModel.GetTypeInfo(right);

            var leftSymbolInfo = semanticModel.GetSymbolInfo(left);
            var rightSymbolInfo = semanticModel.GetSymbolInfo(right);

            var leftType = leftTypeInfo.Type.TypeKind;
            var rightType = leftTypeInfo.Type.TypeKind;

            var ifLeftOverridesEquals = IfTypeOverridesMethod(leftTypeInfo, nameof(Equals));
            var ifRightOverridesEquals = IfTypeOverridesMethod(rightTypeInfo, nameof(Equals));

            if (ifRightOverridesEquals && ifLeftOverridesEquals)
            {
                var newExpression = InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName(leftSymbolInfo.Symbol.Name),
                            IdentifierName(nameof(Equals))))
                    .WithArgumentList(
                        ArgumentList(
                            SingletonSeparatedList(
                                Argument(
                                    IdentifierName(rightSymbolInfo.Symbol.Name)))));

                var newRoot = root.ReplaceNode(equalsOperation.Syntax, newExpression);

                return await Task.FromResult(document.WithSyntaxRoot(newRoot));
            }

            return await Task.FromResult(document);
        }

        public bool IfTypeOverridesMethod(TypeInfo typeInfo, string methodName)
        {
            var typeMethods = typeInfo.Type.GetMembers();

            if (typeMethods.IsDefaultOrEmpty) return false;

            var equalsMethod = typeMethods.SingleOrDefault(m => m.Name == methodName) as IMethodSymbol;

            return equalsMethod?.IsOverride ?? false;
        }

        public bool Overrides(MethodInfo baseMethod, Type type)
        {
            if (baseMethod == null) return false;
            if (type == null) return false;
            if (!type.IsSubclassOf(baseMethod.ReflectedType)) return false;

            while (type != baseMethod.ReflectedType)
            {
                var methods = type.GetMethods(BindingFlags.Instance |
                                              BindingFlags.DeclaredOnly |
                                              BindingFlags.Public |
                                              BindingFlags.NonPublic);

                if (methods.Any(m => m.GetBaseDefinition() == baseMethod))
                    return true;

                type = type.BaseType;
            }

            return false;
        }
    }
}