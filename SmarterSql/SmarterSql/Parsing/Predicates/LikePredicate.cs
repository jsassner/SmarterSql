// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Diagnostics;
using Sassner.SmarterSql.Parsing.Expressions;

namespace Sassner.SmarterSql.Parsing.Predicates {
	public class LikePredicate : Predicate {
		#region Member variables

		private readonly string escapeChar;
		private readonly bool isNull;

		#endregion

		public LikePredicate(int startIndex, int endIndex, bool isNull, Expression expression, Expression stringExpression, string escapeChar)
			: base(startIndex, endIndex) {
			this.isNull = isNull;
			this.escapeChar = escapeChar;

			AddExpression(expression);
			AddExpression(stringExpression);
		}

		#region Public properties

		public bool IsNull {
			[DebuggerStepThrough]
			get { return isNull; }
		}

		public string EscapeChar {
			[DebuggerStepThrough]
			get { return escapeChar; }
		}

		#endregion
	}
}