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
	public class KeywordOrderBy {
		#region Parse ORDER BY

		/// <summary>
		/// [ ORDER BY 
		///    {
		///    order_by_expression 
		///  [ COLLATE collation_name ] 
		///  [ ASC | DESC ] 
		///    } [ ,...n ] 
		/// ] 
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="lstTokens"></param>
		/// <param name="startIndex"></param>
		/// <param name="endIndex"></param>
		/// <param name="expressions"></param>
		public static bool ParseOrderBy(Parser parser, List<TokenInfo> lstTokens, ref int startIndex, int endIndex, out List<Expression> expressions) {
			TokenInfo token;
			if (!InStatement.GetIfAllNextValidToken(lstTokens, ref startIndex, out token, TokenKind.KeywordBy)) {
				expressions = null;
				return false;
			}
			expressions = new List<Expression>();
			// order_by_expression [ ,...n ]
			while (true) {
				startIndex++;
				int expressionStart = startIndex;
				Expression expression;
				if (!Expression.FindExpression(parser, lstTokens, ref startIndex, endIndex, out expression)) {
					expressions = null;
					return false;
				}
				expressions.Add(expression);

				// See if the column expression is a declared Column Alias
				if (expression is ColumnExpression) {
					ColumnExpression columnExpression = (ColumnExpression)expression;
					if (null == columnExpression.Alias) {
						StatementSpans currentSpan = parser.SegmentUtils.GetStatementSpan(startIndex);
						ColumnAlias columnAlias;
						if (ColumnAlias.IsColumnAliasInSegment(parser, currentSpan, columnExpression.Column.Token.UnqoutedImage, out columnAlias)) {
							// It was. Change type to ColumnAlias instead
							lstTokens[columnExpression.StartIndex].TokenContextType = TokenContextType.ColumnAlias;
						}
					}
				}

				Parser.DumpTokens(lstTokens, expressionStart, startIndex);
				InStatement.GetIfAllNextValidToken(lstTokens, ref startIndex, out token, TokenKind.KeywordCollate, TokenKind.ValueString);
				InStatement.GetIfAnyNextValidToken(lstTokens, ref startIndex, out token, TokenKind.KeywordAsc, TokenKind.KeywordDesc);
				if (startIndex >= endIndex || !InStatement.GetIfAllNextValidToken(lstTokens, ref startIndex, out token, TokenKind.Comma)) {
					break;
				}
			}
			return true;
		}

		#endregion
	}
}