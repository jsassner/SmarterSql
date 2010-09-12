// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using Sassner.SmarterSql.Parsing.Expressions;
using Sassner.SmarterSql.Parsing.Predicates;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Parsing.Keywords {
	public class KeywordCaseEnd {
		#region Parse CASE-WHEN-THEN-ELSE-END

		/// <summary>
		/// Simple CASE function: 
		/// CASE input_expression 
		///      WHEN when_expression THEN result_expression 
		///     [ ...n ] 
		///      [ 
		///     ELSE else_result_expression 
		///      ] 
		/// END 
		/// Searched CASE function:
		/// CASE
		///      WHEN Boolean_expression THEN result_expression 
		///     [ ...n ] 
		///      [ 
		///     ELSE else_result_expression 
		///      ] 
		/// END
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="lstTokens"></param>
		/// <param name="statementSpan"></param>
		/// <param name="expressions"></param>
		public static bool ParseCase(Parser parser, List<TokenInfo> lstTokens, StatementSpans statementSpan, out Expression expressions) {
			if (null == statementSpan || statementSpan.StartIndex >= statementSpan.EndIndex || -1 == statementSpan.EndIndex) {
				expressions = null;
				return false;
			}
			int startIndex = statementSpan.StartIndex;
			int endIndex = statementSpan.EndIndex;
			TokenInfo token = InStatement.GetNextNonCommentToken(lstTokens, ref startIndex);
			if (null == token || token.Kind != TokenKind.KeywordCase) {
				expressions = null;
				return false;
			}
			startIndex++;
			token = InStatement.GetNextNonCommentToken(lstTokens, ref startIndex);
			if (null == token) {
				expressions = null;
				return false;
			}
			expressions = new CaseEndExpression(statementSpan.StartIndex, statementSpan.EndIndex);
			CaseEndExpression caseEndExpression = (CaseEndExpression)expressions;

			bool isSimpleCase = false;
			if (token.Kind != TokenKind.KeywordWhen) {
				isSimpleCase = true;
				// It's a Simple CASE function
				int startOfExpression = startIndex;
				Expression expression;
				if (!Expression.FindExpression(parser, lstTokens, ref startIndex, endIndex, out expression)) {
					expressions = null;
					return false;
				}
				caseEndExpression.InputExpression = expression;
				Parser.DumpTokens(lstTokens, startOfExpression, startIndex);
			}

			// Parse WHEN -> THEN
			for (int i = 0; i < statementSpan.WhenIndexes.Count; i++) {
				if (i >= statementSpan.ThenIndexes.Count) {
					break;
				}
				int startExpressionIndex = statementSpan.WhenIndexes[i] + 1;
				int endExpressionIndex = statementSpan.ThenIndexes[i] - 1;
				int index = startExpressionIndex;

				if (isSimpleCase) {
					Expression whenExpression;
					if (!Expression.FindExpression(parser, lstTokens, ref index, endExpressionIndex, out whenExpression)) {
						return false;
					}
					caseEndExpression.WhenExpressions.Add(whenExpression);
				} else {
					List<Predicate> predicates;
					if (!KeywordSearchCondition.ParseSearchCondition(parser, lstTokens, index, endExpressionIndex, Instance.TextEditor.SysObjects, out predicates)) {
						return false;
					}
					if (null != predicates) {
						foreach (Predicate predicate in predicates) {
							caseEndExpression.WhenPredicates.Add(predicate);
						}
					}
				}

				startExpressionIndex = statementSpan.ThenIndexes[i] + 1;
				index = startExpressionIndex;
				if (i + 1 == statementSpan.WhenIndexes.Count) {
					if (-1 != statementSpan.ElseIndex) {
						endExpressionIndex = statementSpan.ElseIndex - 1;
					} else {
						endExpressionIndex = statementSpan.EndIndex - 1;
					}
				} else {
					endExpressionIndex = statementSpan.WhenIndexes[i + 1] - 1;
				}
				Expression thenExpression;
				if (!Expression.FindExpression(parser, lstTokens, ref index, endExpressionIndex, out thenExpression)) {
					return false;
				}
				Parser.DumpTokens(lstTokens, startExpressionIndex, endExpressionIndex);
				caseEndExpression.ThenExpressions.Add(thenExpression);
			}

			// Parse ELSE
			if (-1 != statementSpan.ElseIndex) {
				int startExpressionIndex = statementSpan.ElseIndex + 1;
				int index = startExpressionIndex;
				int endExpressionIndex = statementSpan.EndIndex - 1;
				Expression elseExpression;
				if (!Expression.FindExpression(parser, lstTokens, ref index, endExpressionIndex, out elseExpression)) {
					return false;
				}
				Parser.DumpTokens(lstTokens, startExpressionIndex, endExpressionIndex);
				caseEndExpression.ElseExpression = elseExpression;
			}
			return true;
		}

		#endregion
	}
}