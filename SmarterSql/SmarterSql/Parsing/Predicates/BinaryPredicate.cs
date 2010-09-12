// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using Sassner.SmarterSql.Parsing.Expressions;

namespace Sassner.SmarterSql.Parsing.Predicates {
	public class BinaryPredicate : Predicate {
		#region Member variables

		#endregion

		public BinaryPredicate(int startIndex, int endIndex, Expression expression) : base(startIndex, endIndex) {
			AddExpression(expression);
		}

		#region Public properties

		#endregion
	}
}