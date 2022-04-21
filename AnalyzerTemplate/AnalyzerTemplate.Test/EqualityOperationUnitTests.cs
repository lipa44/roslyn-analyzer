using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyEqualityCS = AnalyzerTemplate.Test.Verifiers.CSharpCodeFixVerifier<
    AnalyzerTemplate.EqualsExpressionAnalyzer,
    AnalyzerTemplate.EqualsExpressionFixProvider>;

namespace AnalyzerTemplate.Test
{
    public class EqualityOperationUnitTests
    {
        [TestMethod]
        public async Task TwoClassesWithoutEqualityOverride_Diagnostic()
        {
              var testCode = @"
      using System;
     
      namespace AnalyzerTestCases;

      public static class Test
      {
          public class StudentWithoutEqualsOverride
          {
              public int Age { get; set; }
          }

          public static bool IfInterfacesEquals(StudentWithoutEqualsOverride s1, StudentWithoutEqualsOverride s2) => {|#0:s1 == s2|};
      }";

            var fixedTestCode = @"
    using System;
   
    namespace AnalyzerTestCases;

    public static class Test
    {
        public class StudentWithoutEqualsOverride
        {
            public int Age { get; set; }
        }

        public static bool IfInterfacesEquals(StudentWithoutEqualsOverride s1, StudentWithoutEqualsOverride s2) => s1.Equals(s2);
    }";

            var expectedDiagnosticResult = VerifyEqualityCS
                .Diagnostic(EqualsExpressionAnalyzer.DiagnosticId)
                .WithArguments("s1 == s2")
                .WithLocation(0);

            await VerifyEqualityCS.VerifyCodeFixAsync(testCode, expectedDiagnosticResult, fixedTestCode);
        }
    }
}