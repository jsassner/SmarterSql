// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Parsing.Expressions {
	public class ScalarSubqueryExpression : Expression {
		#region Member variables

		private readonly StatementSpans span;

		#endregion

		public ScalarSubqueryExpression(StatementSpans span) : base(span.StartIndex, null) {
			this.span = span;
		}

		#region Public properties

		public StatementSpans Span {
			get { return span; }
		}

		#endregion

		protected override bool GetParsedDataType(Connection connection, List<SysObject> lstSysObjects, List<TableSource> lstFoundTableSources, out ParsedDataType _parsedDataType) {
			if (null == span.Columns || 0 == span.Columns.Count) {
				_parsedDataType = null;
				return false;
			}
			_parsedDataType = span.Columns[0].ParsedDataType;
			return true;
		}
	}
}