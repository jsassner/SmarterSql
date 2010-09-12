// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;

namespace Sassner.SmarterSql.Parsing.Keywords {
	public class KeywordProcedure {
		#region Parse CREATE/ALTER PROCEDURE

		/// <summary>
		/// { CREATE | ALTER } { PROC | PROCEDURE } [schema_name.] procedure_name [ ; number ] 
		///    [ { @parameter [ type_schema_name. ] data_type }  [ VARYING ] [ = default ] [ OUT | OUTPUT ]
		///    ] [ ,...n ] 
		///    [ WITH <procedure_option> [ ,...n ] ]
		///    [ FOR REPLICATION ] 
		/// AS { <sql_statement> [;][ ...n ] | <method_specifier> }
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="lstTokens"></param>
		/// <param name="i"></param>
		public static void HandleCreateAlterProcedure(Parser parser, List<TokenInfo> lstTokens, ref int i) {
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
					nextNextToken.TokenContextType = TokenContextType.Procedure;
					i = index;
				} else {
					return;
				}
			} else {
				nextToken.TokenContextType = TokenContextType.Procedure;
			}

			InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.LeftParenthesis);

			while (true) {
				// @parameter
				if (!InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.Variable)) {
					return;
				}
				TokenInfo tokenVariable = nextToken;
				nextToken.TokenContextType = TokenContextType.Variable;
				int variableIndex = i;

				// data_type
				ParsedDataType parsedDataType = ParsedDataType.ParseDataType(lstTokens, ref i);
				if (null == parsedDataType) {
					return;
				}
				parser.DeclaredLocalVariables.Add(new LocalVariable(tokenVariable.Token.UnqoutedImage, variableIndex, parsedDataType));

				// [ VARYING ]
				InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordVarying);
				// [ = default ]
				if (InStatement.GetIfAllNextValidToken(lstTokens, ref i, out nextToken, TokenKind.Assign)) {
					if (!InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.ValueNumber, TokenKind.ValueString, TokenKind.KeywordNull)) {
						return;
					}
				}
				// [ OUT | OUTPUT ]
				InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordOut, TokenKind.KeywordOutput);

				if (!InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.Comma)) {
					if (null == nextToken) {
						return;
					} else {
						break;
					}
				}
			}

			InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.RightParenthesis);

			// [ WITH <procedure_option> [ ,...n ] ]
			//<procedure_option> ::= 
			//    [ ENCRYPTION ]
			//    [ RECOMPILE ]
			//    [ EXECUTE_AS_Clause ]
			//
			//<EXECUTE_AS_Clause> ::= 
			//    { EXEC | EXECUTE } AS { CALLER | SELF | OWNER | 'user_name' } 
			if (InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordWith)) {
				while (true) {
					if (!InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordEncryption, TokenKind.KeywordRecompile, TokenKind.KeywordExec)) {
						return;
					}
					if (nextToken.Kind == TokenKind.KeywordExec) {
						if (!InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordAs)) {
							return;
						}
						if (!InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordCaller, TokenKind.KeywordSelf, TokenKind.KeywordOwner, TokenKind.ValueString)) {
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
			}

			// [ FOR REPLICATION ]
			InStatement.GetIfAllNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordFor, TokenKind.KeywordReplication);
			// AS
			InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordAs);
		}

		#endregion
	}
}