using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnalyzerTemplate.Test;

using VerifyNullReturnCS = AnalyzerTemplate.Test.Verifiers.CSharpCodeFixVerifier<
    AnalyzerTemplate.CollectionsNullReturnAnalyzer,
    AnalyzerTemplate.CollectionsNullReturnCodeFixProvider>;

using VerifyNullYieldReturnCS = AnalyzerTemplate.Test.Verifiers.CSharpCodeFixVerifier<
    AnalyzerTemplate.CollectionsNullYieldReturnAnalyzer,
    AnalyzerTemplate.CollectionsNullYieldReturnCodeFixProvider>;


public class CollectionsNullReturnUnitTests
{
    [TestMethod]
    public async Task TestName()
    {

    }
}