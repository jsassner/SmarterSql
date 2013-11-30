// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.Parsing.Expressions;
using Sassner.SmarterSql.Parsing.Keywords;
using Sassner.SmarterSql.Parsing.Predicates;
using Sassner.SmarterSql.ParsingObjects;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;
using Sassner.SmarterSql.Utils;
using Sassner.SmarterSql.Utils.Helpers;
using Sassner.SmarterSql.Utils.Marker;
using Sassner.SmarterSql.Utils.Segment;
using Sassner.SmarterSql.Utils.SqlErrors;

namespace Sassner.SmarterSql.Parsing {
	public class Parser : IDisposable {
		#region Member variables

		private const string ClassName = "Parser";
		private readonly ListOfScannedSqlErrors listOfScannedSqlErrors = new ListOfScannedSqlErrors();

		// Tokens
		private readonly List<Cursor> calledCursors = new List<Cursor>(50);
		private readonly List<ScannedItem> calledLabels = new List<ScannedItem>(100);
		private readonly List<LocalVariable> calledLocalVariables = new List<LocalVariable>(150);
		private readonly List<ScannedItem> calledTransactions = new List<ScannedItem>(50);
		private readonly List<ColumnAlias> declaredColumnAliases = new List<ColumnAlias>(250);
		private readonly List<Cursor> declaredCursors = new List<Cursor>(50);
		private readonly List<ScannedItem> declaredLabels = new List<ScannedItem>(100);
		private readonly List<LocalVariable> declaredLocalVariables = new List<LocalVariable>(150);
		private readonly List<ScannedItem> declaredTransactions = new List<ScannedItem>(50);

		private readonly List<ScannedTable> lstScannedTables = new List<ScannedTable>(50);
		private readonly List<TableSource> lstTableSources = new List<TableSource>(250);
		private readonly List<TemporaryTable> lstTemporaryTables = new List<TemporaryTable>(250);
		private BackgroundWorker bgWorkerDoFullParse;
		private List<TokenInfo> lstTokens = new List<TokenInfo>();
		private Markers markers = new Markers();
		private int previousTokenRangeStartIndex = -1;
		private SegmentUtils segmentUtils = new SegmentUtils(Instance.StaticData);
		// Our temporary sysobject id
		private int sysObjectId;

		#endregion

		#region Public properties

		public List<TokenInfo> RawTokens {
			[DebuggerStepThrough]
			get { return lstTokens; }
			set { lstTokens = value; }
		}

		public ListOfScannedSqlErrors ListOfScannedSqlErrors {
			[DebuggerStepThrough]
			get { return listOfScannedSqlErrors; }
		}

		public List<ScannedSqlError> ScannedSqlErrors {
			[DebuggerStepThrough]
			get { return ListOfScannedSqlErrors.ScannedSqlErrors; }
			set { ListOfScannedSqlErrors.ScannedSqlErrors = value; }
		}

		public List<ScannedItem> DeclaredTransactions {
			[DebuggerStepThrough]
			get { return declaredTransactions; }
		}

		public List<ScannedItem> CalledTransactions {
			[DebuggerStepThrough]
			get { return calledTransactions; }
		}

		public List<ScannedItem> CalledLabels {
			[DebuggerStepThrough]
			get { return calledLabels; }
		}

		public List<ScannedItem> DeclaredLabels {
			[DebuggerStepThrough]
			get { return declaredLabels; }
		}

		public int SysObjectId {
			[DebuggerStepThrough]
			get { return sysObjectId; }
		}

		public List<ScannedTable> ScannedTables {
			[DebuggerStepThrough]
			get { return lstScannedTables; }
		}

		public List<Cursor> CalledCursors {
			[DebuggerStepThrough]
			get { return calledCursors; }
		}

		public List<Cursor> DeclaredCursors {
			[DebuggerStepThrough]
			get { return declaredCursors; }
		}

		public List<ColumnAlias> DeclaredColumnAliases {
			[DebuggerStepThrough]
			get { return declaredColumnAliases; }
		}

		public Markers Markers {
			[DebuggerStepThrough]
			get { return markers; }
			set { markers = value; }
		}

		public List<LocalVariable> DeclaredLocalVariables {
			[DebuggerStepThrough]
			get { return declaredLocalVariables; }
		}

		public List<LocalVariable> CalledLocalVariables {
			[DebuggerStepThrough]
			get { return calledLocalVariables; }
		}

		public List<TableSource> TableSources {
			[DebuggerStepThrough]
			get { return lstTableSources; }
		}

		public List<TemporaryTable> TemporaryTables {
			[DebuggerStepThrough]
			get { return lstTemporaryTables; }
		}

		public SegmentUtils SegmentUtils {
			get { return segmentUtils; }
			set { segmentUtils = value; }
		}

		#endregion

		#region Scan for known tokens

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lstSysObjects"></param>
		/// <param name="bgWorker"></param>
		public void ParseTokens(List<SysObject> lstSysObjects, BackgroundWorker bgWorker) {
			if (null == segmentUtils) {
				return;
			}

			bgWorkerDoFullParse = bgWorker;
			try {
				#region Clear lists

				calledLocalVariables.Clear();
				declaredLocalVariables.Clear();
				lstTableSources.Clear();
				lstScannedTables.Clear();
				lstTemporaryTables.Clear();
				calledLabels.Clear();
				declaredLabels.Clear();
				calledCursors.Clear();
				declaredCursors.Clear();
				declaredColumnAliases.Clear();
				calledTransactions.Clear();
				declaredTransactions.Clear();

				#endregion

				SysObject.RemoveTemporarySysObjects(lstSysObjects);

				// Our temporary table id counter
				sysObjectId = 0;

				previousTokenRangeStartIndex = 0;
				ParseTokenRange(null, 0, lstTokens.Count, lstSysObjects);
				bgWorkerDoFullParse = null;

				#region When all table alias are added, translate the derived tables

				// When all table alias are added, translate the derived tables
				// First sort the list, deepest table first
				lstScannedTables.Sort(ScannedTable.ScannedTableComparison);
				foreach (ScannedTable scannedTable in lstScannedTables) {
					Token startToken = null;
					if (scannedTable.SqlType == Common.enSqlTypes.CTE) {
						// For CTE's that are not used, we need to find and correct the tablesource
						foreach (TableSource tableSource in lstTableSources) {
							if (scannedTable.StartIndex <= tableSource.StartIndex && scannedTable.EndIndex >= tableSource.EndIndex) {
								scannedTable.StartTableIndex = tableSource.Table.StartIndex;
								scannedTable.EndTableIndex = tableSource.Table.EndIndex;
								break;
							}
						}
					} else if (scannedTable.SqlType == Common.enSqlTypes.DerivedTable) {
						startToken = lstTokens[scannedTable.StartIndex + 1].Token;
					}

					CreateScannedTable(startToken, lstSysObjects, scannedTable);
				}

				#endregion
			} catch (Exception e) {
				Common.LogEntry(ClassName, "ParseTokens", e, Common.enErrorLvl.Error);
			}
		}

		/// <summary>
		/// Parse a range of tokens
		/// </summary>
		/// <param name="currentStartSpan"></param>
		/// <param name="startIndexRange"></param>
		/// <param name="endIndexRange"></param>
		/// <param name="lstSysObjects"></param>
		internal void ParseTokenRange(StatementSpans currentStartSpan, int startIndexRange, int endIndexRange, List<SysObject> lstSysObjects) {
			if (startIndexRange < previousTokenRangeStartIndex) {
				Common.LogEntry(ClassName, "ParseTokenRange", "Tried to rescan a previous range. startIndexRange=" + startIndexRange + ", endIndexRange=" + endIndexRange + ", previousTokenRangeStartIndex=" + previousTokenRangeStartIndex, Common.enErrorLvl.Warning);
				return;
			}
			previousTokenRangeStartIndex = startIndexRange;

			int i = startIndexRange;
			while (i < endIndexRange) {
				if (null != bgWorkerDoFullParse && bgWorkerDoFullParse.CancellationPending) {
					return;
				}
				int currentI = i;
				TokenInfo token = InStatement.GetNextNonCommentToken(lstTokens, ref i);
				if (null == token) {
					return;
				}

				if (token.Kind == TokenKind.KeywordSelect) {
					#region SELECT

					KeywordSelect.ParseSelectForColumns(this, out currentStartSpan, lstSysObjects, ref i, lstTokens, ref sysObjectId);
					i++;

					#endregion
				} else if (token.Kind == TokenKind.KeywordJoin || token.Kind == TokenKind.KeywordFrom) {
					#region JOIN & FROM

					TableSource.ParseTableSource(this, currentStartSpan, lstSysObjects, ref i, lstTokens, token, ref sysObjectId);

					#endregion
				} else if (token.Kind == TokenKind.KeywordWhere || token.Kind == TokenKind.KeywordHaving) {
					#region WHERE & HAVING

					int startIndex = i + 1;
					int endIndex = segmentUtils.GetNextMainTokenEnd(currentStartSpan, i);
					token = InStatement.GetNextNonCommentToken(lstTokens, startIndex);
					if (null == token) {
						return;
					}
					if (token.Kind == TokenKind.KeywordCurrent) {
						KeywordUpdate.ParseCurrentOf(this, lstTokens, startIndex, endIndex);
					} else {
						List<Predicate> predicates;
						KeywordSearchCondition.ParseSearchCondition(this, lstTokens, startIndex, endIndex, lstSysObjects, out predicates);
					}
					i = endIndex;

					#endregion
				} else if (token.Kind == TokenKind.KeywordGroup) {
					#region GROUP BY

					int startIndex = i;
					int endIndex = segmentUtils.GetNextMainTokenEnd(currentStartSpan, i);
					KeywordGroupBy.ParseGroupBy(this, lstTokens, startIndex, endIndex);
					i = endIndex;

					#endregion
				} else if (token.Kind == TokenKind.KeywordOrder) {
					#region ORDER BY

					int startIndex = i;
					int endIndex = segmentUtils.GetNextMainTokenEnd(currentStartSpan, i);
					List<Expression> orderByExpressions;
					KeywordOrderBy.ParseOrderBy(this, lstTokens, ref startIndex, endIndex, out orderByExpressions);
					i = endIndex;

					#endregion
				} else if (token.Kind == TokenKind.KeywordInsert) {
					#region INSERT

					KeywordInsert.ParseInsertStatement(this, out currentStartSpan, lstSysObjects, ref i, lstTokens);

					#endregion
				} else if (token.Kind == TokenKind.KeywordDelete) {
					#region DELETE

					KeywordDelete.ParseDeleteStatement(this, out currentStartSpan, lstSysObjects, ref i, lstTokens);

					#endregion
				} else if (token.Kind == TokenKind.KeywordUpdate) {
					#region UPDATE

					KeywordUpdate.ParseUpdateStatement(this, out currentStartSpan, lstSysObjects, ref i, lstTokens);

					#endregion
				} else if (token.Kind == TokenKind.KeywordDeclare) {
					#region DECLARE

					i++;
					TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
					if (null != nextToken) {
						if (nextToken.Kind == TokenKind.Name && !nextToken.Token.UnqoutedImage.StartsWith("@")) {
							KeywordDeclare.ParseDeclareCursor(this, lstTokens, ref i);
						} else {
							// Parse a declaration expression
							while (true) {
								nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
								if (null == nextToken) {
									break;
								}
								TokenInfo startToken = nextToken;
								startToken.TokenContextType = TokenContextType.Variable;
								int variableIndex = i;

								// [ AS ]
								InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordAs);

								ParsedDataType parsedDataType = ParsedDataType.ParseDataType(lstTokens, ref i);
								if (null == parsedDataType) {
									break;
								}

								if (parsedDataType.DataType.Kind == TokenKind.KeywordCursor) {
									declaredCursors.Add(new Cursor(startToken.Token.UnqoutedImage, variableIndex));
								}

								// Create a Variable object
								LocalVariable objVariable = new LocalVariable(startToken.Token.UnqoutedImage, variableIndex, parsedDataType);

								declaredLocalVariables.Add(objVariable);

								// Table datatype?
								if (parsedDataType.DataType.Kind == TokenKind.KeywordTable) {
									KeywordTable.ParseCreateTable(this, lstTokens, ref variableIndex, lstSysObjects, ref sysObjectId);
									break;
								}

								// More declarations?
								int offset2 = i + 1;
								nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref offset2);
								if (null == nextToken || nextToken.Kind != TokenKind.Comma) {
									break;
								}
								i = offset2 + 1;
							}
						}
					}

					#endregion
				} else if (token.Kind == TokenKind.KeywordFor) {
					#region FOR UPDATE (cursor)

					// [ FOR UPDATE [ OF column_name [ ,...n ] ] ]
					i++;
					TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
					if (null != nextToken && nextToken.Kind == TokenKind.KeywordUpdate) {
						i++;
						nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
						if (null != nextToken && nextToken.Kind == TokenKind.KeywordOf) {
							i++;
						}
						bool isNextSysColumn = true;

						while (isNextSysColumn) {
							nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
							if (null != nextToken && nextToken.Type == TokenType.Identifier) {
								nextToken.TokenContextType = TokenContextType.CursorColumnUpdatable;
							}
							i++;
							nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
							if (!(null != nextToken && nextToken.Kind == TokenKind.Comma)) {
								isNextSysColumn = false;
							} else {
								i++;
							}
						}
					}

					#endregion
				} else if (token.Kind == TokenKind.KeywordDeallocate || token.Kind == TokenKind.KeywordOpen || token.Kind == TokenKind.KeywordClose) {
					#region DEALLOCATE/OPEN/CLOSE (cursor)

					i++;
					int offset = i;
					TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
					if (null != nextToken && nextToken.Kind == TokenKind.KeywordGlobal) {
						offset++;
						nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
					}
					if (null != nextToken && nextToken.Kind == TokenKind.Name) {
						nextToken.TokenContextType = TokenContextType.Cursor;
						calledCursors.Add(new Cursor(nextToken.Token.UnqoutedImage, offset));
					}

					#endregion
				} else if (token.Kind == TokenKind.KeywordFetch) {
					#region FETCH (cursor)

					i++;
					TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
					if (null != nextToken) {
						if (nextToken.Kind == TokenKind.KeywordNext || nextToken.Kind == TokenKind.KeywordPrior || nextToken.Kind == TokenKind.KeywordFirst || nextToken.Kind == TokenKind.KeywordLast) {
							// OK
						} else if (nextToken.Kind == TokenKind.KeywordAbsolute || nextToken.Kind == TokenKind.KeywordRelative) {
							i++;
							InStatement.GetNextNonCommentToken(lstTokens, ref i);
						}
						i++;
						nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
						if (null != nextToken && nextToken.Kind == TokenKind.KeywordFrom) {
							i++;
							nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
						}
						if (null != nextToken && nextToken.Kind == TokenKind.KeywordGlobal) {
							i++;
							nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
						}
						if (null != nextToken && nextToken.Kind == TokenKind.Name) {
							nextToken.TokenContextType = TokenContextType.Cursor;
							calledCursors.Add(new Cursor(nextToken.Token.UnqoutedImage, i));
							i++;
							nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
							if (null != nextToken && nextToken.Kind == TokenKind.KeywordInto) {
								i++;
								nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
								while (null != nextToken && nextToken.Kind == TokenKind.Variable) {
									nextToken.TokenContextType = TokenContextType.Variable;
									i++;
									nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
									if (null == nextToken || nextToken.Kind != TokenKind.Comma) {
										break;
									}
									i++;
									nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
								}
							}
						}
					}

					#endregion
				} else if (token.Kind == TokenKind.SystemVariable) {
					#region @@systemvariable usage

					token.TokenContextType = TokenContextType.SystemVariable;
					i++;

					#endregion
				} else if (token.Kind == TokenKind.Variable) {
					#region @variable usage

					token.TokenContextType = TokenContextType.Variable;
					LocalVariable.AddCalledVariable(this, token, i);
					i++;

					#endregion
				} else if (token.Kind == TokenKind.KeywordEnable) {
					#region Enable Trigger

					i++;
					TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
					if (null != nextToken && nextToken.Kind == TokenKind.KeywordTrigger) {
						KeywordTrigger.HandleEnableDisableTrigger(lstTokens, ref i);
					}

					#endregion
				} else if (token.Kind == TokenKind.KeywordDisable) {
					#region Disable Trigger

					i++;
					TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
					if (null != nextToken && nextToken.Kind == TokenKind.KeywordTrigger) {
						KeywordTrigger.HandleEnableDisableTrigger(lstTokens, ref i);
					}

					#endregion
				} else if (token.Kind == TokenKind.KeywordCreate) {
					#region CREATE TABLE/PROCEDURE/FUNCTION/VIEW/TRIGGER

					i++;
					TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
					if (null != nextToken && nextToken.Kind == TokenKind.KeywordTable) {
						i++;
						nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
						if (null != nextToken && nextToken.Type == TokenType.Identifier) {
							KeywordTable.ParseCreateTable(this, lstTokens, ref i, lstSysObjects, ref sysObjectId);
						}
					} else if (null != nextToken && (nextToken.Kind == TokenKind.KeywordProc || nextToken.Kind == TokenKind.KeywordProcedure)) {
						KeywordProcedure.HandleCreateAlterProcedure(this, lstTokens, ref i);
					} else if (null != nextToken && nextToken.Kind == TokenKind.KeywordFunction) {
						KeywordFunction.HandleCreateAlterFunction(this, lstTokens, ref i);
					} else if (null != nextToken && nextToken.Kind == TokenKind.KeywordView) {
						KeywordView.HandleCreateAlterView(lstTokens, ref i);
					} else if (null != nextToken && nextToken.Kind == TokenKind.KeywordTrigger) {
						KeywordTrigger.HandleCreateAlterTrigger(currentStartSpan, lstTokens, ref i, ref sysObjectId, lstSysObjects);
					}

					#endregion
				} else if (token.Kind == TokenKind.KeywordAlter) {
					#region ALTER PROCEDURE/FUNCTION/VIEW/TABLE

					i++;
					TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
					if (null != nextToken && (nextToken.Kind == TokenKind.KeywordProc || nextToken.Kind == TokenKind.KeywordProcedure)) {
						KeywordProcedure.HandleCreateAlterProcedure(this, lstTokens, ref i);
					} else if (null != nextToken && nextToken.Kind == TokenKind.KeywordFunction) {
						KeywordFunction.HandleCreateAlterFunction(this, lstTokens, ref i);
					} else if (null != nextToken && nextToken.Kind == TokenKind.KeywordView) {
						KeywordView.HandleCreateAlterView(lstTokens, ref i);
					} else if (null != nextToken && nextToken.Kind == TokenKind.KeywordTrigger) {
						KeywordTrigger.HandleCreateAlterTrigger(currentStartSpan, lstTokens, ref i, ref sysObjectId, lstSysObjects);
					} else if (null != nextToken && nextToken.Kind == TokenKind.KeywordTable) {
						KeywordTable.HandleAlterTable(this, lstTokens, ref i, ref sysObjectId, lstSysObjects);
					}

					#endregion
				} else if (token.Kind == TokenKind.KeywordDrop) {
					#region DROP TABLE/PROCEDURE

					i++;
					TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
					if (null != nextToken && nextToken.Kind == TokenKind.KeywordTable) {
						// DROP TABLE [ database_name . [ schema_name ] . | schema_name . ] table_name [ ,...n ] [ ; ]
						i++;
						bool isNextTableSource = true;

						while (isNextTableSource) {
							// Normal multi part name
							TokenInfo server_name;
							TokenInfo database_name;
							TokenInfo schema_name;
							TokenInfo object_name;
							int endTableIndex;
							if (ParseTableOrViewName(i, out endTableIndex, out server_name, out database_name, out schema_name, out object_name)) {
								// Move the pointer (i) to the token after the table name
								i = endTableIndex;
							}
							i++;
							nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
							if (!(null != nextToken && nextToken.Kind == TokenKind.Comma)) {
								isNextTableSource = false;
							} else {
								i++;
							}
						}
					} else if (null != nextToken && (nextToken.Kind == TokenKind.KeywordProc || nextToken.Kind == TokenKind.KeywordProcedure)) {
						// DROP { PROC | PROCEDURE } { [ schema_name. ] procedure } [ ,...n ]
						i++;
						bool isNextSprocName = true;

						while (isNextSprocName) {
							nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
							if (null != nextToken && nextToken.Type == TokenType.Identifier) {
								int index = i + 1;
								TokenInfo nextNextToken = InStatement.GetNextNonCommentToken(lstTokens, ref index);
								if (null != nextNextToken && nextNextToken.Kind == TokenKind.Dot) {
									index++;
									nextNextToken = InStatement.GetNextNonCommentToken(lstTokens, ref index);
									if (null != nextNextToken && nextNextToken.Type == TokenType.Identifier) {
										nextToken.TokenContextType = TokenContextType.SysObjectSchema;
										nextNextToken.TokenContextType = TokenContextType.Procedure;
									}
								} else {
									nextToken.TokenContextType = TokenContextType.Procedure;
								}
							}
							i++;
							nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
							if (!(null != nextToken && nextToken.Kind == TokenKind.Comma)) {
								isNextSprocName = false;
							} else {
								i++;
							}
						}
					}

					#endregion
				} else if (token.Kind == TokenKind.KeywordWith) {
					#region CTE

					KeywordCTE.ParseCTE(this, lstTokens, ref i);

					#endregion
				} else if (token.Kind == TokenKind.Comma) {
					#region a FROM statment or CTE

					// If comma, and previous statement was a tablesource try to match the next tokens as a table
					if (i > 0) {
						int prevIndex = InStatement.GetPreviousNonCommentToken(lstTokens, i - 1, i - 1);
						bool isTableToScan = false;
						foreach (TableSource tableSource in lstTableSources) {
							if (tableSource.EndIndex == prevIndex) {
								isTableToScan = true;
								break;
							}
						}

						// Either a table as defined above, or NOT a CTE
						if (isTableToScan) {
							// A tablesource with a comma before
							TableSource.ParseTableSource(this, currentStartSpan, lstSysObjects, ref i, lstTokens, token, ref sysObjectId);
						} else {
							// A possible new CTE. Need to recursivly check backwards for WITH statement
							// WITH alias1 (nCol, nCol) AS (SELECT nCol, nCol FROM tablesource), alias2 (nCol, nCol) AS (SELECT nCol, nCol FROM tablesource), alias3 (nCol, nCol) AS (SELECT nCol, nCol FROM tablesource)
							int currentIndex = InStatement.GetNextNonCommentToken(lstTokens, i, i);
							while (currentIndex > 1) {
								currentIndex--;
								TokenInfo previoustoken = InStatement.GetPreviousNonCommentToken(lstTokens, ref currentIndex);
								if (null != previoustoken && previoustoken.Kind == TokenKind.RightParenthesis) {
									currentIndex = previoustoken.MatchingParenToken;
									if (currentIndex > 1) {
										currentIndex--;
										previoustoken = InStatement.GetPreviousNonCommentToken(lstTokens, ref currentIndex);
										if (null != previoustoken && previoustoken.Kind == TokenKind.KeywordAs) {
											currentIndex--;
											previoustoken = InStatement.GetPreviousNonCommentToken(lstTokens, ref currentIndex);
											if (null != previoustoken && previoustoken.Kind == TokenKind.RightParenthesis) {
												currentIndex = InStatement.GetPreviousNonCommentToken(lstTokens, previoustoken.MatchingParenToken - 1, previoustoken.MatchingParenToken - 1);
											}
											previoustoken = InStatement.GetPreviousNonCommentToken(lstTokens, ref currentIndex);
											if (null != previoustoken && previoustoken.Type == TokenType.Identifier) {
												currentIndex--;
												previoustoken = InStatement.GetPreviousNonCommentToken(lstTokens, ref currentIndex);
												if (null != previoustoken && previoustoken.Kind == TokenKind.KeywordWith) {
													KeywordCTE.ParseCTE(this, lstTokens, ref i);
													break;
												}
												if (null != previoustoken && previoustoken.Kind == TokenKind.Comma) {
													// It might be another CTE. Continue scanning
												} else {
													break;
												}
											}
										}
									}
								} else {
									break;
								}
							}
						}
					}
					i++;

					#endregion
				} else if (token.Kind == TokenKind.KeywordGoto) {
					#region GOTO label

					i++;
					token = InStatement.GetNextNonCommentToken(lstTokens, ref i);
					if (null != token && token.Type == TokenType.Identifier) {
						calledLabels.Add(new Label(token.Token.UnqoutedImage, i));
						token.TokenContextType = TokenContextType.Label;
					}

					#endregion
				} else if (token.Kind == TokenKind.KeywordUse) {
					#region USE databasename

					i++;
					token = InStatement.GetNextNonCommentToken(lstTokens, ref i);
					if (null != token && token.Type == TokenType.Identifier) {
						token.TokenContextType = TokenContextType.Database;
					}

					#endregion
				} else if (token.Kind == TokenKind.KeywordExec) {
					#region EXECUTE or EXEC

					token = InStatement.GetNextNonCommentToken(lstTokens, i + 1);
					if (null != token) {
						if (token.Kind == TokenKind.LeftParenthesis) {
							// <Execute a character string>   or    <Execute a pass-through command against a linked server>
							KeywordExecute.ParseExecuteString(this, lstTokens, ref i);
						} else {
							// Execute a stored procedure or function
							KeywordExecute.ParseExecuteSprocOrFunction(this, lstTokens, ref i);
						}
					}
					i++;

					#endregion
				} else if (token.Type == TokenType.Label) {
					#region Label

					declaredLabels.Add(new Label(token.Token.UnqoutedImage, i));
					token.TokenContextType = TokenContextType.Label;
					i++;

					#endregion
				} else if (token.Kind == TokenKind.KeywordBegin) {
					#region BEGIN

					//BEGIN
					//     { 
					//        sql_statement | statement_block 
					//     } 
					//END
					//BEGIN { TRAN | TRANSACTION } 
					//    [ { transaction_name | @tran_name_variable }
					//      [ WITH MARK [ 'description' ] ]
					//    ]
					//[ ; ]
					//BEGIN DISTRIBUTED { TRAN | TRANSACTION } 
					//     [ transaction_name | @tran_name_variable ] 
					//[ ; ]
					//BEGIN TRY
					//     { sql_statement | statement_block }
					//END TRY
					//BEGIN CATCH
					//          [ { sql_statement | statement_block } ]
					//END CATCH
					//[ ; ]

					InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out token, TokenKind.KeywordDistributed);
					if (InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out token, TokenKind.KeywordTransaction)) {
						i++;
						token = InStatement.GetNextNonCommentToken(lstTokens, i);
						if (null != token) {
							if (token.Kind == TokenKind.ValueString) {
								token.TokenContextType = TokenContextType.TransactionName;
								declaredTransactions.Add(new TransactionLabel(token.Token.UnqoutedImage, i));
							} else if (token.Kind == TokenKind.Variable) {
								token.TokenContextType = TokenContextType.Variable;
								calledLocalVariables.Add(new LocalVariable(token.Token.UnqoutedImage, i));
							}

							if (InStatement.GetIfAllNextValidToken(lstTokens, ref i, out token, TokenKind.KeywordWith, TokenKind.KeywordMark)) {
								InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out token, TokenKind.ValueString);
							}
						}
					} else if (InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out token, TokenKind.KeywordTry)) {
					} else if (InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out token, TokenKind.KeywordCatch)) {
					} else {
						i++;
					}

					#endregion
				} else if (token.Kind == TokenKind.KeywordCommit) {
					#region COMMIT

					//COMMIT { TRAN | TRANSACTION } [ transaction_name | @tran_name_variable ] ]
					//[ ; ]
					//COMMIT [ WORK ]
					//[ ; ]

					if (InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out token, TokenKind.KeywordTransaction)) {
						i++;
						token = InStatement.GetNextNonCommentToken(lstTokens, i);
						if (null != token) {
							if (token.Kind == TokenKind.ValueString) {
								token.TokenContextType = TokenContextType.TransactionName;
								calledTransactions.Add(new TransactionLabel(token.Token.UnqoutedImage, i));
							} else if (token.Kind == TokenKind.Variable) {
								token.TokenContextType = TokenContextType.Variable;
								calledLocalVariables.Add(new LocalVariable(token.Token.UnqoutedImage, i));
							}
						}
					} else {
						InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out token, TokenKind.KeywordWork);
					}

					#endregion
				} else if (token.Kind == TokenKind.KeywordRollback) {
					#region ROLLBACK

					//ROLLBACK { TRAN | TRANSACTION } 
					//     [ transaction_name | @tran_name_variable | savepoint_name | @savepoint_variable ] 
					//[ ; ]
					//ROLLBACK [ WORK ]
					//[ ; ]

					if (InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out token, TokenKind.KeywordTransaction)) {
						i++;
						token = InStatement.GetNextNonCommentToken(lstTokens, i);
						if (null != token) {
							if (token.Kind == TokenKind.Name) {
								token.TokenContextType = TokenContextType.TransactionName;
								calledTransactions.Add(new TransactionLabel(token.Token.UnqoutedImage, i));
							} else if (token.Kind == TokenKind.Variable) {
								token.TokenContextType = TokenContextType.Variable;
								calledLocalVariables.Add(new LocalVariable(token.Token.UnqoutedImage, i));
							}
						}
					} else {
						if (!InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out token, TokenKind.KeywordWork)) {
							i++;
						}
					}

					#endregion
				} else if (token.Kind == TokenKind.KeywordSave) {
					#region SAVE

					//SAVE { TRAN | TRANSACTION } { savepoint_name | @savepoint_variable }
					//[ ; ]

					if (InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out token, TokenKind.KeywordTransaction)) {
						i++;
						token = InStatement.GetNextNonCommentToken(lstTokens, i);
						if (null != token) {
							if (token.Kind == TokenKind.Name) {
								token.TokenContextType = TokenContextType.TransactionName;
								declaredTransactions.Add(new TransactionLabel(token.Token.UnqoutedImage, i));
							} else if (token.Kind == TokenKind.Variable) {
								token.TokenContextType = TokenContextType.Variable;
								calledLocalVariables.Add(new LocalVariable(token.Token.UnqoutedImage, i));
							}
						}
					} else {
						InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out token, TokenKind.KeywordWork);
					}

					#endregion
				} else if (token.Kind == TokenKind.KeywordTruncate) {
					#region TRUNCATE

					i++;
					TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref i);
					if (null != nextToken && nextToken.Kind == TokenKind.KeywordTable) {
						// TRUNCATE TABLE [ database_name . [ schema_name ] . | schema_name . ] table_name [ ; ]
						i++;
						// Normal multi part name
						TokenInfo server_name;
						TokenInfo database_name;
						TokenInfo schema_name;
						TokenInfo object_name;
						int endTableIndex;
						if (ParseTableOrViewName(i, out endTableIndex, out server_name, out database_name, out schema_name, out object_name)) {
							// Move the pointer (i) to the token after the table name
							i = endTableIndex;
						}
						i++;
					}

					#endregion
				} else {
					// Skip to next token
					i++;
				}

				if (i < 0) {
					return;
				}
				// Safety check that we don't get a never-ending loop
				if (currentI >= i) {
					Common.LogEntry(ClassName, "ParseTokenRange", "Never ending loop detected. previous i=" + currentI + ", and new i=" + i, Common.enErrorLvl.Warning);
					i = currentI + 1;
				}
			}
		}

		#endregion

		#region Debug

		internal static void DumpTokens(List<TokenInfo> tokens, int startIndex, int endIndex) {
			if (endIndex >= tokens.Count) {
				endIndex = tokens.Count - 1;
			}
			if (startIndex >= tokens.Count) {
				startIndex = tokens.Count - 1;
			}
			string expression = "";
			for (int i = startIndex; i <= endIndex; i++) {
				expression += tokens[i].Token.UnqoutedImage + " ";
			}
			//			Common.LogEntry(ClassName, "xxx", startIndex + " till " + endIndex + " (" + expression + ")", Common.enErrorLvl.Information);
		}

		#endregion

		#region Create table & sysobject

		private void CreateScannedTable(Token startToken, List<SysObject> lstSysObjects, ScannedTable scannedTable) {
			try {
				// Create a sysObjects entry for dervied/temporary table
				SysObject sysObject = SysObject.CreateSysObject(this, lstSysObjects, lstTokens, scannedTable, ref sysObjectId);

				if (scannedTable.SqlType == Common.enSqlTypes.DerivedTable || scannedTable.SqlType == Common.enSqlTypes.CTE) {
					StatementSpans ss;
					Common.GetStatementSpan(scannedTable.Span, segmentUtils.StartTokenIndexes, scannedTable.ParenLevel, out ss);
					Table table = new Table(scannedTable.Servername, scannedTable.Databasename, scannedTable.Schema, scannedTable.Name, scannedTable.Alias, sysObject, ss, scannedTable.StartTableIndex, scannedTable.EndTableIndex, false);

					int endIndex;
					if (scannedTable.SqlType == Common.enSqlTypes.DerivedTable) {
						endIndex = scannedTable.EndIndex;
						// A derived table can add it's column names after the alias
						int index = InStatement.GetNextNonCommentToken(lstTokens, endIndex + 1, endIndex + 1);
						TokenInfo token = InStatement.GetNextNonCommentToken(lstTokens, ref index);
						if (null != token) {
							int matchingParenIndex = token.MatchingParenToken;
							if (token.Kind == TokenKind.LeftParenthesis && -1 != matchingParenIndex && matchingParenIndex > index) {
								endIndex = matchingParenIndex;
							}
						}
					} else if (scannedTable.SqlType == Common.enSqlTypes.CTE) {
						endIndex = scannedTable.EndIndex;
					} else {
						endIndex = InStatement.GetPreviousNonCommentToken(lstTokens, lstTokens.Count - 1, lstTokens.Count - 1);
						Common.LogEntry(ClassName, "CreateScannedTable", "Unhandled sqltype " + scannedTable, Common.enErrorLvl.Error);
					}

					lstTableSources.Add(new TableSource(startToken, table, scannedTable.StartIndex, endIndex));
				} else if (scannedTable.SqlType == Common.enSqlTypes.Temporary) {
					// Create temporary table
					int startIndex = InStatement.GetNextNonCommentToken(lstTokens, scannedTable.StartIndex + 1, scannedTable.StartIndex + 1);
					int endIndex = InStatement.GetNextNonCommentToken(lstTokens, scannedTable.EndIndex + 1, scannedTable.EndIndex + 1);
					if (0 == startIndex) {
						startIndex = endIndex;
					}
					TextSpan span = TextSpanHelper.CreateSpanFromTokens(lstTokens[startIndex], lstTokens[endIndex]);
					StatementSpans statementSpan = new StatementSpans(lstTokens[startIndex], startIndex, lstTokens[endIndex], endIndex, scannedTable.ParenLevel, true) {
						SegmentStartToken = null
					};
					segmentUtils.StartTokenIndexes.Add(statementSpan);
					TemporaryTable tempTable = new TemporaryTable(scannedTable.Name, sysObject, span, scannedTable.ParenLevel, statementSpan, startIndex, endIndex);
					lstTemporaryTables.Add(tempTable);
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "CreateScannedTable", e, Common.enErrorLvl.Error);
			}
		}

		/// <summary>
		/// Create an Table object
		/// </summary>
		/// <param name="currentStartSpan"></param>
		/// <param name="tokenContext"></param>
		/// <param name="lstSysObjects"></param>
		/// <param name="Servername"></param>
		/// <param name="Databasename"></param>
		/// <param name="Schema"></param>
		/// <param name="Table"></param>
		/// <param name="Alias"></param>
		/// <param name="startIndex"></param>
		/// <param name="endIndex"></param>
		/// <param name="startTableIndex"></param>
		/// <param name="endTableIndex"></param>
		/// <param name="isTemporary"></param>
		/// <param name="addedSysObject"></param>
		/// <param name="addedTableSource"></param>
		internal bool CreateTableEntry(StatementSpans currentStartSpan, Token tokenContext, List<SysObject> lstSysObjects, string Servername, string Databasename, string Schema, string Table, string Alias, int startIndex, int endIndex, int startTableIndex, int endTableIndex, bool isTemporary, out SysObject addedSysObject, out TableSource addedTableSource) {
			try {
				addedSysObject = null;
				// First see if we need to parse a temporary table. Mostly it's from an SELECT.. INTO #temptable
				foreach (ScannedTable scannedTable in lstScannedTables) {
					if ((scannedTable.SqlType == Common.enSqlTypes.Temporary || scannedTable.SqlType == Common.enSqlTypes.CTE) && Table.Equals(scannedTable.Name, StringComparison.OrdinalIgnoreCase)) {
						addedSysObject = SysObject.CreateSysObject(this, lstSysObjects, lstTokens, scannedTable, ref sysObjectId);
						lstScannedTables.Remove(scannedTable);
						break;
					}
				}

				// Create a Table object
				SysObject sysObject;
				if (null != Instance.TextEditor.ActiveConnection && Instance.TextEditor.ActiveConnection.SysObjectExists(Table, out sysObject)) {
					if (null == addedSysObject) {
						addedSysObject = sysObject;
					}
					StatementSpans ss = segmentUtils.GetStatementSpan(startTableIndex);
					Table table = new Table(Servername, Databasename, Schema, Table, Alias, sysObject, ss, startTableIndex, endTableIndex, isTemporary);
					addedTableSource = new TableSource(tokenContext, table, startIndex, endIndex);
					lstTableSources.Add(addedTableSource);
					if (null != currentStartSpan) {
						currentStartSpan.AddTableSource(addedTableSource);
					}
					return true;
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "CreateTableEntry", e, Common.enErrorLvl.Error);
			}

			addedSysObject = null;
			addedTableSource = null;
			return false;
		}

		#endregion

		#region Scan for errors

		/// <summary>
		/// Scan for errors. It handles schemas and aliases for:
		/// * rowset functions
		/// * #temp objects
		/// * @table variables
		/// * derived tables
		/// * cte
		/// * tables
		/// * views
		/// * table valued functions
		/// </summary>
		/// <param name="bgWorkerScanForErrors"></param>
		/// <returns>True if all went well, else false</returns>
		public bool ScanForErrors(BackgroundWorker bgWorkerScanForErrors) {
			if (null == segmentUtils || null == TextEditor.CurrentWindowData || null == TextEditor.CurrentWindowData.PrimaryActiveView || null == Instance.TextEditor.SysObjects) {
				return false;
			}

			ScannedSqlErrors = new List<ScannedSqlError>();
			try {
				try {
					// Validate sysobject?
					if (Instance.Settings.ScanErrorValidateSysObjects) {
						ValidateSysObjects();
					}

					foreach (TableSource tableSource in lstTableSources) {
						int startIndex = (tableSource.Table.SysObject.SqlType == Common.enSqlTypes.DerivedTable ? tableSource.Table.EndIndex : tableSource.Table.StartIndex);
						int endIndex = tableSource.Table.EndIndex;
						int tokenIndex = tableSource.StartIndex;

						// Require tablesource alias?
						if (Instance.Settings.ScanForErrorRequireTableSourceAlias) {
							if (0 == tableSource.Table.Alias.Length) {
								if (tableSource.TokenContext != Tokens.kwInsertToken && tableSource.TokenContext != Tokens.kwDeleteToken && tableSource.TokenContext != Tokens.kwUpdateToken && tableSource.TokenContext != Tokens.kwIntoToken) {
									ScannedSqlErrors.Add(new ScannedSqlError("No tablesource alias found", new SqlErrorAddSysObjectAlias(this, tableSource), startIndex, endIndex, tokenIndex));
								}
							}
						}

						// Require tablesource schema?
						if (Instance.Settings.ScanForErrorRequireSchema) {
							if (0 == tableSource.Table.Schema.Length && !tableSource.Table.SysObject.IsTemporary) {
								ScannedSqlErrors.Add(new ScannedSqlError("No tablesource schema found", new SqlErrorAddSysObjectSchema(tableSource), startIndex, endIndex, tokenIndex));
							}
						}

						// Require AS token?
						if (Instance.Settings.ScanForErrorRequireTokenAs) {
							if (tableSource.TokenContext != Tokens.kwInsertToken && tableSource.TokenContext != Tokens.kwDeleteToken && tableSource.TokenContext != Tokens.kwUpdateToken) {
								if (tableSource.Table.Alias.Length > 0) {
									int indexAs = tableSource.Table.EndIndex - 1;
									if (indexAs >= tableSource.Table.StartIndex) {
										if (lstTokens[indexAs].Kind != TokenKind.KeywordAs) {
											ScannedSqlErrors.Add(new ScannedSqlError("No AS keyword found", new SqlErrorAddSysObjectAlias(this, tableSource), startIndex, endIndex, tokenIndex));
										}
									} else {
										ScannedSqlErrors.Add(new ScannedSqlError("No AS keyword found", new SqlErrorAddSysObjectAlias(this, tableSource), startIndex, endIndex, tokenIndex));
									}
								}
							}
						}
					}

					// Require alias on column?
					if (Instance.Settings.ScanForErrorRequireColumnTableAlias) {
						ValidateColumnAliases();
					}

					// Unknown tokens creates a squiggle?
					if (Instance.Settings.ScanForUnknownTokens) {
						ValidateUnknownTokens();
					}

					// Scan for missing ...
					Label.ScanForLabelErrors(this);
					TransactionLabel.ScanForTransactionLabelErrors(this);
					Cursor.ScanForCursorErrors(this);
					LocalVariable.ScanForVariableErrors(this);

					ScanForEqualsNull();

				} catch (Exception e) {
					Common.LogEntry(ClassName, "ScanForErrors", e, Common.enErrorLvl.Error);
				}

				// Copy errors on same tablesource to all same tablesources
				for (int i = 0; i < ScannedSqlErrors.Count; i++) {
					ScannedSqlError scannedSqlError1 = ScannedSqlErrors[i];
					for (int j = i + 1; j < ScannedSqlErrors.Count; j++) {
						ScannedSqlError scannedSqlError2 = ScannedSqlErrors[j];
						if (scannedSqlError1.IsSameTableSource(scannedSqlError2)) {
							scannedSqlError1.AddScannedSqlError(scannedSqlError2);
							scannedSqlError2.AddScannedSqlError(scannedSqlError1);
						}
					}
				}

				return true;
			} catch (Exception e) {
				Common.LogEntry(ClassName, "ScanForErrors", e, Common.enErrorLvl.Error);
				return false;
			}
		}

		/// <summary>
		/// Scan for "= NULL"
		/// </summary>
		private void ScanForEqualsNull() {
			int index = 0;
			while (index < lstTokens.Count) {
				TokenInfo token = InStatement.GetNextNonCommentToken(lstTokens, ref index);
				index++;
				TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref index);

				if (null != token && null != nextToken && token.Kind == TokenKind.Assign && nextToken.Kind == TokenKind.KeywordNull) {
					Common.SegmentType segmentType = segmentUtils.GetCurrentSegmentType(null, index);
					if (segmentType != Common.SegmentType.START) {
						ScannedSqlErrors.Add(new ScannedSqlError("Using = NULL is not recommended. Use IS NULL instead", index, index, index));
					}
				}
				index++;
			}
		}

		/// <summary>
		/// Scan unknown tokens (from TokenContextType)
		/// </summary>
		private void ValidateUnknownTokens() {
			int index = 0;
			while (index < lstTokens.Count) {
				TokenInfo token = lstTokens[index];

				if (token.Type == TokenType.Identifier && token.TokenContextType == TokenContextType.Unknown) {
					foreach (ColumnAlias columnAlias in declaredColumnAliases) {
						if (columnAlias.Name.Equals(token.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase)) {
							token.TokenContextType = TokenContextType.ColumnAlias;
							break;
						}
					}
					if (token.TokenContextType == TokenContextType.Unknown) {
						ScannedSqlErrors.Add(new ScannedSqlError("Unknown token name", index, index, index));
					}
				}
				index++;
			}
		}

		/// <summary>
		/// Validate table sources
		/// </summary>
		private void ValidateSysObjects() {
			int index = 0;

			Connection activeConnection = Instance.TextEditor.ActiveConnection;
			Server activeServer = Instance.TextEditor.ActiveServer;
			if (null == activeConnection || null == activeServer) {
				return;
			}

			while (index < lstTokens.Count) {
				TokenInfo token = lstTokens[index];

				string unqoutedImage = token.Token.UnqoutedImage;
				List<SysObjectSchema> schemas;
				SysObject sysObject;

				switch (token.TokenContextType) {
					case TokenContextType.Unknown:
						if (token.Kind == TokenKind.Name) {
							schemas = activeConnection.GetSysObjectSchemas();
							foreach (SysObjectSchema schema in schemas) {
								if (unqoutedImage.Equals(schema.Schema, StringComparison.OrdinalIgnoreCase)) {
									token.TokenContextType = TokenContextType.SysObjectSchema;
									break;
								}
							}
							if (token.TokenContextType == TokenContextType.Unknown) {
								if (activeConnection.SysObjectExists(unqoutedImage, out sysObject)) {
									token.TokenContextType = TokenContextType.SysObject;
								}
							}
						}
						break;
					case TokenContextType.Server:
						// Validate server name
						bool serverNameIsOk = false;
						List<SysServer> sysServers = activeServer.GetSysServers();
						foreach (SysServer sysServer in sysServers) {
							if (unqoutedImage.Equals(sysServer.ServerName, StringComparison.OrdinalIgnoreCase)) {
								serverNameIsOk = true;
								break;
							}
						}
						if (!serverNameIsOk) {
							ScannedSqlErrors.Add(new ScannedSqlError("Unknown server name", index, index, index));
						}
						break;
					case TokenContextType.Database:
						// Validate database name
						bool databaseNameIsOk = false;
						List<Database> databases = activeServer.GetDataBases();
						foreach (Database database in databases) {
							if (unqoutedImage.Equals(database.DataBaseName, StringComparison.OrdinalIgnoreCase)) {
								databaseNameIsOk = true;
								break;
							}
						}
						if (!databaseNameIsOk) {
							ScannedSqlErrors.Add(new ScannedSqlError("Unknown database name", index, index, index));
						}
						break;
					case TokenContextType.SysObjectSchema:
						// Validate schema name
						bool schemaNameIsOk = false;
						schemas = activeConnection.GetSysObjectSchemas();
						foreach (SysObjectSchema schema in schemas) {
							if (unqoutedImage.Equals(schema.Schema, StringComparison.OrdinalIgnoreCase)) {
								schemaNameIsOk = true;
								break;
							}
						}
						if (!schemaNameIsOk) {
							ScannedSqlErrors.Add(new ScannedSqlError("Unknown schema name", index, index, index));
						}
						break;
					case TokenContextType.TempTable:
					case TokenContextType.SysObject:
						// Validate sysobject name
						if (!activeConnection.SysObjectExists(unqoutedImage, out sysObject)) {
							ScannedSqlErrors.Add(new ScannedSqlError("Unknown sysobject name", index, index, index));
						}
						break;
				}
				index++;
			}
		}

		/// <summary>
		/// Validate that column alias are correct, i.e. that they exists for sysobject columns
		/// </summary>
		private void ValidateColumnAliases() {
			if (null == segmentUtils.StartTokenIndexesSortByInnerLevel) {
				return;
			}

			List<int> handledTokenIndexes = new List<int>();

			foreach (StatementSpans span in segmentUtils.StartTokenIndexesSortByInnerLevel) {
				List<TableSource> lstFoundTableSources;
				if (segmentUtils.GetStatementSpanTablesources(span, out lstFoundTableSources)) {
					int index = span.StartIndex;
					while (index <= span.EndIndex) {
						TokenInfo token = lstTokens[index];
						int arrayIndex;

						if (token.TokenContextType == TokenContextType.SysObjectAlias && (arrayIndex = handledTokenIndexes.BinarySearch(index)) < 0) {
							handledTokenIndexes.Insert(~arrayIndex, index);
							bool foundAlias = false;
							foreach (TableSource tableSource in lstFoundTableSources) {
								if (tableSource.Table.Alias.Equals(token.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase) || tableSource.Table.Alias.Equals(token.Token.Image, StringComparison.OrdinalIgnoreCase)) {
									foundAlias = true;
									break;
								}
							}
							if (!foundAlias) {
								ScannedSqlErrors.Add(new ScannedSqlError("Unknown sysobject alias '" + token.Token.Image + "'", null, index, index, index));
							}

						} else if (token.TokenContextType == TokenContextType.SysObjectColumn && (arrayIndex = handledTokenIndexes.BinarySearch(index)) < 0) {
							handledTokenIndexes.Insert(~arrayIndex, index);

							int offset = index - 1;
							bool foundCorrectSysobject = false;
							string column = token.Token.UnqoutedImage;
							int nbOfColumnMatches = 0;
							List<TableSource> lstDuplicateTokens = new List<TableSource>();
							string alias = "";

							TokenInfo prevToken = InStatement.GetPreviousNonCommentToken(lstTokens, ref offset);
							if (null != prevToken && prevToken.Kind == TokenKind.Dot) {
								offset--;
								prevToken = InStatement.GetPreviousNonCommentToken(lstTokens, ref offset);
								if (null != prevToken) {
									alias = prevToken.Token.UnqoutedImage;

									switch (prevToken.TokenContextType) {
										case TokenContextType.SysObjectAlias:
											foreach (TableSource tableSource in lstFoundTableSources) {
												if (tableSource.Table.Alias.Equals(prevToken.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase) || tableSource.Table.Alias.Equals(prevToken.Token.Image, StringComparison.OrdinalIgnoreCase)) {
													foundCorrectSysobject = true;
													foreach (SysObjectColumn sysObjectColumn in tableSource.Table.SysObject.Columns) {
														if (sysObjectColumn.ColumnName.Equals(token.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase) || sysObjectColumn.ColumnName.Equals(token.Token.Image, StringComparison.OrdinalIgnoreCase)) {
															nbOfColumnMatches++;
															lstDuplicateTokens.Add(tableSource);
														}
													}
													break;
												}
											}
											break;

										case TokenContextType.SysObject:
											foreach (TableSource tableSource in lstFoundTableSources) {
												if (tableSource.Table.TableName.Equals(prevToken.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase) || tableSource.Table.TableName.Equals(prevToken.Token.Image, StringComparison.OrdinalIgnoreCase)) {
													foundCorrectSysobject = true;
													foreach (SysObjectColumn sysObjectColumn in tableSource.Table.SysObject.Columns) {
														if (sysObjectColumn.ColumnName.Equals(token.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase) || sysObjectColumn.ColumnName.Equals(token.Token.Image, StringComparison.OrdinalIgnoreCase)) {
															nbOfColumnMatches++;
															lstDuplicateTokens.Add(tableSource);
														}
													}
													break;
												}
											}
											break;
									}
								}
							}
							bool statmentSpanDoesntNeedAlias = (span.SegmentStartToken.StartToken == Tokens.kwDeleteToken || span.SegmentStartToken.StartToken == Tokens.kwUpdateToken);

							if (!foundCorrectSysobject) {
								foreach (TableSource tableSource in lstFoundTableSources) {
									foreach (SysObjectColumn sysObjectColumn in tableSource.Table.SysObject.Columns) {
										if (sysObjectColumn.ColumnName.Equals(token.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase) || sysObjectColumn.ColumnName.Equals(token.Token.Image, StringComparison.OrdinalIgnoreCase)) {
											nbOfColumnMatches++;
											lstDuplicateTokens.Add(tableSource);
										}
									}
								}
								if (statmentSpanDoesntNeedAlias) {
									foreach (TableSource tableSource in lstDuplicateTokens) {
										if (tableSource.TokenContext == Tokens.kwUpdateToken || tableSource.TokenContext == Tokens.kwDeleteToken) {
											int indexEndMain = segmentUtils.GetNextMainTokenEnd(span, span.StartIndex);
											if (index >= tableSource.StartIndex && index < indexEndMain) {
												nbOfColumnMatches = 1;
												foundCorrectSysobject = true;
											}
											break;
										}
									}
								}
							}

							if (0 == nbOfColumnMatches) {
								if (alias.Length > 0) {
									ScannedSqlErrors.Add(new ScannedSqlError("Unknown column name in sysobject with alias '" + alias + "'", index, index, index));
								} else {
									ScannedSqlErrors.Add(new ScannedSqlError("Unknown column name", index, index, index));
								}
							} else if (nbOfColumnMatches == 1 && !foundCorrectSysobject) {
								if (alias.Length > 0) {
									ScannedSqlErrors.Add(new ScannedSqlError("Unknown column name in sysobject with alias '" + alias + "' in this context", index, index, index));
								} else {
									if (!statmentSpanDoesntNeedAlias) {
										TableSource tableSource = lstDuplicateTokens[0];
										if (tableSource.Table.Alias.Length > 0) {
											ScannedSqlErrors.Add(new ScannedSqlError("No sysobject alias found, but column '" + column + "' was found in table source '" + tableSource.Table.TableName + "'.", new SqlErrorAddColumnAlias(tableSource), index, index, index));
										} else {
											ScannedSqlErrors.Add(new ScannedSqlError("No sysobject alias found", new SqlErrorAddColumnAlias(tableSource), index, index, index));
										}
									}
								}
							} else if (nbOfColumnMatches > 1) {
								ScannedSqlErrors.Add(new ScannedSqlError("Ambiguous column", new SqlErrorAmbigousColumn(lstDuplicateTokens), index, index, index));
							}
						}
						index++;
					}
				}
			}
		}

		#endregion

		#region Handle multi part names

		/// <summary>
		/// Parse a tokenlist for Multipart Names
		/// Multipart names can have the following formats
		///   server_name.[database_name].[schema_name].object_name
		/// | database_name.[schema_name].object_name
		/// | schema_name.object_name
		/// | object_name
		/// </summary>
		/// <param name="startIndex"></param>
		/// <param name="endIndex"></param>
		/// <param name="server_name"></param>
		/// <param name="database_name"></param>
		/// <param name="schema_name"></param>
		/// <param name="object_name"></param>
		/// <returns>True if all went well, else false</returns>
		public bool ParseTableOrViewName(int startIndex, out int endIndex, out TokenInfo server_name, out TokenInfo database_name, out TokenInfo schema_name, out TokenInfo object_name) {
			List<TokenInfo> tokens = new List<TokenInfo>(10);
			bool blnAllOk = true;
			int offset = startIndex;

			try {
				TokenInfo prevToken = null;

				while (offset < lstTokens.Count) {
					TokenInfo token = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
					if (null == token) {
						break;
					}

					// The two tokens need to be adjacent to each others
					if (null != prevToken && !TextSpanHelper.IsAdjacent(lstTokens[offset - 1].Span, token.Span)) {
						break;
					}

					if (!(TokenType.Identifier == token.Type || TokenType.Delimiter == token.Type)) {
						break;
					}

					if (TokenType.Identifier == token.Type) {
						if (null != prevToken && TokenType.Identifier == prevToken.Type) {
							// Two identifiers in a row is illegal
							break;
						}
						tokens.Add(token);
						//tokens.Insert(0, token);
					} else if (TokenType.Delimiter == token.Type && TokenKind.Dot != token.Kind) {
						break;
					} else {
						if (null == prevToken) {
							blnAllOk = false;
							break;
						}
						if (TokenType.Delimiter == prevToken.Type && TokenKind.Dot == prevToken.Kind) {
							// Two dots in a row - insert a empty item
							tokens.Add(null);
							//tokens.Insert(0, null);
						} else if (token.Kind != TokenKind.Dot) {
							break;
						}
					}

					prevToken = token;
					offset++;
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "ParseTableOrViewName", e, Common.enErrorLvl.Error);
				blnAllOk = false;
			}

			int i = tokens.Count;
			object_name = (i > 0 ? tokens[--i] : null);
			if (null != object_name) {
				object_name.TokenContextType = TokenContextType.SysObject;
			}

			schema_name = (i > 0 ? tokens[--i] : null);
			if (null != schema_name) {
				schema_name.TokenContextType = TokenContextType.SysObjectSchema;
			}

			database_name = (i > 0 ? tokens[--i] : null);
			if (null != database_name) {
				database_name.TokenContextType = TokenContextType.Database;
			}

			server_name = (i > 0 ? tokens[--i] : null);
			if (null != server_name) {
				server_name.TokenContextType = TokenContextType.Server;
			}

			// Set the endIndex to be the previous non-comment token
			endIndex = InStatement.GetPreviousNonCommentToken(lstTokens, offset - 1, offset - 1);

			if (0 == tokens.Count) {
				blnAllOk = false;
			}

			return blnAllOk;
		}

		#endregion

		#region Utilities

		/// <summary>
		/// Find the table alias.
		/// </summary>
		/// <param name="index">Index shall point to the token after the table/view/etc name</param>
		/// <param name="endIndex"></param>
		/// <param name="Alias"></param>
		internal bool ExtractTableAlias(int index, ref int endIndex, out string Alias) {
			Alias = string.Empty;
			bool foundAsToken = false;

			try {
				TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref index);
				if (null != nextToken) {
					// Keyword AS ? Just skip it
					if (nextToken.Kind == TokenKind.KeywordAs) {
						foundAsToken = true;
						endIndex = index;
						index++;
						nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref index);
						if (null == nextToken) {
							// No more tokens, i.e. no alias
							return true;
						}
					}

					// Get the alias
					int aliasIndex = index;
					index++;
					TokenInfo nextNextToken = InStatement.GetNextNonCommentToken(lstTokens, ref index);
					if (Common.IsIdentifier(nextToken, nextNextToken)) {
						Alias = nextToken.Token.UnqoutedImage;
						nextToken.TokenContextType = TokenContextType.SysObjectAlias;

						// Only advance one step if there is a token and it's not a reserverd word unless we found the AS token
						if (null != nextNextToken && (foundAsToken || !Tokens.isReservedWord(nextNextToken))) {
							if (endIndex + 1 < lstTokens.Count) {
								endIndex = InStatement.GetNextNonCommentToken(lstTokens, endIndex + 1, endIndex + 1);
							}
						} else {
							endIndex = aliasIndex;
						}
						return true;
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "ExtractTableAlias", e, Common.enErrorLvl.Error);
			}

			return false;
		}

		#endregion

		#region IDisposable Members

		///<summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose() {
		}

		#endregion

		public void DisposeMarkers() {
			if (null != markers) {
				markers.Dispose();
				markers = null;
			}
		}
	}
}
