namespace AnalyzerTestCasesForExtensionTesting;

public class Program
{
    public interface IAboba { }

    public class OverridenOperatorBase
    {
        public static bool operator ==(OverridenOperatorBase a, OverridenOperatorBase b) => false;
        public static bool operator !=(OverridenOperatorBase a, OverridenOperatorBase b) => true;
    }

    public class NotOverridenOperatorInherited : OverridenOperatorBase { }
    public class NotOverridenOperator { }

    public class StudentWithEqualityOverride
    {
        public int Age { get; set; }

        public static bool operator ==(StudentWithEqualityOverride a, StudentWithEqualityOverride b) => false;
        public static bool operator !=(StudentWithEqualityOverride a, StudentWithEqualityOverride b) => true;
        public override bool Equals(object? obj)
        {
            return false;
        }
    }

    public class GenericStudent<T>
    {
        public T Age { get; set; }
    }

    public class StudentWithOverrideInBase : StudentWithEqualityOverride
    {
        public int Age { get; set; }
    }

    public static void Main()
    {
    }

    public static GenericStudent<int> StudentReturnStatement()
    {
        return null;
    }

    public static List<int> ListReturnStatement()
    {
        return null;
    }

    public static IEnumerable<int> ListRet1urnStatement()
    {
        return null;
    }

    public static IEnumerable<IEnumerable<IEnumerable<int>>> ListRet11urnStatement()
    {
        yield return null;
    }

    public static IEnumerator<int> ListRet1urnStsdfatement()
    {
        return null;
    }

    public static ICollection<int> ListR2eturnStatement()
    {
        return null;
    }

    public static int[] ArrayReturnStatement()
    {
        return null;
    }

    public static IEnumerable<List<int>> YieldIEnumerableReturnStatement()
    {
        return null;
    }

    public static IEnumerable<int[]> YieldIEnumerableReturnStatement1()
    {
        yield return null;
    }

    public static IEnumerator<List<int>> YieldIEnumeratorReturnStatement()
    {
        yield return null;
    }

    public static IEnumerable<IEnumerable<int>> YieldIEnumeratorReturnStatement1()
    {
        yield return null;
    }

    public bool IfEqualBothWithoutOverride(StudentWithOverrideInBase s1, StudentWithOverrideInBase s2) => s1 == s2; // ok
    public bool IfEqualBothWithOverride(StudentWithEqualityOverride s1, StudentWithEqualityOverride s2) => s1 == s2; // ok
    public bool IfEqualLeftWithOverride(StudentWithEqualityOverride s1, object s2) => s1 == s2; // ok
    public bool IfEqualLeftWithOverrideInBase(StudentWithOverrideInBase s1, object s2) => s1 == s2; // ok
    public bool IfEqualRightWithOverride(object s1, StudentWithEqualityOverride s2) => s1 == s2; // ok
    public bool IfEqualBothWithoutOverrideOverride(object s1, object s2) => s1 == s2; // ok
    public bool IfEqualStructs(int s1, int s2) => s1 == s2; // ok

    public bool IfEqualBothWithoutOverride(NotOverridenOperator s1, NotOverridenOperator s2) => s1 == s2; // ok
    public bool IfEqualBothWithOverride(OverridenOperatorBase s1, OverridenOperatorBase s2) => s1 == s2; // ok
    public bool IfEqualOneWithOverride(NotOverridenOperator s1, object s2) => s1 == s2;

    public bool IfEqualInterfaces(IAboba i1, IAboba i2) => i1 == i2; // ok
}