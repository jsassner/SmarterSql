// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Parsing.Keywords {
	public class KeywordExecute {
		#region Parse EXECUTE sproc or function

		/// <summary>
		/// [ { EXEC | EXECUTE } ]
		///    { 
		///      [ @return_status = ]
		///      { module_name [ ;number ] | @module_name_var } 
		///        [ [ @parameter = ] { value 
		///                           | @variable [ OUTPUT ] 
		///                           | [ DEFAULT ] 
		///                           }
		///        ]
		///      [ ,...n ]
		///      [ WITH RECOMPILE ]
		///    }
		/// [;]
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="lstTokens"></param>
		/// <param name="i"></param>
		public static void ParseExecuteSprocOrFunction(Parser parser, List<TokenInfo> lstTokens, ref int i) {
			StatementSpans span = parser.SegmentUtils.GetNextStatementSpanStart(i);
			int endIndex = (null == span ? lstTokens.Count - 1 : span.EndIndex);

			i++;
			TokenInfo token = InStatement.GetNextNonCommentToken(lstTokens, ref i);
			int offset = i + 1;
			TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
			if (null != token && token.Kind == TokenKind.Variable && null != nextToken && nextToken.Kind == TokenKind.Assign) {
				// [ @return_status = ]
				parser.CalledLocalVariables.Add(new LocalVariable(token.Token.UnqoutedImage, i));
				token.TokenContextType = TokenContextType.Variable;

				i = InStatement.GetNextNonCommentToken(lstTokens, offset + 1, offset + 1);
				token = InStatement.GetNextNonCommentToken(lstTokens, ref i);
			}

			if (null == token) {
				return;
			}
			if (token.Kind == TokenKind.Variable) {
				// @module_name_var
				parser.CalledLocalVariables.Add(new LocalVariable(token.Token.UnqoutedImage, i));
				token.TokenContextType = TokenContextType.Variable;
			} else {
				// module_name
				TokenInfo server_name;
				TokenInfo database_name;
				TokenInfo schema_name;
				TokenInfo object_name;
				int endTableIndex;
				if (!parser.ParseTableOrViewName(i, out endTableIndex, out server_name, out database_name, out schema_name, out object_name)) {
					return;
				}
				object_name.TokenContextType = TokenContextType.Procedure;
				i = endTableIndex;
			}

			// Get parameters
			offset = i + 1;
			token = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
			while (null != token && offset <= endIndex) {
				int offset2 = offset + 1;
				nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref offset2);
				if (token.Kind == TokenKind.Variable) {
					if (null != nextToken && nextToken.Kind == TokenKind.Assign) {
						token.TokenContextType = TokenContextType.Parameter;
						offset = offset2 + 1;
						token = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
						if (null == token) {
							break;
						}
					}
				}

				if (token.Kind == TokenKind.ValueString || token.Kind == TokenKind.ValueNumber) {
					// Ok
				} else if (token.Kind == TokenKind.KeywordNull) {
					// Ok
				} else if (token.Kind == TokenKind.KeywordDefault) {
					// Ok
				} else if (token.Kind == TokenKind.Variable) {
					parser.CalledLocalVariables.Add(new LocalVariable(token.Token.UnqoutedImage, offset));
					token.TokenContextType = TokenContextType.Variable;
					offset2 = offset + 1;
					token = InStatement.GetNextNonCommentToken(lstTokens, ref offset2);
					if (null != token && token.Kind == TokenKind.KeywordOutput) {
						offset = offset2;
						// Ok
					}
				} else {
					// Error
					return;
				}

				// Abort if not a comma
				offset++;
				token = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
				if (null == token) {
					break;
				}
				if (token.Kind == TokenKind.KeywordWith) {
					//					offset++;
					break;
				}
				if (token.Kind != TokenKind.Comma) {
					break;
				}
				offset++;
				token = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
			}

			i = endIndex;
		}

		#endregion

		#region Parse EXECUTE string

		public static void ParseExecuteString(Parser parser, List<TokenInfo> lstTokens, ref int i) {
			i++;
			InStatement.GetNextNonCommentToken(lstTokens, ref i);
			i++;
			int offset = i;
			TokenInfo token = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
			// Find all string parameters
			while (null != token && token.Kind != TokenKind.RightParenthesis) {
				if (token.Kind == TokenKind.Variable) {
					token.TokenContextType = TokenContextType.Variable;
					parser.CalledLocalVariables.Add(new LocalVariable(token.Token.UnqoutedImage, offset));
				} else if (token.Kind == TokenKind.ValueString || token.Kind == TokenKind.ValueNumber) {
					// Ok, value
				} else if (token.Kind == TokenKind.Add) {
					// Ok, +
				} else if (token.Kind == TokenKind.Comma) {
					// Ok, comma
				} else if (token.Kind == TokenKind.RightParenthesis) {
					break;
				}
				offset++;
				token = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
			}
			i = offset;
			offset++;
			token = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
			if (null == token || !(token.Kind == TokenKind.KeywordAs || token.Kind == TokenKind.KeywordAt)) {
				return;
			}

			if (token.Kind == TokenKind.KeywordAs) {
				offset++;
				token = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
				if (null != token && (token.Kind == TokenKind.KeywordLogin || token.Kind == TokenKind.KeywordUser)) {
					offset++;
					token = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
					if (null == token || token.Kind != TokenKind.Assign) {
						return;
					}
					offset++;
					token = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
					if (null == token || token.Kind != TokenKind.ValueString) {
						return;
					}
					token.TokenContextType = TokenContextType.Known;
				} else {
					return;
				}
			}

			if (token.Kind == TokenKind.KeywordAt) {
				offset++;
				token = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
				token.TokenContextType = TokenContextType.LinkedServer;
			}
			i = offset;
		}

		#endregion
	}
}