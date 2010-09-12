// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using System.Diagnostics;
using Sassner.SmarterSql.Parsing.Expressions;

namespace Sassner.SmarterSql.Parsing.Predicates {
	public abstract class Predicate {
		#region Public properties

		protected readonly int endIndex;
		protected readonly int startIndex;
		protected List<Expression> expressions = new List<Expression>();
		protected bool isPredicateAnd;
		protected bool isPredicateNot;
		protected bool isPredicateOr;

		#endregion

		protected Predicate(int startIndex, int endIndex) {
			this.startIndex = startIndex;
			this.endIndex = endIndex;
		}

		#region Public properties

		public List<Expression> Expressions {
			[DebuggerStepThrough]
			get { return expressions; }
		}

		public bool IsPredicateNot {
			[DebuggerStepThrough]
			get { return isPredicateNot; }
			set { isPredicateNot = value; }
		}

		public bool IsPredicateAnd {
			[DebuggerStepThrough]
			get { return isPredicateAnd; }
			set { isPredicateAnd = value; }
		}

		public bool IsPredicateOr {
			[DebuggerStepThrough]
			get { return isPredicateOr; }
			set { isPredicateOr = value; }
		}

		public int StartIndex {
			[DebuggerStepThrough]
			get { return startIndex; }
		}

		public int EndIndex {
			[DebuggerStepThrough]
			get { return endIndex; }
		}

		#endregion

		protected void AddExpression(Expression expression) {
			expressions.Add(expression);
		}
	}
}
