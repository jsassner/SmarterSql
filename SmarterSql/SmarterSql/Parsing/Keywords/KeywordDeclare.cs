// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Parsing.Keywords {
	public class KeywordDeclare {
		#region Parse DECLARE cursor

		/// <summary>
		/// Parse DECLARE cursor statement
		/// 			// SQL 92 Syntax
		// DECLARE cursor_name [ INSENSITIVE ] [ SCROLL ] CURSOR 
		//      FOR select_statement 
		//      [ FOR { READ ONLY | UPDATE [ OF column_name [ ,...n ] ] } ]
		// [;]
		// Transact-SQL Extended Syntax
		// DECLARE cursor_name CURSOR [ LOCAL | GLOBAL ] 
		//      [ FORWARD_ONLY | SCROLL ] 
		//      [ STATIC | KEYSET | DYNAMIC | FAST_FORWARD ] 
		//      [ READ_ONLY | SCROLL_LOCKS | OPTIMISTIC ] 
		//      [ TYPE_WARNING ] 
		//      FOR select_statement 
		//      [ FOR UPDATE [ OF column_name [ ,...n ] ] ]
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="lstTokens"></param>
		/// <param name="i"></param>
		public static void ParseDeclareCursor(Parser parser, List<TokenInfo> lstTokens, ref int i) {
			TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
			// CURSOR without preceding @. CURSOR with preceeding @ is handled below
			nextToken.TokenContextType = TokenContextType.Cursor;
			parser.DeclaredCursors.Add(new Cursor(nextToken.Token.UnqoutedImage, i));
			StatementSpans currentSpan = parser.SegmentUtils.GetStatementSpan(i);
			if (null == currentSpan) {
				return;
			}
			int endIndex = currentSpan.EndIndex;
			while (i < endIndex) {
				if (!InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordInsensitive, TokenKind.KeywordScroll, TokenKind.KeywordCursor, TokenKind.KeywordLocal, TokenKind.KeywordGlobal, TokenKind.KeywordForwardOnly, TokenKind.KeywordScroll, TokenKind.KeywordStatic, TokenKind.KeywordKeyset, TokenKind.KeywordDynamic, TokenKind.KeywordFastForward, TokenKind.KeywordReadOnly, TokenKind.KeywordScrollLocks, TokenKind.KeywordOptimistic, TokenKind.KeywordTypeWarning)) {
					break;
				}
			}
			if (InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordFor)) {
				if (InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordSelect)) {
					parser.ParseTokenRange(currentSpan, i, currentSpan.EndIndex, Instance.TextEditor.SysObjects);
				}
			}
		}

		#endregion
	}
}