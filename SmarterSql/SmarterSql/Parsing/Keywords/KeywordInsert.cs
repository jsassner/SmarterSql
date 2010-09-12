// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Parsing.Keywords {
	public class KeywordInsert {
		#region Parse INSERT

		/// <summary>
		/// Parse an INSERT statement
		/// INSERT 
		///		[ TOP ( expression ) [ PERCENT ] ] 
		///		[ INTO ] 
		///		{ <object> | rowset_function_limited [ WITH ( <Table_Hint_Limited> [ ...n ] ) ] }
		///		{
		///		[ ( column_list ) ] 
		///		[ <OUTPUT Clause> ]
		///			{ VALUES ( { DEFAULT | NULL | expression } [ ,...n ] ) 
		///			| derived_table 
		///			| execute_statement 
		///			} 
		///		}
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="currentStartSpan"></param>
		/// <param name="lstSysObjects"></param>
		/// <param name="i"></param>
		/// <param name="lstTokens"></param>
		public static void ParseInsertStatement(Parser parser, out StatementSpans currentStartSpan, List<SysObject> lstSysObjects, ref int i, List<TokenInfo> lstTokens) {
			currentStartSpan = parser.SegmentUtils.GetStatementSpan(i);
			int startIndex = i;

			TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
			if (null != nextToken && nextToken.Kind == TokenKind.KeywordInsert) {
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

				// Handle: [ INTO ]
				if (null != nextToken && nextToken.Kind == TokenKind.KeywordInto) {
					i = InStatement.GetNextNonCommentToken(lstTokens, i + 1, i + 1);
					nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
				}

				// Handle: { <object> [ WITH ( <Table_Hint_Limited> [ ...n ] ) ] }
				if (null != nextToken) {
					if (nextToken.Kind == TokenKind.Name || nextToken.Kind == TokenKind.Variable || nextToken.Kind == TokenKind.TemporaryObject) {
						TokenInfo server_name;
						TokenInfo database_name;
						TokenInfo schema_name;
						TokenInfo object_name;
						int startTableIndex = i;
						int endIndex;
						if (parser.ParseTableOrViewName(startTableIndex, out endIndex, out server_name, out database_name, out schema_name, out object_name)) {
							string servername = (null != server_name ? server_name.Token.UnqoutedImage : "");
							string databasename = (null != database_name ? database_name.Token.UnqoutedImage : "");
							string schemaname = (null != schema_name ? schema_name.Token.UnqoutedImage : "");
							string objectname = (null != object_name ? object_name.Token.UnqoutedImage : "");
							TokenInfo tokenStart = ((server_name ?? database_name) ?? schema_name) ?? object_name;

							if (null == tokenStart) {
								return;
							}

							int endTableIndex = endIndex;
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

							// Handle: [ ( column_list ) ]
							offset = i + 1;
							nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
							if (null != nextToken && nextToken.Kind == TokenKind.LeftParenthesis && -1 != nextToken.MatchingParenToken && offset < nextToken.MatchingParenToken) {
								i = InStatement.GetNextNonCommentToken(lstTokens, nextToken.MatchingParenToken, nextToken.MatchingParenToken);

								offset++;
								// Set token context
								while (offset < i) {
									nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
									if (null != nextToken && nextToken.Type == TokenType.Identifier) {
										nextToken.TokenContextType = TokenContextType.NewColumnAlias;
									}
									offset++;
								}
							}

							// TODO: Handle OUTPUT Clause

							// Handle: { VALUES ( { DEFAULT | NULL | expression } [ ,...n ] ) }
							offset = i;
							nextToken = InStatement.GetNextNonCommentToken(lstTokens, offset + 1);
							if (null != nextToken && nextToken.Kind == TokenKind.KeywordValues) {
								offset++;
								nextToken = InStatement.GetNextNonCommentToken(lstTokens, offset + 1);
								if (null != nextToken && nextToken.Kind == TokenKind.LeftParenthesis && -1 != nextToken.MatchingParenToken && i < nextToken.MatchingParenToken) {
									i = InStatement.GetNextNonCommentToken(lstTokens, nextToken.MatchingParenToken, nextToken.MatchingParenToken);

									offset++;
									// Set token context
									while (offset < i) {
										nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
										if (null != nextToken && nextToken.Type == TokenType.Identifier) {
											nextToken.TokenContextType = TokenContextType.Known;
										}
										offset++;
									}
								}
							}

							endIndex = i;
							SysObject addedSysObject;
							TableSource addedTableSource;
							parser.CreateTableEntry(currentStartSpan, Tokens.kwInsertToken, lstSysObjects, servername, databasename, schemaname, objectname, "", startIndex, endIndex, startTableIndex, endTableIndex, false, out addedSysObject, out addedTableSource);
						}
					} else {
						// TODO: Handle: { rowset_function_limited [ WITH ( <Table_Hint_Limited> [ ...n ] ) ] }
					}
				}
			}
		}

		#endregion
	}
}