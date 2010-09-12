// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Parsing.Keywords {
	public class KeywordFreeTextTable {
		/// <summary>
		/// Parse OPENQUERY statement
		///   FREETEXTTABLE (table , { column_name | (column_list) | * } 
		///            , 'freetext_string' 
		///          [ , LANGUAGE language_term ] 
		///          [ , top_n_by_rank ]
		///   )
		/// </summary>
		/// <returns></returns>
		public static bool ParseFreeTextTable(Parser parser, StatementSpans currentStartSpan, List<TokenInfo> lstTokens, ref int i, List<SysObject> lstSysObjects, int startStatementIndex, ref int sysObjectId, out SysObject addedSysObject, out TableSource addedTableSource, TokenInfo startToken) {
			List<SysObjectColumn> lstSysObjectColumn = new List<SysObjectColumn>();
			int offset = i;
			TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, offset);
			if (null != nextToken && nextToken.Kind == TokenKind.KeywordFreeTextTable) {
				int startRowsetIndex = offset;
				offset++;
				nextToken = InStatement.GetNextNonCommentToken(lstTokens, offset);
				if (null != nextToken && nextToken.Kind == TokenKind.LeftParenthesis && -1 != nextToken.MatchingParenToken && offset < nextToken.MatchingParenToken) {
					int endStatementIndex = nextToken.MatchingParenToken;
					offset++;

					// [ catalog. ] [ schema. ] object 
					TokenInfo server_name;
					TokenInfo catalog_name;
					TokenInfo schema_name;
					TokenInfo object_name;
					int endIndex;
					SysObject foundSysObject = null;
					if (parser.ParseTableOrViewName(offset, out endIndex, out server_name, out catalog_name, out schema_name, out object_name)) {
						offset = endIndex;
						foreach (SysObject sysObject in lstSysObjects) {
							if (sysObject.ObjectName.Equals(object_name.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase) && (null == schema_name || sysObject.Schema.Schema.Equals(schema_name.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase))) {
								foundSysObject = sysObject;
								// Include all columns
								foreach (SysObjectColumn sysObjectColumn in foundSysObject.Columns) {
									lstSysObjectColumn.Add(sysObjectColumn);
								}
								// Add KEY and RANK
								lstSysObjectColumn.Add(new SysObjectColumn(sysObject, "KEY", new ParsedDataType(Tokens.kwIntToken, "4"), false, false));
								lstSysObjectColumn.Add(new SysObjectColumn(sysObject, "RANK", new ParsedDataType(Tokens.kwIntToken, "4"), false, false));
								break;
							}
						}
						if (null == foundSysObject) {
							addedSysObject = null;
							addedTableSource = null;
							return false;
						}
					} else {
						addedSysObject = null;
						addedTableSource = null;
						return false;
					}

					if (!InStatement.GetIfAllNextValidToken(lstTokens, ref offset, out nextToken, TokenKind.Comma)) {
						addedSysObject = null;
						addedTableSource = null;
						return false;
					}

					// { column_name | (column_list) | * }
					offset++;
					nextToken = InStatement.GetNextNonCommentToken(lstTokens, offset);

					if (null != nextToken) {
						if (nextToken.Kind == TokenKind.Multiply) {
							// All ok

						} else if (nextToken.Kind == TokenKind.LeftParenthesis && -1 != nextToken.MatchingParenToken && offset < nextToken.MatchingParenToken) {
							// Include supplied columns
							int endColumnListIndex = nextToken.MatchingParenToken;
							offset++;
							while (offset < endColumnListIndex) {
								nextToken = InStatement.GetNextNonCommentToken(lstTokens, offset);
								if (null == nextToken) {
									addedSysObject = null;
									addedTableSource = null;
									return false;
								}

								if (nextToken.Kind == TokenKind.Name) {
									bool foundColumn = false;
									foreach (SysObjectColumn sysObjectColumn in foundSysObject.Columns) {
										if (sysObjectColumn.ColumnName.Equals(nextToken.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase) || sysObjectColumn.ColumnName.Equals(nextToken.Token.Image, StringComparison.OrdinalIgnoreCase)) {
											nextToken.TokenContextType = TokenContextType.Known;
											foundColumn = true;
											break;
										}
									}
									if (!foundColumn) {
										addedSysObject = null;
										addedTableSource = null;
										return false;
									}
								} else if (nextToken.Kind != TokenKind.Comma) {
									break;
								}
								offset++;
							}
							offset = endColumnListIndex;
							
						} else if (nextToken.Kind == TokenKind.Name) {
							// One column
							foreach (SysObjectColumn sysObjectColumn in foundSysObject.Columns) {
								if (sysObjectColumn.ColumnName.Equals(nextToken.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase) || sysObjectColumn.ColumnName.Equals(nextToken.Token.Image, StringComparison.OrdinalIgnoreCase)) {
									nextToken.TokenContextType = TokenContextType.Known;
									break;
								}
							}
						} else {
							addedSysObject = null;
							addedTableSource = null;
							return false;
						}
					}

					if (!InStatement.GetIfAllNextValidToken(lstTokens, ref offset, out nextToken, TokenKind.Comma)) {
						addedSysObject = null;
						addedTableSource = null;
						return false;
					}

					// 'freetext_string'
					offset++;
					nextToken = InStatement.GetNextNonCommentToken(lstTokens, offset);
					if (null != nextToken && nextToken.Kind == TokenKind.ValueString) {
						// [ , LANGUAGE language_term ] 
						InStatement.GetIfAllNextValidToken(lstTokens, ref offset, out nextToken, TokenKind.Comma, TokenKind.KeywordLanguage, TokenKind.ValueString);
						// [ , top_n_by_rank ]
						InStatement.GetIfAllNextValidToken(lstTokens, ref offset, out nextToken, TokenKind.Comma, TokenKind.ValueNumber);
						i = endStatementIndex;

						int statementStartIndex = startStatementIndex;
						int statementEndIndex = i;
						int tableStartIndex = startRowsetIndex;
						int realEndIndex;
						if (KeywordDerivedTable.GetAliasAndSearchCondition(parser, statementStartIndex, tableStartIndex, statementEndIndex, out realEndIndex,
							lstTokens, lstSysObjects, startStatementIndex, currentStartSpan, startToken, Common.enSqlTypes.Rowset, ref sysObjectId,
							out addedSysObject, out addedTableSource, startStatementIndex, lstSysObjectColumn)) {
							i = realEndIndex;
						}

						return true;
					}
				}
			}

			i = offset;
			addedSysObject = null;
			addedTableSource = null;
			return false;
		}
	}
}
