// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.Parsing.Expressions;
using Sassner.SmarterSql.Parsing.Predicates;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Parsing.Keywords {
	public class KeywordSearchCondition {
		#region Member variables

		private const string ClassName = "KeywordSearchCondition";

		#endregion

		#region ParseSearchCondition

		/// <summary>
		/// Parse a search condition segment for tokens
		/// < search_condition > ::= 
		///    { [ NOT ] <predicate> | ( <search_condition> ) } 
		///    [ { AND | OR } [ NOT ] { <predicate> | ( <search_condition> ) } ] 
		///    [ ,...n ] 
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="lstTokens"></param>
		/// <param name="startIndex"></param>
		/// <param name="endIndex"></param>
		/// <param name="lstSysObjects"></param>
		public static bool ParseSearchCondition(Parser parser, List<TokenInfo> lstTokens, int startIndex, int endIndex, List<SysObject> lstSysObjects, out List<Predicate> predicates) {
			bool firstPredicate = true;
			predicates = new List<Predicate>();
			Predicate predicate;

			while (startIndex + 1 < endIndex) {
				bool isPredicateNot = false;
				bool isPredicateAnd = false;
				bool isPredicateOr = false;
				TokenInfo token;

				#region { AND | OR } [ NOT ]

				if (!firstPredicate) {
					if (InStatement.GetIfAnyNextValidToken(lstTokens, ref startIndex, out token, TokenKind.KeywordAnd)) {
						isPredicateAnd = true;
					} else if (InStatement.GetIfAnyNextValidToken(lstTokens, ref startIndex, out token, TokenKind.KeywordOr)) {
						isPredicateOr = true;
					}
					if (InStatement.GetIfAnyNextValidToken(lstTokens, ref startIndex, out token, TokenKind.KeywordNot)) {
						isPredicateNot = true;
					}
					startIndex++;
				} else {
					startIndex--;
					if (InStatement.GetIfAnyNextValidToken(lstTokens, ref startIndex, out token, TokenKind.KeywordNot)) {
						isPredicateNot = true;
					}
					startIndex++;
				}

				#endregion

				token = InStatement.GetNextNonCommentToken(lstTokens, ref startIndex);
				if (null != token && token.Kind == TokenKind.LeftParenthesis) {
					int offset = startIndex + 1;
					TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
					if (null != nextToken && nextToken.Kind == TokenKind.KeywordSelect) {
						StatementSpans span = parser.SegmentUtils.GetStatementSpan(startIndex + 1);
						if (null != span) {
							span.IsSubSelect = true;

							if (!GetPredicate(parser, lstTokens, ref startIndex, endIndex, lstSysObjects, out predicate)) {
								return false;
							}
							if (null != predicate) {
								predicates.Add(predicate);
								predicate.IsPredicateNot = isPredicateNot;
								predicate.IsPredicateAnd = isPredicateAnd;
								predicate.IsPredicateOr = isPredicateOr;
							}
						}
					} else {
						// ( <search_condition> )
						int startSubIndex = startIndex;
						int endSubIndex = token.MatchingParenToken;
						if (-1 == endSubIndex || startSubIndex > endSubIndex) {
							return false;
						}
						List<Predicate> subQueryPredicates;
						if (!ParseSearchCondition(parser, lstTokens, startSubIndex + 1, endSubIndex - 1, lstSysObjects, out subQueryPredicates)) {
							return false;
						}
						if (null == subQueryPredicates || 0 == subQueryPredicates.Count) {
							return false;
						}
						Parser.DumpTokens(lstTokens, startSubIndex, endSubIndex);
						startIndex = endSubIndex;

						Predicate lastPredicate = subQueryPredicates[subQueryPredicates.Count - 1];
						foreach (Predicate subQueryPredicate in subQueryPredicates) {
							predicates.Add(subQueryPredicate);
						}

						offset = startIndex + 1;
						token = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
						if (offset < endIndex) {
							if (token.Kind != TokenKind.KeywordAnd && token.Kind != TokenKind.KeywordOr && token.Kind != TokenKind.KeywordNot) {
								if (null == lastPredicate.Expressions || 0 == lastPredicate.Expressions.Count) {
									return false;
								}
								Expression expression = lastPredicate.Expressions[lastPredicate.Expressions.Count - 1];
								startIndex = offset;
								if (!GetPredicateFromExpression(parser, lstTokens, lstSysObjects, ref startIndex, endIndex, startSubIndex, expression, out predicate)) {
									return false;
								}
								predicates.Add(predicate);
								startIndex = InStatement.GetNextNonCommentToken(lstTokens, predicate.EndIndex, predicate.EndIndex);
							}
						}
					}
				} else {
					if (!GetPredicate(parser, lstTokens, ref startIndex, endIndex, lstSysObjects, out predicate)) {
						return false;
					}
					if (null != predicate) {
						predicates.Add(predicate);
						predicate.IsPredicateNot = isPredicateNot;
						predicate.IsPredicateAnd = isPredicateAnd;
						predicate.IsPredicateOr = isPredicateOr;
					}
				}

				firstPredicate = false;

				int tokenIndex = InStatement.GetNextNonCommentToken(lstTokens, startIndex + 1, startIndex + 1);
				if (tokenIndex > endIndex) {
					break;
				}
			}
			return true;
		}

		#endregion

		#region GetPredicate

		/// <summary>
		/// Get a Predicate from a Search Condition
		/// <predicate> ::= 
		///    {
		///      CONTAINS ( { column_name | (column_list) | * } , '< contains_search_condition >' [ , LANGUAGE language_term ] )
		///    | FREETEXT ( { column | * } , 'freetext_string' ) 
		///    | EXISTS ( subquery )
		///    | expression { = | <> | != | > | >= | !> | < | <= | !< } expression 
		///    | string_expression [ NOT ] LIKE string_expression [ ESCAPE 'escape_character' ] 
		///    | expression [ NOT ] BETWEEN expression AND expression 
		///    | expression IS [ NOT ] NULL 
		///    | expression [ NOT ] IN ( subquery | expression [ ,...n ] ) 
		///    | expression { = | <> | != | > | >= | !> | < | <= | !< } { ALL | SOME | ANY} ( subquery ) 
		///    } 
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="lstTokens"></param>
		/// <param name="startIndex"></param>
		/// <param name="endIndex"></param>
		/// <param name="lstSysObjects"></param>
		/// <returns></returns>
		public static bool GetPredicate(Parser parser, List<TokenInfo> lstTokens, ref int startIndex, int endIndex, List<SysObject> lstSysObjects, out Predicate predicate) {
			Expression expression;
			TokenInfo token = InStatement.GetNextNonCommentToken(lstTokens, ref startIndex);

			if (null != token && token.Kind == TokenKind.KeywordContains) {
				#region Parse CONTAINS

				int startIndexContains = startIndex;
				List<Expression> columnNameExpressions = new List<Expression>();
				// CONTAINS ( { column_name | (column_list) | * } , '< contains_search_condition >' [ , LANGUAGE language_term ] )
				startIndex++;
				if (InStatement.GetIfAnyNextValidToken(lstTokens, ref startIndex, out token, TokenKind.Name)) {
					if (!ColumnExpression.ParseColumnName(parser, lstTokens, ref startIndex, out expression)) {
						predicate = null;
						return false;
					}
					columnNameExpressions.Add(expression);
				} else if (InStatement.GetIfAnyNextValidToken(lstTokens, ref startIndex, out token, TokenKind.LeftParenthesis)) {
					int columnListStart = startIndex + 1;
					int columnListEnd = token.MatchingParenToken;
					if (-1 != columnListEnd && columnListStart < columnListEnd) {
						while (startIndex < columnListEnd) {
							if (lstTokens[startIndex].Kind == TokenKind.Name) {
								if (!ColumnExpression.ParseColumnName(parser, lstTokens, ref startIndex, out expression)) {
									predicate = null;
									return false;
								}
								columnNameExpressions.Add(expression);
							}
							startIndex++;
						}
					} else {
						predicate = null;
						return false;
					}
				} else if (InStatement.GetIfAnyNextValidToken(lstTokens, ref startIndex, out token, TokenKind.Multiply)) {
					// Ok
				} else {
					predicate = null;
					return false;
				}

				if (!InStatement.GetIfAllNextValidToken(lstTokens, ref startIndex, out token, TokenKind.Comma)) {
					predicate = null;
					return false;
				}
				string searchCondition = string.Empty;
				if (InStatement.GetIfAnyNextValidToken(lstTokens, ref startIndex, out token, TokenKind.ValueString)) {
					searchCondition = token.Token.UnqoutedImage;
					// Ok
				} else if (InStatement.GetIfAnyNextValidToken(lstTokens, ref startIndex, out token, TokenKind.Variable)) {
					parser.CalledLocalVariables.Add(new LocalVariable(token.Token.UnqoutedImage, startIndex));
				} else {
					predicate = null;
					return false;
				}
				// [ , LANGUAGE language_term ]
				if (InStatement.GetIfAllNextValidToken(lstTokens, ref startIndex, out token, TokenKind.Comma, TokenKind.KeywordLanguage)) {
					if (InStatement.GetIfAnyNextValidToken(lstTokens, ref startIndex, out token, TokenKind.ValueString, TokenKind.ValueNumber)) {
						token.TokenContextType = TokenContextType.Known;
					}
				}
				startIndex++;
				predicate = new ContainsPredicate(startIndexContains, startIndex, columnNameExpressions, searchCondition);
				return true;

				#endregion
			}

			if (null != token && token.Kind == TokenKind.KeywordFreeText) {
				#region Parse FREETEXT

				int startIndexFreetext = startIndex;
				List<Expression> columnNameExpressions = new List<Expression>();
				// FREETEXT ( { column_name | (column_list) | * } , 'freetext_string' [ , LANGUAGE language_term ] )
				startIndex++;
				if (InStatement.GetIfAnyNextValidToken(lstTokens, ref startIndex, out token, TokenKind.Name)) {
					if (!ColumnExpression.ParseColumnName(parser, lstTokens, ref startIndex, out expression)) {
						predicate = null;
						return false;
					}
					columnNameExpressions.Add(expression);
				} else if (InStatement.GetIfAnyNextValidToken(lstTokens, ref startIndex, out token, TokenKind.LeftParenthesis)) {
					int columnListStart = startIndex + 1;
					int columnListEnd = token.MatchingParenToken;
					if (-1 != columnListEnd && columnListStart < columnListEnd) {
						while (startIndex < columnListEnd) {
							if (lstTokens[startIndex].Kind == TokenKind.Name) {
								if (!ColumnExpression.ParseColumnName(parser, lstTokens, ref startIndex, out expression)) {
									predicate = null;
									return false;
								}
								columnNameExpressions.Add(expression);
							}
							startIndex++;
						}
					} else {
						predicate = null;
						return false;
					}
				} else if (InStatement.GetIfAnyNextValidToken(lstTokens, ref startIndex, out token, TokenKind.Multiply)) {
					// Ok
				} else {
					predicate = null;
					return false;
				}
				if (!InStatement.GetIfAllNextValidToken(lstTokens, ref startIndex, out token, TokenKind.Comma)) {
					predicate = null;
					return false;
				}
				string searchCondition = string.Empty;
				if (InStatement.GetIfAnyNextValidToken(lstTokens, ref startIndex, out token, TokenKind.ValueString)) {
					searchCondition = token.Token.UnqoutedImage;
					// Ok
				} else if (InStatement.GetIfAnyNextValidToken(lstTokens, ref startIndex, out token, TokenKind.Variable)) {
					parser.CalledLocalVariables.Add(new LocalVariable(token.Token.UnqoutedImage, startIndex));
				} else {
					predicate = null;
					return false;
				}
				// [ , LANGUAGE language_term ]
				if (InStatement.GetIfAllNextValidToken(lstTokens, ref startIndex, out token, TokenKind.Comma, TokenKind.KeywordLanguage)) {
					if (InStatement.GetIfAnyNextValidToken(lstTokens, ref startIndex, out token, TokenKind.ValueString, TokenKind.ValueNumber)) {
						token.TokenContextType = TokenContextType.Known;
					}
				}
				startIndex++;
				predicate = new FreeTextPredicate(startIndexFreetext, startIndex, columnNameExpressions, searchCondition);
				return true;

				#endregion
			}

			if (null != token && token.Kind == TokenKind.KeywordExists) {
				#region Parse EXISTS ( subquery )

				int startIndexExists = startIndex;
				// EXISTS ( subquery )
				if (InStatement.GetIfAnyNextValidToken(lstTokens, ref startIndex, out token, TokenKind.LeftParenthesis)) {
					// Parse subquery
					int subqueryStart = startIndex + 1;
					int subqueryEnd = token.MatchingParenToken;
					if (-1 != subqueryEnd && subqueryStart < subqueryEnd) {
						StatementSpans newSpan = parser.SegmentUtils.GetStatementSpan(subqueryStart + 1);
						if (null != newSpan) {
							newSpan.IsSubSelect = true;
						}
						parser.ParseTokenRange(newSpan, subqueryStart, subqueryEnd, lstSysObjects);
						startIndex = subqueryEnd;
						predicate = new ExistsPredicate(startIndexExists, startIndex, newSpan);
						return true;
					}
				}
				predicate = null;
				return false;

				#endregion
			}

			int expressionStart = startIndex;
			if (!Expression.FindExpression(parser, lstTokens, ref startIndex, endIndex, out expression)) {
				predicate = null;
				return false;
			}
			Parser.DumpTokens(lstTokens, expressionStart, startIndex);

			// expression { = | <> | != | > | >= | !> | < | <= | !< } expression
			if (expression is BinaryOperatorExpression) {
				predicate = new BinaryPredicate(expressionStart, startIndex, expression);
				return true;
			}

			// End of tokens?
			if (startIndex >= endIndex) {
				if (null != expression) {
					predicate = new SubPredicate(expressionStart, startIndex, expression);
					return true;
				}
				predicate = null;
				return false;
			}
			startIndex++;

			return GetPredicateFromExpression(parser, lstTokens, lstSysObjects, ref startIndex, endIndex, expressionStart, expression, out predicate);
		}

		#endregion

		#region GetPredicateFromExpression

		/// <summary>
		/// Get a predicate using the supplied expression as the initial expression.
		///    | expression { = | <> | != | > | >= | !> | < | <= | !< } expression 
		///    | string_expression [ NOT ] LIKE string_expression [ ESCAPE 'escape_character' ] 
		///    | expression [ NOT ] BETWEEN expression AND expression 
		///    | expression IS [ NOT ] NULL 
		///    | expression [ NOT ] IN ( subquery | expression [ ,...n ] ) 
		///    | expression { = | <> | != | > | >= | !> | < | <= | !< } { ALL | SOME | ANY} ( subquery ) 
		/// </summary>
		/// <param name="startIndex"></param>
		/// <param name="endIndex"></param>
		/// <param name="parser"></param>
		/// <param name="lstTokens"></param>
		/// <param name="expression"></param>
		/// <param name="lstSysObjects"></param>
		/// <param name="expressionStart"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		private static bool GetPredicateFromExpression(Parser parser, List<TokenInfo> lstTokens, List<SysObject> lstSysObjects, ref int startIndex, int endIndex, int expressionStart, Expression expression, out Predicate predicate) {
			if (null == expression) {
				predicate = null;
				return false;
			}
			TokenInfo token = InStatement.GetNextNonCommentToken(lstTokens, ref startIndex);
			TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, startIndex + 1);
			if (null == token) {
				predicate = null;
				return false;
			}
			if (token.Type == TokenType.Operator) {
				if (null != nextToken) {
					if (nextToken.Kind == TokenKind.KeywordAll || nextToken.Kind == TokenKind.KeywordSome || nextToken.Kind == TokenKind.KeywordAny) {
						#region expression { = | <> | != | > | >= | !> | < | <= | !< } { ALL | SOME | ANY} ( subquery )

						startIndex++;
						if (!InStatement.GetIfAllNextValidToken(lstTokens, ref startIndex, out token, TokenKind.LeftParenthesis)) {
							predicate = null;
							return false;
						}
						int startSubSelect = startIndex + 1;
						int endSubSelect = token.MatchingParenToken;
						if (-1 == endSubSelect || startSubSelect > endSubSelect) {
							predicate = null;
							return false;
						}
						// Parse sub select
						StatementSpans newSpan = parser.SegmentUtils.GetStatementSpan(startSubSelect + 1);
						if (null != newSpan) {
							newSpan.IsSubSelect = true;
						}
						parser.ParseTokenRange(newSpan, startSubSelect, endSubSelect, lstSysObjects);
						startIndex = endSubSelect;
						predicate = new ComparisonSubqueryPredicate(expressionStart, startIndex, expression, nextToken, newSpan);
						return true;

						#endregion
					}

					#region expression { = | <> | != | > | >= | !> | < | <= | !< } expression

					startIndex++;
					Expression expression2;
					if (!Expression.FindExpression(parser, lstTokens, ref startIndex, endIndex, out expression2)) {
						predicate = null;
						return false;
					}
					expression = new BinaryOperatorExpression(expressionStart, startIndex, expression, token, expression2);
					predicate = new BinaryPredicate(expressionStart, startIndex, expression);
					return true;

					#endregion
				}
				predicate = null;
				return false;
			}

			if (token.Kind == TokenKind.KeywordIs) {
				#region expression IS [ NOT ] NULL

				bool isNull = false;
				if (InStatement.GetIfAnyNextValidToken(lstTokens, ref startIndex, out token, TokenKind.KeywordNot)) {
					isNull = true;
				}
				if (!InStatement.GetIfAllNextValidToken(lstTokens, ref startIndex, out token, TokenKind.KeywordNull)) {
					predicate = null;
					return false;
				}
				predicate = new NullPredicate(expressionStart, startIndex, expression, isNull);
				return true;

				#endregion
			} else {
				bool isNull = false;
				if (token.Kind == TokenKind.KeywordNot) {
					isNull = true;
					startIndex++;
				}
				token = InStatement.GetNextNonCommentToken(lstTokens, ref startIndex);
				if (null != token && token.Kind == TokenKind.KeywordLike) {
					#region string_expression [ NOT ] LIKE string_expression [ ESCAPE 'escape_character' ]

					startIndex++;
					expressionStart = startIndex;
					Expression string_expression;
					if (!Expression.FindExpression(parser, lstTokens, ref startIndex, endIndex, out string_expression)) {
						predicate = null;
						return false;
					}
					Parser.DumpTokens(lstTokens, expressionStart, startIndex);
					string escapeChar = string.Empty;
					if (InStatement.GetIfAnyNextValidToken(lstTokens, ref startIndex, out token, TokenKind.KeywordEscape)) {
						if (!InStatement.GetIfAnyNextValidToken(lstTokens, ref startIndex, out token, TokenKind.ValueString)) {
							predicate = null;
							return false;
						}
						escapeChar = token.Token.UnqoutedImage;
					}
					predicate = new LikePredicate(expressionStart, startIndex, isNull, expression, string_expression, escapeChar);
					return true;

					#endregion
				}
				if (null != token && token.Kind == TokenKind.KeywordBetween) {
					#region expression [ NOT ] BETWEEN expression AND expression

					startIndex++;
					expressionStart = startIndex;
					Expression expression1;
					if (!Expression.FindExpression(parser, lstTokens, ref startIndex, endIndex, out expression1)) {
						predicate = null;
						return false;
					}
					Parser.DumpTokens(lstTokens, expressionStart, startIndex);
					if (!InStatement.GetIfAllNextValidToken(lstTokens, ref startIndex, out token, TokenKind.KeywordAnd)) {
						predicate = null;
						return false;
					}
					startIndex++;
					expressionStart = startIndex;
					Expression expression2;
					if (!Expression.FindExpression(parser, lstTokens, ref startIndex, endIndex, out expression2)) {
						predicate = null;
						return false;
					}
					Parser.DumpTokens(lstTokens, expressionStart, startIndex);
					predicate = new BetweenPredicate(expressionStart, startIndex, isNull, expression, expression1, expression2);
					return true;

					#endregion
				}
				if (null != token && token.Kind == TokenKind.KeywordIn) {
					#region expression [ NOT ] IN ( subquery | expression [ ,...n ] )

					if (!InStatement.GetIfAllNextValidToken(lstTokens, ref startIndex, out token, TokenKind.LeftParenthesis)) {
						predicate = null;
						return false;
					}
					startIndex++;
					int startSubSelect = startIndex;
					int endSubSelect = token.MatchingParenToken;
					if (-1 == endSubSelect || startSubSelect > endSubSelect) {
						predicate = null;
						return false;
					}
					token = InStatement.GetNextNonCommentToken(lstTokens, ref startIndex);
					if (null == token) {
						predicate = null;
						return false;
					}
					if (token.Kind == TokenKind.KeywordSelect) {
						// Parse sub select
						StatementSpans newSpan = parser.SegmentUtils.GetStatementSpan(startIndex);
						if (null != newSpan) {
							newSpan.IsSubSelect = true;
						}
						parser.ParseTokenRange(newSpan, startSubSelect, endSubSelect, lstSysObjects);
						predicate = new InSubqueryPredicate(startSubSelect, endSubSelect - 1, isNull, newSpan);
						startIndex = endSubSelect;
						return true;
					}

					// Comma seperated list of expressions
					expressionStart = startIndex;
					List<Expression> expressions = new List<Expression> {
						expression
					};
					while (startIndex < endSubSelect) {
						if (!Expression.FindExpression(parser, lstTokens, ref startIndex, endIndex, out expression)) {
							predicate = null;
							return false;
						}
						expressions.Add(expression);
						startIndex++;
						token = InStatement.GetNextNonCommentToken(lstTokens, ref startIndex);
						if (null != token && token.Kind != TokenKind.Comma) {
							break;
						}
						startIndex++;
					}
					Parser.DumpTokens(lstTokens, expressionStart, startIndex);
					predicate = new InExpressionsPredicate(expressionStart, startIndex, isNull, expressions);
					return true;

					#endregion
				}
				// Unknown. Error
				predicate = null;
				return false;
			}
		}

		#endregion

		#region FindLengthOfSearchCondition

		/// <summary>
		/// Find the end of a derived table
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="lstTokens"></param>
		/// <param name="currentIndex"></param>
		/// <returns></returns>
		internal static int FindLengthOfSearchCondition(Parser parser, List<TokenInfo> lstTokens, int currentIndex) {
			if (currentIndex < lstTokens.Count) {
				try {
					int minIndex = -1;
					int parenLevel = lstTokens[currentIndex].ParenLevel;
					StatementSpans ss;
					Common.GetStatementSpan(lstTokens[currentIndex].Span, parser.SegmentUtils.StartTokenIndexes, parenLevel, out ss);
					if (null != ss) {
						minIndex = ss.EndIndex;
						if (-1 != ss.WhereIndex && ss.WhereIndex < minIndex) {
							minIndex = ss.WhereIndex - 1;
						}
						if (-1 != ss.GroupIndex && ss.GroupIndex < minIndex) {
							minIndex = ss.GroupIndex - 1;
						}
						if (-1 != ss.HavingIndex && ss.HavingIndex < minIndex) {
							minIndex = ss.HavingIndex - 1;
						}
						if (-1 != ss.OrderbyIndex && ss.OrderbyIndex < minIndex) {
							minIndex = ss.OrderbyIndex - 1;
						}

						int i = currentIndex + 1;
						while (i < minIndex) {
							TokenInfo token = InStatement.GetNextNonCommentToken(lstTokens, ref i);
							if (token.ParenLevel == parenLevel) {
								if (token.Kind == TokenKind.Comma) {
									return InStatement.GetPreviousNonCommentToken(lstTokens, i - 1, i - 1);
								}
								if (token.Kind == TokenKind.KeywordContainsTable || token.Kind == TokenKind.KeywordOpenQuery || token.Kind == TokenKind.KeywordFreeTextTable || token.Kind == TokenKind.KeywordOpenRowset || token.Kind == TokenKind.KeywordOpenDataSource) {
									return InStatement.GetPreviousNonCommentToken(lstTokens, i - 1, i - 1);
								}
								if (token.Kind == TokenKind.KeywordOpenXml) {
									return InStatement.GetPreviousNonCommentToken(lstTokens, i - 1, i - 1);
								}
								if (token.Kind == TokenKind.KeywordPivot) {
									return InStatement.GetPreviousNonCommentToken(lstTokens, i - 1, i - 1);
								}
								if (token.Kind == TokenKind.KeywordUnpivot) {
									return InStatement.GetPreviousNonCommentToken(lstTokens, i - 1, i - 1);
								}
								if (token.Kind == TokenKind.KeywordJoin) {
									int startIndex;
									if (InStatement.FindStartOfJoin(lstTokens, i, out startIndex)) {
										return InStatement.GetPreviousNonCommentToken(lstTokens, startIndex - 1, startIndex - 1);
									}
									return currentIndex;
								}
							}
							i++;
						}
					}

					if (-1 != minIndex) {
						return minIndex;
					}
				} catch (Exception e) {
					Common.LogEntry(ClassName, "FindLengthOfSearchCondition", e, Common.enErrorLvl.Error);
				}

				return currentIndex;
			}
			return lstTokens.Count - 1;
		}

		#endregion
	}
}
