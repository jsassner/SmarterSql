// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.Parsing.Predicates;
using Sassner.SmarterSql.Parsing.SelectItems;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Parsing.Keywords {
	public class KeywordDerivedTable {
		#region Derived tables parsing functions

		/// <summary>
		/// Parse a derived table
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="currentStartSpan"></param>
		/// <param name="lstTokens"></param>
		/// <param name="i"></param>
		/// <param name="lstSysObjects"></param>
		/// <param name="startIndex"></param>
		/// <param name="sysObjectId"></param>
		/// <param name="addedSysObject"></param>
		/// <param name="addedTableSource"></param>
		/// <param name="startToken"></param>
		public static bool ParseDerivedTable(Parser parser, StatementSpans currentStartSpan, List<TokenInfo> lstTokens, ref int i,
				List<SysObject> lstSysObjects, int startIndex, ref int sysObjectId, out SysObject addedSysObject, out TableSource addedTableSource,
				TokenInfo startToken) {
			TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
			if (null == nextToken || nextToken.Kind != TokenKind.LeftParenthesis) {
				addedSysObject = null;
				addedTableSource = null;
				return false;
			}

			int offset = nextToken.MatchingParenToken;
			nextToken = InStatement.GetNextNonCommentToken(lstTokens, offset);
			if (null != nextToken && nextToken.Kind == TokenKind.RightParenthesis && offset + 1 < lstTokens.Count) {
				int startSubSelect = InStatement.GetNextNonCommentToken(lstTokens, i + 1, i + 1);
				int endSubSelect = offset - 1;
				if (startSubSelect > endSubSelect) {
					addedSysObject = null;
					addedTableSource = null;
					return false;
				}

				// Parse sub select
				parser.ParseTokenRange(currentStartSpan, startSubSelect, endSubSelect, lstSysObjects);

				StatementSpans ss = parser.SegmentUtils.GetStatementSpan(startSubSelect);
				List<SysObjectColumn> lstSysObjectColumn = new List<SysObjectColumn>();
				if (null != ss && null != ss.SelectItems) {
					// Set the new sysobject for the columns
					foreach (SelectItem selectItem in ss.SelectItems) {
						foreach (SysObjectColumn sysObjectColumn in selectItem.SysObjectColumns) {
							if (null != sysObjectColumn) {
								lstSysObjectColumn.Add(sysObjectColumn);
							}
						}
					}
				}

				int statementStartIndex = i;
				int statementEndIndex = offset;
				int tableStartIndex = startSubSelect;
				int realEndIndex;
				if (GetAliasAndSearchCondition(parser, statementStartIndex, tableStartIndex, statementEndIndex, out realEndIndex, lstTokens, lstSysObjects,
						startSubSelect, currentStartSpan, startToken, Common.enSqlTypes.DerivedTable, ref sysObjectId, out addedSysObject,
						out addedTableSource, startIndex, lstSysObjectColumn)) {
					i = realEndIndex;
					return true;
				}
			}
			addedSysObject = null;
			addedTableSource = null;
			return false;
		}

		public static bool GetAliasAndSearchCondition(Parser parser, int startStatementIndex, int startTableIndex, int endStatementIndex, out int realEndIndex,
					List<TokenInfo> lstTokens, List<SysObject> lstSysObjects, int statementStartIndex, StatementSpans currentStartSpan,
					TokenInfo startToken, Common.enSqlTypes sqlType, ref int sysObjectId, out SysObject addedSysObject,
					out TableSource addedTableSource, int startIndex, List<SysObjectColumn> lstSysObjectColumn) {
			TokenInfo nextToken;
			string Table = string.Empty;
			string Alias;
			int endTableIndex = endStatementIndex + 1;
			if (parser.ExtractTableAlias(endTableIndex, ref endTableIndex, out Alias)) {
				Table = Alias;
			}

			realEndIndex = endTableIndex;
			List<string> lstNewColumNames = null;
			if (sqlType == Common.enSqlTypes.DerivedTable) {
				lstNewColumNames = GetDerivedTableNewColumnNames(lstTokens, ref realEndIndex);
			}
			int startSearchCondition = realEndIndex;
			List<Predicate> predicates;

			if (startToken.Kind == TokenKind.KeywordJoin) {
				realEndIndex = KeywordSearchCondition.FindLengthOfSearchCondition(parser, lstTokens, startSearchCondition);
				while (startSearchCondition < realEndIndex) {
					nextToken = InStatement.GetNextNonCommentToken(lstTokens, startSearchCondition);
					if (null == nextToken) {
						addedSysObject = null;
						addedTableSource = null;
						realEndIndex = 0;
						return false;
					}
					if (nextToken.Kind == TokenKind.KeywordOn) {
						startSearchCondition++;
						break;
					}
					startSearchCondition++;
				}
			}

			// Create a SysObject object
			addedSysObject = SysObject.CreateTemporarySysObject(Table, sqlType, ref sysObjectId);
			lstSysObjects.Add(addedSysObject);
			foreach (SysObjectColumn sysObjectColumn in lstSysObjectColumn) {
				sysObjectColumn.ParentSysObject = addedSysObject;
				addedSysObject.AddColumn(sysObjectColumn);
			}
			// If we have matching new column names, set them
			if (null != lstNewColumNames && lstNewColumNames.Count == addedSysObject.Columns.Count) {
				for (int j = 0; j < addedSysObject.Columns.Count; j++) {
					addedSysObject.Columns[j].ColumnName = lstNewColumNames[j];
				}
			}

			// Create the table source
			StatementSpans ss = parser.SegmentUtils.GetStatementSpan(startTableIndex);
			Table table = new Table("", "", "", Table, Alias, addedSysObject, ss, startTableIndex, endTableIndex, true);
			addedTableSource = new TableSource(lstTokens[startTableIndex].Token, table, startIndex, realEndIndex);
			parser.TableSources.Add(addedTableSource);
			if (null != currentStartSpan) {
				currentStartSpan.AddTableSource(addedTableSource);
			}

			// Find the search conditions and parse them
			KeywordSearchCondition.ParseSearchCondition(parser, lstTokens, startSearchCondition, realEndIndex, lstSysObjects, out predicates);
			return true;
		}

		/// <summary>
		/// Get new column aliases for a derived table
		/// </summary>
		/// <param name="lstTokens"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		private static List<string> GetDerivedTableNewColumnNames(List<TokenInfo> lstTokens, ref int i) {
			TokenInfo token;
			if (!InStatement.GetIfAllNextValidToken(lstTokens, ref i, out token, TokenKind.LeftParenthesis)) {
				return null;
			}
			if (-1 == token.MatchingParenToken || i > token.MatchingParenToken) {
				return null;
			}

			int matchingParenIndex = token.MatchingParenToken;
			List<string> lstNewColumNames = new List<string>();
			i++;
			while (i < matchingParenIndex) {
				token = InStatement.GetNextNonCommentToken(lstTokens, ref i);
				if (null != token) {
					if (token.Type == TokenType.Identifier) {
						lstNewColumNames.Add(token.Token.UnqoutedImage);
						token.TokenContextType = TokenContextType.NewColumnAlias;
					}
				} else {
					break;
				}
				i++;
			}
			return lstNewColumNames;
		}

		#endregion
	}
}