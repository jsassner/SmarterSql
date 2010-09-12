// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.Parsing;
using Sassner.SmarterSql.ParsingObjects;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Tree {
	public class InStatement {
		#region GetIfAnyNextValidToken

		/// <summary>
		/// Return true if the next token is of any kind from the suppled TokenInfo objects, else false
		/// </summary>
		/// <param name="lstTokens"></param>
		/// <param name="i"></param>
		/// <param name="nextToken"></param>
		/// <param name="kindToFind"></param>
		/// <returns></returns>
		public static bool GetIfAnyNextValidToken(List<TokenInfo> lstTokens, ref int i, out TokenInfo nextToken, params TokenKind[] kindToFind) {
			nextToken = null;
			int start = i;
			i++;
			for (int j = 0; j < kindToFind.Length; j++) {
				nextToken = GetNextNonCommentToken(lstTokens, ref i);
				if (null != nextToken && nextToken.Kind == kindToFind[j]) {
					return true;
				}
			}
			i = start;
			return false;
		}

		public static bool GetIfAnyNextValidToken(List<TokenInfo> lstTokens, ref int i, out TokenInfo nextToken, params TokenType[] typeToFind) {
			nextToken = null;
			int start = i;
			i++;
			for (int j = 0; j < typeToFind.Length; j++) {
				nextToken = GetNextNonCommentToken(lstTokens, ref i);
				if (null != nextToken && nextToken.Type == typeToFind[j]) {
					return true;
				}
			}
			i = start;
			return false;
		}

		#endregion

		#region GetIfAllNextValidToken

		/// <summary>
		/// Return true if all of the supplied TokenKinds are found after each others, else false
		/// </summary>
		/// <param name="lstTokens"></param>
		/// <param name="i"></param>
		/// <param name="nextToken"></param>
		/// <param name="kindToFind"></param>
		/// <returns></returns>
		public static bool GetIfAllNextValidToken(List<TokenInfo> lstTokens, ref int i, out TokenInfo nextToken, params TokenKind[] kindToFind) {
			nextToken = null;
			int start = i;
			for (int j = 0; j < kindToFind.Length; j++) {
				i++;
				nextToken = GetNextNonCommentToken(lstTokens, ref i);
				if (null == nextToken || nextToken.Kind != kindToFind[j]) {
					i = start;
					return false;
				}
			}
			return true;
		}

		public static bool GetIfAllNextValidToken(List<TokenInfo> lstTokens, ref int i, out TokenInfo nextToken, params TokenType[] typeToFind) {
			nextToken = null;
			int start = i;
			for (int j = 0; j < typeToFind.Length; j++) {
				i++;
				nextToken = GetNextNonCommentToken(lstTokens, ref i);
				if (null == nextToken || nextToken.Type != typeToFind[j]) {
					i = start;
					return false;
				}
			}
			return true;
		}

		#endregion

		#region GetNextNonCommentToken methods

		/// <summary>
		/// Get first next token not being a comment token
		/// </summary>
		/// <param name="lstTokens"></param>
		/// <param name="currentIndex"></param>
		/// <returns></returns>
		public static TokenInfo GetNextNonCommentToken(List<TokenInfo> lstTokens, ref int currentIndex) {
			TokenInfo token = null;
			if (currentIndex >= 0) {
				while (currentIndex <= lstTokens.Count - 1) {
					token = lstTokens[currentIndex];
					if (token.Type == TokenType.Comment) {
						if (currentIndex <= lstTokens.Count - 1) {
							currentIndex++;
						} else {
							return null;
						}
					} else {
						return token;
					}
				}
			}
			return token;
		}

		/// <summary>
		/// Get first next token not being a comment token
		/// </summary>
		/// <param name="lstTokens"></param>
		/// <param name="currentIndex"></param>
		/// <returns></returns>
		public static TokenInfo GetNextNonCommentToken(List<TokenInfo> lstTokens, int currentIndex) {
			TokenInfo token = null;
			if (currentIndex >= 0) {
				while (currentIndex <= lstTokens.Count - 1) {
					token = lstTokens[currentIndex];
					if (token.Type == TokenType.Comment) {
						if (currentIndex <= lstTokens.Count - 1) {
							currentIndex++;
						} else {
							return null;
						}
					} else {
						return token;
					}
				}
			}
			return token;
		}

		/// <summary>
		/// Get first next token not being a comment token
		/// </summary>
		/// <param name="lstTokens"></param>
		/// <param name="currentIndex"></param>
		/// <param name="defaultIndex"></param>
		/// <returns></returns>
		public static int GetNextNonCommentToken(List<TokenInfo> lstTokens, int currentIndex, int defaultIndex) {
			TokenInfo token;
			if (currentIndex >= 0) {
				while (currentIndex <= lstTokens.Count - 1) {
					token = lstTokens[currentIndex];
					if (token.Type == TokenType.Comment) {
						if (currentIndex <= lstTokens.Count - 1) {
							currentIndex++;
						} else {
							return defaultIndex;
						}
					} else {
						return currentIndex;
					}
				}
			}
			return currentIndex;
		}

		#endregion

		#region GetPreviosNonCommentToken methods

		/// <summary>
		/// Get first previous token that ain't a comment token
		/// </summary>
		/// <param name="lstTokens"></param>
		/// <param name="currentIndex"></param>
		/// <returns></returns>
		public static TokenInfo GetPreviousNonCommentToken(List<TokenInfo> lstTokens, ref int currentIndex) {
			TokenInfo token = null;
			if (currentIndex <= lstTokens.Count - 1) {
				while (currentIndex >= 0) {
					token = lstTokens[currentIndex];
					if (token.Type == TokenType.Comment) {
						if (currentIndex - 1 >= 0) {
							currentIndex--;
						} else {
							return null;
						}
					} else {
						return token;
					}
				}
			}
			return token;
		}

		/// <summary>
		/// Get first previous token that ain't a comment token
		/// </summary>
		/// <param name="lstTokens"></param>
		/// <param name="currentIndex"></param>
		/// <returns></returns>
		public static TokenInfo GetPreviousNonCommentToken(List<TokenInfo> lstTokens, int currentIndex) {
			TokenInfo token = null;
			if (currentIndex <= lstTokens.Count - 1) {
				while (currentIndex >= 0) {
					token = lstTokens[currentIndex];
					if (token.Type == TokenType.Comment) {
						if (currentIndex - 1 >= 0) {
							currentIndex--;
						} else {
							return null;
						}
					} else {
						return token;
					}
				}
			}
			return token;
		}

		/// <summary>
		/// Get first previous token that ain't a comment token
		/// </summary>
		/// <param name="lstTokens"></param>
		/// <param name="currentIndex"></param>
		/// <param name="defaultIndex"></param>
		/// <returns></returns>
		public static int GetPreviousNonCommentToken(List<TokenInfo> lstTokens, int currentIndex, int defaultIndex) {
			TokenInfo token;
			if (currentIndex <= lstTokens.Count - 1) {
				while (currentIndex >= 0) {
					token = lstTokens[currentIndex];
					if (token.Type == TokenType.Comment) {
						if (currentIndex - 1 >= 0) {
							currentIndex--;
						} else {
							return defaultIndex;
						}
					} else {
						return currentIndex;
					}
				}
			}
			return currentIndex;
		}

		#endregion

		public static bool IsDatabase(int currentIndex, List<TokenInfo> tokens, Server activeServer, out Connection foundConnection) {
			foundConnection = null;

			if (currentIndex >= 0) {
				string database = tokens[currentIndex].Token.UnqoutedImage;

				foreach (Connection connection in activeServer.Connections) {
					if (connection.ActiveConnection.DatabaseName.Equals(database, StringComparison.OrdinalIgnoreCase)) {
						foundConnection = connection;
						return true;
					}
				}

				if (Server.AddNewConnection(activeServer, database, out foundConnection)) {
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Find out if the token at the current index is a schema
		/// </summary>
		/// <param name="currentIndex"></param>
		/// <param name="activeServer"></param>
		/// <param name="foundSchema"></param>
		/// <param name="foundConnection"></param>
		/// <param name="tokens"></param>
		/// <param name="activeConnection"></param>
		/// <returns></returns>
		public static bool IsSchema(int currentIndex, List<TokenInfo> tokens, Connection activeConnection, Server activeServer, out SysObjectSchema foundSchema, out Connection foundConnection) {
			if (currentIndex >= 0) {
				string database = activeConnection.ActiveConnection.DatabaseName;
				string schema = tokens[currentIndex].Token.UnqoutedImage;

				if (schema.Equals(".")) {
					schema = activeServer.GetDefaultSchema();
					if (currentIndex - 1 >= 0 && tokens[currentIndex - 1].Kind == TokenKind.Name) {
						database = tokens[currentIndex - 1].Token.UnqoutedImage;
					}
				} else {
					if (currentIndex - 2 >= 0 && tokens[currentIndex - 1].Kind == TokenKind.Dot && tokens[currentIndex - 2].Kind == TokenKind.Name) {
						database = tokens[currentIndex - 2].Token.UnqoutedImage;
					}
				}

				return IsSchema(activeServer, activeConnection, schema, database, out foundSchema, out foundConnection);
			}
			foundSchema = null;
			foundConnection = null;
			return false;
		}

		/// <summary>
		/// See if we are a candidate for auto-generating column_list
		/// INSERT 
		///		[ TOP ( expression ) [ PERCENT ] ] 
		///		[ INTO ] 
		///		{ <object> | rowset_function_limited [ WITH ( <Table_Hint_Limited> [ ...n ] ) ] }
		///		{
		///		[ ( column_list ) ] 
		/// </summary>
		/// <param name="lstTokens"></param>
		/// <param name="currentIndex"></param>
		/// <returns></returns>
		public static bool IsCandidateForInsertColumns(List<TokenInfo> lstTokens, int currentIndex, out TableSource foundTableSource) {
			foundTableSource = null;

			TokenInfo token = GetPreviousNonCommentToken(lstTokens, ref currentIndex);
			if (null != token && token.Kind == TokenKind.LeftParenthesis) {
				currentIndex--;
				token = GetPreviousNonCommentToken(lstTokens, ref currentIndex);
				if (null == token) {
					return false;
				}

				// Handle: [ WITH ( <Table_Hint_Limited> [ ...n ] ) ]
				if (token.Kind == TokenKind.RightParenthesis && -1 != token.MatchingParenToken && currentIndex > token.MatchingParenToken) {
					currentIndex = token.MatchingParenToken - 1;
					token = GetPreviousNonCommentToken(lstTokens, ref currentIndex);
					if (null == token || token.Kind != TokenKind.KeywordWith) {
						return false;
					}
					currentIndex--;
					token = GetPreviousNonCommentToken(lstTokens, ref currentIndex);
					if (null == token) {
						return false;
					}
				}

				// Handle: <object>
				token = null;
				foreach (TableSource tableSource in TextEditor.CurrentWindowData.Parser.TableSources) {
					if (tableSource.Table.EndIndex == currentIndex) {
						foundTableSource = tableSource;
						currentIndex = tableSource.Table.StartIndex - 1;
						token = GetPreviousNonCommentToken(lstTokens, ref currentIndex);
						break;
					}
				}
				if (null != token) {
					// Handle: [ INTO ]
					if (token.Kind == TokenKind.KeywordInto) {
						currentIndex--;
						token = GetPreviousNonCommentToken(lstTokens, ref currentIndex);
						if (null == token) {
							return false;
						}
					}

					// Handle: [ TOP ( expression ) [ PERCENT ] ]
					if (token.Kind == TokenKind.KeywordPercent) {
						currentIndex--;
						token = GetPreviousNonCommentToken(lstTokens, ref currentIndex);
						if (null == token) {
							return false;
						}
					}
					if (token.Kind == TokenKind.RightParenthesis && -1 != token.MatchingParenToken && currentIndex > token.MatchingParenToken) {
						currentIndex = token.MatchingParenToken - 1;
						token = GetPreviousNonCommentToken(lstTokens, ref currentIndex);
						if (null == token || token.Kind != TokenKind.KeywordTop) {
							return false;
						}
						currentIndex--;
						token = GetPreviousNonCommentToken(lstTokens, ref currentIndex);
						if (null == token) {
							return false;
						}
					}

					// Handle: INSERT
					if (token.Kind == TokenKind.KeywordInsert) {
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Is the supplied schemaName a schema in database databaseName?
		/// </summary>
		/// <param name="objActiveServer"></param>
		/// <param name="objActiveConnection"></param>
		/// <param name="schemaName"></param>
		/// <param name="databaseName"></param>
		/// <param name="foundSchema"></param>
		/// <param name="foundConnection"></param>
		/// <returns></returns>
		public static bool IsSchema(Server objActiveServer, Connection objActiveConnection, string schemaName, string databaseName, out SysObjectSchema foundSchema, out Connection foundConnection) {
			foundSchema = null;
			foundConnection = null;

			if (null != objActiveConnection) {
				if (objActiveConnection.ActiveConnection.DatabaseName.Equals(databaseName, StringComparison.OrdinalIgnoreCase)) {
					if (IsSchema(objActiveConnection, schemaName, databaseName, out foundSchema)) {
						foundConnection = objActiveConnection;
						return true;
					}
				} else {
					foreach (Connection connection in objActiveServer.Connections) {
						if (connection.ActiveConnection.DatabaseName.Equals(databaseName, StringComparison.OrdinalIgnoreCase)) {
							foundConnection = connection;
							return IsSchema(connection, schemaName, databaseName, out foundSchema);
						}
					}

					if (Server.AddNewConnection(objActiveServer, databaseName, out objActiveConnection)) {
						foundConnection = objActiveConnection;
						return IsSchema(objActiveConnection, schemaName, databaseName, out foundSchema);
					}
					return false;
				}
			}

			return false;
		}

		/// <summary>
		/// Is the supplied schemaName a schema in database databaseName?
		/// </summary>
		/// <param name="objActiveConnection"></param>
		/// <param name="schemaName"></param>
		/// <param name="databaseName"></param>
		/// <param name="foundSchema"></param>
		/// <returns></returns>
		private static bool IsSchema(Connection objActiveConnection, string schemaName, string databaseName, out SysObjectSchema foundSchema) {
			foundSchema = null;
			if (null != objActiveConnection) {
				List<SysObjectSchema> sysObjectSchemas = objActiveConnection.GetSysObjectSchemas();
				foreach (SysObjectSchema schema in sysObjectSchemas) {
					if (null != schema.Schema && schemaName.Equals(schema.Schema, StringComparison.OrdinalIgnoreCase) && schema.Connection.ActiveConnection.DatabaseName.Equals(databaseName, StringComparison.OrdinalIgnoreCase)) {
						foundSchema = schema;
						return true;
					}
				}
			}
			return false;
		}

		//DECLARE 
		//     {{ @local_variable [AS] data_type }
		//    | { @cursor_variable_name CURSOR }
		//    | { @table_variable_name [AS] < table_type_definition > }
		//     } [ ,...n]
		public static bool IsInStatementDeclare(List<TokenInfo> lstTokens, int currentIndex) {
			int parenLevel = -1;

			while (currentIndex >= 0) {
				TokenInfo token = lstTokens[currentIndex];
				if (-1 == parenLevel) {
					parenLevel = token.ParenLevel;
				}
				if (token.ParenLevel == parenLevel) {
					if (token.Kind == TokenKind.KeywordDeclare) {
						return true;
					}
					if (!(token.Type == TokenType.Comment || token.Type == TokenType.DataType || token.Kind == TokenKind.Comma || token.Kind == TokenKind.RightParenthesis || token.Type == TokenType.Identifier || token.Kind == TokenKind.KeywordAs || token.Kind == TokenKind.KeywordCursor || token.Kind == TokenKind.Variable)) {
						return false;
					}
				}
				currentIndex--;
			}
			return false;
		}

		/// <summary>
		/// Find out if this token is an table alias. Look for AS keyword and/or table name / derived table
		/// </summary>
		/// <param name="lstTokens"></param>
		/// <param name="currentIndex"></param>
		/// <param name="table"></param>
		/// <returns></returns>
		public static bool IsTableAlias(List<TokenInfo> lstTokens, int currentIndex, string table) {
			if (currentIndex - 1 >= 0) {
				currentIndex--;
				TokenInfo token = GetPreviousNonCommentToken(lstTokens, ref currentIndex);

				if (token.Kind == TokenKind.KeywordAs) {
					if (currentIndex - 1 >= 0) {
						currentIndex--;
						token = GetPreviousNonCommentToken(lstTokens, ref currentIndex);
					}
				}
				if (token.Type == TokenType.Identifier) {
					if (token.Token.UnqoutedImage.Equals(table, StringComparison.OrdinalIgnoreCase)) {
						return true;
					}
				} else if (token.Kind == TokenKind.RightParenthesis) {
					return true;
				}
			}
			return false;
		}

		//	DELETE 
		//		[ TOP ( expression ) [ PERCENT ] ] 
		//		[ FROM ] 
		//  .....
		public static bool IsInStatementDelete(List<TokenInfo> lstTokens, int currentIndex) {
			int parenLevel = -1;

			while (currentIndex >= 0) {
				TokenInfo token = lstTokens[currentIndex];
				if (-1 == parenLevel) {
					parenLevel = token.ParenLevel;
				}
				if (token.ParenLevel == parenLevel) {
					if (token.Kind == TokenKind.KeywordDelete) {
						return true;
					}
					if (!(token.Type == TokenType.Comment || token.Kind == TokenKind.KeywordFrom || token.Kind == TokenKind.KeywordPercent || token.Kind == TokenKind.KeywordTop || token.Kind == TokenKind.RightParenthesis)) {
						return false;
					}
				}
				currentIndex--;
			}
			return false;
		}

		//[ { EXEC | EXECUTE } ]
		//    { 
		//      [ @return_status = ]
		//      { module_name [ ;number ] | @module_name_var } 
		//
		// NOTE: Only part of Execute statement check is validated
		public static bool IsInStatementExecute(List<TokenInfo> lstTokens, int currentIndex) {
			while (currentIndex >= 0) {
				TokenInfo token = lstTokens[currentIndex];
				if (token.Kind == TokenKind.KeywordExec) {
					return true;
				}
				if (!(token.Type == TokenType.Comment || token.Kind == TokenKind.Assign || token.Kind == TokenKind.Variable)) {
					return false;
				}
				currentIndex--;
			}
			return false;
		}

		/// <summary>
		/// Is this a TableSample statement? If so return the end index of the last token
		///    TABLESAMPLE [SYSTEM] ( sample_number [ PERCENT | ROWS ] ) [ REPEATABLE ( repeat_seed ) ]
		/// </summary>
		/// <param name="tokens"></param>
		/// <param name="currentIndex"></param>
		/// <param name="statementEndIndex"></param>
		/// <returns></returns>
		public static bool IsInStatementTableSample(List<TokenInfo> tokens, int currentIndex, out int statementEndIndex) {
			statementEndIndex = -1;

			TokenInfo token = GetNextNonCommentToken(tokens, ref currentIndex);
			if (null != token && token.Kind == TokenKind.KeywordTableSample) {
				currentIndex++;
				token = GetNextNonCommentToken(tokens, ref currentIndex);
				if (null != token && token.Kind == TokenKind.KeywordSystem) {
					currentIndex++;
					token = GetNextNonCommentToken(tokens, ref currentIndex);
				}
				if (null != token) {
					int matchingParenToken = token.MatchingParenToken;

					// ... ( sample_number [ PERCENT | ROWS ] ) [ REPEATABLE ( repeat_seed ) ]
					if (token.Kind == TokenKind.LeftParenthesis && -1 != matchingParenToken && currentIndex < matchingParenToken) {
						currentIndex++;
						token = GetNextNonCommentToken(tokens, ref currentIndex);
						if (null != token && token.Kind == TokenKind.ValueNumber) {
							currentIndex++;
							token = GetNextNonCommentToken(tokens, ref currentIndex);
							if (null != token && (token.Kind == TokenKind.KeywordPercent || token.Kind == TokenKind.KeywordRows)) {
								currentIndex++;
								token = GetNextNonCommentToken(tokens, ref currentIndex);
							}
							if (null != token) {
								currentIndex = matchingParenToken + 1;
								token = GetNextNonCommentToken(tokens, ref currentIndex);
								if (null != token && token.Kind == TokenKind.KeywordRepeatable) {
									currentIndex++;
									token = GetNextNonCommentToken(tokens, ref currentIndex);
									if (null != token) {
										matchingParenToken = token.MatchingParenToken;
										if (token.Kind == TokenKind.LeftParenthesis && -1 != matchingParenToken && currentIndex < matchingParenToken) {
											currentIndex++;
											token = GetNextNonCommentToken(tokens, ref currentIndex);
											if (null != token && token.Kind == TokenKind.ValueNumber) {
												statementEndIndex = matchingParenToken;
												return true;
											}
										}
									}
								} else {
									statementEndIndex = currentIndex;
									return true;
								}
							}
						}
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Is this a WITH(tablehint) statement? If so return the end index of the last token
		///   WITH ( < table_hint > [ [ , ]...n ] )
		/// </summary>
		/// <param name="tokens"></param>
		/// <param name="currentIndex"></param>
		/// <param name="staticData"></param>
		/// <param name="statementEndIndex"></param>
		/// <returns></returns>
		public static bool IsInStatementWithTableHint(List<TokenInfo> tokens, int currentIndex, StaticData staticData, out int statementEndIndex) {
			statementEndIndex = -1;

			TokenInfo token = GetNextNonCommentToken(tokens, ref currentIndex);
			if (null != token && token.Kind == TokenKind.KeywordWith) {
				currentIndex++;
				token = GetNextNonCommentToken(tokens, ref currentIndex);
				if (null != token) {
					int matchingParenToken = token.MatchingParenToken;
					if (token.Kind == TokenKind.LeftParenthesis && -1 != matchingParenToken && currentIndex < matchingParenToken) {
						int endIndex = matchingParenToken;

						currentIndex++;
						// Make a note if we find any valid table hints
						bool foundTableHint = false;
						while (currentIndex < endIndex) {
							token = GetNextNonCommentToken(tokens, ref currentIndex);
							if (null != token) {
								if (token.Kind == TokenKind.Comma || token.Kind == TokenKind.LeftParenthesis || token.Kind == TokenKind.RightParenthesis || token.Kind == TokenKind.ValueNumber) {
									// Do nothing
								} else {
									bool foundLocalTableHint = false;
									foreach (Token tableHint in staticData.TableHints) {
										if (token.Kind == tableHint.Kind) {
											foundTableHint = true;
											foundLocalTableHint = true;
											break;
										}
									}
									if (!foundLocalTableHint) {
										return false;
									}
								}
							} else {
								return false;
							}
							currentIndex++;
						}

						statementEndIndex = endIndex;
						return foundTableHint;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Find the start of the Join statement.
		///	  <table_source> <join_type> <table_source> ON <search_condition>
		///     <join_type> ::= [ { CROSS | { INNER | { { LEFT | RIGHT | FULL } [ OUTER ] } } [ <join_hint> ] } ] JOIN
		///     <join_hint> ::= 
		///        { LOOP | HASH | MERGE | REMOTE }
		/// </summary>
		/// <param name="tokens"></param>
		/// <param name="currentIndex">currentIndex points at the JOIN token</param>
		/// <param name="startIndex"></param>
		/// <returns></returns>
		public static bool FindStartOfJoin(List<TokenInfo> tokens, int currentIndex, out int startIndex) {
			startIndex = -1;

			TokenInfo token = GetPreviousNonCommentToken(tokens, ref currentIndex);
			if (null != token && token.Kind == TokenKind.KeywordJoin) {
				bool joinHintFound = false;
				currentIndex--;
				token = GetPreviousNonCommentToken(tokens, ref currentIndex);
				if (null != token) {
					if (token.Kind == TokenKind.KeywordLoop || token.Kind == TokenKind.KeywordHash || token.Kind == TokenKind.KeywordMerge || token.Kind == TokenKind.KeywordRemote) {
						joinHintFound = true;
						currentIndex--;
						token = GetPreviousNonCommentToken(tokens, ref currentIndex);
					} else if (token.Kind == TokenKind.KeywordInner || token.Kind == TokenKind.KeywordCross) {
						startIndex = currentIndex;
						return true;
					}

					if (null != token) {
						bool requireLeftRightFull = false;
						if (token.Kind == TokenKind.KeywordOuter) {
							requireLeftRightFull = true;
							currentIndex--;
							token = GetPreviousNonCommentToken(tokens, ref currentIndex);
						}

						if (null != token) {
							if (token.Kind == TokenKind.KeywordLeft || token.Kind == TokenKind.KeywordRight || token.Kind == TokenKind.KeywordFull) {
								startIndex = currentIndex;
								return true;
							}
						}
						// Since INNER and OUTER keywords are optional, a join hint may be the start of the Join statement
						if (joinHintFound) {
							currentIndex++;
							GetNextNonCommentToken(tokens, ref currentIndex);
							startIndex = currentIndex;
							return true;
						}
						if (!requireLeftRightFull) {
							startIndex = currentIndex;
							return true;
						}
					}
				}
			}

			return false;
		}
	}
}