﻿// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;
using Sassner.SmarterSql.Utils;
using Sassner.SmarterSql.Utils.Helpers;

namespace Sassner.SmarterSql.Parsing.Keywords {
	public class KeywordTable {
		#region Parse CREATE TABLE

		/// <summary>
		/// Parse a CREATE TABLE
		/// 		// CREATE TABLE [ database_name . [ schema_name ] . | schema_name . ] table_name 
		///        ( { <column_definition> | <computed_column_definition> } [ <table_constraint> ] [ ,...n ] ) 
		///    [ ON { partition_scheme_name ( partition_column_name ) | filegroup 
		///        | "default" } ] 
		///    [ { TEXTIMAGE_ON { filegroup | "default" } ] 
		/// [ ; ]
		/// </summary>
		/// <param name="lstTokens"></param>
		/// <param name="i"></param>
		public static void ParseCreateTable(Parser parser, List<TokenInfo> lstTokens, ref int i, List<SysObject> lstSysObjects, ref int sysObjectId) {
			List<SysObjectColumn> lstColumns = new List<SysObjectColumn>();
			int statementStartIndex = i;

			// Normal multi part name
			TokenInfo server_name;
			TokenInfo database_name;
			TokenInfo schema_name;
			TokenInfo object_name;
			int endTableIndex;
			if (parser.ParseTableOrViewName(i, out endTableIndex, out server_name, out database_name, out schema_name, out object_name)) {
				if (null == object_name) {
					return;
				}
				SysObjectSchema sysObjectSchema;
				bool sysObjectExists = Connection.SysObjectExists(server_name, database_name, schema_name, object_name, out sysObjectSchema);
				int parenLevelParen = object_name.ParenLevel;
				TokenInfo token;
				if (object_name.Token.UnqoutedImage.StartsWith("@")) {
					object_name.TokenContextType = TokenContextType.TempTable;
					// [ AS ]
					InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out token, TokenKind.KeywordAs);
					// Should be TABLE
					if (!InStatement.GetIfAllNextValidToken(lstTokens, ref i, out token, TokenKind.KeywordTable)) {
						return;
					}
					i++;
				} else {
					i = endTableIndex + 1;
				}

				token = InStatement.GetNextNonCommentToken(lstTokens, ref i);
				if (null == token || token.Kind != TokenKind.LeftParenthesis) {
					return;
				}
				if (-1 == token.MatchingParenToken || i > token.MatchingParenToken) {
					return;
				}

				int parenLevel = token.ParenLevel;
				SysObject sysObject = null;
				if (!sysObjectExists) {
					// Create a temporary table
					sysObject = SysObject.CreateTemporarySysObject(sysObjectSchema, object_name.Token.UnqoutedImage, Common.enSqlTypes.Temporary, ref sysObjectId);
				}

				if (!ParseTableColumns(ref i, lstTokens, parenLevelParen, lstColumns, sysObject)) {
					return;
				}

				if (!sysObjectExists && null != sysObject) {
					string tableName = object_name.Token.UnqoutedImage;
					foreach (SysObjectColumn column in lstColumns) {
						sysObject.AddColumn(column);
					}
					lstSysObjects.Add(sysObject);
					// Create temporary table
					TextSpan span = TextSpanHelper.CreateSpanFromTokens(lstTokens[statementStartIndex], lstTokens[i]);
					StatementSpans statementSpan = null;
					foreach (StatementSpans statementSpans in parser.SegmentUtils.StartTokenIndexesSortByInnerLevel) {
						if (statementSpans.StartIndex <= statementStartIndex && statementSpans.EndIndex >= statementStartIndex) {
							statementSpan = statementSpans;
							break;
						}
					}
					if (null == statementSpan) {
						return;
					}
					TemporaryTable tempTable = new TemporaryTable(tableName, sysObject, span, parenLevel, statementSpan, statementSpan.StartIndex, i);
					parser.TemporaryTables.Add(tempTable);
				}
			}
			return;
		}

		public static bool ParseTableColumns(ref int i, List<TokenInfo> lstTokens, int parenLevelParen, List<SysObjectColumn> lstColumns, SysObject sysObject) {
			TokenInfo token = InStatement.GetNextNonCommentToken(lstTokens, ref i);

			while (null != token && token.Kind != TokenKind.RightParenthesis) {
				string columnName = string.Empty;
				ParsedDataType parsedDataType = null;
				bool isNullable = true;
				bool isIdentity = false;

				i++;
				token = InStatement.GetNextNonCommentToken(lstTokens, ref i);
				if (null != token && token.Kind == TokenKind.Name) {
					token.TokenContextType = TokenContextType.TableColumn;
					columnName = token.Token.UnqoutedImage;

					parsedDataType = ParsedDataType.ParseDataType(lstTokens, ref i);
					if (null != parsedDataType) {
						// Ok
					} else if (token.Kind == TokenKind.KeywordAs) {
						// TODO: Handle <computed_column_definition>
					} else {
						// Unknown token
						return false;
					}
				} else if (null != token && token.Type == TokenType.Keyword) {
					// Probably a CONSTRAINT of some kind
					// TODO: Parse them?
				}

				TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
				int parenLevelColumn = (null != nextToken ? nextToken.ParenLevel : parenLevelParen);
				while (null != nextToken) {
					if ((nextToken.ParenLevel == parenLevelParen && nextToken.Kind == TokenKind.RightParenthesis) || (nextToken.ParenLevel == parenLevelColumn && nextToken.Kind == TokenKind.Comma)) {
						break;
					}

					if (nextToken.Kind == TokenKind.Name) {
						nextToken.TokenContextType = TokenContextType.Known;
					} else if (nextToken.Kind == TokenKind.KeywordIdentity) {
						isIdentity = true;
						isNullable = false;
					} else if (nextToken.Kind == TokenKind.KeywordNull) {
						TokenInfo prevToken = InStatement.GetPreviousNonCommentToken(lstTokens, i - 1);
						isNullable = !(prevToken.Kind == TokenKind.KeywordNot);
					}
					i++;
					nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
				}

				if (null != nextToken) {
					if (nextToken.Kind == TokenKind.RightParenthesis) {
						if (null != parsedDataType) {
							lstColumns.Add(new SysObjectColumn(sysObject, columnName, parsedDataType, isNullable, isIdentity));
						}

						// We are done, no more column definitions
						break;
					}
					if (nextToken.Kind != TokenKind.Comma) {
						// Expected comma, but got something else
						return false;
					}
					if (null != sysObject && null != parsedDataType) {
						lstColumns.Add(new SysObjectColumn(sysObject, columnName, parsedDataType, isNullable, isIdentity));
					}
				}
			}
			return true;
		}

		#endregion

		/// <summary>
		/// ALTER TABLE [ database_name . [ schema_name ] . | schema_name . ] table_name 
		///{ 
		///    ALTER COLUMN column_name 
		///    { 
		///        [ type_schema_name. ] type_name [ ( { precision [ , scale ] 
		///            | max | xml_schema_collection } ) ] 
		///        [ COLLATE collation_name ] 
		///        [ NULL | NOT NULL ] [ SPARSE ]
		///    | {ADD | DROP } 
		///        { ROWGUIDCOL | PERSISTED | NOT FOR REPLICATION | SPARSE }
		///    } 
		///        | [ WITH { CHECK | NOCHECK } ]
		///
		///    | ADD 
		///    { 
		///        <column_definition>
		///      | <computed_column_definition>
		///      | <table_constraint> 
		///      | <column_set_definition> 
		///    } [ ,...n ]
		///
		///    | DROP 
		///    { 
		///        [ CONSTRAINT ] constraint_name [ WITH ( <drop_clustered_constraint_option> [ ,...n ] ) ]
		///        | COLUMN column_name 
		///    } [ ,...n ] 
		///
		///    | [ WITH { CHECK | NOCHECK } ] { CHECK | NOCHECK } CONSTRAINT 
		///        { ALL | constraint_name [ ,...n ] } 
		///
		///    | { ENABLE | DISABLE } TRIGGER 
		///        { ALL | trigger_name [ ,...n ] }
		///
		///    | { ENABLE | DISABLE } CHANGE_TRACKING 
		///        [ WITH ( TRACK_COLUMNS_UPDATED = { ON | OFF } ) ]
		///
		///    | SWITCH [ PARTITION source_partition_number_expression ]
		///        TO target_table 
		///        [ PARTITION target_partition_number_expression ]
		///
		///    | SET ( FILESTREAM_ON = { partition_scheme_name | filegroup | 
		///                "default" | "NULL" } )
		///
		///    | REBUILD 
		///      [ [PARTITION = ALL]
		///        [ WITH ( <rebuild_option> [ ,...n ] ) ] 
		///      | [ PARTITION = partition_number 
		///           [ WITH ( <single_partition_rebuild_option> [ ,...n ] )]
		///        ]
		///      ]
		///
		///    | (<table_option>)
		///}
		///[ ; ]
		/// 
		/// <column_set_definition> ::= 
		///     column_set_name XML COLUMN_SET FOR ALL_SPARSE_COLUMNS
		/// 
		/// <drop_clustered_constraint_option> ::=  
		///     { 
		///         MAXDOP = max_degree_of_parallelism
		/// 
		///       | ONLINE = {ON | OFF }
		///       | MOVE TO { partition_scheme_name ( column_name ) | filegroup | "default" }
		///     }
		/// <table_option> ::=
		///     {
		///         SET ( LOCK_ESCALATION = { AUTO | TABLE | DISABLE } )
		///     }
		/// 
		/// <single_partition_rebuild__option> ::=
		/// {
		///       SORT_IN_TEMPDB = { ON | OFF }
		///     | MAXDOP = max_degree_of_parallelism
		///     | DATA_COMPRESSION = { NONE | ROW | PAGE} }
		/// }
		/// </summary>
		/// <param name="lstTokens"></param>
		/// <param name="i"></param>
		/// <param name="sysObjectId"></param>
		/// <param name="lstSysObjects"></param>
		public static void HandleAlterTable(Parser parser, List<TokenInfo> lstTokens, ref int i, ref int sysObjectId, List<SysObject> lstSysObjects) {
			i++;
			// ALTER TABLE [ database_name . [ schema_name ] . | schema_name . ] table_name {

			TokenInfo server_name;
			TokenInfo database_name;
			TokenInfo schema_name;
			TokenInfo object_name;
			int endTableIndex;
			if (parser.ParseTableOrViewName(i, out endTableIndex, out server_name, out database_name, out schema_name, out object_name)) {
				if (null == object_name) {
					return;
				}

				i++;
				TokenInfo token = InStatement.GetNextNonCommentToken(lstTokens, ref i);
				if (null == token) {
					return;
				}
				if (token.Kind == TokenKind.KeywordDrop) {
					ParseAlterTableDrop(lstTokens, ref i);
				}

			}
		}

		/// <summary>
		///    | DROP 
		///    { 
		///        [ CONSTRAINT ] constraint_name [ WITH ( <drop_clustered_constraint_option> [ ,...n ] ) ]
		///        | COLUMN column_name 
		///    } [ ,...n ] 
		/// 
		/// <drop_clustered_constraint_option> ::=  
		///     { 
		///         MAXDOP = max_degree_of_parallelism
		///       | ONLINE = {ON | OFF }
		///       | MOVE TO { partition_scheme_name ( column_name ) | filegroup | "default" }
		///     }
		/// </summary>
		/// <param name="lstTokens"></param>
		/// <param name="i"></param>
		private static void ParseAlterTableDrop(List<TokenInfo> lstTokens, ref int i) {
			TokenInfo token;
			do {
				i++;
				token = InStatement.GetNextNonCommentToken(lstTokens, ref i);
				if (null == token) {
					return;
				}

				// COLUMN column_name 
				if (token.Kind == TokenKind.KeywordColumn) {
					token = InStatement.GetNextNonCommentToken(lstTokens, ref i);
					if (null == token) {
						return;
					}
					token.TokenContextType = TokenContextType.TableColumn;
				} else {
					// [ CONSTRAINT ]
					if (token.Kind == TokenKind.KeywordConstraint) {
						i++;
					}
					token = InStatement.GetNextNonCommentToken(lstTokens, ref i);
					if (null == token) {
						return;
					}
					// constraint_name
					token.TokenContextType = TokenContextType.Constraint;

					// TODO: handle <drop_clustered_constraint_option>
					if (InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out token, TokenKind.KeywordWith, TokenKind.LeftParenthesis)) {
						if (-1 == token.MatchingParenToken || i > token.MatchingParenToken) {
							return;
						}
						int startIndex = i;
						int endIndex = token.MatchingParenToken;
						while (i < endIndex) {
							i++;
							token = InStatement.GetNextNonCommentToken(lstTokens, ref i);
							if (null == token) {
								return;
							}
							if (token.Kind == TokenKind.KeywordMaxDop) {
							} else if (token.Kind == TokenKind.KeywordOnline) {
							} else if (token.Kind == TokenKind.KeywordMove) {
							}
//								TokenKind.KeywordOnline
//								TokenKind.KeywordOn
//								TokenKind.KeywordOff
//								TokenKind.KeywordMove
//								TokenKind.KeywordTo
						}
					}
				}

				i++;
				token = InStatement.GetNextNonCommentToken(lstTokens, ref i);
			} while (null != token && token.Kind == TokenKind.Comma);
		}
	}
}
