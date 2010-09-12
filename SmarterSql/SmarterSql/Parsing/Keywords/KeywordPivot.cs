// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.Parsing.Expressions;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Parsing.Keywords {
	public class KeywordPivot {
		public static bool ParsePivot(Parser parser, ref int offset, ref int sysObjectId, List<TokenInfo> lstTokens, SysObject addedSysObject, List<SysObject> lstSysObjects, int startIndex, int i, StatementSpans currentStartSpan) {
			//<pivoted_table> ::=
			//        table_source PIVOT <pivot_clause> [AS] table_alias
			//
			//<pivot_clause> ::=
			//    (   aggregate_function ( value_column ) 
			//        FOR pivot_column 
			//        IN ( <column_list> ) 
			//    ) 
			TokenInfo nextToken;
			if (!InStatement.GetIfAllNextValidToken(lstTokens, ref offset, out nextToken, TokenKind.LeftParenthesis)) {
				return false;
			}
			if (-1 == nextToken.MatchingParenToken) {
				return false;
			}

			const string tablename = "Pivot";
			SysObject sysObject = SysObject.CreateTemporarySysObject(tablename, Common.enSqlTypes.DerivedTable, ref sysObjectId);
			// Copy the columns from the previous sysobject
			foreach (SysObjectColumn column in addedSysObject.Columns) {
				sysObject.AddColumn(column);
			}
			lstSysObjects.Add(sysObject);

			int endOfPivot = nextToken.MatchingParenToken;
			offset++;
			// aggregate_function ( value_column ) 
			Expression aggregateExpression;
			if (!Expression.FindExpression(parser, lstTokens, ref offset, endOfPivot, out aggregateExpression) || null == aggregateExpression) {
				return false;
			}
			ScalarFunctionExpression scalarFunctionExpression = aggregateExpression as ScalarFunctionExpression;
			if (null == scalarFunctionExpression || scalarFunctionExpression.Expressions.Count != 1 || !(scalarFunctionExpression.Expressions[0] is ColumnExpression)) {
				return false;
			}
			ColumnExpression columnToRemoveExpression = scalarFunctionExpression.Expressions[0] as ColumnExpression;
			if (null == columnToRemoveExpression) {
				return false;
			}
			// Remove the "value_column" from the previous tablesource
			foreach (SysObjectColumn column in sysObject.Columns) {
				if (column.ColumnName.Equals(columnToRemoveExpression.Column.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase)) {
					sysObject.Columns.Remove(column);
					break;
				}
			}

			if (!InStatement.GetIfAllNextValidToken(lstTokens, ref offset, out nextToken, TokenKind.KeywordFor)) {
				return false;
			}
			offset++;
			// pivot_column
			Expression pivotColumnExpression;
			if (!Expression.FindExpression(parser, lstTokens, ref offset, endOfPivot, out pivotColumnExpression) || null == pivotColumnExpression) {
				return false;
			}
			ColumnExpression pivotColumnToRemoveExpression = pivotColumnExpression as ColumnExpression;
			if (null == pivotColumnToRemoveExpression) {
				return false;
			}
			// Remove the "pivot_column" from the previous tablesource
			foreach (SysObjectColumn column in sysObject.Columns) {
				if (column.ColumnName.Equals(pivotColumnToRemoveExpression.Column.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase)) {
					sysObject.Columns.Remove(column);
					break;
				}
			}

			if (!InStatement.GetIfAllNextValidToken(lstTokens, ref offset, out nextToken, TokenKind.KeywordIn, TokenKind.LeftParenthesis)) {
				return false;
			}
			int endOfColumnList = nextToken.MatchingParenToken;
			if (-1 == endOfColumnList) {
				return false;
			}
			// <column_list>
			while (offset < endOfColumnList) {
				offset++;
				nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
				if (null == nextToken) {
					return false;
				}
				sysObject.AddColumn(new SysObjectColumn(sysObject, nextToken.Token.Image, pivotColumnExpression.ParsedDataType, false, false));
				nextToken.TokenContextType = TokenContextType.PivotValue;

				offset++;
				nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
				if (null == nextToken || nextToken.Kind != TokenKind.Comma) {
					break;
				}
			}
			offset = endOfColumnList - 1;
			if (!InStatement.GetIfAllNextValidToken(lstTokens, ref offset, out nextToken, TokenKind.RightParenthesis, TokenKind.RightParenthesis)) {
				return false;
			}
			offset++;
			string alias;
			if (!parser.ExtractTableAlias(offset, ref offset, out alias)) {
				return false;
			}

			StatementSpans ss = parser.SegmentUtils.GetStatementSpan(offset);
			Table table = new Table("", "", "", tablename, alias, sysObject, ss, i + 1, offset, false);
			TableSource tableSource = new TableSource(lstTokens[i + 1].Token, table, startIndex, offset);
			parser.TableSources.Add(tableSource);
			if (null != currentStartSpan) {
				currentStartSpan.AddTableSource(tableSource);
			}

			return true;
		}

		public static bool ParseUnpivot(Parser parser, ref int offset, ref int sysObjectId, List<TokenInfo> lstTokens, SysObject addedSysObject, List<SysObject> lstSysObjects, int startIndex, int i, StatementSpans currentStartSpan) {
			//<unpivoted_table> ::=
			//        table_source UNPIVOT <unpivot_clause> table_alias
			//
			//<unpivot_clause> ::=
			//        ( value_column FOR pivot_column IN ( <column_list> ) ) 
			TokenInfo nextToken;
			if (!InStatement.GetIfAllNextValidToken(lstTokens, ref offset, out nextToken, TokenKind.LeftParenthesis)) {
				return false;
			}
			if (-1 == nextToken.MatchingParenToken) {
				return false;
			}

			const string tablename = "Unpivot";
			SysObject sysObject = SysObject.CreateTemporarySysObject(tablename, Common.enSqlTypes.DerivedTable, ref sysObjectId);
			// Copy the columns from the previous sysobject
			foreach (SysObjectColumn column in addedSysObject.Columns) {
				sysObject.AddColumn(column);
			}
			lstSysObjects.Add(sysObject);

			int endOfPivot = nextToken.MatchingParenToken;
			offset++;
			// aggregate_function ( value_column ) 
			Expression aggregateExpression;
			if (!Expression.FindExpression(parser, lstTokens, ref offset, endOfPivot, out aggregateExpression) || null == aggregateExpression) {
				return false;
			}
			lstTokens[offset].TokenContextType = TokenContextType.PivotValue;
			ColumnExpression columnToAddExpression = aggregateExpression as ColumnExpression;
			if (null == columnToAddExpression) {
				return false;
			}
			// Add the "value_column" from the previous tablesource
			sysObject.Columns.Add(new SysObjectColumn(sysObject, columnToAddExpression.Column.Token.UnqoutedImage, columnToAddExpression.ParsedDataType, false, false));

			if (!InStatement.GetIfAllNextValidToken(lstTokens, ref offset, out nextToken, TokenKind.KeywordFor)) {
				return false;
			}
			offset++;
			// pivot_column
			Expression pivotColumnExpression;
			if (!Expression.FindExpression(parser, lstTokens, ref offset, endOfPivot, out pivotColumnExpression) || null == pivotColumnExpression) {
				return false;
			}
			lstTokens[offset].TokenContextType = TokenContextType.PivotValue;
			ColumnExpression pivotColumnToAddExpression = pivotColumnExpression as ColumnExpression;
			if (null == pivotColumnToAddExpression) {
				return false;
			}
			// Add the "pivot_column" from the previous tablesource
			sysObject.Columns.Add(new SysObjectColumn(sysObject, pivotColumnToAddExpression.Column.Token.UnqoutedImage, pivotColumnToAddExpression.ParsedDataType, false, false));

			if (!InStatement.GetIfAllNextValidToken(lstTokens, ref offset, out nextToken, TokenKind.KeywordIn, TokenKind.LeftParenthesis)) {
				return false;
			}
			int endOfColumnList = nextToken.MatchingParenToken;
			if (-1 == endOfColumnList) {
				return false;
			}
			// <column_list>
			while (offset < endOfColumnList) {
				offset++;
				Expression columnExpression;
				if (!Expression.FindExpression(parser, lstTokens, ref offset, endOfColumnList, out columnExpression) || null == columnExpression) {
					return false;
				}
				ColumnExpression columnDataExpression = columnExpression as ColumnExpression;
				if (null == columnDataExpression) {
					return false;
				}
				foreach (SysObjectColumn column in sysObject.Columns) {
					if (column.ColumnName.Equals(columnDataExpression.Column.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase)) {
						sysObject.Columns.Remove(column);
						break;
					}
				}

				offset++;
				nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
				if (null == nextToken || nextToken.Kind != TokenKind.Comma) {
					break;
				}
			}
			offset = endOfColumnList - 1;
			if (!InStatement.GetIfAllNextValidToken(lstTokens, ref offset, out nextToken, TokenKind.RightParenthesis, TokenKind.RightParenthesis)) {
				return false;
			}
			offset++;
			string alias;
			if (!parser.ExtractTableAlias(offset, ref offset, out alias)) {
				return false;
			}

			StatementSpans ss = parser.SegmentUtils.GetStatementSpan(offset);

			Table table = new Table("", "", "", tablename, alias, sysObject, ss, i + 1, offset, false);
			TableSource tableSource = new TableSource(lstTokens[i + 1].Token, table, startIndex, offset);
			parser.TableSources.Add(tableSource);
			if (null != currentStartSpan) {
				currentStartSpan.AddTableSource(tableSource);
			}
			return true;
		}
	}
}