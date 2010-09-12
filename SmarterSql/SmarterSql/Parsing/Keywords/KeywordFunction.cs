// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;

namespace Sassner.SmarterSql.Parsing.Keywords {
	public class KeywordFunction {
		#region Parse CREATE/ALTER FUNCTION

		/// <summary>
		/// Parse CREATE/ALTER FUNCTION
		/// CREATE FUNCTION [ schema_name. ] function_name 
		/// ( [ { @parameter_name [ AS ][ type_schema_name. ] parameter_data_type 
		///     [ = default ] } 
		///     [ ,...n ]
		///   ]
		/// )
		/// RETURNS return_data_type
		///     [ WITH <function_option> [ ,...n ] ]
		///     [ AS ]
		///     BEGIN 
		///                function_body 
		///         RETURN scalar_expression
		///     END
		/// [ ; ]
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="lstTokens"></param>
		/// <param name="i"></param>
		public static void HandleCreateAlterFunction(Parser parser, List<TokenInfo> lstTokens, ref int i) {
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
					nextNextToken.TokenContextType = TokenContextType.Function;
					i = index;
				} else {
					return;
				}
			} else {
				nextToken.TokenContextType = TokenContextType.Function;
			}

			//( [ { @parameter_name [ AS ][ type_schema_name. ] parameter_data_type 
			//    [ = default ] } 
			//    [ ,...n ]
			//  ]
			//)
			if (!InStatement.GetIfAllNextValidToken(lstTokens, ref i, out nextToken, TokenKind.LeftParenthesis)) {
				return;
			}
			while (true) {
				// @parameter
				if (!InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.Variable)) {
					return;
				}
				TokenInfo tokenVariable = nextToken;
				nextToken.TokenContextType = TokenContextType.Variable;
				int variableIndex = i;

				// [ AS ]
				InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordAs);

				// data_type
				ParsedDataType parsedDataType = ParsedDataType.ParseDataType(lstTokens, ref i);
				if (null == parsedDataType) {
					return;
				}
				parser.DeclaredLocalVariables.Add(new LocalVariable(tokenVariable.Token.UnqoutedImage, variableIndex, parsedDataType));

				// [ = default ]
				if (InStatement.GetIfAllNextValidToken(lstTokens, ref i, out nextToken, TokenKind.Assign)) {
					if (!InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.ValueNumber, TokenKind.ValueString, TokenKind.KeywordNull)) {
						return;
					}
				}

				if (!InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.Comma)) {
					if (null == nextToken) {
						return;
					} else {
						break;
					}
				}
			}
			if (!InStatement.GetIfAllNextValidToken(lstTokens, ref i, out nextToken, TokenKind.RightParenthesis)) {
				return;
			}

			//RETURNS return_data_type
			//RETURNS @return_variable TABLE < table_type_definition >
			if (!InStatement.GetIfAllNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordReturns)) {
				return;
			}
			i++;
			nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
			if (null == nextToken) {
				return;
			}
			if (nextToken.Type == TokenType.DataType) {
				; // Ok
			} else if (nextToken.Kind == TokenKind.Variable) {
				int parenLevelParen = nextToken.ParenLevel;
				nextToken.TokenContextType = TokenContextType.Variable;
				ParsedDataType parsedDataType = new ParsedDataType(Tokens.kwTableToken, "");
				parser.DeclaredLocalVariables.Add(new LocalVariable(nextToken.Token.UnqoutedImage, i, parsedDataType));
				if (!InStatement.GetIfAllNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordTable, TokenKind.LeftParenthesis)) {
					return;
				}
				if (-1 != nextToken.MatchingParenToken && i < nextToken.MatchingParenToken) {
					List<SysObjectColumn> lstColumns = new List<SysObjectColumn>();
					SysObject sysObject = null;
					if (!KeywordTable.ParseTableColumns(ref i, lstTokens, parenLevelParen, lstColumns, sysObject)) {
						return;
					}

					// TODO: Parse all tokens in this segment (table columns)
					i = nextToken.MatchingParenToken;
				}
			} else {
				return;
			}

			// [ WITH <function_option> [ ,...n ] ]
			// <function_option>::= 
			//{
			//    [ ENCRYPTION ]
			//  | [ SCHEMABINDING ]
			//  | [ RETURNS NULL ON NULL INPUT | CALLED ON NULL INPUT ]
			//  | [ EXECUTE_AS_Clause ]
			//}
			//<EXECUTE_AS_Clause> ::= 
			//    { EXEC | EXECUTE } AS { CALLER | SELF | OWNER | 'user_name' } 
			if (InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordWith)) {
				while (true) {
					if (!InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordEncryption, TokenKind.KeywordSchemabinding, TokenKind.KeywordReturns, TokenKind.KeywordCalled, TokenKind.KeywordExec)) {
						return;
					}
					if (nextToken.Kind == TokenKind.KeywordExec) {
						if (!InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordAs)) {
							return;
						}
						if (!InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordCaller, TokenKind.KeywordSelf, TokenKind.KeywordOwner, TokenKind.ValueString)) {
							return;
						}
					} else if (nextToken.Kind == TokenKind.KeywordReturns) {
						InStatement.GetIfAllNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordNull, TokenKind.KeywordOn, TokenKind.KeywordNull, TokenKind.KeywordInput);
					} else if (nextToken.Kind == TokenKind.KeywordCalled) {
						InStatement.GetIfAllNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordOn, TokenKind.KeywordNull, TokenKind.KeywordInput);
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

			// [ AS ]
			InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordAs);

			// EXTERNAL NAME <method_specifier>
			//<method_specifier>::=
			//    assembly_name.class_name.method_name
			//
			if (InStatement.GetIfAllNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordExternal, TokenKind.KeywordName)) {
				int startIndex = i;
				if (InStatement.GetIfAllNextValidToken(lstTokens, ref i, out nextToken, TokenKind.ValueString, TokenKind.Dot, TokenKind.ValueString, TokenKind.Dot, TokenKind.ValueString)) {
					while (startIndex < i) {
						if (lstTokens[startIndex].Kind == TokenKind.ValueString) {
							lstTokens[startIndex].TokenContextType = TokenContextType.Known;
						}
						startIndex++;
					}
				}
			}
		}

		#endregion
	}
}