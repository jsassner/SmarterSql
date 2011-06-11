using NUnit.Framework;
using Sassner.SmarterSql.Utils;

namespace SmarterSql.UnitTests {
	[TestFixture]
	public class CamelCaseMatcherTest : AssertionHelper {
		[Test]
		public void VerifyMatch() {
			string query = "KonViewerServerImpl3";

			Assert.IsTrue(CamelCaseMatcher.matchCamelCase("kon", query));
			Assert.IsTrue(CamelCaseMatcher.matchCamelCase("VSe", query));
			Assert.IsTrue(CamelCaseMatcher.matchCamelCase("server", query));
			Assert.IsTrue(CamelCaseMatcher.matchCamelCase("KVSerI", query));
			Assert.IsFalse(CamelCaseMatcher.matchCamelCase("KVSeriI", query));
			Assert.IsTrue(CamelCaseMatcher.matchCamelCase("kvs", query));
			Assert.IsTrue(CamelCaseMatcher.matchCamelCase("konvsei", query));
			Assert.IsTrue(CamelCaseMatcher.matchCamelCase("kvis", query));
			Assert.IsTrue(CamelCaseMatcher.matchCamelCase("Kon", query));
			Assert.IsTrue(CamelCaseMatcher.matchCamelCase("KVS", query));
			Assert.IsTrue(CamelCaseMatcher.matchCamelCase("KVI", query));
			Assert.IsTrue(CamelCaseMatcher.matchCamelCase("KoVSI", query));
			Assert.IsTrue(CamelCaseMatcher.matchCamelCase("K*3", query));
			Assert.IsTrue(CamelCaseMatcher.matchCamelCase("k*3", query));

			query = "checkRraltimeExitResultForUpdate";
			Assert.IsTrue(CamelCaseMatcher.matchCamelCase("checkre", query));
			Assert.IsTrue(CamelCaseMatcher.matchCamelCase("checkrer", query));
			Assert.IsTrue(CamelCaseMatcher.matchCamelCase("checkrerF", query));
			Assert.IsFalse(CamelCaseMatcher.matchCamelCase("checkrrF", query));
			Assert.IsTrue(CamelCaseMatcher.matchCamelCase("cReR", query));
			Assert.IsTrue(CamelCaseMatcher.matchCamelCase("cRR", query));

			query = "isSamePosition";
			Assert.IsTrue(CamelCaseMatcher.matchCamelCase("is", query));
			query = "shouldBeVisiblePosition";
			Assert.IsTrue(CamelCaseMatcher.matchCamelCase("is", query));

			query = "BR_ref_person_counterpart";
			Assert.IsTrue(CamelCaseMatcher.matchCamelCase("br_", query));
			Assert.IsTrue(CamelCaseMatcher.matchCamelCase("br_per", query));
			Assert.IsTrue(CamelCaseMatcher.matchCamelCase("br_co", query));
		}
	}
}
