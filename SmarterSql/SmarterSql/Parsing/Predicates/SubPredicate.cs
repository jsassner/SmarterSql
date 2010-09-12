// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using System.Diagnostics;
using Sassner.SmarterSql.Parsing.Expressions;

namespace Sassner.SmarterSql.Parsing.Predicates {
	public class SubPredicate : Predicate {
		#region Member variables

		private readonly Expression expression;

		#endregion

		public SubPredicate(int startIndex, int endIndex, Expression expression) : base(startIndex, endIndex) {
			this.expression = expression;
			AddExpression(expression);
		}

		#region Public properties

		public Expression Expression {
			[DebuggerStepThrough]
			get { return expression; }
		}

		#endregion
	}
}
