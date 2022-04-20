namespace AnalyzerTestCasesForExtensionTesting;

public class Program
{
    public interface IAboba
    {
    }

    public class OperatorBase
    {
        public static bool operator ==(OperatorBase a, OperatorBase b) => false;
        public static bool operator !=(OperatorBase a, OperatorBase b) => true;
    }

    public class OperatorInherited : OperatorBase { }

    public class StudentWithOverride
    {
        public int Age { get; set; }

        public static bool operator ==(StudentWithOverride a, StudentWithOverride b) => false;
        public static bool operator !=(StudentWithOverride a, StudentWithOverride b) => true;
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

    public static StudentWithoutOverride StudentReturnStatement()
    {
        return null;
    }

    public static List<int> ListReturnStatement()
    {
        return null;
    }

    public static int[] ArrayReturnStatement()
    {
        return null;
    }

    public static IEnumerable<List<int>> YieldIEnumerableReturnStatement()
    {
        yield return null;
    }

    public static IEnumerator<List<int>> YieldIEnumeratorReturnStatement()
    {
        yield return null;
    }

    public static bool IfEqualWithOneOverride(StudentWithoutOverride s1, StudentWithOverride s2) => s1 == s2;

    public static bool IfEqualWithOneOverride(int s1, StudentWithOverride s2) => s1 == s2;

    public static bool IfEqualBothWithoutOverride(StudentWithoutOverride s10, StudentWithoutOverride s2) => s10 == s2;

    public static bool IfEqualBothWithOverride(StudentWithOverride s10, StudentWithOverride s2) => s10 == s2;

    public static bool IfEqualOneWithOverride(StudentWithOverride s1, StudentWithoutOverride s2) => s1 == s2;

    public static bool IfEqualOneWithOverride(StudentWithoutOverride s1, StudentWithOverride s2) => s1 == s2;

    public static bool IfEqualOneWithOverride(OperatorBase s1, OperatorInherited s2) => s1 == s2;

    public static bool IfEqualOneWithOverride(IAboba s1, IAboba s2) => s1 == s2;
}