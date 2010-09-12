// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using System.Diagnostics;
using Sassner.SmarterSql.Parsing.Expressions;

namespace Sassner.SmarterSql.Parsing.SelectItems {
	internal class SelectItemExpression : SelectItem {
		#region Member variables

		private readonly string columnName;
		private readonly List<Expression> expressions;
		private readonly bool isAliasFirst;

		#endregion

		public SelectItemExpression(int startIndex, int endIndex, string columnName, bool isAliasFirst, List<Expression> expressions) {
			this.startIndex = startIndex;
			this.endIndex = endIndex;
			this.columnName = columnName;
			this.isAliasFirst = isAliasFirst;
			this.expressions = expressions;
		}

		#region Public properties

		public string ColumnName {
			[DebuggerStepThrough]
			get { return columnName; }
		}

		public bool IsAliasFirst {
			[DebuggerStepThrough]
			get { return isAliasFirst; }
		}

		public List<Expression> Expressions {
			[DebuggerStepThrough]
			get { return expressions; }
		}

		#endregion
	}
}