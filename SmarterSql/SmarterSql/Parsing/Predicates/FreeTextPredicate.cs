// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using Sassner.SmarterSql.Parsing.Expressions;

namespace Sassner.SmarterSql.Parsing.Predicates {
	public class FreeTextPredicate : Predicate {
		#region Member variables

		private readonly string searchCondition;

		#endregion

		public FreeTextPredicate(int startIndex, int endIndex, List<Expression> columnNameExpressions, string searchCondition)
			: base(startIndex, endIndex) {
			this.searchCondition = searchCondition;

			expressions = columnNameExpressions;
		}

		#region Public properties

		public string SearchCondition {
			get { return searchCondition; }
		}

		#endregion
	}
}