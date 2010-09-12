// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using System;
using System.Collections.Generic;
using EnvDTE;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.Parsing;
using Sassner.SmarterSql.ParsingObjects;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;
using Sassner.SmarterSql.UI.Controls;
using Sassner.SmarterSql.Utils.Tooltips;

namespace Sassner.SmarterSql.Utils {
	public class Intellisense : IDisposable {
		#region Member variables

		private const string ClassName = "Intellisense";

		private readonly TextEditor textEditor;
		private bool isDisposed;

		#endregion

		public Intellisense(TextEditor textEditor) {
			this.textEditor = textEditor;
		}

		#region HandleIntellisenseItem

		public void HandleIntellisenseItem(string Keypress, TextSelection Selection) {
			if (isDisposed || null == TextEditor.CurrentWindowData || null == TextEditor.CurrentWindowData.Parser) {
				return;
			}

			try {
				Parser parser = TextEditor.CurrentWindowData.Parser;
				List<TokenInfo> lstTokens = parser.RawTokens;

				#region Set things up

				textEditor.NeedToRedrawDebugWindow = true;

				int currentLine;
				int currentColumn;

				IVsTextView activeView = TextEditor.CurrentWindowData.ActiveView;
				activeView.GetCaretPos(out currentLine, out currentColumn);
				IVsTextLines ppBuffer;
				activeView.GetBuffer(out ppBuffer);
				int lineLength;
				string currentLineText;
				ppBuffer.GetLengthOfLine(currentLine, out lineLength);
				ppBuffer.GetLineText(currentLine, 0, currentLine, lineLength, out currentLineText);

				// Get an EditPoint at the current position
				EditPoint curSel = Selection.ActivePoint.CreateEditPoint();

				// Split the current row in two
				string leftOfCursor = string.Empty;
				string rightOfCursor = string.Empty;
				if (currentLineText.Length >= currentColumn) {
					leftOfCursor = currentLineText.Substring(0, currentColumn);
					rightOfCursor = currentLineText.Substring(currentColumn);
				}
				string wordLeftOfCursor = string.Empty;
				string wordRightOfCursor = string.Empty;

				// Get current token
				bool IsInsideToken;
				TokenInfo prevToken = null;
				TokenInfo prevprevToken = null;
				TokenInfo prevprevprevToken = null;
				int tokenIndex = TextEditor.GetTokenIndex(lstTokens, currentLine, currentColumn, out IsInsideToken);
				if (-1 != tokenIndex) {
					lock (lstTokens) {
						prevToken = lstTokens[tokenIndex];
						if (prevToken.Token is ErrorToken) {
							return;
						}
						if (tokenIndex > 0) {
							prevprevToken = lstTokens[tokenIndex - 1];
							if (tokenIndex - 1 > 0) {
								prevprevprevToken = lstTokens[tokenIndex - 2];
							}
						}
					}

					try {
						if (currentColumn - prevToken.Span.iStartIndex < prevToken.Token.UnqoutedImage.Length && currentColumn - prevToken.Span.iStartIndex > 0) {
							// Get the text we just wrote. Mostly it's a NameToken, but can also be SymbolToken etc
							wordLeftOfCursor = prevToken.Token.UnqoutedImage.Substring(0, currentColumn - prevToken.Span.iStartIndex);
							wordRightOfCursor = prevToken.Token.UnqoutedImage.Substring(currentColumn - prevToken.Span.iStartIndex);
						} else {
							wordLeftOfCursor = prevToken.Token.UnqoutedImage;
						}
					} catch (Exception) {
						// Do nothing
					}

					textEditor.IsInComment = false;
					textEditor.IsInString = false;

					if (prevToken.Kind == TokenKind.SingleLineComment && currentLine == prevToken.Span.iStartLine) {
						textEditor.IsInComment = true;
					} else if (prevToken.Kind == TokenKind.MultiLineComment) {
						textEditor.IsInComment = true;
					} else if (prevToken.Kind == TokenKind.ValueString && IsInsideToken) {
						textEditor.IsInString = true;
					}
				}

				#endregion

				if (Instance.Settings.EnableAutoInsertPairParanthesesAndQuotes && !textEditor.IsInComment && Keypress[0] == Common.chrKeyCitation) {
					#region Handle '

					//  Before					   After insert          After code run
					//select 'johan_			select 'johan'_			select 'johan'_			-- Even numbers to the left, nothing to the right. Don't do anything
					//
					//select 'johan_'			select 'johan'_'		select 'johan'_			-- Even numbers to the left, exists to the right. Delete one right
					//select 'johan''''_'		select 'johan'''''_'	select 'johan'''''_		-- Even numbers to the left, exists to the right. Delete one right
					//select 'johan''_'			select 'johan'''_'		select 'johan'''_		-- Even numbers to the left, exists to the right. Delete one right
					//
					//select 'johan'_			select 'johan''_		select 'johan''_'		-- Odd numbers to the left, nothing to the right. Add and move left
					//select 'johan'''_			select 'johan''''_		select 'johan''''_'		-- Odd numbers to the left, nothing to the right. Add and move left
					//
					//select 'johan'_''			select 'johan''_''		select 'johan''_'''		-- Odd numbers to the left, exists to the right. Add and move left
					//select 'johan'''_'		select 'johan''''_'		select 'johan'''''_		-- Odd numbers to the left, exists to the right. Add and move left

					int nbOfCitationLeft = leftOfCursor.Length - leftOfCursor.Replace("'", string.Empty).Length;
					bool existsToRight = rightOfCursor.StartsWith("'");
					if (0 == rightOfCursor.Length || rightOfCursor.StartsWith(" ") || rightOfCursor.StartsWith("\t") || existsToRight) {
						if (nbOfCitationLeft % 2 == 0 && !existsToRight) {
							// Even numbers to the left, nothing to the right. Don't do anything
						} else if (nbOfCitationLeft % 2 == 0 && existsToRight) {
							// Even numbers to the left, exists to the right. Delete one right
							Selection.Delete(1);
						} else if (nbOfCitationLeft % 2 == 1) {
							// Odd numbers to the left, ignoring to the right. Add and move left
							Selection.Insert("'", (int)vsInsertFlags.vsInsertFlagsCollapseToStart);
						}
					}

					textEditor.HidePopupWindow(false);

					#endregion

					return;
				}

				if (textEditor.IsInString || textEditor.IsInComment) {
					textEditor.PopupWindow.CanShowPopup = false;
				} else {
					if (Keypress[0] == Common.chrKeyEnter || Keypress[0] == Common.chrKeyTab || Keypress[0] == Common.chrKeySpace || Keypress[0] == Common.chrKeyDelete || Keypress[0] == Common.chrKeyBackSpace) {
						#region Do nothing

						#endregion
					} else if (Keypress.Equals("@")) {
						#region Handle @

						textEditor.PopupWindow.CanShowPopup = true;
						PopupWindow.ShowPopupWithVariables(textEditor, curSel, prevprevToken, "@", wordRightOfCursor, tokenIndex);

						#endregion
					} else if (Keypress.Equals(".")) {
						#region Handle .

						if (null != prevToken && prevToken.Kind == TokenKind.ValueNumber) {
							// Do nothing
						} else {
							if (null != prevprevToken && currentLine == prevprevToken.Span.iStartLine) {
								ShowPopupAfterDot(tokenIndex, curSel, prevToken, prevprevToken, string.Empty, wordRightOfCursor);
							}
						}

						#endregion
					} else if (Keypress.Equals(",")) {
						#region Handle ,

						if (-1 != tokenIndex && null != prevToken && !textEditor.ToolTipMethodInfo.ToolTipEnabled && !textEditor.ToolTipMethodInfo.Visible) {
							while (tokenIndex >= 0) {
								TokenInfo token = lstTokens[tokenIndex];
								if (token.Kind == TokenKind.RightParenthesis && -1 != token.MatchingParenToken && token.MatchingParenToken < tokenIndex) {
									tokenIndex = token.MatchingParenToken;
								} else if (token.Kind == TokenKind.LeftParenthesis) {
									tokenIndex--;
									if (tokenIndex >= 0) {
										token = lstTokens[tokenIndex];
										if (token.Span.iStartLine == prevToken.Span.iStartLine && token.Type == TokenType.Keyword) {
											foreach (Method method in textEditor.StaticData.Methods) {
												if (method.MethodToken.Kind == token.Kind) {
													// Find end token (either a paranthesis or a new line
													tokenIndex++;
													if (tokenIndex < lstTokens.Count) {
														int colStart = lstTokens[tokenIndex].Span.iStartIndex;
														int colEnd = -1;

														if (-1 != lstTokens[tokenIndex].MatchingParenToken && tokenIndex < lstTokens[tokenIndex].MatchingParenToken && lstTokens[tokenIndex].Span.iStartLine == lstTokens[lstTokens[tokenIndex].MatchingParenToken].Span.iStartLine) {
															tokenIndex = lstTokens[tokenIndex].MatchingParenToken;
															token = lstTokens[tokenIndex];
															colEnd = token.Span.iEndIndex + 1;
														} else {
															int parenLevel = token.ParenLevel;
															tokenIndex++;
															while (tokenIndex < lstTokens.Count) {
																token = lstTokens[tokenIndex];
																if (token.Span.iStartLine == prevToken.Span.iStartLine) {
																	if (-1 != token.MatchingParenToken && tokenIndex < token.MatchingParenToken && prevToken.Span.iStartLine == lstTokens[token.MatchingParenToken].Span.iStartLine) {
																		tokenIndex = lstTokens[tokenIndex].MatchingParenToken;
																		token = lstTokens[tokenIndex];
																	}
																	if (parenLevel == token.ParenLevel && token.Kind == TokenKind.RightParenthesis && token.Span.iStartLine == prevToken.Span.iStartLine) {
																		colEnd = token.Span.iEndIndex;
																		break;
																	}
																	tokenIndex++;
																} else {
																	// Backup one since we move to the next line
																	tokenIndex--;
																	token = lstTokens[tokenIndex];
																	break;
																}
															}
														}

														if (-1 == colEnd) {
															colEnd = token.Span.iEndIndex + 1;
														}
														ToolTipLiveTemplateIntelligent objTTLT = new ToolTipLiveTemplateIntelligent(textEditor.ToolTipMethodInfo, activeView, token.Span.iStartLine, colStart + 1, colEnd - 1, method, tokenIndex);
														IVsTextLines textLines;
														activeView.GetBuffer(out textLines);
														string wholeLine;
														textLines.GetLineText(token.Span.iStartLine, colStart, token.Span.iStartLine, colEnd, out wholeLine);
														int initialSelectedParameter = objTTLT.GetParameterIndex(wholeLine, colStart, currentColumn);
														ToolTipWindow.ShowIntelligentToolTipWindow(textEditor.ToolTipMethodInfo, objTTLT, initialSelectedParameter, colStart - currentColumn + 1, tokenIndex, textEditor.FontEditor);
														textEditor.ToolTipMethodInfo.PositionToolTip(Common.enPosition.Bottom);
													}
													break;
												}
											}
											break;
										}
										break;
									}
									break;
								}
								tokenIndex--;
							}
						}

						#endregion
					} else if (Keypress.Equals("(")) {
						#region Handle (

						TableSource foundTableSource;
						if (InStatement.IsCandidateForInsertColumns(lstTokens, tokenIndex, out foundTableSource)) {
							if (foundTableSource.Table.SysObject.Columns.Count > 0) {
								ToolTipWindow.ShowNormalToolTipWindow(textEditor.ToolTip, "Press Tab to insert column list.", new ToolTipLiveTemplateInsertInsertColumnList(textEditor.ToolTip, foundTableSource.Table.SysObject), textEditor.FontEditor);
							}
							return;
						}
						if (null != prevprevToken && prevprevToken.Kind == TokenKind.Name && prevprevToken.TokenContextType == TokenContextType.SysObject) {
							string functionName = prevprevToken.Token.UnqoutedImage;
							List<SysObject> lstSysObjects = textEditor.ActiveConnection.GetSysObjects();
							foreach (SysObject sysObject in lstSysObjects) {
								if (functionName.Equals(sysObject.ObjectName, StringComparison.OrdinalIgnoreCase)) {
									if (sysObject.Parameters.Count > 0 && (Common.enSqlTypes.TableValuedFunctions == sysObject.SqlType || Common.enSqlTypes.ScalarValuedFunctions == sysObject.SqlType)) {
										ToolTipWindow.ShowNormalToolTipWindow(textEditor.ToolTip, "Press Tab to insert parameter list.", new ToolTipLiveTemplateInsertFunctionParamList(textEditor.ToolTip, sysObject), textEditor.FontEditor);
										return;
									}
								}
							}
						}

						int line;
						int col;
						int lengthToRight;
						if (null != prevprevToken && currentLine == prevprevToken.Span.iStartLine) {
							foreach (Method method in textEditor.StaticData.Methods) {
								if (method.MethodToken.Kind == prevprevToken.Kind) {
									int startPos;
									int endPos;

									if (Instance.Settings.EnableAutoInsertPairParanthesesAndQuotes) {
										int charsToMoveLeft;
										TextEditor.HandleInsertRightParanthesis(")", Selection, curSel, activeView, out line, out col, out charsToMoveLeft, out lengthToRight);

										startPos = col - charsToMoveLeft;
										endPos = col - (charsToMoveLeft == 0 ? 0 : 1) + lengthToRight;
									} else {
										activeView.GetCaretPos(out line, out col);
										startPos = col;
										endPos = col;
									}

									ToolTipLiveTemplateIntelligent objTTLT = new ToolTipLiveTemplateIntelligent(textEditor.ToolTipMethodInfo, activeView, line, startPos, endPos, method, tokenIndex);
									ToolTipWindow.ShowIntelligentToolTipWindow(textEditor.ToolTipMethodInfo, objTTLT, 0, 0, tokenIndex, textEditor.FontEditor);
									return;
								}
							}
						}

						// Handle inserting right paranthesis if not being an method
						if (Instance.Settings.EnableAutoInsertPairParanthesesAndQuotes) {
							int charsToMoveLeft;
							TextEditor.HandleInsertRightParanthesis(")", Selection, curSel, activeView, out line, out col, out charsToMoveLeft, out lengthToRight);
							textEditor.ScheduleFullReparse();
						}

						#endregion
					} else if (Keypress.Equals(")")) {
						#region Handle )

						// If we are closing the method paranthesis, close the tooltip method window
						if (textEditor.ToolTipMethodInfo.ToolTipEnabled || textEditor.ToolTipMethodInfo.Visible) {
							if (null != prevToken && prevToken.MatchingParenToken == ((ToolTipLiveTemplateIntelligent)textEditor.ToolTipMethodInfo.LiveTemplate).StartTokenIndex) {
								textEditor.ToolTipMethodInfo.HideToolTip();
							}
						}

						if (Instance.Settings.EnableAutoInsertPairParanthesesAndQuotes) {
							TextEditor.HandleInsertLeftParanthesis(Selection, curSel, activeView, leftOfCursor, rightOfCursor);
							textEditor.ScheduleFullReparse();
						}

						#endregion
					} else if (Keypress.Equals("[")) {
						#region Handle (

						// Handle inserting right bracket if not being an method
						if (Instance.Settings.EnableAutoInsertPairParanthesesAndQuotes) {
							int line;
							int col;
							int charsToMoveLeft;
							int lengthToRight;
							TextEditor.HandleInsertRightParanthesis("]", Selection, curSel, activeView, out line, out col, out charsToMoveLeft, out lengthToRight);
							textEditor.ScheduleFullReparse();
						}

						#endregion
					} else if (Keypress.Equals("]")) {
						#region Handle ]

						if (Instance.Settings.EnableAutoInsertPairParanthesesAndQuotes) {
							if (rightOfCursor.StartsWith("]")) {
								Selection.CharRight(false, 1);
								Selection.DeleteLeft(1);
							}
						}

						#endregion
					} else {
						#region Handle rest of them

						textEditor.PopupWindow.CanShowPopup = true;
						if (null != prevToken && currentLine == prevToken.Span.iStartLine) {
							if (null != prevprevToken && currentLine == prevprevToken.Span.iStartLine) {
								if (prevprevToken.Kind == TokenKind.Dot) {
									prevToken = lstTokens[tokenIndex - 1];
									prevprevToken = lstTokens[tokenIndex - 2];
									ShowPopupAfterDot(tokenIndex - 1, curSel, prevToken, prevprevToken, wordLeftOfCursor, wordRightOfCursor);
									return;
								}

								// AS - don't show any intellisense
								if (prevprevToken.Kind == TokenKind.KeywordAs) {
									return;
								}

								// DateTime methods
								if (null != prevprevprevToken && (prevprevprevToken.Kind == TokenKind.KeywordDateAdd || prevprevprevToken.Kind == TokenKind.KeywordDateDiff || prevprevprevToken.Kind == TokenKind.KeywordDatePart || prevprevprevToken.Kind == TokenKind.KeywordDateName)) {
									PopupWindow.ShowPopupWithDateParameters(textEditor, curSel, prevToken, wordLeftOfCursor, wordRightOfCursor);
									return;
								}

								// USE
								if (prevprevToken.Kind == TokenKind.KeywordUse) {
									PopupWindow.ShowPopupWithDataBases(textEditor, curSel, prevToken, wordLeftOfCursor, wordRightOfCursor);
									return;
								}

								// SET xxx
								if (prevprevToken.Kind == TokenKind.KeywordSet) {
									// Only complete with SET types if in an SET segment
									bool isInSegment = false;
									foreach (StatementSpans spans in parser.SegmentUtils.StartTokenIndexes) {
										if (spans.StartIndex < tokenIndex && spans.EndIndex >= tokenIndex && spans.SegmentStartToken.StartToken.Kind != TokenKind.KeywordSet) {
											isInSegment = true;
											break;
										}
									}
									if (!isInSegment) {
										PopupWindow.ShowPopupWithSetTypes(textEditor, curSel, prevToken, wordLeftOfCursor, wordRightOfCursor, tokenIndex);
										return;
									}
								}

								// GROUP/ORDER BY
								if (prevprevToken.Kind == TokenKind.KeywordBy) {
									int offset = InStatement.GetPreviousNonCommentToken(lstTokens, tokenIndex - 1, tokenIndex - 1);
									offset--;
									prevprevToken = InStatement.GetPreviousNonCommentToken(lstTokens, ref offset);
									if (null != prevprevToken && (prevprevToken.Kind == TokenKind.KeywordOrder || prevprevToken.Kind == TokenKind.KeywordGroup)) {
										PopupWindow.ShowPopupWithScannedTables(textEditor, parser, curSel, wordLeftOfCursor, wordRightOfCursor, prevprevToken);
									}
									return;
								}

								// DECLARE
								if (InStatement.IsInStatementDeclare(lstTokens, tokenIndex - 1)) {
									PopupWindow.ShowPopupWithDataTypes(textEditor, curSel, prevToken, wordLeftOfCursor, wordRightOfCursor);
									return;
								}
							}

							List<IntellisenseData> lstIntellisenseItems;
							//							if ((toolTipMethodInfo.ToolTipEnabled || toolTipMethodInfo.Visible) && ((ToolTipLiveTemplateIntelligent)toolTipMethodInfo.LiveTemplate).ParameterHasIntellisense) {
							//								lstIntellisenseItems = ((ToolTipLiveTemplateIntelligent)toolTipMethodInfo.LiveTemplate).GetParameterIntellisense();
							//							} else {
							// No special action, so show everything we have that match
							lstIntellisenseItems = GetAllIntellisenseItems(tokenIndex);
							//							}

							// See that we have an initial match
							foreach (IntellisenseData idItem in lstIntellisenseItems) {
								if (idItem.MainText.StartsWith(wordLeftOfCursor, StringComparison.OrdinalIgnoreCase)) {
									textEditor.PopupWindow.ShowPopup(lstIntellisenseItems, wordLeftOfCursor, wordRightOfCursor, curSel, prevprevToken);
									break;
								}
							}
						}

						#endregion
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "HandleAfterKeyPress", e, Common.enErrorLvl.Error);
			}
		}

		#endregion

		#region ShowPopupAfterDot

		private void ShowPopupAfterDot(int tokenIndex, EditPoint curSel, TokenInfo prevToken, TokenInfo prevprevToken, string wordLeftOfCursor, string wordRightOfCursor) {
			textEditor.PopupWindow.CanShowPopup = true;

			SysObjectSchema foundSchema;
			Connection foundConnection;
			List<TokenInfo> lstTokens = textEditor.RawTokens;
			Server ActiveServer = textEditor.ActiveServer;
			Connection ActiveConnection = textEditor.ActiveConnection;

			if (InStatement.IsDatabase(tokenIndex - 1, lstTokens, ActiveServer, out foundConnection)) {
				PopupWindow.ShowPopupWithSchemas(textEditor, foundConnection, curSel, prevprevToken, wordLeftOfCursor, wordRightOfCursor);
			} else if (InStatement.IsSchema(tokenIndex - 1, lstTokens, ActiveConnection, ActiveServer, out foundSchema, out foundConnection)) {
				List<Common.enSqlTypes> lstSqlTypes = new List<Common.enSqlTypes>();

				if (InStatement.IsInStatementExecute(lstTokens, tokenIndex - 2)) {
					lstSqlTypes.Add(Common.enSqlTypes.SPs);
				} else {
					lstSqlTypes.Add(Common.enSqlTypes.Tables);
					lstSqlTypes.Add(Common.enSqlTypes.DerivedTable);
					lstSqlTypes.Add(Common.enSqlTypes.CTE);
					lstSqlTypes.Add(Common.enSqlTypes.Temporary);
					lstSqlTypes.Add(Common.enSqlTypes.Views);
					lstSqlTypes.Add(Common.enSqlTypes.TableValuedFunctions);
					lstSqlTypes.Add(Common.enSqlTypes.ScalarValuedFunctions);
				}
				PopupWindow.ShowPopupWithSysObjects(textEditor, TextEditor.CurrentWindowData.Parser, foundConnection, curSel, tokenIndex, prevprevToken, wordLeftOfCursor, wordRightOfCursor, lstSqlTypes, foundSchema.Schema, false);
			} else {
				PopupWindow.ShowPopupWithColumns(textEditor, TextEditor.CurrentWindowData.Parser, curSel, wordLeftOfCursor, wordRightOfCursor, string.Empty, prevprevToken.Token.UnqoutedImage, tokenIndex, prevToken);
			}
		}

		#endregion

		#region ConstructSqlTypesList

		public static List<Common.enSqlTypes> ConstructSqlTypesList(TokenInfo prevToken) {
			List<Common.enSqlTypes> lstSqlTypes = new List<Common.enSqlTypes>();
			//			switch (cmdPrevious) {
			//				case Common.enCommands.EXEC:
			//					lstSqlTypes.Add(Common.enSqlTypes.SPs);
			//					break;
			//				default:
			// List tables/views/functions in a new window
			lstSqlTypes.Add(Common.enSqlTypes.Tables);
			lstSqlTypes.Add(Common.enSqlTypes.DerivedTable);
			lstSqlTypes.Add(Common.enSqlTypes.CTE);
			lstSqlTypes.Add(Common.enSqlTypes.Temporary);
			lstSqlTypes.Add(Common.enSqlTypes.Views);
			lstSqlTypes.Add(Common.enSqlTypes.TableValuedFunctions);
			//					break;
			//			}
			return lstSqlTypes;
		}

		#endregion

		#region GetAllIntellisenseItems

		/// <summary>
		/// Retrieve intellisense items
		/// </summary>
		/// <param name="tokenIndex"></param>
		/// <returns></returns>
		public List<IntellisenseData> GetAllIntellisenseItems(int tokenIndex) {
			try {
				Parser parser = TextEditor.CurrentWindowData.Parser;

				// Show sql commands, scanned table alias, live templates and sysobjects
				List<IntellisenseData> lstAllIntellisenseItems = new List<IntellisenseData>();

				try {
					StatementSpans ss;
					List<TableSource> foundTableSources;
					if (parser.SegmentUtils.GetUniqueStatementSpanTablesources(tokenIndex, out foundTableSources, out ss, false)) {
						foreach (TableSource tableSource in foundTableSources) {
							// Add table alias
							if (tableSource.Table.Alias.Length > 0) {
								lstAllIntellisenseItems.Add(tableSource.Table);
							}
						}

						// Get declared column aliases
						ColumnAlias.AddColumnAliasesInSegment(parser, ss, lstAllIntellisenseItems);
					}
					if (parser.SegmentUtils.GetUniqueStatementSpanTablesources(tokenIndex, out foundTableSources, out ss, true)) {
						foreach (TableSource tableSource in foundTableSources) {
							// Add columns
							foreach (SysObjectColumn column in tableSource.Table.SysObject.Columns) {
								lstAllIntellisenseItems.Add(column);
							}
						}
					}

					// Datatypes
					foreach (DataType dataType in textEditor.StaticData.DataTypes.Values) {
						lstAllIntellisenseItems.Add(dataType);
					}

					List<LocalVariable> allVariablesInBatch = LocalVariable.GetAllVariablesInBatch(parser, tokenIndex);
					foreach (LocalVariable variable in allVariablesInBatch) {
						lstAllIntellisenseItems.Add(variable);
					}

					foreach (ScannedItem scannedItem in parser.DeclaredTransactions) {
						lstAllIntellisenseItems.Add(scannedItem);
					}

					if (null != textEditor.ActiveServer) {
						// Databases
						List<Database> lstDataBases = textEditor.ActiveServer.GetDataBases();
						foreach (Database database in lstDataBases) {
							lstAllIntellisenseItems.Add(database);
						}
					}

					// Add sql commands (SELECT, FROM etc etc)
					foreach (SqlCommand sqlCmd2 in textEditor.StaticData.SqlCommands) {
						bool skipSqlCmd = false;
						DataType type;
						if (textEditor.StaticData.DataTypes.TryGetValue(sqlCmd2.Token, out type)) {
							skipSqlCmd = true;
						}
						if (!skipSqlCmd) {
							lstAllIntellisenseItems.Add(sqlCmd2);
						}
					}

					// Add live templates
					foreach (LiveTemplate template in textEditor.StaticData.LiveTemplate) {
						lstAllIntellisenseItems.Add(template);
					}

					if (null != textEditor.ActiveConnection) {
						// Add SysObjects
						List<SysObject> lstSysObjects = textEditor.ActiveConnection.GetSysObjects();
						foreach (SysObject sysObject in lstSysObjects) {
							if (Common.enSqlTypes.DerivedTable != sysObject.SqlType) {
								lstAllIntellisenseItems.Add(sysObject);
							}
						}

						// Add schemas
						List<SysObjectSchema> lstSysObjectsSchemas = textEditor.ActiveConnection.GetSysObjectSchemas();
						foreach (SysObjectSchema schema in lstSysObjectsSchemas) {
							lstAllIntellisenseItems.Add(schema);
						}
					}
				} catch (Exception e) {
					Common.LogEntry(ClassName, "GetAllIntellisenseItems", e, Common.enErrorLvl.Error);
				}

				return lstAllIntellisenseItems;
			} catch (Exception e) {
				Common.LogEntry(ClassName, "GetAllIntellisenseItems", e, Common.enErrorLvl.Error);
				return new List<IntellisenseData>();
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose() {
			isDisposed = true;
		}

		#endregion
	}
}
