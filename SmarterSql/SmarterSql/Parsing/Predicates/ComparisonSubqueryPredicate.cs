// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using Sassner.SmarterSql.Parsing.Expressions;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Parsing.Predicates {
	public class ComparisonSubqueryPredicate : Predicate {
		#region Member variables

		private readonly StatementSpans span;
		private readonly TokenInfo subQueryOperator;

		#endregion

		public ComparisonSubqueryPredicate(int startIndex, int endIndex, Expression expression, TokenInfo subQueryOperator, StatementSpans span)
			: base(startIndex, endIndex) {
			this.subQueryOperator = subQueryOperator;
			this.span = span;

			expressions.Add(expression);
		}

		#region Public properties

		public TokenInfo SubQueryOperator {
			get { return subQueryOperator; }
		}

		public StatementSpans Span {
			get { return span; }
		}

		#endregion
	}
}