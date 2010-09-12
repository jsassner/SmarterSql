// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using Sassner.SmarterSql.Parsing.Expressions;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;

namespace Sassner.SmarterSql.Parsing.Keywords {
	public class KeywordGroupBy {
		#region Parse GROUP BY

		/// <summary>
		/// [ GROUP BY [ ALL ] group_by_expression [ ,...n ] 
		//    [ WITH { CUBE | ROLLUP } ] 
		//  ]
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="lstTokens"></param>
		/// <param name="startIndex"></param>
		/// <param name="endIndex"></param>
		public static void ParseGroupBy(Parser parser, List<TokenInfo> lstTokens, int startIndex, int endIndex) {
			TokenInfo token;
			if (!InStatement.GetIfAllNextValidToken(lstTokens, ref startIndex, out token, TokenKind.KeywordBy)) {
				return;
			}
			// [ ALL ]
			InStatement.GetIfAnyNextValidToken(lstTokens, ref startIndex, out token, TokenKind.KeywordAll);
			// group_by_expression [ ,...n ]
			while (true) {
				startIndex++;
				int expressionStart = startIndex;
				Expression expression;
				if (!Expression.FindExpression(parser, lstTokens, ref startIndex, endIndex, out expression)) {
					return;
				}
				Parser.DumpTokens(lstTokens, expressionStart, startIndex);
				if (startIndex >= endIndex || !InStatement.GetIfAllNextValidToken(lstTokens, ref startIndex, out token, TokenKind.Comma)) {
					break;
				}
			}
			// [ WITH { CUBE | ROLLUP } ] 
			InStatement.GetIfAllNextValidToken(lstTokens, ref startIndex, out token, TokenKind.KeywordWith, TokenKind.KeywordCube);
			InStatement.GetIfAllNextValidToken(lstTokens, ref startIndex, out token, TokenKind.KeywordWith, TokenKind.KeywordRollup);
		}

		#endregion
	}
}