// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Parsing.Expressions {
	public class SubExpressions : Expression {
		#region Member variables

		private const string ClassName = "SubExpressions";

		private readonly int endIndex;
		private readonly List<Expression> expressions;
		private readonly int startIndex;

		#endregion

		public SubExpressions(int startIndex, int endIndex, List<Expression> expressions)
			: base(startIndex, null) {
			this.startIndex = startIndex;
			this.endIndex = endIndex;
			this.expressions = expressions;
		}

		#region Public properties

		public int StartIndex {
			get { return startIndex; }
		}

		public int EndIndex {
			get { return endIndex; }
		}

		public List<Expression> Expressions {
			get { return expressions; }
		}

		#endregion

		protected override bool GetParsedDataType(Connection connection, List<SysObject> lstSysObjects, List<TableSource> lstFoundTableSources, out ParsedDataType _parsedDataType) {
			try {
				List<ParsedDataType> parsedDataTypes = new List<ParsedDataType>();
				foreach (Expression expression in expressions) {
					ParsedDataType dataType = GetParsedDataType(connection, lstSysObjects, lstFoundTableSources, expression);
					if (null != dataType) {
						parsedDataTypes.Add(dataType);
					}
				}
				_parsedDataType = ParsedDataType.GetPrecedence(parsedDataTypes);
				return true;
			} catch (Exception e) {
				Common.LogEntry(ClassName, "GetParsedDataType", e, Common.enErrorLvl.Error);
				_parsedDataType = null;
				return false;
			}
		}
	}
}