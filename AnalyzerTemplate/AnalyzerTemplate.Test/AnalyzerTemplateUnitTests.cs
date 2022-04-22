using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using VerifyCS = AnalyzerTemplate.Test.Verifiers.CSharpCodeFixVerifier<
    AnalyzerTemplate.CollectionsNullReturnAnalyzer,
    AnalyzerTemplate.CollectionsNullReturnCodeFixProvider>;

using VerifyEqualityCS = AnalyzerTemplate.Test.Verifiers.CSharpCodeFixVerifier<
    AnalyzerTemplate.EqualsExpressionAnalyzer,
    AnalyzerTemplate.EqualsExpressionFixProvider>;

namespace AnalyzerTemplate.Test;

[TestClass]
public class AnalyzerTemplateUnitTest
{
    [TestMethod]
    public async Task TestMethod1()
    {
        var test = @"";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task IfContainWrongCollectionReturnStatement()
    {
        var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace AnalyzerTestCases;

    public static class Program
    {
        public static void Main()
        {
            HuiSosi1();
        }

        public static List<int> HuiSosi1()
        {
            List<int> a = new List<int>();
            Console.WriteLine(""SOSI HUI)))))"");

            |#0:return null|;
        }
    }";

        var expected = VerifyCS.Diagnostic("AnalyzerTemplate")
            .WithLocation(0)
            .WithArguments("return null");

        Console.ReadLine();
    }
}