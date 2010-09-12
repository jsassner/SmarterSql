// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.Parsing.Expressions;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Parsing.Keywords {
	public class KeywordUpdate {
		#region Parse UPDATE

		/// <summary>
		/// UPDATE 
		///    [ TOP ( expression ) [ PERCENT ] ] 
		///    { <object> | rowset_function_limited 
		///     [ WITH ( <Table_Hint_Limited> [ ...n ] ) ]
		///    }
		///    SET 
		///        { column_name = { expression | DEFAULT | NULL }
		///          | { udt_column_name.{ { property_name = expression 
		///                                | field_name = expression } 
		///                               | method_name ( argument [ ,...n ] ) 
		///                              } 
		///            }
		///          | column_name { .WRITE ( expression , @Offset , @Length ) }
		///          | @variable = expression 
		///          | @variable = column = expression [ ,...n ] 
		///        } [ ,...n ] 
		///    [ <OUTPUT Clause> ]
		///    [FROM] ....
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="currentStartSpan"></param>
		/// <param name="lstSysObjects"></param>
		/// <param name="i"></param>
		/// <param name="lstTokens"></param>
		public static void ParseUpdateStatement(Parser parser, out StatementSpans currentStartSpan, List<SysObject> lstSysObjects, ref int i, List<TokenInfo> lstTokens) {
			currentStartSpan = parser.SegmentUtils.GetStatementSpan(i);
			if (null == currentStartSpan) {
				return;
			}
			int startIndex = i;

			TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
			if (null != nextToken && nextToken.Kind == TokenKind.KeywordUpdate) {
				i++;
				nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);

				// Handle: [ TOP ( expression ) [ PERCENT ] ]
				if (null != nextToken && nextToken.Kind == TokenKind.KeywordTop) {
					i++;
					nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
					if (null != nextToken && nextToken.Kind == TokenKind.LeftParenthesis && -1 != nextToken.MatchingParenToken && i < nextToken.MatchingParenToken) {
						i = InStatement.GetNextNonCommentToken(lstTokens, nextToken.MatchingParenToken + 1, nextToken.MatchingParenToken + 1);
						nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
						if (null != nextToken && nextToken.Kind == TokenKind.KeywordPercent) {
							i = InStatement.GetNextNonCommentToken(lstTokens, i + 1, i + 1);
						}
						nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
					}
				}

				string servername = "";
				string databasename = "";
				string schemaname = "";
				string objectname = "";
				TokenInfo tokenStart = null;
				int endTableIndex = 0;
				int startTableIndex = 0;
				int endIndex;

				// Handle: { <object> [ WITH ( <Table_Hint_Limited> [ ...n ] ) ] }
				if (null != nextToken) {
					if (nextToken.Kind == TokenKind.Name || nextToken.Kind == TokenKind.TemporaryObject || nextToken.Kind == TokenKind.Variable) {
						TokenInfo server_name;
						TokenInfo database_name;
						TokenInfo schema_name;
						TokenInfo object_name;
						startTableIndex = i;
						if (parser.ParseTableOrViewName(startTableIndex, out endIndex, out server_name, out database_name, out schema_name, out object_name)) {
							servername = (null != server_name ? server_name.Token.UnqoutedImage : "");
							databasename = (null != database_name ? database_name.Token.UnqoutedImage : "");
							schemaname = (null != schema_name ? schema_name.Token.UnqoutedImage : "");
							objectname = (null != object_name ? object_name.Token.UnqoutedImage : "");
							tokenStart = ((server_name ?? database_name) ?? schema_name) ?? object_name;
							if (null == tokenStart) {
								return;
							}

							if (nextToken.Kind == TokenKind.Variable) {
								parser.CalledLocalVariables.Add(new LocalVariable(nextToken.Token.UnqoutedImage, startTableIndex));
							}

							endTableIndex = endIndex;
							i = endIndex;

							// Handle: [ WITH ( <Table_Hint_Limited> [ ...n ] ) ]
							int offset = i;
							nextToken = InStatement.GetNextNonCommentToken(lstTokens, offset + 1);
							if (null != nextToken && nextToken.Kind == TokenKind.KeywordWith) {
								offset++;
								nextToken = InStatement.GetNextNonCommentToken(lstTokens, offset + 1);
								if (null != nextToken && nextToken.Kind == TokenKind.LeftParenthesis && -1 != nextToken.MatchingParenToken && i < nextToken.MatchingParenToken) {
									i = InStatement.GetNextNonCommentToken(lstTokens, nextToken.MatchingParenToken, nextToken.MatchingParenToken);
									// TODO: Verify that it's only members from <Table_Hint_Limited> here
								}
							}
						}
					} else {
						// TODO: Handle: { rowset_function_limited [ WITH ( <Table_Hint_Limited> [ ...n ] ) ] }
					}
				}

				// SET 
				//* { column_name = { expression | DEFAULT | NULL }
				//  | { udt_column_name.{ { property_name = expression
				//                        | field_name = expression
				//                        } 
				//                      | method_name ( argument [ ,...n ] ) 
				//                      } 
				//    }
				//  | column_name { .WRITE ( expression , @Offset , @Length ) }
				//* | @variable = expression 
				//  | @variable = column = expression [ ,...n ] 
				//  } [ ,...n ]
				i++;
				List<Expression> setExpressions = new List<Expression>();
				nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
				if (null != tokenStart && null != nextToken && nextToken.Kind == TokenKind.KeywordSet) {
					endIndex = currentStartSpan.EndIndex;

					while (i < endIndex) {
						i++;
						nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
						if (null == nextToken) {
							break;
						}
						if (nextToken.Kind == TokenKind.Name || nextToken.Kind == TokenKind.TemporaryObject) {
							// column_name = { expression | DEFAULT | NULL }
							Expression expression;
							if (!Expression.FindExpression(parser, lstTokens, ref i, endIndex, out expression)) {
								break;
							}
							setExpressions.Add(expression);
						} else if (nextToken.Kind == TokenKind.Variable) {
							// @variable = expression
							nextToken.TokenContextType = TokenContextType.Variable;
							Expression expression;
							if (!Expression.FindExpression(parser, lstTokens, ref i, endIndex, out expression)) {
								break;
							}
							setExpressions.Add(expression);
						}

						i++;
						nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
						if (null == nextToken || nextToken.Kind != TokenKind.Comma) {
							break;
						}
					}

					if (i < endIndex) {
						// Parse rest of the UPDATE statement
						nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
						if (null != nextToken) {
							parser.ParseTokenRange(currentStartSpan, i, endIndex, lstSysObjects);
						}
					}

					SysObject addedSysObject;
					TableSource addedTableSource;
					parser.CreateTableEntry(currentStartSpan, Tokens.kwUpdateToken, lstSysObjects, servername, databasename, schemaname, objectname, "", startIndex, endIndex, startTableIndex, endTableIndex, false, out addedSysObject, out addedTableSource);

					List<TableSource> foundTableSources;
					if (parser.SegmentUtils.GetUniqueStatementSpanTablesources(currentStartSpan, out foundTableSources, false)) {
						Connection connection = Instance.TextEditor.ActiveConnection;
						foreach (Expression expression in setExpressions) {
							Expression.GetParsedDataType(connection, lstSysObjects, foundTableSources, expression);
						}
					}

					i = endIndex;
				}
			}
		}

		#endregion

		#region Parse CURRENT OF

		/// <summary>
		/// { [ CURRENT OF { { [ GLOBAL ] cursor_name } | cursor_variable_name } ] }
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="lstTokens"></param>
		/// <param name="startIndex"></param>
		/// <param name="endIndex"></param>
		public static bool ParseCurrentOf(Parser parser, List<TokenInfo> lstTokens, int startIndex, int endIndex) {
			TokenInfo token = InStatement.GetNextNonCommentToken(lstTokens, startIndex);
			if (null == token || token.Kind != TokenKind.KeywordCurrent) {
				return false;
			}
			if (!InStatement.GetIfAllNextValidToken(lstTokens, ref startIndex, out token, TokenKind.KeywordOf)) {
				return false;
			}
			InStatement.GetIfAllNextValidToken(lstTokens, ref startIndex, out token, TokenKind.KeywordGlobal);
			startIndex++;
			token = InStatement.GetNextNonCommentToken(lstTokens, startIndex);
			if (null != token && (token.Kind == TokenKind.Name || token.Kind == TokenKind.Variable)) {
				token.TokenContextType = TokenContextType.Cursor;
				parser.CalledCursors.Add(new Cursor(token.Token.UnqoutedImage, startIndex));
				return true;
			}

			return false;
		}

		#endregion
	}
}