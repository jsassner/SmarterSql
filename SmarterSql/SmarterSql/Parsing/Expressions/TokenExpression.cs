// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.ParsingUtils;

namespace Sassner.SmarterSql.Parsing.Expressions {
	internal class TokenExpression : Expression {
		#region Member variables

		private readonly TokenInfo token;

		#endregion

		public TokenExpression(int tokenIndex, TokenInfo token) : base(tokenIndex, null) {
			this.token = token;
		}

		#region Public properties

		public TokenInfo Token {
			get { return token; }
		}

		#endregion

		protected override bool GetParsedDataType(Connection connection, List<SysObject> lstSysObjects, List<TableSource> lstFoundTableSources, out ParsedDataType _parsedDataType) {
			_parsedDataType = null;
			return false;
		}
	}
}