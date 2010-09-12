// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using System.Diagnostics;
using Sassner.SmarterSql.Objects;

namespace Sassner.SmarterSql.Parsing.Expressions {
	public class ScalarFunctionExpression : Expression {
		#region Member variables

		private readonly int endIndex;
		private readonly List<Expression> overPartitionExpressions = new List<Expression>();
		private readonly int startIndex;
		private List<Expression> expressions = new List<Expression>();

		#endregion

		public ScalarFunctionExpression(int startIndex, int endIndex, ParsedDataType parsedDataType) : base(startIndex, parsedDataType) {
			this.startIndex = startIndex;
			this.endIndex = endIndex;
		}

		#region Public properties

		public List<Expression> OverPartitionExpressions {
			[DebuggerStepThrough]
			get { return overPartitionExpressions; }
		}

		public List<Expression> Expressions {
			[DebuggerStepThrough]
			get { return expressions; }
			set { expressions = value; }
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

		public void AddExpression(Expression expression) {
			expressions.Add(expression);
		}

		public void AddOverPartitionExpression(Expression expression) {
			overPartitionExpressions.Add(expression);
		}

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
	}
}