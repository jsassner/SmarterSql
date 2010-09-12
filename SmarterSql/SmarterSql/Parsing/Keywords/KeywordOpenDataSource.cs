// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Parsing.Keywords {
	public class KeywordOpenDataSource {
		/// <summary>
		/// Parse OPENQUERY statement
		///   OPENDATASOURCE ( provider_name, init_string )
		/// </summary>
		/// <returns></returns>
		public static bool ParseOpenDataSource(Parser parser, StatementSpans currentStartSpan, List<TokenInfo> lstTokens, ref int i, List<SysObject> lstSysObjects, int startStatementIndex, ref int sysObjectId, out SysObject addedSysObject, out TableSource addedTableSource, TokenInfo startToken) {
			int offset = i;
			TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, offset);
			if (null != nextToken && nextToken.Kind == TokenKind.KeywordOpenDataSource) {
				offset++;
				nextToken = InStatement.GetNextNonCommentToken(lstTokens, offset);
				if (null != nextToken && nextToken.Kind == TokenKind.LeftParenthesis && -1 != nextToken.MatchingParenToken && offset < nextToken.MatchingParenToken) {
					int endSubIndex = nextToken.MatchingParenToken;
					offset++;
					TokenInfo providerName = InStatement.GetNextNonCommentToken(lstTokens, offset);
					if (null != providerName && providerName.Type == TokenType.Identifier && InStatement.GetIfAllNextValidToken(lstTokens, ref offset, out nextToken, TokenKind.Comma)) {
						offset++;
						TokenInfo initString = InStatement.GetNextNonCommentToken(lstTokens, offset);
						if (null != initString && initString.Type == TokenType.String) {
							offset++;
							nextToken = InStatement.GetNextNonCommentToken(lstTokens, offset);
							if (offset == endSubIndex && null != nextToken && nextToken.Kind == TokenKind.RightParenthesis) {
								providerName.TokenContextType = TokenContextType.Known;
								initString.TokenContextType = TokenContextType.Known;
								i = offset;
								addedSysObject = null;
								addedTableSource = null;
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
