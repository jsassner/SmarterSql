// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using Sassner.SmarterSql.Objects;

namespace Sassner.SmarterSql.Parsing.Expressions {
	public class ConstantExpression : Expression {
		#region Member variables

		#endregion

		public ConstantExpression(int tokenIndex, ParsedDataType parsedDataType) : base(tokenIndex, parsedDataType) {
		}

		#region Public properties

		#endregion

		protected override bool GetParsedDataType(Connection connection, List<SysObject> lstSysObjects, List<TableSource> lstFoundTableSources, out ParsedDataType _parsedDataType) {
			_parsedDataType = parsedDataType;
			return true;
		}
	}
}