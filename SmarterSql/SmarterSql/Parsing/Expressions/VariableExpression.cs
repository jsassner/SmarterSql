// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using Sassner.SmarterSql.Objects;

namespace Sassner.SmarterSql.Parsing.Expressions {
	public class VariableExpression : Expression {
		#region Member variables

		#endregion

		public VariableExpression(int tokenIndex, ParsedDataType parsedDataType) : base(tokenIndex, parsedDataType) {
		}

		#region Public properties

		#endregion

		protected override bool GetParsedDataType(Connection connection, List<SysObject> lstSysObjects, List<TableSource> lstFoundTableSources, out ParsedDataType _parsedDataType) {
			_parsedDataType = parsedDataType;
			return true;
		}
	}
}