using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using VerifyNullYieldReturnCS = AnalyzerTemplate.Test.Verifiers.CSharpCodeFixVerifier<
    AnalyzerTemplate.CollectionsNullYieldReturnAnalyzer,
    AnalyzerTemplate.CollectionsNullYieldReturnCodeFixProvider>;

namespace AnalyzerTemplate.Test
{
    [TestClass]
    public class CollectionsNullYieldReturnUnitTests
    {
        [TestMethod]
        [Ignore]
        public async Task ReturnTypeListReturnsNull_Diagnostic()
        {
            var testCode = @"
using System.Collections.Generic;

public static class Test
{
    public static IEnumerable<List<int>> ListYieldReturnStatement()
    {
        yield return {|#0:null|};
    }
}";

            var fixedTestCode = @"
using System.Collections.Generic;

public static class Test
{
    public static IEnumerable<List<int>> ListYieldReturnStatement()
    {
        yield return new List<int>();
    }
}";

            var expectedDiagnosticResult = VerifyNullYieldReturnCS
                .Diagnostic(CollectionsNullYieldReturnAnalyzer.DiagnosticId)
                .WithArguments("null")
                .WithLocation(0);

            await VerifyNullYieldReturnCS.VerifyCodeFixAsync(testCode, expectedDiagnosticResult, fixedTestCode);
        }
        
        [TestMethod]
        public async Task ReturnTypeArrayReturnsNull_Diagnostic()
        {
            var testCode = @"
using System.Collections.Generic;

public static class Test
{
    public static IEnumerable<int[]> ArrayYieldReturnStatement()
    {
        yield return {|#0:null|};
    }
}";

            var fixedTestCode = @"
using System.Collections.Generic;

public static class Test
{
    public static IEnumerable<int[]> ArrayYieldReturnStatement()
    {
        yield return new int[] { };
    }
}";

            var expectedDiagnosticResult = VerifyNullYieldReturnCS
                .Diagnostic(CollectionsNullYieldReturnAnalyzer.DiagnosticId)
                .WithArguments("null")
                .WithLocation(0);

            await VerifyNullYieldReturnCS.VerifyCodeFixAsync(testCode, expectedDiagnosticResult, fixedTestCode);
        }

        [TestMethod]
        [Ignore]
        public async Task ReturnTypeIEnumerableReturnsNull_Diagnostic()
        {
            var testCode = @"
public static class Test
{
    public static IEnumerable<IEnumerable<int>> IEnumerableYieldReturnStatement()
    {
        yield return {|#0:null|};
    }
}";

            var fixedTestCode = @"
public static class Test
{
    public static IEnumerable<IEnumerable<int>> IEnumerableYieldReturnStatement()
    {
        yield return Array.Empty<int>();
    }
}";

            var expectedDiagnosticResult = VerifyNullYieldReturnCS
                .Diagnostic(CollectionsNullYieldReturnAnalyzer.DiagnosticId)
                .WithArguments("null")
                .WithLocation(0);

            await VerifyNullYieldReturnCS.VerifyCodeFixAsync(testCode, expectedDiagnosticResult, fixedTestCode);
        }

        [TestMethod]
        [Ignore]
        public async Task ReturnTypeICollectionReturnsNull_Diagnostic()
        {
            var testCode = @"
public static class Test
{
    public static IEnumerable<ICollection<int>> ICollectionYieldReturnStatement()
    {
        yield return {|#0:null|};
    }
}";

            var fixedTestCode = @"
public static class Test
{
    public static IEnumerable<ICollection<int>> ICollectionYieldReturnStatement()
    {
        yield return Array.Empty<int>();
    }
}";

            var expectedDiagnosticResult = VerifyNullYieldReturnCS
                .Diagnostic(CollectionsNullYieldReturnAnalyzer.DiagnosticId)
                .WithArguments("null")
                .WithLocation(0);

            await VerifyNullYieldReturnCS.VerifyCodeFixAsync(testCode, expectedDiagnosticResult, fixedTestCode);
        }

        [TestMethod]
        public async Task ReturnTypeListReturnsList_NoDiagnostic()
        {
            var testCode = @"
using System.Collections.Generic;

public static class Test
{
    public static IEnumerable<List<int>> ListYieldReturnStatement()
    {
        yield return new List<int>();
    }
}";

            await VerifyNullYieldReturnCS.VerifyAnalyzerAsync(testCode);
        }

        [TestMethod]
        public async Task ReturnTypeArrayReturnsArray_NoDiagnostic()
        {
            var testCode = @"
using System.Collections.Generic;

public static class Test
{
    public static IEnumerable<int[]> ArrayReturnStatement()
    {
        yield return new int[] { };
    }
}";

            await VerifyNullYieldReturnCS.VerifyAnalyzerAsync(testCode);
        }
    }
}