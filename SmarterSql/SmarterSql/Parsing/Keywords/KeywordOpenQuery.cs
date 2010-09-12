// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Parsing.Keywords {
	public class KeywordOpenQuery {
		/// <summary>
		/// Parse OPENQUERY statement
		///   OPENQUERY ( linked_server ,'query' )
		/// </summary>
		/// <returns></returns>
		public static bool ParseOpenQuery(Parser parser, StatementSpans currentStartSpan, List<TokenInfo> lstTokens, ref int i, List<SysObject> lstSysObjects, int startStatementIndex, ref int sysObjectId, out SysObject addedSysObject, out TableSource addedTableSource, TokenInfo startToken) {
			List<SysObjectColumn> lstSysObjectColumn = new List<SysObjectColumn>();
			int offset = i;
			TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, offset);
			if (null != nextToken && nextToken.Kind == TokenKind.KeywordOpenQuery) {
				int startRowsetIndex = offset;
				offset++;
				nextToken = InStatement.GetNextNonCommentToken(lstTokens, offset);
				if (null != nextToken && nextToken.Kind == TokenKind.LeftParenthesis && -1 != nextToken.MatchingParenToken && offset < nextToken.MatchingParenToken) {
					int endSubIndex = nextToken.MatchingParenToken;
					offset++;
					TokenInfo linkedServerToken = InStatement.GetNextNonCommentToken(lstTokens, offset);
					if (null != linkedServerToken && linkedServerToken.Type == TokenType.Identifier && InStatement.GetIfAllNextValidToken(lstTokens, ref offset, out nextToken, TokenKind.Comma)) {
						offset++;
						TokenInfo queryToken = InStatement.GetNextNonCommentToken(lstTokens, offset);
						if (null != queryToken && queryToken.Type == TokenType.String) {
							offset++;
							nextToken = InStatement.GetNextNonCommentToken(lstTokens, offset);
							if (offset == endSubIndex && null != nextToken && nextToken.Kind == TokenKind.RightParenthesis) {
								linkedServerToken.TokenContextType = TokenContextType.LinkedServer;
								queryToken.TokenContextType = TokenContextType.Known;
								i = offset;

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
				}
			}

			i = offset;
			addedSysObject = null;
			addedTableSource = null;
			return false;
		}
	}
}