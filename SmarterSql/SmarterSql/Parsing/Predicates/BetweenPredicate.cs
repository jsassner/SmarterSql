// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using Sassner.SmarterSql.Parsing.Expressions;

namespace Sassner.SmarterSql.Parsing.Predicates {
	public class BetweenPredicate : Predicate {
		#region Member variables

		private readonly bool isNull;

		#endregion

		public BetweenPredicate(int startIndex, int endIndex, bool isNull, Expression expression, Expression expression1, Expression expression2)
			: base(startIndex, endIndex) {
			this.isNull = isNull;

			AddExpression(expression);
			AddExpression(expression1);
			AddExpression(expression2);
		}

		#region Public properties

		public bool IsNull {
			get { return isNull; }
		}

		#endregion
	}
}