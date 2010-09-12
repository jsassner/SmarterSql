// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sassner.SmarterSql.Parsing;
using Sassner.SmarterSql.Parsing.Expressions;
using Sassner.SmarterSql.Parsing.Keywords;
using Sassner.SmarterSql.Parsing.Predicates;
using Sassner.SmarterSql.ParsingObjects;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Objects {
	public class TableSource {
		#region Member variables

		private const string ClassName = "TableSource";

		private readonly Table table;
		private readonly Token tokenContext;
		private int endIndex;
		private int startIndex;

		#endregion

		public TableSource(Token tokenContext, Table table, int startIndex, int endIndex) {
			this.tokenContext = tokenContext;
			this.table = table;
			this.endIndex = endIndex;
			this.startIndex = startIndex;
		}

		///<summary>
		///Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		///</returns>
		///<filterpriority>2</filterpriority>
		[DebuggerStepThrough]
		public override string ToString() {
			return tokenContext.UnqoutedImage + ": " + table.TableName + " AS '" + table.Alias + "', startIndex=" + startIndex + ", endIndex=" + endIndex + ", IsTemporary=" + table.IsTemporary + ", type=" + table.SysObject.SqlType;
		}

		/// <summary>
		/// Retrive unique tablesources, not including aliases
		/// </summary>
		/// <param name="statementSpan"></param>
		/// <param name="tableSources"></param>
		/// <returns></returns>
		public static List<TableSource> GetUniqueTableSourceSysObjects(StatementSpans statementSpan, List<TableSource> tableSources) {
			if (null != statementSpan.NearbyUniqueTableSourcesSysObject) {
				return statementSpan.NearbyUniqueTableSourcesSysObject;
			}

			List<TableSource> newTableSources = new List<TableSource>();
			foreach (TableSource oldTableSource in tableSources) {
				bool foundTableSource = false;
				foreach (TableSource newTableSource in newTableSources) {
					if (oldTableSource.Table.SysObject == newTableSource.Table.SysObject) {
						foundTableSource = true;
						break;
					}
				}
				if (!foundTableSource) {
					newTableSources.Add(oldTableSource);
				}
			}

			statementSpan.NearbyUniqueTableSourcesSysObject = newTableSources;
			return newTableSources;
		}

		/// <summary>
		/// Retrive unique tablesources including unique aliases
		/// </summary>
		/// <param name="statementSpan"></param>
		/// <param name="tableSources"></param>
		/// <returns></returns>
		public static List<TableSource> GetUniqueTableSources(StatementSpans statementSpan, List<TableSource> tableSources) {
			if (null != statementSpan.NearbyUniqueTableSources) {
				return statementSpan.NearbyUniqueTableSources;
			}

			List<TableSource> newTableSources = new List<TableSource>();
			foreach (TableSource oldTableSource in tableSources) {
				bool foundTableSource = false;
				foreach (TableSource newTableSource in newTableSources) {
					if ((0 == oldTableSource.Table.Alias.Length || oldTableSource.Table.Alias.Equals(newTableSource.Table.Alias, StringComparison.OrdinalIgnoreCase)) && oldTableSource.Table.SysObject == newTableSource.Table.SysObject) {
						foundTableSource = true;
						break;
					}
				}
				if (!foundTableSource) {
					newTableSources.Add(oldTableSource);
				}
			}

			statementSpan.NearbyUniqueTableSources = newTableSources;
			return newTableSources;
		}

		#region Parse a tablesource

		/// <summary>
		/// Parse a table source
		///	  [ FROM { <table_source> } [ ,...n ] ] 
		/// A * means it's implemented
		/// 
		///	  <table_source> ::= {
		///	*   table_or_view_name [ [ AS ] table_alias ] [ <tablesample_clause> ] [ WITH ( < table_hint > [ [ , ]...n ] ) ] 
		///	* | rowset_function [ [ AS ] table_alias ] [ ( bulk_column_alias [ ,...n ] ) ] 
		///	* | user_defined_function [ [ AS ] table_alias ] [ (column_alias [ ,...n ] ) ]
		///	* | OPENXML <openxml_clause> 
		///	* | derived_table [ AS ] table_alias [ ( column_alias [ ,...n ] ) ] 
		///	  | <joined_table> 
		///	* | <pivoted_table> 
		///	* | <unpivoted_table>
		///	* | @variable [ [ AS ] table_alias ]
		///	  | @variable.function_call ( expression [ ,...n ] ) [ [ AS ] table_alias ] [ (column_alias [ ,...n ] ) ]
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="currentStartSpan"></param>
		/// <param name="lstSysObjects"></param>
		/// <param name="i"></param>
		/// <param name="lstTokens"></param>
		/// <param name="startToken"></param>
		public static void ParseTableSource(Parser parser, StatementSpans currentStartSpan, List<SysObject> lstSysObjects, ref int i, List<TokenInfo> lstTokens, TokenInfo startToken, ref int sysObjectId) {
			try {
				int startIndex;
				int offset;
				SysObject addedSysObject = null;
				TableSource addedTableSource = null;

				// Find start token
				switch (startToken.Kind) {
					case TokenKind.KeywordFrom:
						startIndex = InStatement.GetNextNonCommentToken(lstTokens, i + 1, i);
						break;

					case TokenKind.KeywordJoin:
						if (!InStatement.FindStartOfJoin(lstTokens, i, out startIndex)) {
							startIndex = InStatement.GetNextNonCommentToken(lstTokens, i, i);
						}
						break;

					case TokenKind.Comma:
						startIndex = InStatement.GetNextNonCommentToken(lstTokens, i + 1, i);
						break;

					default:
						startIndex = InStatement.GetNextNonCommentToken(lstTokens, i, i);
						break;
				}

				i++;
				TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
				if (null != nextToken) {
					if (nextToken.Kind == TokenKind.LeftParenthesis) {
						KeywordDerivedTable.ParseDerivedTable(parser, currentStartSpan, lstTokens, ref i, lstSysObjects, startIndex, ref sysObjectId, out addedSysObject, out addedTableSource, startToken);

					} else if (nextToken.Kind == TokenKind.KeywordContainsTable) {
						KeywordContainsTable.ParseContainsTable(parser, currentStartSpan, lstTokens, ref i, lstSysObjects, startIndex, ref sysObjectId, out addedSysObject, out addedTableSource, startToken);

					} else if (nextToken.Kind == TokenKind.KeywordOpenQuery) {
						KeywordOpenQuery.ParseOpenQuery(parser, currentStartSpan, lstTokens, ref i, lstSysObjects, startIndex, ref sysObjectId, out addedSysObject, out addedTableSource, startToken);

					} else if (nextToken.Kind == TokenKind.KeywordFreeTextTable) {
						KeywordFreeTextTable.ParseFreeTextTable(parser, currentStartSpan, lstTokens, ref i, lstSysObjects, startIndex, ref sysObjectId, out addedSysObject, out addedTableSource, startToken);

					} else if (nextToken.Kind == TokenKind.KeywordOpenRowset) {
						KeywordOpenRowset.ParseOpenRowSet(parser, currentStartSpan, lstTokens, ref i, lstSysObjects, startIndex, ref sysObjectId, out addedSysObject, out addedTableSource, startToken);

					} else if (nextToken.Kind == TokenKind.KeywordOpenDataSource) {
						KeywordOpenDataSource.ParseOpenDataSource(parser, currentStartSpan, lstTokens, ref i, lstSysObjects, startIndex, ref sysObjectId, out addedSysObject, out addedTableSource, startToken);

					} else if (nextToken.Kind == TokenKind.KeywordOpenXml) {
						KeywordOpenXml.ParseOpenXml(parser, currentStartSpan, lstTokens, ref i, lstSysObjects, startIndex, ref sysObjectId, out addedSysObject, out addedTableSource, startToken);

					} else {
						string servername = string.Empty;
						string databasename = string.Empty;
						string schemaname = string.Empty;
						string table = string.Empty;
						bool isCTE = false;
						int startTableIndex = i;
						int endTableIndex = -1;
						TokenInfo tokenStart = null;

						// CTE usage (SELECT .. FROM (CTE))?
						foreach (ScannedTable scannedTable in parser.ScannedTables) {
							if (Common.enSqlTypes.CTE == scannedTable.SqlType && scannedTable.Name.Equals(nextToken.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase)) {
								table = nextToken.Token.UnqoutedImage;
								tokenStart = nextToken;
								endTableIndex = i;
								isCTE = true;
								break;
							}
						}

						if (!isCTE) {
							// Normal multi part name
							TokenInfo server_name;
							TokenInfo database_name;
							TokenInfo schema_name;
							TokenInfo object_name;
							if (parser.ParseTableOrViewName(startTableIndex, out endTableIndex, out server_name, out database_name, out schema_name, out object_name)) {
								servername = (null != server_name ? server_name.Token.UnqoutedImage : string.Empty);
								databasename = (null != database_name ? database_name.Token.UnqoutedImage : string.Empty);
								schemaname = (null != schema_name ? schema_name.Token.UnqoutedImage : string.Empty);
								table = (null != object_name ? object_name.Token.UnqoutedImage : string.Empty);
								tokenStart = ((server_name ?? database_name) ?? schema_name) ?? object_name;
								// Move the pointer (i) to the token after the table or view name
								i = endTableIndex;
							}
						}

						if (table.Length > 0 && -1 != endTableIndex && null != tokenStart) {
							string alias;
							int endIndex = endTableIndex;

							offset = i + 1;
							nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
							int statementEndIndex;

							// Table-valued function?
							if (null != nextToken && nextToken.Kind == TokenKind.LeftParenthesis) {
								if (-1 == nextToken.MatchingParenToken) {
									return;
								}

								int endOfParameters = nextToken.MatchingParenToken;
								// Parse function parameters
								while (offset < endOfParameters) {
									nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
									if (nextToken.Type == TokenType.Identifier || nextToken.Kind == TokenKind.KeywordNull) {
										nextToken.TokenContextType = TokenContextType.Parameter;
									}
									offset++;
								}

								i = endOfParameters;
								endTableIndex = i;
								endIndex = endTableIndex;

								// Get table alias if it's there
								offset = InStatement.GetNextNonCommentToken(lstTokens, i + 1, i + 1);
								if (parser.ExtractTableAlias(offset, ref endTableIndex, out alias)) {
									endIndex = endTableIndex;
								}

								// Table valued functions can have column aliases
								offset = endTableIndex + 1;
								nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
								if (null != nextToken) {
									if (nextToken.Kind == TokenKind.LeftParenthesis && -1 != nextToken.MatchingParenToken && offset < nextToken.MatchingParenToken) {
										endIndex = nextToken.MatchingParenToken;
										// Parse column alias
										while (offset < endIndex) {
											nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
											if (nextToken.Type == TokenType.Identifier) {
												nextToken.TokenContextType = TokenContextType.NewColumnAlias;
											}
											offset++;
										}
										statementEndIndex = endIndex;
									} else {
										statementEndIndex = offset - 1;
									}
								} else {
									statementEndIndex = offset - 1;
								}
							} else {
								// Get table alias if it's there
								offset = InStatement.GetNextNonCommentToken(lstTokens, i + 1, i + 1);
								if (parser.ExtractTableAlias(offset, ref endTableIndex, out alias)) {
									endIndex = endTableIndex;
								}

								// table_or_view_name [ [ AS ] table_alias ] [ <tablesample_clause> ] [ WITH ( < table_hint > [ [ , ]...n ] ) ]

								// <tablesample_clause> ::= TABLESAMPLE [SYSTEM] ( sample_number [ PERCENT | ROWS ] ) [ REPEATABLE ( repeat_seed ) ]
								offset = InStatement.GetNextNonCommentToken(lstTokens, endIndex + 1, endIndex + 1);
								if (InStatement.IsInStatementTableSample(lstTokens, offset, out statementEndIndex)) {
									i = statementEndIndex;
									endIndex = statementEndIndex;
								}

								// WITH ( < table_hint > [ [ , ]...n ] )
								offset = InStatement.GetNextNonCommentToken(lstTokens, endIndex + 1, endIndex + 1);
								if (InStatement.IsInStatementWithTableHint(lstTokens, offset, Instance.TextEditor.StaticData, out statementEndIndex)) {
									i = statementEndIndex;
									endIndex = statementEndIndex;
								}
							}

							parser.CreateTableEntry(currentStartSpan, startToken.Token, lstSysObjects, servername, databasename, schemaname, table, alias, startIndex, endIndex, startTableIndex, endTableIndex, false, out addedSysObject, out addedTableSource);

							// If a join statement, include the search_conditions
							if (startToken.Kind == TokenKind.KeywordJoin) {
								endIndex = KeywordSearchCondition.FindLengthOfSearchCondition(parser, lstTokens, endTableIndex);
								int startOfSearchCondition = (-1 != statementEndIndex ? statementEndIndex + 2 : endTableIndex + 2);
								List<Predicate> predicates;
								KeywordSearchCondition.ParseSearchCondition(parser, lstTokens, startOfSearchCondition, endIndex, lstSysObjects, out predicates);
							}

							i = endIndex;
						}
					}
				}

				// Pivot / Unpivot ?
				offset = i + 1;
				nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
				if (null == nextToken || !(nextToken.Kind == TokenKind.KeywordPivot || nextToken.Kind == TokenKind.KeywordUnpivot) || null == addedTableSource) {
					return;
				}

				switch (nextToken.Kind) {
					case TokenKind.KeywordPivot:
						KeywordPivot.ParsePivot(parser, ref offset, ref sysObjectId, lstTokens, addedSysObject, lstSysObjects, startIndex, i, currentStartSpan);
						break;
					case TokenKind.KeywordUnpivot:
						KeywordPivot.ParseUnpivot(parser, ref offset, ref sysObjectId, lstTokens, addedSysObject, lstSysObjects, startIndex, i, currentStartSpan);
						break;
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "ParseTableSource", e, Common.enErrorLvl.Error);
			}
		}

		#endregion

		#region Public properties

		public Token TokenContext {
			[DebuggerStepThrough]
			get { return tokenContext; }
		}

		public Table Table {
			[DebuggerStepThrough]
			get { return table; }
		}

		public int StartIndex {
			[DebuggerStepThrough]
			get { return startIndex; }
			set { startIndex = value; }
		}

		public int EndIndex {
			[DebuggerStepThrough]
			get { return endIndex; }
			set { endIndex = value; }
		}

		#endregion
	}
}