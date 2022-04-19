namespace AnalyzerTestCasesForExtensionTesting;

public class Program
{
    public class StudentWithOverride
    {
        public int Age { get; set; }

        public override bool Equals(object? obj)
        {
            return false;
        }
    }

    public class StudentWithoutOverride
    {
        public int Age { get; set; }
    }

    public static void Main()
    {
    }

    public static StudentWithoutOverride StudentReturnStatement() => null;

    public static List<int> ListReturnStatement() => null;

    public static int[] ArrayReturnStatement() => null;

    public static IEnumerable<List<int>> YieldIEnumerableReturnStatement()
    {
        yield return new List<int>();
    }
    
    public static IEnumerator<List<int>> YieldIEnumeratorReturnStatement()
    {
        yield return new List<int>();
    }

    public static bool IfEqualWithOneOverride(StudentWithoutOverride s1, StudentWithOverride s2) => s1 == s2;

    public static bool IfEqualWithOneOverride(int s1, StudentWithOverride s2) => s1 == s2;

    public static bool IfEqualBothWithoutOverride(StudentWithoutOverride s10, StudentWithoutOverride s2) => s10 == s2;

    public static bool IfEqualBothWithOverride(StudentWithOverride s10, StudentWithOverride s2) => s10 == s2;
    
    public static bool IfEqualOneWithOverride(StudentWithOverride s1, StudentWithoutOverride s2) => s1 == s2;
}