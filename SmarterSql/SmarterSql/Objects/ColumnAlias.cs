// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sassner.SmarterSql.Parsing;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Objects {
	public class ColumnAlias : ScannedItem {
		#region Member variables

		private readonly StatementSpans statementSpan;

		#endregion

		public ColumnAlias(string columnAlias, int tokenIndex, StatementSpans statementSpan) : base(columnAlias, tokenIndex) {
			this.statementSpan = statementSpan;
		}

		#region Public properties

		public StatementSpans StatementSpan {
			[DebuggerStepThrough]
			get { return statementSpan; }
		}

		public override string GetToolTip {
			[DebuggerStepThrough]
			get { return "Column alias " + name; }
		}

		#endregion

		/// <summary>
		/// If a column alias exists in the supplied StatementSpan, return it
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="span"></param>
		/// <param name="columnName"></param>
		/// <param name="columnAlias"></param>
		/// <returns></returns>
		public static bool IsColumnAliasInSegment(Parser parser, StatementSpans span, string columnName, out ColumnAlias columnAlias) {
			if (null == span) {
				columnAlias = null;
				return false;
			}

			foreach (ColumnAlias alias in parser.DeclaredColumnAliases) {
				if (alias.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase)) {
					if (alias.StatementSpan == span) {
						columnAlias = alias;
						return true;
					}
					StatementSpans prevSpan = span.JoinedSpanPrev;
					while (null != prevSpan) {
						if (prevSpan == alias.StatementSpan) {
							columnAlias = alias;
							return true;
						}
						prevSpan = prevSpan.JoinedSpanPrev;
					}
					StatementSpans nextSpan = span.JoinedSpanNext;
					while (null != nextSpan) {
						if (nextSpan == alias.StatementSpan) {
							columnAlias = alias;
							return true;
						}
						nextSpan = nextSpan.JoinedSpanNext;
					}
				}
			}
			columnAlias = null;
			return false;
		}

		/// <summary>
		/// Add all column aliases in the supplied statemenspan to the list
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="span"></param>
		/// <param name="items"></param>
		public static void AddColumnAliasesInSegment(Parser parser, StatementSpans span, List<IntellisenseData> items) {
			foreach (ColumnAlias columnAlias in parser.DeclaredColumnAliases) {
				if (columnAlias.StatementSpan == span) {
					items.Add(columnAlias);
				} else {
					StatementSpans prevSpan = span.JoinedSpanPrev;
					while (null != prevSpan) {
						if (prevSpan == columnAlias.StatementSpan) {
							items.Add(columnAlias);
							break;
						}
						prevSpan = prevSpan.JoinedSpanPrev;
					}
					StatementSpans nextSpan = span.JoinedSpanNext;
					while (null != nextSpan) {
						if (nextSpan == columnAlias.StatementSpan) {
							items.Add(columnAlias);
							break;
						}
						nextSpan = nextSpan.JoinedSpanNext;
					}
				}
			}
		}
	}
}