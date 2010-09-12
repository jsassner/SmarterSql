using System;
using System.Collections.Generic;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Parsing.Keywords {
	public class KeywordOpenRowset {
		/// <summary>
		/// OPENROWSET 
		///    ( { 'provider_name' ,
		///          { 'datasource' ; 'user_id' ; 'password' 
		///          | 'provider_string'
		///          } 
		///        , { [ catalog. ] [ schema. ] object 
		///          | 'query' 
		///          } 
		///      | BULK 'data_file' , 
		///           { FORMATFILE = 'format_file_path' [ <bulk_options> ]
		///           | SINGLE_BLOB | SINGLE_CLOB | SINGLE_NCLOB
		///	          }
		///      } ) 
		///    
		///    <bulk_options> ::=
		///       [ , CODEPAGE = { 'ACP' | 'OEM' | 'RAW' | 'code_page' } ] 
		///       [ , ERRORFILE = 'file_name' ]
		///       [ , FIRSTROW = first_row ] 
		///       [ , LASTROW = last_row ] 
		///       [ , MAXERRORS = maximum_errors ] 
		///       [ , ROWS_PER_BATCH = rows_per_batch ] 
		///</summary>
		///<returns></returns>
		public static bool ParseOpenRowSet(Parser parser, StatementSpans currentStartSpan, List<TokenInfo> lstTokens, ref int i, List<SysObject> lstSysObjects, int startStatementIndex, ref int sysObjectId, out SysObject addedSysObject, out TableSource addedTableSource, TokenInfo startToken) {
			List<SysObjectColumn> lstSysObjectColumn = new List<SysObjectColumn>();
			int offset = i;
			TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, offset);
			if (null != nextToken && nextToken.Kind == TokenKind.KeywordOpenRowset) {
				int startRowsetIndex = offset;
				offset++;
				nextToken = InStatement.GetNextNonCommentToken(lstTokens, offset);
				if (null != nextToken && nextToken.Kind == TokenKind.LeftParenthesis && -1 != nextToken.MatchingParenToken && offset < nextToken.MatchingParenToken) {
					offset++;
					// { 'provider_name' ,
					//     { 'datasource' ; 'user_id' ; 'password' 
					//     | 'provider_string'
					//     } 
					//   , { [ catalog. ] [ schema. ] object 
					//     | 'query' 
					//     } 

					nextToken = InStatement.GetNextNonCommentToken(lstTokens, offset);
					if (null != nextToken) {
						if (nextToken.Kind == TokenKind.ValueString) {
							// 'provider_name'
							nextToken.TokenContextType = TokenContextType.Known;
							if (!InStatement.GetIfAllNextValidToken(lstTokens, ref offset, out nextToken, TokenKind.Comma)) {
								addedSysObject = null;
								addedTableSource = null;
								return false;
							}

							//     { 'datasource' ; 'user_id' ; 'password' 
							//     | 'provider_string'
							if (InStatement.GetIfAllNextValidToken(lstTokens, ref offset, out nextToken, TokenKind.ValueString, TokenKind.Semicolon, TokenKind.ValueString, TokenKind.Semicolon, TokenKind.ValueString, TokenKind.Comma)) {
								// { 'datasource' ; 'user_id' ; 'password',
							} else if (InStatement.GetIfAllNextValidToken(lstTokens, ref offset, out nextToken, TokenKind.ValueString, TokenKind.Comma)) {
								// 'provider_string',
							} else {
								addedSysObject = null;
								addedTableSource = null;
								return false;
							}

							//   , { [ catalog. ] [ schema. ] object 
							//     | 'query' 
							offset++;
							nextToken = InStatement.GetNextNonCommentToken(lstTokens, offset);
							if (null != nextToken && nextToken.Kind == TokenKind.ValueString) {
								// 'query' 
								nextToken.TokenContextType = TokenContextType.Known;
							} else {
								// [ catalog. ] [ schema. ] object 
								TokenInfo server_name;
								TokenInfo catalog_name;
								TokenInfo schema_name;
								TokenInfo object_name;
								int endIndex;
								if (parser.ParseTableOrViewName(offset, out endIndex, out server_name, out catalog_name, out schema_name, out object_name)) {
									offset = endIndex;
									foreach (SysObject sysObject in lstSysObjects) {
										if (sysObject.ObjectName.Equals(object_name.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase) && (null == schema_name || sysObject.Schema.Schema.Equals(schema_name.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase))) {
											foreach (SysObjectColumn sysObjectColumn in sysObject.Columns) {
												lstSysObjectColumn.Add(sysObjectColumn);
											}
											break;
										}
									}

								} else {
									addedSysObject = null;
									addedTableSource = null;
									return false;
								}
							}
							i = offset;
						} else if (nextToken.Kind == TokenKind.KeywordBulk) {
							///      | BULK 'data_file' , 
							///           { FORMATFILE = 'format_file_path' [ <bulk_options> ]
							///           | SINGLE_BLOB | SINGLE_CLOB | SINGLE_NCLOB
							///	          }
							if (InStatement.GetIfAllNextValidToken(lstTokens, ref offset, out nextToken, TokenKind.ValueString)) {
								nextToken.TokenContextType = TokenContextType.Known;
								if (InStatement.GetIfAnyNextValidToken(lstTokens, ref offset, out nextToken, TokenKind.KeywordSingleBlob, TokenKind.KeywordSingleClob, TokenKind.KeywordSingleNClob)) {
									// Ok
								} else if (InStatement.GetIfAllNextValidToken(lstTokens, ref offset, out nextToken, TokenKind.KeywordFormatFile, TokenKind.Assign, TokenKind.ValueString)) {
									// 'format_file_path'
									nextToken.TokenContextType = TokenContextType.Known;

									///    <bulk_options> ::=
									///       [ , CODEPAGE = { 'ACP' | 'OEM' | 'RAW' | 'code_page' } ] 
									///       [ , ERRORFILE = 'file_name' ]
									///       [ , FIRSTROW = first_row ] 
									///       [ , LASTROW = last_row ] 
									///       [ , MAXERRORS = maximum_errors ] 
									///       [ , ROWS_PER_BATCH = rows_per_batch ] 
									if (!InStatement.GetIfAllNextValidToken(lstTokens, ref offset, out nextToken, TokenKind.KeywordCodePage, TokenKind.Assign, TokenKind.ValueString)) {
										if (!InStatement.GetIfAllNextValidToken(lstTokens, ref offset, out nextToken, TokenKind.KeywordErrorFile, TokenKind.Assign, TokenKind.ValueString)) {
											if (!InStatement.GetIfAllNextValidToken(lstTokens, ref offset, out nextToken, TokenKind.KeywordFirstRow, TokenKind.Assign, TokenKind.ValueNumber)) {
												if (!InStatement.GetIfAllNextValidToken(lstTokens, ref offset, out nextToken, TokenKind.KeywordLastRow, TokenKind.Assign, TokenKind.ValueNumber)) {
													if (!InStatement.GetIfAllNextValidToken(lstTokens, ref offset, out nextToken, TokenKind.KeywordMaxErrors, TokenKind.Assign, TokenKind.ValueNumber)) {
														if (!InStatement.GetIfAllNextValidToken(lstTokens, ref offset, out nextToken, TokenKind.KeywordRowsPerBatch, TokenKind.Assign, TokenKind.ValueNumber)) {
															addedSysObject = null;
															addedTableSource = null;
															return false;
														}
													}
												}
											}
										}
									}
								} else {
									addedSysObject = null;
									addedTableSource = null;
									return false;
								}
								i = offset;
							}
						} else {
							addedSysObject = null;
							addedTableSource = null;
							return false;
						}

						// Ending right paranthesis?
						offset = i;
						if (!InStatement.GetIfAllNextValidToken(lstTokens, ref offset, out nextToken, TokenKind.RightParenthesis)) {
							addedSysObject = null;
							addedTableSource = null;
							return false;
						}
						i = offset;

						int statementStartIndex = startStatementIndex;
						int statementEndIndex = i;
						int tableStartIndex = startRowsetIndex;
						int realEndIndex;
						if (KeywordDerivedTable.GetAliasAndSearchCondition(parser, statementStartIndex, tableStartIndex, statementEndIndex, out realEndIndex,
								lstTokens, lstSysObjects, startStatementIndex, currentStartSpan, startToken, Common.enSqlTypes.Rowset, ref sysObjectId,
								out addedSysObject, out addedTableSource, startStatementIndex, lstSysObjectColumn)) {
							i = realEndIndex;
							return true;
						}
					}
				}
			}
			addedSysObject = null;
			addedTableSource = null;
			return false;
		}
	}
}
