// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;

namespace Sassner.SmarterSql.Parsing.Keywords {
	public class KeywordView {
		#region Parse CREATE/ALTER VIEW

		//CREATE VIEW [ schema_name . ] view_name
		//  [ (column [ ,...n ] ) ] 
		//  [ WITH <view_attribute> [ ,...n ] ] 
		//  AS select_statement 
		//  [ WITH CHECK OPTION ] [ ; ]
		//
		//<view_attribute> ::= 
		//{
		//    [ ENCRYPTION ]
		//    [ SCHEMABINDING ]
		//    [ VIEW_METADATA ]
		//} 
		public static void HandleCreateAlterView(List<TokenInfo> lstTokens, ref int i) {
			i++;
			TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
			if (null == nextToken || nextToken.Type != TokenType.Identifier) {
				return;
			}
			int index = i + 1;
			TokenInfo nextNextToken = InStatement.GetNextNonCommentToken(lstTokens, ref index);
			if (null != nextNextToken && nextNextToken.Kind == TokenKind.Dot) {
				index++;
				nextNextToken = InStatement.GetNextNonCommentToken(lstTokens, ref index);
				if (null != nextNextToken && nextNextToken.Type == TokenType.Identifier) {
					nextToken.TokenContextType = TokenContextType.SysObjectSchema;
					nextNextToken.TokenContextType = TokenContextType.View;
					i = index;
				} else {
					return;
				}
			} else {
				nextToken.TokenContextType = TokenContextType.View;
			}

			// [ (column [ ,...n ] ) ]
			if (InStatement.GetIfAllNextValidToken(lstTokens, ref i, out nextToken, TokenKind.LeftParenthesis)) {
				while (true) {
					i++;
					nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
					if (null == nextToken) {
						return;
					}
					if (nextToken.Kind == TokenKind.RightParenthesis) {
						break;
					} else if (nextToken.Kind == TokenKind.Comma) {
						; //Ok
					} else if (nextToken.Kind == TokenKind.Name) {
						nextToken.TokenContextType = TokenContextType.NewColumnAlias;
					} else {
						return;
					}
				}
			}

			// [ WITH <view_attribute> [ ,...n ] ]
			//<view_attribute> ::= 
			//{
			//    [ ENCRYPTION ]
			//    [ SCHEMABINDING ]
			//    [ VIEW_METADATA ]
			//} 
			if (InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordWith)) {
				while (true) {
					if (!InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordEncryption, TokenKind.KeywordSchemabinding, TokenKind.KeywordView_Metadata)) {
						return;
					}

					if (!InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.Comma)) {
						if (null == nextToken) {
							return;
						} else {
							break;
						}
					}
				}
			}
		}

		#endregion
	}
}