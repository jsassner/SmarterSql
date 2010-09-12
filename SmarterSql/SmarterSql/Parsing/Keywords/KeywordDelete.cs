// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Parsing.Keywords {
	public class KeywordDelete {
		#region Parse DELETE

		/// <summary>
		/// Parse an DELETE statement
		/// DELETE 
		///		[ TOP ( expression ) [ PERCENT ] ] 
		///		[ FROM ] 
		///			{ table_name [ WITH ( <table_hint_limited> [ ...n ] ) ]
		///			| view_name 
		///			| rowset_function_limited 
		///			| table_valued_function
		///		}
		///		[ <OUTPUT Clause> ]
		///    [ FROM <table_source> [ ,...n ] ] 
		///    [ WHERE { <search_condition> 
		///            | { [ CURRENT OF 
		///                   { { [ GLOBAL ] cursor_name } 
		///                       | cursor_variable_name 
		///                   } 
		///                ]
		///              }
		///            } 
		///    ] 
		///    [ OPTION ( <Query Hint> [ ,...n ] ) ] 
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="currentStartSpan"></param>
		/// <param name="lstSysObjects"></param>
		/// <param name="i"></param>
		/// <param name="lstTokens"></param>
		public static void ParseDeleteStatement(Parser parser, out StatementSpans currentStartSpan, List<SysObject> lstSysObjects, ref int i, List<TokenInfo> lstTokens) {
			currentStartSpan = parser.SegmentUtils.GetStatementSpan(i);
			if (null == currentStartSpan) {
				return;
			}
			int startIndex = i;

			TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
			if (null != nextToken && nextToken.Kind == TokenKind.KeywordDelete) {
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

				// Handle: [ FROM ]
				if (null != nextToken && nextToken.Kind == TokenKind.KeywordFrom) {
					i = InStatement.GetNextNonCommentToken(lstTokens, i + 1, i + 1);
					nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
				}

				// Handle: { table_name [ WITH ( <table_hint_limited> [ ...n ] ) ] }
				if (null != nextToken) {
					if (nextToken.Kind == TokenKind.Name) {
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

							//  [ <OUTPUT Clause> ]
							// TODO: Handle OUTPUT Clause

							//  [ FROM <table_source> [ ,...n ] ] 
							if (-1 != currentStartSpan.FromIndex) {
								int startRangeIndex = currentStartSpan.FromIndex;
								if (i <= startRangeIndex) {
									int endRangeIndex = parser.SegmentUtils.GetNextMainTokenEnd(currentStartSpan, startRangeIndex + 1);
									parser.ParseTokenRange(currentStartSpan, startRangeIndex, endRangeIndex, Instance.TextEditor.SysObjects);
									i = endRangeIndex;
								}
							}

							//    [ WHERE { <search_condition> 
							if (-1 != currentStartSpan.WhereIndex) {
								int startRangeIndex = currentStartSpan.WhereIndex;
								int endRangeIndex = parser.SegmentUtils.GetNextMainTokenEnd(currentStartSpan, startRangeIndex + 1);
								parser.ParseTokenRange(currentStartSpan, startRangeIndex, endRangeIndex, Instance.TextEditor.SysObjects);
								i = endRangeIndex;
							}
							//    [ OPTION ( <Query Hint> [ ,...n ] ) ] 
							// TODO: Handle OPTION query hints

							endIndex = currentStartSpan.EndIndex;
							SysObject addedSysObject;
							TableSource addedTableSource;
							parser.CreateTableEntry(currentStartSpan, Tokens.kwDeleteToken, lstSysObjects, servername, databasename, schemaname, objectname, "", startIndex, endIndex, startTableIndex, endTableIndex, false, out addedSysObject, out addedTableSource);
						}
					} else {
						// TODO: Handle: { view_name | rowset_function_limited | table_valued_function }
					}
				}
			}
		}

		#endregion
	}
}