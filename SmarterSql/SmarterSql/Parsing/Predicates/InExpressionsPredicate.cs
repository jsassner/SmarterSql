// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using Sassner.SmarterSql.Parsing.Expressions;

namespace Sassner.SmarterSql.Parsing.Predicates {
	public class InExpressionsPredicate : Predicate {
		#region Member variables

		private readonly bool isNull;

		#endregion

		public InExpressionsPredicate(int startIndex, int endIndex, bool isNull, List<Expression> expressions)
			: base(startIndex, endIndex) {
			this.isNull = isNull;
			this.expressions = expressions;
		}

		#region Public properties

		public bool IsNull {
			get { return isNull; }
		}

		#endregion
	}
}