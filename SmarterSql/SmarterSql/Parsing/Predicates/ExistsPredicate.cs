// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Parsing.Predicates {
	public class ExistsPredicate : Predicate {
		#region Member variables

		private readonly StatementSpans span;

		#endregion

		public ExistsPredicate(int startIndex, int endIndex, StatementSpans span)
			: base(startIndex, endIndex) {
			this.span = span;
		}

		#region Public properties

		public StatementSpans Span {
			get { return span; }
		}

		#endregion
	}
}