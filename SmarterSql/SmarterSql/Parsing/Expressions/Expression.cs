// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.Objects.Functions;
using Sassner.SmarterSql.Parsing.Keywords;
using Sassner.SmarterSql.ParsingObjects;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Parsing.Expressions {
	public abstract class Expression {
		#region Member variables

		protected readonly int tokenIndex;
		protected ParsedDataType parsedDataType;

		#endregion

		protected Expression(int tokenIndex, ParsedDataType parsedDataType) {
			this.tokenIndex = tokenIndex;
			this.parsedDataType = parsedDataType;
		}

		#region Public properties

		public ParsedDataType ParsedDataType {
			[DebuggerStepThrough]
			get { return parsedDataType; }
		}

		public int TokenIndex {
			[DebuggerStepThrough]
			get { return tokenIndex; }
		}

		#endregion

		#region Abstract methods

		protected abstract bool GetParsedDataType(Connection connection, List<SysObject> lstSysObjects, List<TableSource> lstFoundTableSources, out ParsedDataType parsedDataType);

		#endregion

		#region GetParsedDataType methods

		/// <summary>
		/// Retrieve the datatype from the list of expressions
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="lstSysObjects"></param>
		/// <param name="foundTableSources"></param>
		/// <param name="expressions"></param>
		/// <returns></returns>
		public static ParsedDataType GetParsedDataType(Connection connection, List<SysObject> lstSysObjects, List<TableSource> foundTableSources, List<Expression> expressions) {
			ParsedDataType parsedDataType = null;
			foreach (Expression expression in expressions) {
				ParsedDataType foundParsedDataType;
				if (expression.GetParsedDataType(connection, lstSysObjects, foundTableSources, out foundParsedDataType) && null == parsedDataType) {
					parsedDataType = foundParsedDataType;
				}
			}
			return parsedDataType;
		}

		/// <summary>
		/// Get the datatype from the expression
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="lstSysObjects"></param>
		/// <param name="foundTableSources"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static ParsedDataType GetParsedDataType(Connection connection, List<SysObject> lstSysObjects, List<TableSource> foundTableSources, Expression expression) {
			ParsedDataType parsedDataType;
			if (!expression.GetParsedDataType(connection, lstSysObjects, foundTableSources, out parsedDataType)) {
				parsedDataType = null;
			}
			return parsedDataType;
		}

		#endregion

		#region Find expression

		/// <summary>
		/// Find an expression. An expression is a column name, a constant, a function, a variable, a scalar subquery, or any combination
		/// of column names, constants, and functions connected by an operator or operators, or a subquery.
		/// The expression can also contain the CASE function.
		/// {
		///*   constant  -- string or number
		///* | variable 
		///* | [ table_name. ] column
		///* | ( scalar_subquery ) 
		///* | ( expression )
		///* | { unary_operator } expression 
		///* | scalar_function
		///* | expression { binary_operator } expression 
		///* | ranking_windowed_function
		///* | aggregate_windowed_function
		///* | CASE ... END
		/// }
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="lstTokens"></param>
		/// <param name="startIndex"></param>
		/// <param name="endIndex"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static bool FindExpression(Parser parser, List<TokenInfo> lstTokens, ref int startIndex, int endIndex, out Expression expression) {
			int originalStartIndex;
			if (endIndex >= lstTokens.Count) {
				endIndex = lstTokens.Count - 1;
			}
			if (startIndex > endIndex) {
				startIndex = endIndex;
				expression = null;
				return false;
			}

			TokenInfo token = InStatement.GetNextNonCommentToken(lstTokens, ref startIndex);
			if (null == token) {
				expression = null;
				return false;
			}

			// Set start of binary operator
			int startBinaryOperator = startIndex;

			string unqoutedImage = token.Token.UnqoutedImage;
			switch (token.Kind) {
				case TokenKind.ValueString:
					expression = new ConstantExpression(startIndex, new ParsedDataType(Tokens.kwCharToken, token.Token.Image.Length.ToString()));
					break;
				case TokenKind.ValueNumber:
					expression = unqoutedImage.Contains(".") ? new ConstantExpression(startIndex, new ParsedDataType(Tokens.kwFloatToken, "8")) : new ConstantExpression(startIndex, new ParsedDataType(Tokens.kwIntToken, "4"));
					break;
				case TokenKind.KeywordNull:
					expression = new TokenExpression(startIndex, token);
					break;
				case TokenKind.SystemVariable:
					expression = null;
					GlobalVariable globalVariable;
					if (Instance.TextEditor.StaticData.GlobalVariables.TryGetValue(unqoutedImage, out globalVariable)) {
						expression = new VariableExpression(startIndex, globalVariable.ParsedDataType);
						token.TokenContextType = TokenContextType.SystemVariable;
					}
					break;
				case TokenKind.Variable:
					expression = null;
					LocalVariable variable = LocalVariable.GetLocalVariable(parser, unqoutedImage, startIndex);
					if (null != variable) {
						expression = new VariableExpression(startIndex, variable.ParsedDataType);
						token.TokenContextType = TokenContextType.Variable;
						parser.CalledLocalVariables.Add(new LocalVariable(unqoutedImage, startIndex));
					}
					break;
				case TokenKind.KeywordCase:
					StatementSpans span = parser.SegmentUtils.GetCaseStatementSpan(startIndex);
					if (null != span) {
						KeywordCaseEnd.ParseCase(parser, lstTokens, span, out expression);
						startIndex = span.EndIndex;
					} else {
						expression = null;
						return false;
					}
					break;
				case TokenKind.Twiddle:
				case TokenKind.Subtract:
				case TokenKind.Add:
					// { unary_operator } expression 
					int offset = startIndex + 1;
					originalStartIndex = offset;
					if (!(FindExpression(parser, lstTokens, ref offset, endIndex, out expression) && offset >= originalStartIndex) || null == expression) {
						return false;
					}
					startIndex = offset;
					break;
				case TokenKind.LeftParenthesis:
					expression = null;
					int subExpressionStartIndex = InStatement.GetNextNonCommentToken(lstTokens, startIndex + 1, startIndex + 1);
					int subExpressionEndIndex = token.MatchingParenToken - 1;
					if (-1 != subExpressionEndIndex && subExpressionStartIndex < subExpressionEndIndex) {
						if (lstTokens[subExpressionStartIndex].Kind == TokenKind.KeywordSelect) {
							// ( scalar_subquery )
							StatementSpans newSpan = parser.SegmentUtils.GetStatementSpan(subExpressionStartIndex + 1);
							if (null != newSpan) {
								newSpan.IsSubSelect = true;
							}
							parser.ParseTokenRange(newSpan, subExpressionStartIndex, subExpressionEndIndex, Instance.TextEditor.SysObjects);
							expression = new ScalarSubqueryExpression(newSpan);
						} else {
							// ( expression )
							List<Expression> expressions = new List<Expression>();
							expression = new SubExpressions(subExpressionStartIndex, subExpressionEndIndex, expressions);
							while (subExpressionStartIndex < subExpressionEndIndex) {
								Expression subExpression;
								originalStartIndex = subExpressionStartIndex;
								if (!(FindExpression(parser, lstTokens, ref subExpressionStartIndex, subExpressionEndIndex, out subExpression) && subExpressionStartIndex >= originalStartIndex) || null == subExpression) {
									return false;
								}
								expressions.Add(subExpression);
								subExpressionStartIndex++;
							}
						}
						startIndex = subExpressionEndIndex + 1;
					}
					if (null == expression) {
						return false;
					}
					break;
				default:
					expression = null;
					Function function;

					// scalar_function ?
					if (Instance.StaticData.ScalarFunctions.TryGetValue(token.Token, out function)) {
						originalStartIndex = startIndex;
						if (!(ParseFunction(parser, lstTokens, ref startIndex, ref expression, function) && startIndex >= originalStartIndex)) {
							startIndex = originalStartIndex;
							expression = null;
						}
					}

					if (null == expression) {
						// Ranking function ?
						if (Instance.StaticData.RankingFunctions.TryGetValue(token.Token, out function)) {
							originalStartIndex = startIndex;
							if (!(ParseFunction(parser, lstTokens, ref startIndex, ref expression, function) && startIndex >= originalStartIndex)) {
								startIndex = originalStartIndex;
								expression = null;
							}
						}
					}

					token = InStatement.GetNextNonCommentToken(lstTokens, ref startIndex);
					TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, startIndex + 1);
					if (null == expression && Common.IsIdentifier(token, nextToken)) {
						// [ table_name. ] column
						originalStartIndex = startIndex;
						if (!(ColumnExpression.ParseColumnName(parser, lstTokens, ref startIndex, out expression) && startIndex >= originalStartIndex) || null == expression) {
							expression = null;
							return false;
						}

						if (!ParseUserDefinedScalarFunction(ref startIndex, lstTokens, parser, ref expression))
							return false;
					}

					if (null == expression && token.Type == TokenType.Keyword) {
						return false;
					}
					break;
			}

			Expression expression1 = expression;
			if (null == expression1) {
				return false;
			}

			int operatorIndex = startIndex + 1;
			TokenInfo operatorToken = InStatement.GetPreviousNonCommentToken(lstTokens, ref operatorIndex);
			if (null == operatorToken || !(operatorToken.Token is OperatorToken)) {
				expression = expression1;
				return true;
			}

			startIndex = operatorIndex + 1;

			Expression expression2;
			originalStartIndex = startIndex;
			if (!(FindExpression(parser, lstTokens, ref startIndex, endIndex, out expression2) && startIndex >= originalStartIndex) || null == expression2) {
				expression = expression1;
				// It was not a BinaryOperatorExpression, so set startIndex to point just before operator
				startIndex = operatorIndex - 1;
				return (null != expression);
			}

			expression = new BinaryOperatorExpression(startBinaryOperator, startIndex, expression1, operatorToken, expression2);
			return true;
		}

		private static bool ParseUserDefinedScalarFunction(ref int startIndex, List<TokenInfo> lstTokens, Parser parser, ref Expression expression) {
			// Is user defined scalar functions ?
			int indexScalarFunction = startIndex + 1;
			TokenInfo scalarFunctionToken = InStatement.GetPreviousNonCommentToken(lstTokens, ref indexScalarFunction);
			if (expression is ColumnExpression && null != scalarFunctionToken && scalarFunctionToken.Kind == TokenKind.LeftParenthesis) {
				ColumnExpression columnExpression = (ColumnExpression)expression;

				foreach (ScalarFunction scalarFunction in Instance.TextEditor.SysObjectScalarFunctions) {
					if (scalarFunction.SysObject.ObjectName.Equals(columnExpression.Column.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase) && (null == columnExpression.Alias || scalarFunction.SysObject.Schema.Schema.Equals(columnExpression.Alias.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase))) {
						int originalStartIndex = startIndex;
						if (!(ParseFunction(parser, lstTokens, ref startIndex, ref expression, scalarFunction) && startIndex >= originalStartIndex)) {
							return false;
						}
						columnExpression.Column.TokenContextType = TokenContextType.SysObject;
						if (null != columnExpression.Alias) {
							columnExpression.Alias.TokenContextType = TokenContextType.SysObjectSchema;
						}
						break;
					}
				}
			}
			return true;
		}

		#endregion

		#region Parse functions (scalar, aggregate, ranking, user created)

		/// <summary>
		/// Parse a scalar / rank function
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="lstTokens"></param>
		/// <param name="startIndex"></param>
		/// <param name="expression"></param>
		/// <param name="function"></param>
		/// <returns></returns>
		private static bool ParseFunction(Parser parser, List<TokenInfo> lstTokens, ref int startIndex, ref Expression expression, Function function) {
			// Find correct datatype the method/Function returns
			ParsedDataType parsedDataType = function.GetDataType();

			if (function.IsMethod) {
				startIndex++;
				TokenInfo token = InStatement.GetNextNonCommentToken(lstTokens, ref startIndex);
				if (null == token) {
					return false;
				}
				int offset = startIndex + 1;
				int methodEndIndex = token.MatchingParenToken - 1;
				if (methodEndIndex < 0 || startIndex > methodEndIndex) {
					// Not a function
					return false;
				}

				int originalStartIndex;
				ScalarFunctionExpression functionExpression = new ScalarFunctionExpression(startIndex, methodEndIndex, parsedDataType);
				expression = functionExpression;
				while (offset <= methodEndIndex) {
					Expression subExpression;
					originalStartIndex = offset;
					if (FindExpression(parser, lstTokens, ref offset, methodEndIndex, out subExpression) && originalStartIndex < offset && null != subExpression) {
						functionExpression.AddExpression(subExpression);
					} else {
						offset = originalStartIndex;
						token = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
						if (null == token) {
							return false;
						}
						if (token.Kind == TokenKind.Comma) {
							// Ok, just continue
						} else {
							subExpression = new TokenExpression(offset, token);
							functionExpression.AddExpression(subExpression);
						}
					}
					offset++;
				}
				startIndex = methodEndIndex + 1;

				// OVER PARTITION used?
				originalStartIndex = startIndex;
				if (!(KeywordOverClause.ParseOverClause(parser, lstTokens, functionExpression, ref startIndex) && startIndex >= originalStartIndex)) {
					return false;
				}
			} else {
				expression = new ScalarFunctionExpression(startIndex, startIndex, parsedDataType);
			}
			return true;
		}

		#endregion
	}
}