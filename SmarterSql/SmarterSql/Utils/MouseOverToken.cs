// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.Parsing;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.PInvoke;
using Sassner.SmarterSql.Tree;
using Sassner.SmarterSql.UI.Subclassing;
using Sassner.SmarterSql.Utils.Helpers;
using Sassner.SmarterSql.Utils.Marker;

namespace Sassner.SmarterSql.Utils {
	public class MouseOverToken : IDisposable {
		#region Member variables

		private const string ClassName = "MouseOverToken";
		private readonly TextEditor textEditor;

		private int lastCheckedForTokenUnderMouseCursorXPos = -1;
		private int lastCheckedForTokenUnderMouseCursorYPos = -1;
		private NativeWIN32.RECT rectTokenUnderMouseCursor;
		private Squiggle squiggleUnderMouseCursor;
		private TokenInfo tokenUnderMouseCursor;

		#endregion

		public MouseOverToken(TextEditor textEditor) {
			this.textEditor = textEditor;
		}

		#region IDisposable Members

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose() {
		}

		#endregion

		public void HandleMouseCursorOverToken(WindowData windowData) {
			try {
				if (null != windowData && null != windowData.SplitterRoot) {
					int xPos = windowData.SplitterRoot.CurrentXPos;
					int yPos = windowData.SplitterRoot.CurrentYPos;

					// If the tooltip is visible, check that the mouse cursor hasn't moved out of the token-box
					if (null != squiggleUnderMouseCursor) {
						if (null == tokenUnderMouseCursor || (null != tokenUnderMouseCursor && !RectHelper.ContainsInclusive(rectTokenUnderMouseCursor, xPos, yPos))) {
							InvalidateSquiggleUnderMouseCursor();
							tokenUnderMouseCursor = null;
						}
					} else {
						if (null != tokenUnderMouseCursor && RectHelper.ContainsInclusive(rectTokenUnderMouseCursor, xPos, yPos)) {
							// Do nothing. We are over an known token that shouldn't have a squiggle
						} else {
							tokenUnderMouseCursor = null;
						}
					}

					// If no token under mouse cursor found and the mouse position has been moved, scan for token
					if (null == tokenUnderMouseCursor && (xPos != lastCheckedForTokenUnderMouseCursorXPos || yPos != lastCheckedForTokenUnderMouseCursorYPos)) {
						lastCheckedForTokenUnderMouseCursorXPos = xPos;
						lastCheckedForTokenUnderMouseCursorYPos = yPos;

						try {
							NativeWIN32.RECT rectToken;
							int indexOfToken;
							if (GetTokenAtCoordinates(xPos, yPos, out rectToken, out indexOfToken)) {
								if (textEditor.RawTokens[indexOfToken] != tokenUnderMouseCursor) {
									tokenUnderMouseCursor = textEditor.RawTokens[indexOfToken];
									rectTokenUnderMouseCursor = rectToken;

									if (!tokenUnderMouseCursor.HasError) {
										string toolTipText = GetTokenToolTip(windowData.Parser, indexOfToken);
										if (!string.IsNullOrEmpty(toolTipText)) {
											IVsTextLines ppBuffer;
											TextEditor.CurrentWindowData.ActiveView.GetBuffer(out ppBuffer);
											squiggleUnderMouseCursor = Markers.CreateSquiggle(ppBuffer, "TokenUnderCursor", textEditor.RawTokens, indexOfToken, indexOfToken, toolTipText, (int)MARKERTYPE.MARKER_CODESENSE_ERROR, null);
											if (null != squiggleUnderMouseCursor) {
												// Enable tooltip
												squiggleUnderMouseCursor.Marker.SetVisualStyle((uint)(MARKERVISUAL.MV_TIP_FOR_BODY));
												squiggleUnderMouseCursor.Invalidated += squiggleUnderMouseCursor_Invalidated;
											}
										}
									}
								}
							}
						} catch (Exception e) {
							Common.LogEntry(ClassName, "HandleMouseCursorOverToken", e, "Create new squiggle: ", Common.enErrorLvl.Error);
						}
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "HandleMouseCursorOverToken", e, Common.enErrorLvl.Error);
			}
		}

		internal void InvalidateSquiggleUnderMouseCursor() {
			if (null != squiggleUnderMouseCursor) {
				squiggleUnderMouseCursor.Invalidated -= squiggleUnderMouseCursor_Invalidated;
				squiggleUnderMouseCursor.Dispose();
				squiggleUnderMouseCursor = null;
			}
		}

		private void squiggleUnderMouseCursor_Invalidated(Squiggle squiggle) {
			InvalidateSquiggleUnderMouseCursor();
		}

		private bool GetTokenAtCoordinates(int x, int y, out NativeWIN32.RECT rectToken, out int indexOfToken) {
			rectToken = new NativeWIN32.RECT();
			if (null == textEditor.RawTokens || 0 == textEditor.RawTokens.Count) {
				indexOfToken = -1;
				return false;
			}

			try {
				IntPtr activeHwnd = TextEditor.CurrentWindowData.ActiveHwnd;
				if (IntPtr.Zero == activeHwnd) {
					indexOfToken = -1;
					return false;
				}
				if (!NativeWIN32.IsValidWindowHandle(activeHwnd)) {
					Common.LogEntry(ClassName, "GetTokenAtCoordinates", "Not a valid hwnd", Common.enErrorLvl.Warning);
					indexOfToken = -1;
					return false;
				}

				IVsTextView activeView = TextEditor.CurrentWindowData.ActiveView;
				if (null != activeView) {
					// Get the current position in the code window
					int piMinUnit;
					int piMaxUnit;
					int piVisibleUnits;
					int piFirstVisibleUnit;
					if (NativeMethods.Succeeded(activeView.GetScrollInfo(NativeWIN32.SB_VERT, out piMinUnit, out piMaxUnit, out piVisibleUnits, out piFirstVisibleUnit))) {
						// Get line height
						int piLineHeight;
						if (NativeMethods.Succeeded(activeView.GetLineHeight(out piLineHeight))) {
							// Calculate the row number the mouse cursor is on
							int line = piFirstVisibleUnit + (y / piLineHeight);
							int newLine;
							if (!Common.FindActualLineNumber(activeView, line, out newLine)) {
								indexOfToken = -1;
								return false;
							}
							//Debug.WriteLine(line + " -> " + newLine);

							// Find the token on the current line
							lock (textEditor.RawTokens) {
								for (int i = 0; i < textEditor.RawTokens.Count; i++) {
									TokenInfo token = textEditor.RawTokens[i];

									if (token.Span.iStartLine == newLine) {
										POINT[] ptStart = GetSafePointOfLineColumn(newLine, token.Span.iStartIndex, activeView);
										POINT[] ptEnd = GetSafePointOfLineColumn(newLine, token.Span.iEndIndex, activeView);
										if (-1 != ptStart[0].x && -1 != ptStart[0].y && -1 != ptEnd[0].x && -1 != ptEnd[0].y) {
											if (ptStart[0].x < x && ptEnd[0].x > x) {
												rectToken.Left = ptStart[0].x;
												rectToken.Top = ptStart[0].y;
												rectToken.Right = ptEnd[0].x;
												rectToken.Bottom = ptEnd[0].y + piLineHeight;
												//Debug.WriteLine(RectHelper.Format(rectToken));
												indexOfToken = i;
												return true;
											}
										} else {
											Common.LogEntry(ClassName, "GetTokenAtCoordinates", "Unable to retrive start point of line (" + line + "->" + newLine + ", " + token.Span.iStartIndex + " . Aborting.", Common.enErrorLvl.Warning);
											indexOfToken = -1;
											return false;
										}
									} else if (token.Span.iStartLine > newLine) {
										break;
									}
								}
							}
						} else {
							Common.LogEntry(ClassName, "GetTokenAtCoordinates", "Unable to retrive texteditor line height. Aborting.", Common.enErrorLvl.Warning);
						}
					} else {
						Common.LogEntry(ClassName, "GetTokenAtCoordinates", "Unable to retrive scrollinfo. Aborting.", Common.enErrorLvl.Warning);
					}
				} else {
					Common.LogEntry(ClassName, "GetTokenAtCoordinates", "Unable to retrive ActiveView object. Aborting.", Common.enErrorLvl.Warning);
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "GetTokenAtCoordinates", e, "Error when getting token. ", Common.enErrorLvl.Error);
			}

			indexOfToken = -1;
			return false;
		}

		private static POINT[] GetSafePointOfLineColumn(int row, int col, IVsTextView activeView) {
			POINT[] ppt = new POINT[1];
			for (int i = 0; i < 5; i++) {
				try {
					activeView.GetPointOfLineColumn(row, col, ppt);
					return ppt;
				} catch (Exception) {
					Thread.Sleep(5);
				}
			}
			ppt[0].x = -1;
			ppt[0].y = -1;
			return ppt;
		}

		private string GetTokenToolTip(Parser parser, int indexOfToken) {
			try {
				if (null == textEditor.ActiveConnection) {
					return "";
				}

				TokenInfo token = textEditor.RawTokens[indexOfToken];
				StatementSpans currentSpan = parser.SegmentUtils.GetStatementSpan(indexOfToken);

				// Sysobject?
				if (token.TokenContextType == TokenContextType.SysObject || token.TokenContextType == TokenContextType.TempTable || token.TokenContextType == TokenContextType.Table || token.TokenContextType == TokenContextType.Procedure || token.TokenContextType == TokenContextType.Function || token.TokenContextType == TokenContextType.View) {
					List<SysObject> lstSysObjects = textEditor.ActiveConnection.GetSysObjects();
					foreach (SysObject sysObject in lstSysObjects) {
						if (sysObject.ObjectName.Equals(token.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase) || sysObject.ObjectName.Equals(token.Token.Image, StringComparison.OrdinalIgnoreCase)) {
							return sysObject.GetToolTip;
						}
					}
				}

				// Schema?
				if (token.TokenContextType == TokenContextType.SysObjectSchema) {
					List<SysObjectSchema> lstSysObjectSchemas = textEditor.ActiveConnection.GetSysObjectSchemas();
					foreach (SysObjectSchema sysObjectSchema in lstSysObjectSchemas) {
						if (null != sysObjectSchema.Schema && (sysObjectSchema.Schema.Equals(token.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase) || sysObjectSchema.Schema.Equals(token.Token.Image, StringComparison.OrdinalIgnoreCase))) {
							return sysObjectSchema.GetToolTip;
						}
					}
				}

				// Variable?
				if (token.TokenContextType == TokenContextType.Variable) {
					LocalVariable variable = LocalVariable.GetLocalVariable(parser, token.Token.UnqoutedImage, indexOfToken);
					if (null != variable) {
						return variable.GetToolTip;
					}
				}

				// Cursor ?
				if (token.TokenContextType == TokenContextType.Cursor) {
					return "Cursor";
				}

				// Alias
				if (token.TokenContextType == TokenContextType.ColumnAlias) {
					ColumnAlias columnAlias;
					if (ColumnAlias.IsColumnAliasInSegment(parser, currentSpan, token.Token.UnqoutedImage, out columnAlias)) {
						return columnAlias.GetToolTip;
					}
					return "";
				}

				// Server?
				if (token.TokenContextType == TokenContextType.Server) {
					List<SysServer> sysServers = textEditor.ActiveServer.GetSysServers();
					foreach (SysServer sysServer in sysServers) {
						if (sysServer.ServerName.Equals(token.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase) || sysServer.ServerName.Equals(token.Token.Image, StringComparison.OrdinalIgnoreCase)) {
							return sysServer.GetToolTip;
						}
					}
				}

				// Database?
				if (token.TokenContextType == TokenContextType.Database) {
					List<Database> dataBases = textEditor.ActiveServer.GetDataBases();
					foreach (Database database in dataBases) {
						if (database.DataBaseName.Equals(token.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase) || database.DataBaseName.Equals(token.Token.Image, StringComparison.OrdinalIgnoreCase)) {
							return database.GetToolTip;
						}
					}
				}

				if (token.TokenContextType == TokenContextType.SysObjectColumn || token.TokenContextType == TokenContextType.SysObjectAlias) {
					StatementSpans ss;
					List<TableSource> foundTableSources;
					if (parser.SegmentUtils.GetStatementSpanTablesources(indexOfToken, out foundTableSources, out ss)) {
						// Alias?
						if (token.TokenContextType == TokenContextType.SysObjectAlias) {
							foreach (TableSource tableSource in foundTableSources) {
								if (tableSource.Table.Alias.Equals(token.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase) || tableSource.Table.Alias.Equals(token.Token.Image, StringComparison.OrdinalIgnoreCase)) {
									return tableSource.Table.GetToolTip;
								}
							}
						}

						// Column?
						int offset = indexOfToken - 1;
						TokenInfo prevToken = InStatement.GetPreviousNonCommentToken(textEditor.RawTokens, offset);
						if (null != prevToken && prevToken.Kind == TokenKind.Dot) {
							offset--;
							prevToken = InStatement.GetPreviousNonCommentToken(textEditor.RawTokens, offset);
							if (null != prevToken && prevToken.TokenContextType == TokenContextType.SysObjectAlias) {
								// Find the tablename/alias and then find the column for that table
								foreach (TableSource tableSource in foundTableSources) {
									if (tableSource.Table.TableName.Equals(prevToken.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase) || tableSource.Table.TableName.Equals(prevToken.Token.Image, StringComparison.OrdinalIgnoreCase) || tableSource.Table.Alias.Equals(prevToken.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase) || tableSource.Table.Alias.Equals(prevToken.Token.Image, StringComparison.OrdinalIgnoreCase)) {
										foreach (SysObjectColumn sysObjectColumn in tableSource.Table.SysObject.Columns) {
											if (sysObjectColumn.ColumnName.Equals(token.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase) || sysObjectColumn.ColumnName.Equals(token.Token.Image, StringComparison.OrdinalIgnoreCase)) {
												return sysObjectColumn.GetToolTip;
											}
										}
										break;
									}
								}
								// Table alias found, but not one we know of. Return empty handed
								return string.Empty;
							}
						}
						// Column, but without the alias to give us the table
						string tooltipText = string.Empty;
						bool statmentSpanDoesntNeedAlias = (ss.SegmentStartToken.StartToken == Tokens.kwDeleteToken || ss.SegmentStartToken.StartToken == Tokens.kwUpdateToken);
						foreach (TableSource tableSource in foundTableSources) {
							foreach (SysObjectColumn sysObjectColumn in tableSource.Table.SysObject.Columns) {
								if (sysObjectColumn.ColumnName.Equals(token.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase) || sysObjectColumn.ColumnName.Equals(token.Token.Image, StringComparison.OrdinalIgnoreCase)) {
									int indexEndMain = parser.SegmentUtils.GetNextMainTokenEnd(ss, ss.StartIndex);
									if (statmentSpanDoesntNeedAlias && indexOfToken >= tableSource.StartIndex && indexOfToken < indexEndMain) {
										return sysObjectColumn.GetToolTip;
									}
									if (tooltipText.Length > 0) {
										return "Ambiguous column.";
									}
									tooltipText = sysObjectColumn.GetToolTip;
								}
							}
						}
						if (tooltipText.Length > 0) {
							return tooltipText;
						}
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "GetTokenToolTip", e, Common.enErrorLvl.Error);
			}

			// No idea what it is
			return string.Empty;
		}
	}
}