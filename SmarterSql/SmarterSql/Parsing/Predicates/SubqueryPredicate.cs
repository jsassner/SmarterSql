// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;

namespace Sassner.SmarterSql.Parsing.Predicates {
	public class SubqueryPredicate : Predicate {
		#region Member variables

		private readonly List<Predicate> predicates;

		#endregion

		public SubqueryPredicate(int startIndex, int endIndex, List<Predicate> predicates) : base(startIndex, endIndex) {
			this.predicates = predicates;
		}

		#region Public properties

		public List<Predicate> Predicates {
			get { return predicates; }
		}

		#endregion
	}
}