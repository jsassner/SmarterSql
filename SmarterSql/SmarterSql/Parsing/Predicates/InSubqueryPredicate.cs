// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Parsing.Predicates {
	public class InSubqueryPredicate : Predicate {
		#region Member variables

		private readonly bool isNull;
		private readonly StatementSpans span;

		#endregion

		public InSubqueryPredicate(int startIndex, int endIndex, bool isNull, StatementSpans span)
			: base(startIndex, endIndex) {
			this.isNull = isNull;
			this.span = span;
		}

		#region Public properties

		public bool IsNull {
			get { return isNull; }
		}

		public StatementSpans Span {
			get { return span; }
		}

		#endregion
	}
}