// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.ParsingUtils;

namespace Sassner.SmarterSql.Parsing.Expressions {
	public class BinaryOperatorExpression : Expression {
		#region Member variables

		private readonly int endIndex;
		private readonly Expression expression1;
		private readonly Expression expression2;
		private readonly TokenInfo operatorToken;
		private readonly int startIndex;

		#endregion

		public BinaryOperatorExpression(int startIndex, int endIndex, Expression expression1, TokenInfo operatorToken, Expression expression2) : base(startIndex, expression1.ParsedDataType) {
			this.startIndex = startIndex;
			this.endIndex = endIndex;
			this.expression1 = expression1;
			this.operatorToken = operatorToken;
			this.expression2 = expression2;
		}

		#region Public properties

		public int StartIndex {
			get { return startIndex; }
		}

		public int EndIndex {
			get { return endIndex; }
		}

		public Expression Expression1 {
			get { return expression1; }
		}

		public TokenInfo OperatorToken {
			get { return operatorToken; }
		}

		public Expression Expression2 {
			get { return expression2; }
		}

		#endregion

		protected override bool GetParsedDataType(Connection connection, List<SysObject> lstSysObjects, List<TableSource> lstFoundTableSources, out ParsedDataType _parsedDataType) {
			List<Expression> expressions = new List<Expression> {
				expression1,
				expression2
			};
			List<ParsedDataType> parsedDataTypes = new List<ParsedDataType>();
			foreach (Expression expression in expressions) {
				ParsedDataType dataType = GetParsedDataType(connection, lstSysObjects, lstFoundTableSources, expression);
				if (null != dataType) {
					parsedDataTypes.Add(dataType);
				}
			}
			_parsedDataType = ParsedDataType.GetPrecedence(parsedDataTypes);
			return true;
		}
	}
}