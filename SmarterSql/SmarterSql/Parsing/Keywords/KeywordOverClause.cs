// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using Sassner.SmarterSql.Parsing.Expressions;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;

namespace Sassner.SmarterSql.Parsing.Keywords {
	public class KeywordOverClause {
		#region Parse OVER CLAUSE method

		/// <summary>
		/// Parse a OVER CLAUSE
		/// Aggregate Window Functions
		///   <OVER_CLAUSE> :: = OVER ( [ PARTITION BY value_expression , ... [ n ] ] )
		/// Ranking Window Functions
		///   <OVER_CLAUSE> :: = OVER ( [ PARTITION BY value_expression , ... [ n ] ] <ORDER BY_Clause> )
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="lstTokens"></param>
		/// <param name="functionExpression"></param>
		/// <param name="startIndex"></param>
		/// <returns></returns>
		public static bool ParseOverClause(Parser parser, List<TokenInfo> lstTokens, ScalarFunctionExpression functionExpression, ref int startIndex) {
			int offset = startIndex + 1;
			TokenInfo token = InStatement.GetPreviousNonCommentToken(lstTokens, ref offset);
			if (null == token || token.Kind != TokenKind.KeywordOver) {
				return true;
			}
			offset++;
			token = InStatement.GetPreviousNonCommentToken(lstTokens, ref offset);
			if (null == token || token.Kind != TokenKind.LeftParenthesis || -1 == token.MatchingParenToken) {
				return false;
			}
			int overClauseEndIndex = token.MatchingParenToken;

			offset++;
			token = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
			if (null == token) {
				return false;
			}
			if (token.Kind == TokenKind.KeywordOrder) {
				List<Expression> orderByExpressions;
				if (!KeywordOrderBy.ParseOrderBy(parser, lstTokens, ref offset, overClauseEndIndex, out orderByExpressions)) {
					return false;
				}
				foreach (Expression orderByExpression in orderByExpressions) {
					functionExpression.AddExpression(orderByExpression);
				}
				startIndex = overClauseEndIndex;
				return true;
			}
			offset--;
			if (!InStatement.GetIfAllNextValidToken(lstTokens, ref offset, out token, TokenKind.KeywordPartition, TokenKind.KeywordBy)) {
				return false;
			}

			// [ value_expression , ... [ n ] ]
			offset++;
			while (offset < overClauseEndIndex) {
				Expression subExpression;
				if (Expression.FindExpression(parser, lstTokens, ref offset, overClauseEndIndex, out subExpression)) {
					functionExpression.AddExpression(subExpression);
				} else {
					token = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
					if (null == token) {
						return false;
					}
					if (token.Kind == TokenKind.KeywordOrder) {
						List<Expression> orderByExpressions;
						if (!KeywordOrderBy.ParseOrderBy(parser, lstTokens, ref offset, overClauseEndIndex, out orderByExpressions)) {
							return false;
						}
						foreach (Expression orderByExpression in orderByExpressions) {
							functionExpression.AddExpression(orderByExpression);
						}
						break;
					}
					if (token.Kind != TokenKind.Comma) {
						return false;
					}
				}
				offset++;
			}
			startIndex = overClauseEndIndex;
			return true;
		}

		#endregion
	}
}