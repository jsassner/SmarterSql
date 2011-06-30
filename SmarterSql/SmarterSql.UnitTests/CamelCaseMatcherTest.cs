using System.Text.RegularExpressions;
using NUnit.Framework;
using Sassner.SmarterSql.Utils;

namespace SmarterSql.UnitTests {
	[TestFixture]
	public class CamelCaseMatcherTest : AssertionHelper {
		[Test]
		public void VerifyMatch() {
			string query = "KonViewerServerImpl3";

			Assert.IsTrue(CamelCaseMatcher.MatchCamelCase("kon", query));
			Assert.IsTrue(CamelCaseMatcher.MatchCamelCase("VSe", query));
			Assert.IsTrue(CamelCaseMatcher.MatchCamelCase("server", query));
			Assert.IsTrue(CamelCaseMatcher.MatchCamelCase("KVSerI", query));
			Assert.IsFalse(CamelCaseMatcher.MatchCamelCase("KVSeriI", query));
			Assert.IsTrue(CamelCaseMatcher.MatchCamelCase("kvs", query));
			Assert.IsTrue(CamelCaseMatcher.MatchCamelCase("konvsei", query));
			Assert.IsTrue(CamelCaseMatcher.MatchCamelCase("kvis", query));
			Assert.IsTrue(CamelCaseMatcher.MatchCamelCase("Kon", query));
			Assert.IsTrue(CamelCaseMatcher.MatchCamelCase("KVS", query));
			Assert.IsTrue(CamelCaseMatcher.MatchCamelCase("KVI", query));
			Assert.IsTrue(CamelCaseMatcher.MatchCamelCase("KoVSI", query));
			Assert.IsTrue(CamelCaseMatcher.MatchCamelCase("K*3", query));
			Assert.IsTrue(CamelCaseMatcher.MatchCamelCase("k*3", query));

			query = "checkRraltimeExitResultForUpdate";
			Assert.IsTrue(CamelCaseMatcher.MatchCamelCase("checkre", query));
			Assert.IsTrue(CamelCaseMatcher.MatchCamelCase("checkrer", query));
			Assert.IsTrue(CamelCaseMatcher.MatchCamelCase("checkrerF", query));
			Assert.IsFalse(CamelCaseMatcher.MatchCamelCase("checkrrF", query));
			Assert.IsTrue(CamelCaseMatcher.MatchCamelCase("cReR", query));
			Assert.IsTrue(CamelCaseMatcher.MatchCamelCase("cRR", query));

			query = "isSamePosition";
			Assert.IsTrue(CamelCaseMatcher.MatchCamelCase("is", query));
			query = "shouldBeVisiblePosition";
			Assert.IsTrue(CamelCaseMatcher.MatchCamelCase("is", query));

			query = "BR_ref_person_counterpart";
			Assert.IsTrue(CamelCaseMatcher.MatchCamelCase("br_", query));
			Assert.IsTrue(CamelCaseMatcher.MatchCamelCase("br_per", query));
			Assert.IsTrue(CamelCaseMatcher.MatchCamelCase("br_co", query));

			query = "@strTest";
			Assert.IsTrue(CamelCaseMatcher.MatchCamelCase("@str", query));
			Assert.IsTrue(CamelCaseMatcher.MatchCamelCase("@test", query));
			Assert.IsFalse(CamelCaseMatcher.MatchCamelCase("@atet", query));

			query = "@TblOrders";
			Assert.IsTrue(CamelCaseMatcher.MatchCamelCase("@tbl", query));

			Match match = CamelCaseMatcher.GetMatchCamelCase("@tbl", query);
			Assert.Greater(match.Groups.Count, 1);
			Group group = match.Groups[1];
			Assert.AreEqual(group.Index, 0);
			match = CamelCaseMatcher.GetMatchCamelCase("tbl", query);
			group = match.Groups[2];
			Assert.Greater(match.Groups.Count, 1);
			Assert.AreEqual(group.Index, 1);
		}
	}
}
