// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using System.Diagnostics;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.Parsing.Predicates;

namespace Sassner.SmarterSql.Parsing.Expressions {
	public class CaseEndExpression : Expression {
		#region Member variables

		private readonly int endIndex;
		private readonly int startIndex;
		private readonly List<Expression> whenExpressions;
		private readonly List<Predicate> whenPredicates;
		private readonly List<Expression> thenExpressions;
		private Expression elseExpression;
		private Expression inputExpression;

		#endregion

		public CaseEndExpression(int startIndex, int endIndex) : base(startIndex, null) {
			this.startIndex = startIndex;
			this.endIndex = endIndex;
			thenExpressions = new List<Expression>(50);
			whenExpressions = new List<Expression>(50);
			whenPredicates = new List<Predicate>(50);
		}

		#region Public properties

		public Expression InputExpression {
			[DebuggerStepThrough]
			get { return inputExpression; }
			set { inputExpression = value; }
		}

		public int StartIndex {
			[DebuggerStepThrough]
			get { return startIndex; }
		}

		public int EndIndex {
			[DebuggerStepThrough]
			get { return endIndex; }
		}

		public List<Expression> ThenExpressions {
			[DebuggerStepThrough]
			get { return thenExpressions; }
		}

		public Expression ElseExpression {
			[DebuggerStepThrough]
			get { return elseExpression; }
			set { elseExpression = value; }
		}

		public List<Expression> WhenExpressions {
			[DebuggerStepThrough]
			get { return whenExpressions; }
		}

		public List<Predicate> WhenPredicates {
			[DebuggerStepThrough]
			get { return whenPredicates; }
		}

		#endregion

		protected override bool GetParsedDataType(Connection connection, List<SysObject> lstSysObjects, List<TableSource> lstFoundTableSources, out ParsedDataType _parsedDataType) {
			List<ParsedDataType> parsedDataTypes = new List<ParsedDataType>();
			ParsedDataType dataType;

			// Get data type of input expression
			if (null != inputExpression) {
				GetParsedDataType(connection, lstSysObjects, lstFoundTableSources, inputExpression);
			}

			// Get data type of when expressions
			foreach (Expression expression in whenExpressions) {
				dataType = GetParsedDataType(connection, lstSysObjects, lstFoundTableSources, expression);
				if (null != dataType) {
					parsedDataTypes.Add(dataType);
				}
			}

			foreach (Predicate predicate in whenPredicates) {
				foreach (Expression expression in predicate.Expressions) {
					dataType = GetParsedDataType(connection, lstSysObjects, lstFoundTableSources, expression);
					if (null != dataType) {
						parsedDataTypes.Add(dataType);
					}
				}
			}

			// Get data type of else expression
			if (null != elseExpression) {
				GetParsedDataType(connection, lstSysObjects, lstFoundTableSources, elseExpression);
			}

			// Get data type of then expressions
			foreach (Expression expression in thenExpressions) {
				dataType = GetParsedDataType(connection, lstSysObjects, lstFoundTableSources, expression);
				if (null != dataType) {
					parsedDataTypes.Add(dataType);
				}
			}

			_parsedDataType = ParsedDataType.GetPrecedence(parsedDataTypes);
			return true;
		}
	}
}