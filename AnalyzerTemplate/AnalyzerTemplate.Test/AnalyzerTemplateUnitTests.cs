using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = AnalyzerTemplate.Test.CSharpCodeFixVerifier<
    AnalyzerTemplate.CollectionsNullReturnAnalyzer,
    AnalyzerTemplate.CollectionsNullReturnCodeFixProvider>;

namespace AnalyzerTemplate.Test
{
    [TestClass]
    public class AnalyzerTemplateUnitTest
    {
        // No diagnostics expected to show up
        [TestMethod]
        public async Task TestMethod1()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public async Task TestMethod2()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class {|#0:TypeName|}
        {   
        }
    }";

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TYPENAME
        {   
        }
    }";

            var expected = VerifyCS.Diagnostic("AnalyzerTemplate").WithLocation(0).WithArguments("TypeName");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
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

            var expected = VerifyCS.Diagnostic("AnalyzerTemplate").WithLocation(0).WithArguments("return null");
            Console.ReadLine();
        }
    }
}
