using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using VerifyEqualityCS = AnalyzerTemplate.Test.CSharpCodeFixVerifier<
    AnalyzerTemplate.EqualsExpressionAnalyzer,
    AnalyzerTemplate.EqualsExpressionFixProvider>;

namespace AnalyzerTemplate.Test
{
    [TestClass]
    public class EqualityOperationUnitTests
    {
        [TestMethod]
        public async Task TwoClassesWithoutEqualityOverride_Diagnostic()
        {
            var testCode = @"
namespace AnalyzerTestCases;

public class Test
{
    public class StudentWithoutEqualsOverride
    {
        public int Age { get; set; }
    }

    public static bool IfStudentsAreEqual(StudentWithoutEqualsOverride s1, StudentWithoutEqualsOverride s2) => {|#0:s1 == s2|};
}";

            var fixedTestCode = @"
namespace AnalyzerTestCases;

public class Test
{
    public class StudentWithoutEqualsOverride
    {
        public int Age { get; set; }
    }

    public static bool IfStudentsAreEqual(StudentWithoutEqualsOverride s1, StudentWithoutEqualsOverride s2) => s1.Equals(s2);
}";

            var expectedDiagnosticResult = VerifyEqualityCS
                .Diagnostic(EqualsExpressionAnalyzer.DiagnosticId)
                .WithArguments("s1 == s2")
                .WithLocation(0);

            await VerifyEqualityCS.VerifyCodeFixAsync(testCode, expectedDiagnosticResult, fixedTestCode);
        }

        [TestMethod]
        public async Task TwoObjectsWithoutEqualityOverride_Diagnostic()
        {
            var testCode = @"
namespace AnalyzerTestCases;

public class Test
{
    public bool IfObjectsAreEqual(object o1, object o2) => {|#0:o1 == o2|};
}";

            var fixedTestCode = @"
namespace AnalyzerTestCases;

public class Test
{
    public bool IfObjectsAreEqual(object o1, object o2) => o1.Equals(o2);
}";

            var expectedDiagnosticResult = VerifyEqualityCS
                .Diagnostic(EqualsExpressionAnalyzer.DiagnosticId)
                .WithArguments("s1 == s2")
                .WithLocation(0);

            await VerifyEqualityCS.VerifyCodeFixAsync(testCode, expectedDiagnosticResult, fixedTestCode);
        }

        [TestMethod]
        public async Task TwoClassesWithEqualityOverride_NoDiagnostic()
        {
            var testCode = @"
namespace AnalyzerTestCases;

public class Test
{
    public class StudentWithEqualityOverride
    {
        public int Age { get; set; }
        public static bool operator ==(StudentWithEqualityOverride s1, StudentWithEqualityOverride s2) => false;
        public static bool operator !=(StudentWithEqualityOverride s1, StudentWithEqualityOverride s2) => true;
    }

    public bool IfStudentsAreEqual(StudentWithEqualityOverride s1, StudentWithEqualityOverride s2) => s1 == s2;
}";

            await VerifyEqualityCS.VerifyAnalyzerAsync(testCode);
        }

        [TestMethod]
        public async Task TwoClassesWithEqualityOverrideInBase_NoDiagnostic()
        {
            var testCode = @"
namespace AnalyzerTestCases;

public class Test
{
    public class StudentWithEqualityOverride
    {
        public int Age { get; set; }
        public static bool operator ==(StudentWithEqualityOverride s1, StudentWithEqualityOverride s2) => false;
        public static bool operator !=(StudentWithEqualityOverride s1, StudentWithEqualityOverride s2) => true;
    }

    public class StudentWithOverrideInBase : StudentWithEqualityOverride { }

    public bool IfStudentsAreEqual(StudentWithOverrideInBase s1, StudentWithOverrideInBase s2) => s1 == s2;
}";

            await VerifyEqualityCS.VerifyAnalyzerAsync(testCode);
        }

        [TestMethod]
        public async Task TwoStructs_NoDiagnostic()
        {
            var testCode = @"
namespace AnalyzerTestCases;

public class Test
{
    public bool IfStructsAreEqual(int s1, int s2) => s1 == s2;
}";

            await VerifyEqualityCS.VerifyAnalyzerAsync(testCode);
        }

        [TestMethod]
        public async Task LeftWithOverrideRightWithoutOverride_NoDiagnostic()
        {
            var testCode = @"
namespace AnalyzerTestCases;

public class Test
{
    public class StudentWithEqualityOverride
    {
        public int Age { get; set; }
        public static bool operator ==(StudentWithEqualityOverride s1, StudentWithEqualityOverride s2) => false;
        public static bool operator !=(StudentWithEqualityOverride s1, StudentWithEqualityOverride s2) => true;
    }

    public bool IfStudentsAreEqual(StudentWithEqualityOverride s1, object s2) => s1 == s2;
}";

            await VerifyEqualityCS.VerifyAnalyzerAsync(testCode);
        }

        [TestMethod]
        public async Task LeftWithOverrideInBaseRightWithoutOverride_NoDiagnostic()
        {
            var testCode = @"
namespace AnalyzerTestCases;

public class Test
{
    public class StudentWithEqualityOverride
    {
        public int Age { get; set; }
        public static bool operator ==(StudentWithEqualityOverride s1, StudentWithEqualityOverride s2) => false;
        public static bool operator !=(StudentWithEqualityOverride s1, StudentWithEqualityOverride s2) => true;
    }

    public class StudentWithOverrideInBase : StudentWithEqualityOverride { }

    public bool IfStudentsAreEqual(StudentWithOverrideInBase s1, object s2) => s1 == s2;
}";

            await VerifyEqualityCS.VerifyAnalyzerAsync(testCode);
        }

        [TestMethod]
        public async Task RightWithOverrideLeftWithoutOverride_Diagnostic()
        {
            var testCode = @"
namespace AnalyzerTestCases;

public class Test
{
    public class StudentWithEqualityOverride
    {
        public int Age { get; set; }
        public static bool operator ==(StudentWithEqualityOverride s1, StudentWithEqualityOverride s2) => false;
        public static bool operator !=(StudentWithEqualityOverride s1, StudentWithEqualityOverride s2) => true;
    }

    public bool IfStudentsAreEqual(object s1, StudentWithEqualityOverride s2) => {|#0:s1 == s2|};
}";
            
            var fixedTestCode = @"
namespace AnalyzerTestCases;

public class Test
{
    public class StudentWithEqualityOverride
    {
        public int Age { get; set; }
        public static bool operator ==(StudentWithEqualityOverride s1, StudentWithEqualityOverride s2) => false;
        public static bool operator !=(StudentWithEqualityOverride s1, StudentWithEqualityOverride s2) => true;
    }

    public bool IfStudentsAreEqual(object s1, StudentWithEqualityOverride s2) => s1.Equals(s2);
}";

            var expectedDiagnosticResult = VerifyEqualityCS
                .Diagnostic(EqualsExpressionAnalyzer.DiagnosticId)
                .WithArguments("s1 == s2")
                .WithLocation(0);

            await VerifyEqualityCS.VerifyCodeFixAsync(testCode, expectedDiagnosticResult, fixedTestCode);
        }

        [TestMethod]
        public async Task TwoInterfaces_NoDiagnostic()
        {
            var testCode = @"
namespace AnalyzerTestCases;

public static class Test
{
    public interface IStudent
    {
        public int Age { get; set; }
    }

    public static bool IfInterfacesEquals(IStudent s1, IStudent s2) => s1 == s2;
}";

            await VerifyEqualityCS.VerifyAnalyzerAsync(testCode);
        }

        [TestMethod]
        public async Task TwoClassesWithoutOverride_Diagnostic()
        {
            var testCode = @"
namespace AnalyzerTestCases;

public class Test
{
    public class StudentWithoutEqualsOverride
    {
        public int Age { get; set; }
    }

    public bool IfStudentsAreEqual(StudentWithoutEqualsOverride s1, object s2) => {|#0:s1 == s2|};
}";

            var fixedTestCode = @"
namespace AnalyzerTestCases;

public class Test
{
    public class StudentWithoutEqualsOverride
    {
        public int Age { get; set; }
    }

    public bool IfStudentsAreEqual(StudentWithoutEqualsOverride s1, object s2) => s1.Equals(s2);
}";

            var expectedDiagnosticResult = VerifyEqualityCS
                .Diagnostic(EqualsExpressionAnalyzer.DiagnosticId)
                .WithArguments("s1 == s2")
                .WithLocation(0);

            await VerifyEqualityCS.VerifyCodeFixAsync(testCode, expectedDiagnosticResult, fixedTestCode);
        }
    }
}