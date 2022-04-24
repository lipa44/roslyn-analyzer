using System.Collections.Generic;

namespace AnalyzerTestCases
{
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

        public class StudentWithEqualsOverride
        {
            public int Age { get; set; }

            public static bool operator ==(StudentWithEqualsOverride a, StudentWithEqualsOverride b) => false;
            public static bool operator !=(StudentWithEqualsOverride a, StudentWithEqualsOverride b) => true;
            public override bool Equals(object? obj)
            {
                return false;
            }
        }

        public class StudentWithoutEqualsOverride<T>
        {
            public T Age { get; set; }
        }

        public static void Main()
        {
        }

        public static StudentWithoutEqualsOverride<int> StudentReturnStatement()
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

/*    public static bool IfEqualWithOneOverride(StudentWithoutEqualsOverride<int> s1, StudentWithEqualsOverride s2) => s1 == s2;

    public static bool IfEqualWithOneOverride(int s1, StudentWithEqualsOverride s2) => s1 == s2;

    public static bool IfEqualBothWithoutOverride(StudentWithoutEqualsOverride<int> s10, StudentWithoutEqualsOverride<int> s2) => s10 == s2;

    public static bool IfEqualBothWithOverride(StudentWithEqualsOverride s10, StudentWithEqualsOverride s2) => s10 == s2;

    public static bool IfEqualOneWithOverride(StudentWithEqualsOverride s1, StudentWithoutEqualsOverride<int> s2) => s1 == s2;

    public static bool IfEqualOneWithOverride(StudentWithoutEqualsOverride<int> s1, StudentWithEqualsOverride s2) => s1 == s2;*/

        public static bool IfEqualOneWithOverride(OverridenOperatorBase s1, NotOverridenOperatorInherited s2) => s1 == s2;
        public static bool IfEqualOneWithOverride(NotOverridenOperatorInherited s1, OverridenOperatorBase s2) => s1 == s2;
        public static bool IfEqualOneWithOverride(NotOverridenOperator s1, NotOverridenOperator s2) => s1 == s2;
        public static bool IfEqualOneWithOverride(NotOverridenOperator s1, object s2) => s1 == s2;

        public static bool IfEqualOneWithOverride(IAboba s1, IAboba s2) => s1 == s2;
    }
}