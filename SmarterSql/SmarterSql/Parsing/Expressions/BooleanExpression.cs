// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using System.Collections.Generic;
using Sassner.SmarterSql.Objects;

namespace Sassner.SmarterSql.Parsing.Expressions {
	public class BooleanExpression : Expression {
		#region Member variables

		private readonly List<Expression> expressions = new List<Expression>();
		private bool isExpressionAnd;
		private bool isExpressionNot;
		private bool isExpressionOr;

		#endregion

		public BooleanExpression(int tokenIndex) : base(tokenIndex, null) {
		}

		#region Public properties

		public bool IsExpressionNot {
			get { return isExpressionNot; }
			set { isExpressionNot = value; }
		}

		public bool IsExpressionAnd {
			get { return isExpressionAnd; }
			set { isExpressionAnd = value; }
		}

		public bool IsExpressionOr {
			get { return isExpressionOr; }
			set { isExpressionOr = value; }
		}

		#endregion

		protected override bool GetParsedDataType(Connection connection, List<SysObject> lstSysObjects, List<TableSource> lstFoundTableSources, out ParsedDataType _parsedDataType) {
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

		public void AddExpression(Expression expression) {
			expressions.Add(expression);
		}
	}
}
