// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.ParsingObjects;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Parsing.Keywords {
	public class KeywordTrigger {

		#region HandleEnableDisableTrigger

		// ENABLE TRIGGER { [ schema_name . ] trigger_name [ ,...n ] | ALL }
		// ON { object_name | DATABASE | ALL SERVER } [ ; ]
		//
		// DISABLE TRIGGER { [ schema . ] trigger_name [ ,...n ] | ALL }
		// ON { object_name | DATABASE | ALL SERVER } [ ; ]
		public static void HandleEnableDisableTrigger(List<TokenInfo> lstTokens, ref int i) {
			i++;
			TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
			if (null == nextToken) {
				return;
			}

			if (nextToken.Kind == TokenKind.KeywordAll) {
				// Ok
			} else if (nextToken.Type == TokenType.Identifier) {
				// [ schema_name . ] trigger_name [ ,...n ]
				int offset = i - 1;
				while (true) {
					offset++;
					nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
					offset++;
					TokenInfo nextNextToken = InStatement.GetNextNonCommentToken(lstTokens, ref offset);

					if (null != nextNextToken && nextNextToken.Kind == TokenKind.Dot) {
						offset++;
						nextNextToken = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
						if (null != nextNextToken && nextNextToken.Type == TokenType.Identifier) {
							nextToken.TokenContextType = TokenContextType.SysObjectSchema;
							nextNextToken.TokenContextType = TokenContextType.Trigger;
							i = offset;
						} else {
							return;
						}
					} else {
						nextToken.TokenContextType = TokenContextType.Trigger;
					}

					offset++;
					nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
					if (null == nextToken || nextToken.Kind != TokenKind.Comma) {
						break;
					}
					i = offset;
				}
			} else {
				return;
			}

			// ON
			if (!InStatement.GetIfAllNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordOn)) {
				return;
			}

			// { object_name | DATABASE | ALL SERVER }
			Parser parser = TextEditor.CurrentWindowData.Parser;

			if (InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordAll, TokenKind.KeywordDatabase)) {
				if (nextToken.Kind == TokenKind.KeywordAll && !InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordServer)) {
					return;
				}
			} else if (null != nextToken && nextToken.Type == TokenType.Identifier) {
				// Normal multi part name
				TokenInfo server_name;
				TokenInfo database_name;
				TokenInfo schema_name;
				TokenInfo object_name;
				int endTableIndex;
				if (!parser.ParseTableOrViewName(i, out endTableIndex, out server_name, out database_name, out schema_name, out object_name)) {
					return;
				}
			}
		}

		#endregion

		#region Parse CREATE/ALTER TRIGGER

		// --- Trigger on an INSERT, UPDATE, or DELETE statement to a table or view (DML Trigger) ---------
		//CREATE TRIGGER [ schema_name . ]trigger_name 
		// ON { table | view } 
		//[ WITH <dml_trigger_option> [ ,...n ] ]
		//{ FOR | AFTER | INSTEAD OF } 
		//{ [ INSERT ] [ , ] [ UPDATE ] [ , ] [ DELETE ] } 
		//[ WITH APPEND ] 
		//[ NOT FOR REPLICATION ] 
		//AS { sql_statement  [ ; ] [ ...n ] | EXTERNAL NAME <method specifier [ ; ] > }
		//
		// ---- Trigger on a CREATE, ALTER, DROP, GRANT, DENY, REVOKE, or UPDATE STATISTICS statement (DDL Trigger) --------
		//CREATE TRIGGER trigger_name 
		//ON { ALL SERVER | DATABASE } 
		//[ WITH <ddl_trigger_option> [ ,...n ] ]
		//{ FOR | AFTER } { event_type | event_group } [ ,...n ]
		//AS { sql_statement  [ ; ] [ ...n ] | EXTERNAL NAME < method specifier >  [ ; ] }
		//
		//<dml_trigger_option> ::=
		//    [ ENCRYPTION ]
		//    [ EXECUTE AS Clause ]
		//
		public static void HandleCreateAlterTrigger(StatementSpans currentStartSpan, List<TokenInfo> lstTokens, ref int i, ref int sysObjectId, List<SysObject> lstSysObjects) {
			Parser parser = TextEditor.CurrentWindowData.Parser;
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
					nextNextToken.TokenContextType = TokenContextType.Trigger;
					i = index;
				} else {
					return;
				}
			} else {
				nextToken.TokenContextType = TokenContextType.Trigger;
			}

			if (!InStatement.GetIfAllNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordOn)) {
				return;
			}

			// DDL Trigger
			if (InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordAll, TokenKind.KeywordDatabase)) {
				#region DDL Trigger

				bool serverScope = (nextToken.Kind == TokenKind.KeywordAll);
				if (nextToken.Kind == TokenKind.KeywordAll && !InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordServer)) {
					return;
				}

				if (!ParseDmlTrigger(ref i, lstTokens)) {
					return;
				}

				// { FOR | AFTER } 
				if (!InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordFor, TokenKind.KeywordAfter)) {
					return;
				}

				// { event_type | event_group } [ ,...n ]
				int offset = i;
				while (true) {
					offset++;
					TokenInfo token = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
					Token foundToken;
					if (serverScope) {
						if (!Instance.StaticData.DdlStatementsServerScope.TryGetValue(token.Token, out foundToken)) {
							token.TokenContextType = TokenContextType.Unknown;
							return;
						}
					} else {
						if (!Instance.StaticData.DdlStatementsDatabaseScope.TryGetValue(token.Token, out foundToken)) {
							token.TokenContextType = TokenContextType.Unknown;
							return;
						}
					}

					if (!InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.Comma)) {
						break;
					}
				}
				i = offset;

				// AS

				#endregion

			} else {
				#region DML Trigger

				int startTableIndex = i + 1;

				TokenInfo server_name;
				TokenInfo database_name;
				TokenInfo schema_name;
				TokenInfo object_name;
				int endTableIndex;
				if (!parser.ParseTableOrViewName(startTableIndex, out endTableIndex, out server_name, out database_name, out schema_name, out object_name)) {
					return;
				}
				i = endTableIndex;

				// Normal multi part name
				if (!ParseDmlTrigger(ref i, lstTokens)) {
					return;
				}

				// { FOR | AFTER | INSTEAD OF } 
				if (!InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordFor, TokenKind.KeywordAfter, TokenKind.KeywordInstead)) {
					return;
				}
				if (nextToken.Kind == TokenKind.KeywordInstead && !InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordOf)) {
					return;
				}

				// { [ INSERT ] [ , ] [ UPDATE ] [ , ] [ DELETE ] }
				while (true) {
					if (!InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordInsert, TokenKind.KeywordUpdate, TokenKind.KeywordDelete)) {
						return;
					}
					if (!InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.Comma)) {
						break;
					}
				}

				// [ WITH APPEND ]
				InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordWith, TokenKind.KeywordAppend);

				// [ NOT FOR REPLICATION ] 
				InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordNot, TokenKind.KeywordFor, TokenKind.KeywordReplication);

				// AS

				// Create insert / delete logical (conceptual) tables

				SysObject foundSysObject;
				if (!Instance.TextEditor.ActiveConnection.SysObjectExists(schema_name, object_name, out foundSysObject)) {
					return;
				}

				CreateLogicalTable(lstTokens, parser, schema_name, "inserted", string.Empty, ref sysObjectId, lstSysObjects, foundSysObject, startTableIndex, endTableIndex, currentStartSpan);
				CreateLogicalTable(lstTokens, parser, schema_name, "deleted", string.Empty, ref sysObjectId, lstSysObjects, foundSysObject, startTableIndex, endTableIndex, currentStartSpan);

				#endregion
			}
		}

		private static void CreateLogicalTable(List<TokenInfo> lstTokens, Parser parser, TokenInfo schema_name, string Table, string Alias, ref int sysObjectId, List<SysObject> lstSysObjects, SysObject foundSysObject, int startTableIndex, int endTableIndex, StatementSpans currentStartSpan) {
			const Common.enSqlTypes sqlType = Common.enSqlTypes.Temporary;
			SysObject addedSysObject = SysObject.CreateTemporarySysObject(Table, sqlType, ref sysObjectId);
			lstSysObjects.Add(addedSysObject);
			foreach (SysObjectColumn column in foundSysObject.Columns) {
				addedSysObject.AddColumn(new SysObjectColumn(addedSysObject, column.ColumnName, column.ParsedDataType, column.IsNullable, column.IsIdentity));
			}
			// Create the table source
			StatementSpans ss = parser.SegmentUtils.GetStatementSpan(startTableIndex);
			Table table = new Table(string.Empty, string.Empty, (null != schema_name ? schema_name.Token.UnqoutedImage : string.Empty), Table, Alias, addedSysObject, ss, startTableIndex, endTableIndex, true);
			TableSource addedTableSource = new TableSource(lstTokens[startTableIndex].Token, table, startTableIndex, endTableIndex);
			parser.TableSources.Add(addedTableSource);
			if (null != currentStartSpan) {
				currentStartSpan.AddTableSource(addedTableSource);
			}
		}

		#endregion

		#region ParseDmlTrigger

		// [ WITH <dml_trigger_option> [ ,...n ] ]
		//   <dml_trigger_option> ::=
		//      [ ENCRYPTION ]
		//      [ EXECUTE AS Clause ]
		private static bool ParseDmlTrigger(ref int i, List<TokenInfo> lstTokens) {
			TokenInfo nextToken;
			if (InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordWith)) {
				while (true) {
					if (!InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordEncryption, TokenKind.KeywordExec)) {
						return false;
					}
					if (nextToken.Kind == TokenKind.KeywordExec) {
						if (!InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordAs)) {
							return false;
						}
						if (!InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordCaller, TokenKind.KeywordSelf, TokenKind.KeywordOwner, TokenKind.ValueString)) {
							return false;
						}
					}

					if (!InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.Comma)) {
						if (null == nextToken) {
							return false;
						}
						break;
					}
				}
			}
			return true;
		}

		#endregion
	}
}
