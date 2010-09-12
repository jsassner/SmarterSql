// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using Sassner.SmarterSql.Parsing.Expressions;

namespace Sassner.SmarterSql.Parsing.Predicates {
	public class NullPredicate : Predicate {
		#region Member variables

		private readonly bool isNull;

		#endregion

		public NullPredicate(int startIndex, int endIndex, Expression expression, bool isNull)
			: base(startIndex, endIndex) {
			this.isNull = isNull;

			expressions.Add(expression);
		}

		#region Public properties

		public bool IsNull {
			get { return isNull; }
		}

		#endregion
	}
}