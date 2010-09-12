// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.Parsing;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.UI.Controls;
using Sassner.SmarterSql.UI.Subclassing;
using Sassner.SmarterSql.Utils.Helpers;
using Sassner.SmarterSql.Utils.Marker;

namespace Sassner.SmarterSql.Utils.Segment {
	public class TokenUsage : IDisposable {
		#region Member variables

		private const string ClassName = "TokenUsage";

		private readonly DTE2 _applicationObject;
		private readonly TextEditor textEditor;
		private Markers usageMarkers = new Markers();

		#endregion

		public TokenUsage(DTE2 _applicationObject, TextEditor textEditor) {
			this._applicationObject = _applicationObject;
			this.textEditor = textEditor;
		}

		#region IDisposable Members

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose() {
			if (null != usageMarkers) {
				usageMarkers.Dispose();
				usageMarkers = null;
			}
		}

		#endregion

		public void RemoveAll() {
			try {
				usageMarkers.RemoveAll();
				TextEditor.CurrentWindowData.SplitterRoot.SetHighlightStripes(new List<Stripe>());
			} catch (Exception e) {
				Common.LogEntry(ClassName, "RemoveAll", e, Common.enErrorLvl.Error);
			}
		}

		/// <summary>
		/// Find the declaration of:
		///  @variables
		///  tablealiases
		/// </summary>
		public void GotoDeclaration(Parser parser) {
			try {
				int declarationIndex;
				List<int> lstUsages;
				if (FindTokenUsage(parser, out declarationIndex, out lstUsages)) {
					if (-1 != declarationIndex) {
						IVsTextView activeView = TextEditor.CurrentWindowData.ActiveView;
						activeView.SetCaretPos(RawTokens[declarationIndex].Span.iStartLine, RawTokens[declarationIndex].Span.iStartIndex);
						Common.MakeSureCursorIsVisible(activeView, Common.enPosition.Center);
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "GotoDeclaration", e, Common.enErrorLvl.Error);
			}
		}

		/// <summary>
		/// Move the cursor to the previous highlighted usage
		/// </summary>
		internal void HighlightTokenGotoPrevious() {
			try {
				TextSelection Selection = _applicationObject.ActiveDocument.Selection as TextSelection;
				if (null != Selection) {
					int currentLine = Selection.CurrentLine - 1;
					int currentColumn = Selection.ActivePoint.VirtualCharOffset - 1;

					Squiggle currentSquiggle = usageMarkers.GetPreviousSquiggle(currentLine, currentColumn);
					if (null != currentSquiggle) {
						IVsTextView activeView = TextEditor.CurrentWindowData.ActiveView;
						activeView.SetCaretPos(currentSquiggle.Span.iStartLine, currentSquiggle.Span.iStartIndex);
						Common.MakeSureCursorIsVisible(activeView, Common.enPosition.Center);
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "HighlightTokenGotoPrevious", e, "User pressed ctrl+alt+up arrow: ", Common.enErrorLvl.Error);
			}
		}

		/// <summary>
		/// Move the cursor to the next highlighted usage
		/// </summary>
		internal void HighlightTokenGotoNext() {
			try {
				TextSelection Selection = _applicationObject.ActiveDocument.Selection as TextSelection;
				if (null != Selection) {
					int currentLine = Selection.CurrentLine - 1;
					int currentColumn = Selection.ActivePoint.VirtualCharOffset - 1;

					Squiggle currentSquiggle = usageMarkers.GetNextSquiggle(currentLine, currentColumn);
					if (null != currentSquiggle) {
						IVsTextView activeView = TextEditor.CurrentWindowData.ActiveView;
						activeView.SetCaretPos(currentSquiggle.Span.iStartLine, currentSquiggle.Span.iStartIndex);
						Common.MakeSureCursorIsVisible(activeView, Common.enPosition.Center);
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "HighlightTokenGotoNext", e, "User pressed ctrl+alt+down arrow: ", Common.enErrorLvl.Error);
			}
		}

		/// <summary>
		/// Mark the usage of the currently selected entity
		/// </summary>
		internal void HighlightTokenUsage(Parser parser) {
			try {
				usageMarkers.RemoveAll();

				int declarationIndex;
				List<int> lstUsages;
				if (FindTokenUsage(parser, out declarationIndex, out lstUsages)) {
					TokenInfo token;
					IVsTextLines ppBuffer;
					TextEditor.CurrentWindowData.ActiveView.GetBuffer(out ppBuffer);

					// Retrieve the markers
					Guid pguidMarkerYellow = new Guid("{6D799B25-7F32-4173-A4EB-521827D4AE4F}");
					Guid pguidMarkerGreen = new Guid("{CCB8B9D5-1643-4B41-8395-53C5B5ED5284}");
					int piMarkerTypeIDYellow;
					int piMarkerTypeIDGreen;
					bool foundGreenMarker = true;
					bool foundYellowMarker = true;
					if (!NativeMethods.Succeeded(Instance.VsTextMgr.GetRegisteredMarkerTypeID(ref pguidMarkerYellow, out piMarkerTypeIDYellow))) {
						piMarkerTypeIDYellow = (int)MARKERTYPE.MARKER_CODESENSE_ERROR;
						foundYellowMarker = false;
					}
					if (!NativeMethods.Succeeded(Instance.VsTextMgr.GetRegisteredMarkerTypeID(ref pguidMarkerGreen, out piMarkerTypeIDGreen))) {
						piMarkerTypeIDGreen = (int)MARKERTYPE.MARKER_CODESENSE_ERROR;
						foundGreenMarker = false;
					}

					// Create markers
					foreach (int index in lstUsages) {
						token = RawTokens[index];
						int piMarkerTypeID = (index == declarationIndex ? piMarkerTypeIDYellow : piMarkerTypeIDGreen);
						Squiggle squiggle = usageMarkers.CreateMarker(ppBuffer, string.Format("item_{0}_usage_{1}", index, token.Token.UnqoutedImage), RawTokens, index, index, token.Token.UnqoutedImage, null, piMarkerTypeID);

						// If default marker, add a border
						if (!(index == declarationIndex ? foundYellowMarker : foundGreenMarker)) {
							squiggle.Marker.SetVisualStyle((uint)(MARKERVISUAL.MV_BORDER));
						} else {
							// If using one of the BreakPoint markers, remove the glyph
							uint pwdFlags;
							squiggle.Marker.GetVisualStyle(out pwdFlags);
							squiggle.Marker.SetVisualStyle((pwdFlags & ~(uint)MARKERVISUAL.MV_GLYPH));
						}
					}

					List<Stripe> stripes = new List<Stripe>();
					token = RawTokens[declarationIndex];
					stripes.Add(new Stripe(token.Span.iStartLine, token.Span.iStartIndex, token.Span.iStartLine, "Declaration of " + token.Token.UnqoutedImage, Common.StripeType.HightlightDeclaration));
					foreach (int index in lstUsages) {
						if (index != declarationIndex) {
							token = RawTokens[index];
							stripes.Add(new Stripe(token.Span.iStartLine, token.Span.iStartIndex, token.Span.iStartLine, "Usage of " + token.Token.UnqoutedImage, Common.StripeType.HightlightUsage));
						}
					}

					TextEditor.CurrentWindowData.SplitterRoot.SetHighlightStripes(stripes);
				}

				// If we found any matches, make a message in the statusbar
				if (0 != usageMarkers.MarkerCount) {
					StatusBar.SetText("Found " + usageMarkers.MarkerCount + " usages of '" + RawTokens[lstUsages[0]].Token.UnqoutedImage + "' (Press Escape to remove highlightning)");
				} else {
					ToolTipWindow.ShowToolTipWindow(textEditor.ToolTip, "No usage found for this keyword", 0, true, Common.enPosition.Bottom, -2, 0, textEditor.FontEditor);
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "HighlightTokenUsage", e, Common.enErrorLvl.Error);
			}
		}

		/// <summary>
		/// Find the index of all available tokens as the is selected
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="declarationIndex"></param>
		/// <param name="lstUsages"></param>
		/// <returns></returns>
		private bool FindTokenUsage(Parser parser, out int declarationIndex, out List<int> lstUsages) {
			declarationIndex = -1;
			lstUsages = new List<int>();

			try {
				TextSelection Selection = _applicationObject.ActiveDocument.Selection as TextSelection;
				if (null != Selection) {
					int currentLine = Selection.CurrentLine - 1;
					int currentColumn = Selection.ActivePoint.VirtualCharOffset - 1;
					return FindTokenUsage(parser, out declarationIndex, out lstUsages, currentLine, currentColumn);
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "FindTokenUsage", e, Common.enErrorLvl.Error);
			}
			return false;
		}

		/// <summary>
		/// Find the index of all available tokens as the is selected
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="declarationIndex"></param>
		/// <param name="lstUsages"></param>
		/// <param name="currentLine"></param>
		/// <param name="currentColumn"></param>
		/// <returns></returns>
		public bool FindTokenUsage(Parser parser, out int declarationIndex, out List<int> lstUsages, int currentLine, int currentColumn) {
			declarationIndex = -1;
			lstUsages = new List<int>();

			try {
				bool IsInsideToken;
				int currentIndex = TextEditor.GetTokenIndex(RawTokens, currentLine, currentColumn, out IsInsideToken);
				if (-1 == currentIndex) {
					return false;
				}

				// Get the current token, or the token after if adjacent to an delimeter token
				TokenInfo currentToken = RawTokens[currentIndex];
				if (currentToken.Type == TokenType.Delimiter) {
					if (currentIndex + 1 <= RawTokens.Count - 1) {
						if (TextSpanHelper.IsAdjacent(currentToken.Span, RawTokens[currentIndex + 1].Span)) {
							currentToken = RawTokens[currentIndex + 1];
						}
					}
				}

				// Cursor ?
				if (currentToken.TokenContextType == TokenContextType.Cursor) {
					Cursor declaredCursor = Cursor.GetDeclaredCursor(parser, currentToken.Token.UnqoutedImage, currentIndex);
					if (null != declaredCursor) {
						declarationIndex = declaredCursor.TokenIndex;
						lstUsages.Add(declaredCursor.TokenIndex);
					}
					List<Cursor> calledCursors = Cursor.GetAllCursorsInBatch(parser, currentToken.Token.UnqoutedImage, currentIndex);
					foreach (Cursor calledCursor in calledCursors) {
						lstUsages.Add(calledCursor.TokenIndex);
					}
					return true;
				}

				// Variable ?
				if (currentToken.Kind == TokenKind.Variable) {
					LocalVariable declaredVariable = LocalVariable.GetDeclaredVariable(parser, currentToken.Token.UnqoutedImage, currentIndex);
					if (null != declaredVariable) {
						declarationIndex = declaredVariable.TokenIndex;
						lstUsages.Add(declaredVariable.TokenIndex);
					}
					List<LocalVariable> calledLocalVariables = LocalVariable.GetAllVariablesInBatch(parser, currentToken.Token.UnqoutedImage, currentIndex);
					foreach (LocalVariable variable in calledLocalVariables) {
						bool alreadyExists = false;
						foreach (int usage in lstUsages) {
							if (usage == declarationIndex) {
								alreadyExists = true;
								break;
							}
						}
						if (!alreadyExists) {
							lstUsages.Add(variable.TokenIndex);
						}
					}
					return true;
				}

				// Table alias. Find the statement range and then find table alias in that range
				StatementSpans span;
				List<TableSource> lstFoundTableSources;
				if (!parser.SegmentUtils.GetUniqueStatementSpanTablesources(currentIndex, out lstFoundTableSources, out span, false)) {
					return false;
				}
				if (currentToken.TokenContextType == TokenContextType.SysObjectAlias) {
					foreach (TableSource tableSource in lstFoundTableSources) {
						if (tableSource.Table.Alias.Equals(currentToken.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase)) {
							declarationIndex = tableSource.Table.EndIndex;

							if (span.IsSubSelect) {
								span = span.ParentStatmentSpan;
							}
							List<int> tokenIndexes = parser.SegmentUtils.GetTokenIndexesForStatementSpan(span);
							foreach (int tokenIndex in tokenIndexes) {
								TokenInfo token = RawTokens[tokenIndex];
								if (token.TokenContextType == TokenContextType.SysObjectAlias && token.Token.UnqoutedImage.Equals(currentToken.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase)) {
									lstUsages.Add(tokenIndex);
								}
							}
							return true;
						}
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "FindTokenUsage", e, Common.enErrorLvl.Error);
			}
			return false;
		}

		#region Private properties

		private List<TokenInfo> RawTokens {
			get { return textEditor.RawTokens; }
		}

		#endregion
	}
}
