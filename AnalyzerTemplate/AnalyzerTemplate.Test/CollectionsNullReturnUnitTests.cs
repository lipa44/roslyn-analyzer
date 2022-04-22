using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using VerifyNullReturnCS = AnalyzerTemplate.Test.Verifiers.CSharpCodeFixVerifier<
    AnalyzerTemplate.CollectionsNullReturnAnalyzer,
    AnalyzerTemplate.CollectionsNullReturnCodeFixProvider>;

namespace AnalyzerTemplate.Test
{
    [TestClass]
    public class CollectionsNullReturnUnitTests
    {
        [TestMethod]
        public async Task ReturnTypeListReturnsNull_Diagnostic()
        {
            var testCode = @"
using System.Collections.Generic;

public static class Test
{
    public static List<int> ListReturnStatement()
    {
        return {|#0:null|};
    }
}";

            var fixedTestCode = @"
using System.Collections.Generic;

public static class Test
{
    public static List<int> ListReturnStatement()
    {
        return new List<int>();
    }
}";

            var expectedDiagnosticResult = VerifyNullReturnCS
                .Diagnostic(CollectionsNullReturnAnalyzer.DiagnosticId)
                .WithArguments("null")
                .WithLocation(0);

            await VerifyNullReturnCS.VerifyCodeFixAsync(testCode, expectedDiagnosticResult, fixedTestCode);
        }
        
        [TestMethod]
        public async Task ReturnTypeArrayReturnsNull_Diagnostic()
        {
            var testCode = @"
public static class Test
{
    public static int[] ArrayReturnStatement()
    {
        return {|#0:null|};
    }
}";

            var fixedTestCode = @"
public static class Test
{
    public static int[] ArrayReturnStatement()
    {
        return new int[] { };
    }
}";

            var expectedDiagnosticResult = VerifyNullReturnCS
                .Diagnostic(CollectionsNullReturnAnalyzer.DiagnosticId)
                .WithArguments("null")
                .WithLocation(0);

            await VerifyNullReturnCS.VerifyCodeFixAsync(testCode, expectedDiagnosticResult, fixedTestCode);
        }

        [TestMethod]
        [Ignore]
        public async Task ReturnTypeIEnumerableReturnsNull_Diagnostic()
        {
            var testCode = @"
public static class Test
{
    public static IEnumerable<int> IEnumerableReturnStatement()
    {
        return {|#0:null|};
    }
}";

            var fixedTestCode = @"
public static class Test
{
    public static IEnumerable<int> IEnumerableReturnStatement()
    {
        return Array.Empty<int>();
    }
}";

            var expectedDiagnosticResult = VerifyNullReturnCS
                .Diagnostic(CollectionsNullReturnAnalyzer.DiagnosticId)
                .WithArguments("null")
                .WithLocation(0);

            await VerifyNullReturnCS.VerifyCodeFixAsync(testCode, expectedDiagnosticResult, fixedTestCode);
        }

        [TestMethod]
        [Ignore]
        public async Task ReturnTypeICollectionReturnsNull_Diagnostic()
        {
            var testCode = @"
public static class Test
{
    public static ICollection<int> ICollectionReturnStatement()
    {
        return {|#0:null|};
    }
}";

            var fixedTestCode = @"
public static class Test
{
    public static ICollection<int> ICollectionReturnStatement()
    {
        return Array.Empty<int>();
    }
}";

            var expectedDiagnosticResult = VerifyNullReturnCS
                .Diagnostic(CollectionsNullReturnAnalyzer.DiagnosticId)
                .WithArguments("null")
                .WithLocation(0);

            await VerifyNullReturnCS.VerifyCodeFixAsync(testCode, expectedDiagnosticResult, fixedTestCode);
        }

        [TestMethod]
        public async Task ReturnTypeNotCollectionReturnsNull_NoDiagnostic()
        {
            var testCode = @"
public static class Test
{
    public class GenericType<T>
    {
        public T Value { get; set; }
    }

    public static GenericType<int> NotCollectionReturnStatement()
    {
        return null;
    }
}";

            await VerifyNullReturnCS.VerifyAnalyzerAsync(testCode);
        }

        [TestMethod]
        public async Task ReturnTypeListReturnsList_NoDiagnostic()
        {
            var testCode = @"
using System.Collections.Generic;

public static class Test
{
    public static List<int> ListReturnStatement()
    {
        return new List<int>();
    }
}";

            await VerifyNullReturnCS.VerifyAnalyzerAsync(testCode);
        }

        [TestMethod]
        public async Task ReturnTypeArrayReturnsArray_NoDiagnostic()
        {
            var testCode = @"
public static class Test
{
    public static int[] ArrayReturnStatement()
    {
        return new int[] { };
    }
}";

            await VerifyNullReturnCS.VerifyAnalyzerAsync(testCode);
        }
    }
}