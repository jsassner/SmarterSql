// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Parsing;
using Sassner.SmarterSql.ParsingObjects;
using TokenInfo=Sassner.SmarterSql.ParsingUtils.TokenInfo;

namespace Sassner.SmarterSql.Utils.HiddenRegions {
	public class HiddenRegion : IVsHiddenTextClient, IDisposable {
		#region Member variables

		private const string ClassName = "HiddenRegion";

		internal static uint HiddenRegionCookie = 21983;
		private static bool isDisposed;
		private readonly uint docCookie;
		private readonly List<HiddenRegionData> lstHiddenRegions = new List<HiddenRegionData>();
		private readonly IVsTextLines ppBuffer;

		private bool doOutlining = true;
		private IVsHiddenTextSession hiddenTextSession;
		private bool runOnce;

		#endregion

		public HiddenRegion(uint docCookie, IVsTextLines ppBuffer) {
			this.docCookie = docCookie;
			this.ppBuffer = ppBuffer;

			GetHiddenTextSession();
		}

		#region Public properties

		public bool RunOnce {
			[DebuggerStepThrough]
			get { return runOnce; }
		}

		public uint DocCookie {
			[DebuggerStepThrough]
			get { return docCookie; }
		}

		public IVsTextLines activeView {
			[DebuggerStepThrough]
			get { return ppBuffer; }
		}

		public List<HiddenRegionData> HiddenRegions {
			[DebuggerStepThrough]
			get { return lstHiddenRegions; }
		}

		public bool DoOutlining {
			[DebuggerStepThrough]
			get { return doOutlining; }
			set { doOutlining = value; }
		}

		#endregion

		#region Region methods

		/// <summary>
		/// Construct hidden regions from two sources:
		///   * Parsed segments
		///   * Region between "--#region" and "--#endregion"
		///   * Multiline comments
		/// </summary>
		/// <param name="textEditor"></param>
		/// <param name="lstTokens"></param>
		/// <param name="ppBuffer"></param>
		public static void ConstructHiddenRegions(TextEditor textEditor, List<TokenInfo> lstTokens, IVsTextLines ppBuffer) {
			try {
				if (isDisposed || null == TextEditor.CurrentWindowData.HiddenRegion || !Instance.Settings.EnableOutlining) {
					return;
				}

				List<NewHiddenRegion> lstNewHiddenRegion = new List<NewHiddenRegion>();
				bool runOnce = TextEditor.CurrentWindowData.HiddenRegion.RunOnce;

				if (null != TextEditor.CurrentWindowData.Parser.SegmentUtils) {
					// Parsed segments
					foreach (StatementSpans statementSpans in TextEditor.CurrentWindowData.Parser.SegmentUtils.StartTokenIndexes) {
						TextSpan span = new TextSpan {
							iStartLine = statementSpans.Start.Span.iStartLine,
							iStartIndex = statementSpans.Start.Span.iStartIndex,
							iEndLine = statementSpans.End.Span.iEndLine,
							iEndIndex = statementSpans.End.Span.iEndIndex
						};
						if (span.iStartLine != span.iEndLine) {
							NewHiddenRegion newHiddenRegion;
							if (CreateRegion(span, true, false, runOnce, out newHiddenRegion)) {
								lstNewHiddenRegion.Add(newHiddenRegion);
							}
						}
					}

					// Segments that fit together, for example UNIONS
					for (int i = 0; i < TextEditor.CurrentWindowData.Parser.SegmentUtils.StartTokenIndexes.Count; i++) {
						StatementSpans statementSpans = TextEditor.CurrentWindowData.Parser.SegmentUtils.StartTokenIndexes[i];
						StatementSpans startJoinedSpans = null;
						StatementSpans endJoinedSpans;

						while (null != statementSpans.JoinedSpanNext) {
							if (null == startJoinedSpans) {
								startJoinedSpans = statementSpans;
							}
							statementSpans = statementSpans.JoinedSpanNext;
							i++;
						}
						if (null != startJoinedSpans) {
							endJoinedSpans = statementSpans;
							TextSpan span = new TextSpan {
								iStartLine = startJoinedSpans.Start.Span.iStartLine,
								iStartIndex = startJoinedSpans.Start.Span.iStartIndex,
								iEndLine = endJoinedSpans.End.Span.iEndLine,
								iEndIndex = endJoinedSpans.End.Span.iEndIndex
							};
							if (span.iStartLine != span.iEndLine) {
								NewHiddenRegion newHiddenRegion;
								if (CreateRegion(span, true, false, runOnce, out newHiddenRegion)) {
									lstNewHiddenRegion.Add(newHiddenRegion);
								}
							}
						}
					}
				}

				// Multiline comments
				int tokenCommentIndexStart = -1;
				for (int i = 0; i < lstTokens.Count; i++) {
					TokenInfo token = lstTokens[i];
					if (token.Kind == TokenKind.MultiLineComment) {
						if (-1 != tokenCommentIndexStart && ((CommentToken)token.Token).IsComplete) {
							TextSpan span = new TextSpan {
								iStartLine = lstTokens[tokenCommentIndexStart].Span.iStartLine,
								iStartIndex = lstTokens[tokenCommentIndexStart].Span.iStartIndex,
								iEndLine = lstTokens[i].Span.iEndLine,
								iEndIndex = lstTokens[i].Span.iEndIndex
							};
							if (span.iStartLine != span.iEndLine) {
								NewHiddenRegion newHiddenRegion;
								if (CreateRegion(span, true, false, runOnce, out newHiddenRegion)) {
									lstNewHiddenRegion.Add(newHiddenRegion);
								}
							}

							tokenCommentIndexStart = -1;
						} else if (-1 == tokenCommentIndexStart) {
							tokenCommentIndexStart = i;
						}
					}
				}

				// Region between "--#region" and "--#endregion"
				List<int> lstTokenRegionIndexes = new List<int>();
				List<int> lstMissmatchingRegions = new List<int>();
				for (int i = 0; i < lstTokens.Count; i++) {
					TokenInfo token = lstTokens[i];
					if (token.Kind == TokenKind.SingleLineComment) {
						if (token.Token.Image.Trim().StartsWith(Common.RegionStart.Substring(2), StringComparison.OrdinalIgnoreCase)) {
							lstTokenRegionIndexes.Add(i);
						} else if (token.Token.Image.Trim().StartsWith(Common.RegionEnd.Substring(2), StringComparison.OrdinalIgnoreCase)) {
							if (lstTokenRegionIndexes.Count > 0) {
								int tokenIndex1 = lstTokenRegionIndexes[lstTokenRegionIndexes.Count - 1];
								int tokenIndex2 = i;
								lstTokenRegionIndexes.RemoveAt(lstTokenRegionIndexes.Count - 1);
								TextSpan span = new TextSpan {
									iStartLine = lstTokens[tokenIndex1].Span.iStartLine,
									iStartIndex = lstTokens[tokenIndex1].Span.iStartIndex,
									iEndLine = lstTokens[tokenIndex2].Span.iEndLine,
									iEndIndex = lstTokens[tokenIndex2].Span.iEndIndex
								};
								NewHiddenRegion newHiddenRegion;
								if (CreateRegion(span, true, true, runOnce, out newHiddenRegion)) {
									lstNewHiddenRegion.Add(newHiddenRegion);
								}
							} else {
								lstMissmatchingRegions.Add(i);
							}
						}
					}
				}

				textEditor.MissmatchingRegionMarkers.RemoveAll();
				// Create missing starting region markers
				for (int i = 0; i < lstTokenRegionIndexes.Count; i++) {
					textEditor.MissmatchingRegionMarkers.CreateMarker(ppBuffer, "MissmatchingStartingRegions_" + i, textEditor.RawTokens, lstTokenRegionIndexes[i], lstTokenRegionIndexes[i], "Missmatching starting regions", null);
				}
				// Create missing ending region markers
				for (int i = 0; i < lstMissmatchingRegions.Count; i++) {
					textEditor.MissmatchingRegionMarkers.CreateMarker(ppBuffer, "MissmatchingEndingRegions_" + i, textEditor.RawTokens, lstMissmatchingRegions[i], lstMissmatchingRegions[i], "Missmatching ending regions", null);
				}

				// Sort the hidden regions
				lstNewHiddenRegion.Sort(HiddenRegionDataComparison);

				// Process regions
				TextEditor.CurrentWindowData.HiddenRegion.ProcessHiddenRegions(lstNewHiddenRegion);
			} catch (Exception e) {
				Common.LogEntry(ClassName, "ConstructHiddenRegions", e, Common.enErrorLvl.Error);
			}
		}

		private static int HiddenRegionDataComparison(NewHiddenRegion hiddenRegionData1, NewHiddenRegion hiddenRegionData2) {
			if (hiddenRegionData1.tsHiddenText.iStartLine == hiddenRegionData2.tsHiddenText.iStartLine) {
				return hiddenRegionData1.tsHiddenText.iStartIndex - hiddenRegionData2.tsHiddenText.iStartIndex;
			}
			return hiddenRegionData1.tsHiddenText.iStartLine - hiddenRegionData2.tsHiddenText.iStartLine;
		}

		public static void ToggleAllOutlining(TextEditor textEditor) {
			if (null != TextEditor.CurrentWindowData.HiddenRegion && Instance.Settings.EnableOutlining) {
				TextEditor.CurrentWindowData.HiddenRegion.ToggleRegions();
			}
		}

		public static void ToggleOutlining(TextEditor textEditor) {
			if (isDisposed || null == TextEditor.CurrentWindowData.HiddenRegion || !Instance.Settings.EnableOutlining) {
				return;
			}

			try {
				TextDocument document = Instance.ApplicationObject.ActiveWindow.Document.Object("TextDocument") as TextDocument;
				if (null != document) {
					TextSelection selection = document.Selection;
					EditPoint2 epStart = selection.TopPoint.CreateEditPoint() as EditPoint2;
					if (null != epStart) {
						TextSpan span = new TextSpan {
							iStartLine = (epStart.Line - 1),
							iStartIndex = 0,
							iEndIndex = epStart.LineLength,
							iEndLine = (epStart.Line - 1)
						};
						TextEditor.CurrentWindowData.HiddenRegion.ToggleRegion(span);
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "ToggleOutlining", e, Common.enErrorLvl.Error);
			}
		}

		public static bool CreateRegion(TextEditor textEditor) {
			if (isDisposed || null == TextEditor.CurrentWindowData.HiddenRegion || !Instance.Settings.EnableOutlining) {
				Common.LogEntry(ClassName, "CreateRegion", "Unable to create hidden region", Common.enErrorLvl.Error);
				return false;
			}

			try {
				TextDocument document = Instance.ApplicationObject.ActiveWindow.Document.Object("TextDocument") as TextDocument;
				if (null != document) {
					TextSelection selection = document.Selection;
					EditPoint2 epStart = selection.TopPoint.CreateEditPoint() as EditPoint2;
					EditPoint2 epEnd = selection.BottomPoint.CreateEditPoint() as EditPoint2;
					if (null != epStart && null != epEnd) {
						Instance.ApplicationObject.UndoContext.Open("CreateRegion.CreateRegion", true);

						try {
							string indentString = "";

							EditPoint2 epCurrentStart = epStart.CreateEditPoint() as EditPoint2;
							if (selection.Text.Length > 0) {
								// Get indent string
								EditPoint2 epCurrentEnd = epStart.CreateEditPoint() as EditPoint2;
								if (null != epCurrentStart && null != epCurrentEnd) {
									epCurrentStart.StartOfLine();
									epCurrentEnd.EndOfLine();
									string line = epCurrentStart.GetText(epCurrentEnd);
									if (line.Length > 0) {
										int pos = 0;
										while (pos <= line.Length) {
											if (!char.IsWhiteSpace(line[pos])) {
												break;
											}
											pos++;
										}
										if (pos >= 0 && pos <= line.Length) {
											indentString = line.Substring(0, pos);
										}
									}
								}
							} else {
								if (null != epCurrentStart) {
									indentString = SmartIndenter.FormatIndentString(epCurrentStart.DisplayColumn - 1);
								}
							}

							if (Instance.Settings.FormattingBlankLineAroundRegion && null != epCurrentStart) {
								epCurrentStart.Insert(Environment.NewLine);
							}
							epStart.Insert((selection.Text.Length > 0 ? indentString : "") + Common.RegionStart + " ");
							int endLine = epStart.Line;
							int endColumn = epStart.DisplayColumn;
							epStart.Insert(Environment.NewLine + (Instance.Settings.FormattingBlankLineInsideRegion ? Environment.NewLine : ""));

							// End
							if (selection.Text.Length > 0) {
								if (0 == epEnd.DisplayColumn - 1) {
									epEnd.LineUp(1);
									epEnd.EndOfLine();
								}
							} else {
								epEnd = epStart;
							}
							epEnd.Insert((Instance.Settings.FormattingBlankLineInsideRegion ? Environment.NewLine : "") + Environment.NewLine + indentString + Common.RegionEnd);
							if (Instance.Settings.FormattingBlankLineAroundRegion) {
								epEnd.Insert(Environment.NewLine);
							}

							selection.MoveToDisplayColumn(endLine, endColumn, false);

							// Parse tokens
							Instance.TextEditor.ScheduleFullReparse();

							return true;
						} finally {
							if (Instance.ApplicationObject.UndoContext.IsOpen) {
								Instance.ApplicationObject.UndoContext.Close();
							}
						}
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "CreateRegion", e, Common.enErrorLvl.Error);
			}
			return false;
		}

		#endregion

		#region IDisposable Members

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose() {
			isDisposed = true;

			try {
				foreach (HiddenRegionData region in lstHiddenRegions) {
					region.HiddenRegion.Invalidate((int)CHANGE_HIDDEN_REGION_FLAGS.chrDefault);
				}
				lstHiddenRegions.Clear();
			} catch (Exception e) {
				Common.LogEntry(ClassName, "Dispose", e, Common.enErrorLvl.Error);
			}
		}

		#endregion

		#region IVsHiddenTextClient Members

		void IVsHiddenTextClient.OnHiddenRegionChange(IVsHiddenRegion pHidReg, HIDDEN_REGION_EVENT EventCode, int fBufferModifiable) {
			if (isDisposed) {
				return;
			}

			try {
				HiddenRegionData hiddenRegionData = GetHiddenRegionData(pHidReg);
				switch (EventCode) {
					case HIDDEN_REGION_EVENT.hreAfterRegionCollapsed:
						string banner = "";
						try {
							if (null != hiddenRegionData) {
								TextSpan[] span = new TextSpan[1];
								pHidReg.GetSpan(span);

								// Store new values in the hidden region object
								hiddenRegionData.Expanded = false;
								hiddenRegionData.Span = span[0];

								// Get the first line of the text. Parse out the name of the region
								int intLineLength;
								ppBuffer.GetLengthOfLine(span[0].iStartLine, out intLineLength);
								string strBuffer;
								ppBuffer.GetLineText(span[0].iStartLine, 0, span[0].iStartLine, intLineLength, out strBuffer);
								banner = strBuffer.TrimStart().Replace(Common.RegionStart + " ", "").Trim();
							}
						} catch (Exception e) {
							Common.LogEntry(ClassName, "IVsHiddenTextClient.OnHiddenRegionChange", e, Common.enErrorLvl.Error);
						}

						// Set banner text
						if (0 == banner.Length) {
							banner = Common.RegionStart;
						}
						pHidReg.SetBanner(banner);
						break;

					case HIDDEN_REGION_EVENT.hreAfterRegionExpanded:
						try {
							if (null != hiddenRegionData) {
								TextSpan[] span = new TextSpan[1];
								pHidReg.GetSpan(span);

								// Store new values in the hidden region object
								hiddenRegionData.Expanded = true;
								hiddenRegionData.Span = span[0];

								int intLineLength;
								ppBuffer.GetLengthOfLine(span[0].iStartLine, out intLineLength);
								string strBuffer;
								ppBuffer.GetLineText(span[0].iStartLine, 0, span[0].iStartLine, intLineLength, out strBuffer);
							}
						} catch (Exception e) {
							Common.LogEntry(ClassName, "IVsHiddenTextClient.OnHiddenRegionChange", e, Common.enErrorLvl.Error);
						}
						break;

					case HIDDEN_REGION_EVENT.hreRegionDeleted:
					case HIDDEN_REGION_EVENT.hreRegionReloaded:
					case HIDDEN_REGION_EVENT.hreRegionResurrected:
						break;
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "IVsHiddenTextClient.OnHiddenRegionChange", e, Common.enErrorLvl.Error);
			}
		}

		int IVsHiddenTextClient.GetTipText(IVsHiddenRegion pHidReg, string[] pbstrText) {
			if (isDisposed) {
				return NativeMethods.S_OK;
			}

			if (pbstrText != null && pbstrText.Length > 0) {
				string text = "";
				try {
					TextSpan[] aspan = new TextSpan[1];
					NativeMethods.ThrowOnFailure(pHidReg.GetSpan(aspan));
					text = GetText(aspan[0]);
				} catch (Exception) {
					// Do nothing
				}
				pbstrText[0] = text;
			}

			return NativeMethods.S_OK;
		}

		int IVsHiddenTextClient.GetMarkerCommandInfo(IVsHiddenRegion pHidReg, int iItem, string[] pbstrText, uint[] pcmdf) {
			if (pcmdf != null && pcmdf.Length > 0) {
				pcmdf[0] = 0;
			}
			if (pbstrText != null && pbstrText.Length > 0) {
				pbstrText[0] = null;
			}

			return NativeMethods.E_NOTIMPL;
		}

		int IVsHiddenTextClient.ExecMarkerCommand(IVsHiddenRegion pHidReg, int iItem) {
			return NativeMethods.E_NOTIMPL;
		}

		int IVsHiddenTextClient.MakeBaseSpanVisible(IVsHiddenRegion pHidReg, TextSpan[] pBaseSpan) {
			return NativeMethods.E_NOTIMPL;
		}

		void IVsHiddenTextClient.OnBeforeSessionEnd() {
		}

		#endregion

		/// <summary>
		/// Get the IVsHiddenTextSession object
		/// </summary>
		/// <returns></returns>
		private void GetHiddenTextSession() {
			if (isDisposed) {
				return;
			}

			try {
				IVsHiddenTextManager htextmgr = Instance.Sp.GetService(typeof (SVsTextManager)) as IVsHiddenTextManager;
				if (htextmgr != null) {
					int hr = htextmgr.GetHiddenTextSession(ppBuffer, out hiddenTextSession);
					// Remove the old session first
					if (hr != NativeMethods.E_FAIL) {
						hiddenTextSession.Terminate();
					}
					// Now create a new session, and listen for IVsHiddenTextClient events
					NativeMethods.ThrowOnFailure(htextmgr.CreateHiddenTextSession(0, ppBuffer, this, out hiddenTextSession));

					Marshal.ReleaseComObject(htextmgr);
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "GetHiddenTextSession", e, Common.enErrorLvl.Error);
			}
		}

		/// <summary>
		/// Create a NewHiddenRegion object from a TextSpan object.
		/// </summary>
		/// <param name="span"></param>
		/// <param name="expanded"></param>
		/// <param name="isCodeRegion"></param>
		/// <param name="runOnce"></param>
		/// <param name="newHiddenRegion"></param>
		/// <returns></returns>
		public static bool CreateRegion(TextSpan span, bool expanded, bool isCodeRegion, bool runOnce, out NewHiddenRegion newHiddenRegion) {
			if (isDisposed) {
				newHiddenRegion = new NewHiddenRegion();
				return false;
			}

			try {
				if (isCodeRegion && !runOnce && Instance.Settings.AlwaysOutlineRegionsFirstTime) {
					expanded = false;
				}
				newHiddenRegion = new NewHiddenRegion {
					dwBehavior = ((uint)HIDDEN_REGION_BEHAVIOR.hrbClientControlled),
					dwClient = HiddenRegionCookie,
					dwState = (expanded ? (uint)HIDDEN_REGION_STATE.hrsExpanded : (uint)HIDDEN_REGION_STATE.hrsDefault),
					iType = ((int)HIDDEN_REGION_TYPE.hrtCollapsible)
					//dwState = (uint)HIDDEN_REGION_STATE.hrsExpanded;
				};

				IVsTextLines ppBuffer;
				TextEditor.CurrentWindowData.ActiveView.GetBuffer(out ppBuffer);
				int intLineLength;
				ppBuffer.GetLengthOfLine(span.iStartLine, out intLineLength);
				string strBuffer;
				ppBuffer.GetLineText(span.iStartLine, 0, span.iStartLine, intLineLength, out strBuffer);
				string banner = strBuffer.TrimStart().Replace(Common.RegionStart + " ", "").Trim();
				if (0 == banner.Length) {
					banner = Common.RegionStart;
				}

				newHiddenRegion.pszBanner = banner;
				newHiddenRegion.tsHiddenText = span;
				return true;

			} catch (Exception e) {
				Common.LogEntry(ClassName, "CreateRegion", e, Common.enErrorLvl.Error);
				newHiddenRegion = new NewHiddenRegion();
				return false;
			}
		}

		/// <summary>
		/// Add a hidden region
		/// </summary>
		/// <param name="span"></param>
		/// <param name="expanded"></param>
		/// <param name="isCodeRegion"></param>
		/// <returns></returns>
		public bool AddHiddenRegion(TextSpan span, bool expanded, bool isCodeRegion) {
			if (isDisposed) {
				return false;
			}

			try {
				NewHiddenRegion region;
				if (!CreateRegion(span, expanded, isCodeRegion, runOnce, out region)) {
					return false;
				}

				NewHiddenRegion[] chunk = new NewHiddenRegion[1];
				chunk[0] = region;
				IVsEnumHiddenRegions[] ppEnumHiddenRegions = new IVsEnumHiddenRegions[1];
				int hr = hiddenTextSession.AddHiddenRegions((int)CHANGE_HIDDEN_REGION_FLAGS.chrNonUndoable, 1, chunk, ppEnumHiddenRegions);
				if (NativeMethods.Succeeded(hr)) {
					EnumAndAddHiddenRegions(ppEnumHiddenRegions);
					return true;
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "AddHiddenRegion", e, Common.enErrorLvl.Error);
			}
			return false;
		}

		/// <summary>
		/// Remove all hidden regions
		/// </summary>
		public virtual void RemoveHiddenRegions() {
			try {
				IVsEnumHiddenRegions ppenum;
				TextSpan[] aspan = new TextSpan[1];
				aspan[0] = GetDocumentSpan();
				NativeMethods.ThrowOnFailure(hiddenTextSession.EnumHiddenRegions((uint)FIND_HIDDEN_REGION_FLAGS.FHR_BY_CLIENT_DATA, HiddenRegionCookie, aspan, out ppenum));
				uint fetched;
				IVsHiddenRegion[] aregion = new IVsHiddenRegion[1];
				while (ppenum.Next(1, aregion, out fetched) == NativeMethods.S_OK && fetched == 1) {
					NativeMethods.ThrowOnFailure(aregion[0].Invalidate((int)CHANGE_HIDDEN_REGION_FLAGS.chrNonUndoable));
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "RemoveHiddenRegions", e, Common.enErrorLvl.Error);
			}
		}

		/// <summary>
		/// Disable outlining
		/// </summary>
		public void DisableOutlining() {
			doOutlining = false;
			RemoveHiddenRegions();
		}

		/// <summary>
		/// Toggle a specific region for it's expanded/collapsed state
		/// </summary>
		/// <param name="span"></param>
		public void ToggleRegion(TextSpan span) {
			if (isDisposed) {
				return;
			}

			try {
				IVsEnumHiddenRegions ppenum;
				TextSpan[] aspan = new TextSpan[1];
				aspan[0] = span;
				NativeMethods.ThrowOnFailure(hiddenTextSession.EnumHiddenRegions((uint)FIND_HIDDEN_REGION_FLAGS.FHR_BY_CLIENT_DATA, HiddenRegionCookie, aspan, out ppenum));

				IVsHiddenRegion[] aregion = new IVsHiddenRegion[1];
				int minSpanDiff = int.MaxValue;
				IVsHiddenRegion minRegion = null;
				uint fetched;
				while (ppenum.Next(1, aregion, out fetched) == NativeMethods.S_OK && fetched == 1) {
					TextSpan[] textSpans = new TextSpan[1];
					NativeMethods.ThrowOnFailure(aregion[0].GetSpan(textSpans));
					if (textSpans[0].iStartLine <= span.iStartLine && textSpans[0].iEndLine >= span.iStartLine && textSpans[0].iEndLine - textSpans[0].iStartLine < minSpanDiff) {
						minSpanDiff = textSpans[0].iEndLine - textSpans[0].iStartLine;
						minRegion = aregion[0];
					}
				}
				if (null != minRegion) {
					uint dwState;
					minRegion.GetState(out dwState);
					dwState ^= (uint)HIDDEN_REGION_STATE.hrsExpanded;
					NativeMethods.ThrowOnFailure(minRegion.SetState(dwState, (uint)CHANGE_HIDDEN_REGION_FLAGS.chrDefault));
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "ToggleRegion", e, Common.enErrorLvl.Error);
			}
		}

		/// <summary>
		/// Toggle outlining on all regions
		/// </summary>
		public void ToggleRegions() {
			if (isDisposed) {
				return;
			}

			try {
				IVsEnumHiddenRegions ppenum;
				TextSpan[] aspan = new TextSpan[1];
				aspan[0] = GetDocumentSpan();
				NativeMethods.ThrowOnFailure(hiddenTextSession.EnumHiddenRegions((uint)FIND_HIDDEN_REGION_FLAGS.FHR_BY_CLIENT_DATA, HiddenRegionCookie, aspan, out ppenum));
				using (new CompoundViewAction(TextEditor.CurrentWindowData.ActiveView, "ToggleAllRegions")) {
					IVsHiddenRegion[] aregion = new IVsHiddenRegion[1];
					uint fetched;
					while (ppenum.Next(1, aregion, out fetched) == NativeMethods.S_OK && fetched == 1) {
						uint dwState;
						aregion[0].GetState(out dwState);
						dwState ^= (uint)HIDDEN_REGION_STATE.hrsExpanded;
						NativeMethods.ThrowOnFailure(aregion[0].SetState(dwState, (uint)CHANGE_HIDDEN_REGION_FLAGS.chrDefault));
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "ToggleRegions", e, Common.enErrorLvl.Error);
			}
		}

		/// <summary>
		/// Comparison method sorting HiddenRegionData's
		/// </summary>
		/// <param name="hiddenRegionData1"></param>
		/// <param name="hiddenRegionData2"></param>
		/// <returns></returns>
		private static int HiddenRegionDataComparison(HiddenRegionData hiddenRegionData1, HiddenRegionData hiddenRegionData2) {
			if (hiddenRegionData1.Span.iStartLine == hiddenRegionData2.Span.iStartLine) {
				return hiddenRegionData1.Span.iStartIndex - hiddenRegionData2.Span.iStartIndex;
			}
			return hiddenRegionData1.Span.iStartLine - hiddenRegionData2.Span.iStartLine;
		}

		/// <summary>
		/// Enumerate hidden regions of a specific type
		/// </summary>
		/// <param name="regions"></param>
		/// <param name="spans"></param>
		/// <returns></returns>
		private bool EnumHiddenRegions(out List<IVsHiddenRegion> regions, out List<TextSpan> spans) {
			if (isDisposed) {
				regions = null;
				spans = null;
				return false;
			}

			try {
				regions = new List<IVsHiddenRegion>();
				spans = new List<TextSpan>();

				// Compare the existing regions with the new regions and remove any that do not match the new regions.
				IVsEnumHiddenRegions ppEnumHiddenRegions;
				TextSpan[] textSpans = new TextSpan[1];
				textSpans[0] = GetDocumentSpan();
				NativeMethods.ThrowOnFailure(hiddenTextSession.EnumHiddenRegions((uint)FIND_HIDDEN_REGION_FLAGS.FHR_BY_CLIENT_DATA, HiddenRegionCookie, textSpans, out ppEnumHiddenRegions));
				uint fetched;
				IVsHiddenRegion[] existingHiddenRegion = new IVsHiddenRegion[1];

				// Create a list of IVsHiddenRegion objects, sorted in the same order that the
				// authoring sink sorts.  This is necessary because VS core editor does NOT return
				// the regions in the same order that we add them.

				while (ppEnumHiddenRegions.Next(1, existingHiddenRegion, out fetched) == NativeMethods.S_OK && fetched == 1) {
					NativeMethods.ThrowOnFailure(existingHiddenRegion[0].GetSpan(textSpans));
					TextSpan span = textSpans[0];
					int i = spans.Count - 1;
					while (i >= 0) {
						TextSpan span2 = spans[i];
						if (TextSpanHelper.StartsAfterStartOf(span, span2)) {
							break;
						}
						i--;
					}
					spans.Insert(i + 1, span);
					regions.Insert(i + 1, existingHiddenRegion[0]);
					//Common.LogEntry(ClassName, "EnumHiddenRegions", "Found " + GetHiddenRegionData(existingHiddenRegion[0]), Common.enErrorLvl.Information);
				}
				Marshal.ReleaseComObject(ppEnumHiddenRegions);

				return true;
			} catch (Exception e) {
				Common.LogEntry(ClassName, "EnumHiddenRegions", e, Common.enErrorLvl.Error);
				regions = null;
				spans = null;
				return false;
			}
		}

		/// <summary>
		/// Process hidden regions objects. Remove old regions not in use no more and add new ones.
		/// </summary>
		/// <param name="lstNewHiddenRegions"></param>
		public void ProcessHiddenRegions(List<NewHiddenRegion> lstNewHiddenRegions) {
			if (isDisposed || !doOutlining || null == hiddenTextSession) {
				return;
			}

			//			int matched = 0;
			//			int removed = 0;
			//			int added = 0;
			int pos = 0;

			List<TextSpan> spans;
			List<IVsHiddenRegion> regions;
			if (!EnumHiddenRegions(out regions, out spans)) {
				return;
			}

			// Now merge the list found with the list in the AuthoringSink to figure out
			// what matches, what needs to be removed, and what needs to be inserted.
			try {
				NewHiddenRegion r = (pos < lstNewHiddenRegions.Count ? lstNewHiddenRegions[pos] : new NewHiddenRegion());
				for (int i = 0, n = regions.Count; i < n; i++) {
					IVsHiddenRegion region = regions[i];
					TextSpan span = spans[i];

					// In case we're inserting a new region, scan ahead to matching start line
					while (r.tsHiddenText.iStartLine < span.iStartLine) {
						pos++;
						if (pos >= lstNewHiddenRegions.Count) {
							r = new NewHiddenRegion();
							break;
						}
						r = lstNewHiddenRegions[pos];
					}
					if (TextSpanHelper.IsSameSpan(r.tsHiddenText, span)) {
						// This region is already there.
						// matched++;
						lstNewHiddenRegions.RemoveAt(pos);
						r = (pos < lstNewHiddenRegions.Count) ? lstNewHiddenRegions[pos] : new NewHiddenRegion();
					} else {
						HiddenRegionData hiddenRegionData = GetHiddenRegionData(region);
						lstHiddenRegions.Remove(hiddenRegionData);
						// Common.LogEntry(ClassName, "ProcessHiddenRegions", "Remove " + hiddenRegionData, Common.enErrorLvl.Information);
						// removed++;
						NativeMethods.ThrowOnFailure(region.Invalidate((int)CHANGE_HIDDEN_REGION_FLAGS.chrDefault));
					}
				}

				int start = Environment.TickCount;
				if (lstNewHiddenRegions.Count > 0) {
					int count = lstNewHiddenRegions.Count;
					// For very large documents this can take a while, so add them in chunks of 1000 and stop after 5 seconds.
					int maxTime = Environment.TickCount + 5000;
					const int chunkSize = 1000;
					NewHiddenRegion[] chunk = new NewHiddenRegion[chunkSize];
					int now = Environment.TickCount;
					int i = 0;
					while (i < count && start > now - maxTime) {
						int j = 0;
						while (i < count && j < chunkSize) {
							chunk[j] = lstNewHiddenRegions[i];
							//						added++;
							i++;
							j++;
						}
						IVsEnumHiddenRegions[] ppEnumHiddenRegions = new IVsEnumHiddenRegions[1];
						int hr = hiddenTextSession.AddHiddenRegions((int)CHANGE_HIDDEN_REGION_FLAGS.chrNonUndoable, j, chunk, ppEnumHiddenRegions);
						if (NativeMethods.Failed(hr)) {
							break; // stop adding if we start getting errors.
						}
						runOnce = true;
						EnumAndAddHiddenRegions(ppEnumHiddenRegions);
						now = Environment.TickCount;
					}
				}

				//			string debug = String.Format("Hidden Regions: Matched={0}, Removed={1}, Added={2}/{3}", matched, removed, added, lstNewHiddenRegions.Count);
				//			Common.LogEntry(ClassName, "ProcessHiddenRegions", debug, Common.enErrorLvl.Information);
			} catch (Exception e) {
				Common.LogEntry(ClassName, "ProcessHiddenRegions", e, Common.enErrorLvl.Error);
			}
		}

		/// <summary>
		/// Enumerate newly added HiddenRegions, and create a HiddenRegionData object for each
		/// </summary>
		/// <param name="ppEnumHiddenRegions"></param>
		private void EnumAndAddHiddenRegions(IVsEnumHiddenRegions[] ppEnumHiddenRegions) {
			if (isDisposed) {
				return;
			}

			try {
				IVsHiddenRegion[] existingHiddenRegion = new IVsHiddenRegion[1];
				uint fetched;

				while (ppEnumHiddenRegions[0].Next(1, existingHiddenRegion, out fetched) == NativeMethods.S_OK && fetched == 1) {
					TextSpan[] textSpans = new TextSpan[1];
					NativeMethods.ThrowOnFailure(existingHiddenRegion[0].GetSpan(textSpans));
					HiddenRegionData hiddenRegionData = new HiddenRegionData(existingHiddenRegion[0], textSpans[0]);
					lstHiddenRegions.Add(hiddenRegionData);
					//				Common.LogEntry(ClassName, "EnumHiddenRegions", "Adding " + hiddenRegionData, Common.enErrorLvl.Information);
				}
				lstHiddenRegions.Sort(HiddenRegionDataComparison);
			} catch (Exception e) {
				Common.LogEntry(ClassName, "EnumAndAddHiddenRegions", e, Common.enErrorLvl.Error);
			}
		}

		/// <summary>
		/// Get the whole document TextSpan object
		/// </summary>
		/// <returns></returns>
		public TextSpan GetDocumentSpan() {
			TextSpan span = new TextSpan {
				iStartIndex = 0,
				iStartLine = 0
			};
			try {
				NativeMethods.ThrowOnFailure(ppBuffer.GetLastLineIndex(out span.iEndLine, out span.iEndIndex));
			} catch (Exception e) {
				Common.LogEntry(ClassName, "GetDocumentSpan", e, Common.enErrorLvl.Error);
				span = new TextSpan();
			}
			return span;
		}

		/// <summary>
		/// Get the text contained in the supplied TextSpan
		/// </summary>
		/// <param name="span"></param>
		/// <returns></returns>
		private string GetText(TextSpan span) {
			try {
				StringBuilder sbText = new StringBuilder();
				string indentString = "";
				for (int i = span.iStartLine; i <= span.iEndLine; i++) {
					// Only allow 25 lines
					if (i - span.iStartLine > 25) {
						sbText.AppendLine("...");
						break;
					}
					int intLineLength;
					ppBuffer.GetLengthOfLine(i, out intLineLength);
					string strBuffer;
					ppBuffer.GetLineText(i, 0, i, intLineLength, out strBuffer);
					if (i == span.iStartLine) {
						int pos = 0;
						while (pos <= strBuffer.Length) {
							if (!char.IsWhiteSpace(strBuffer[pos])) {
								break;
							}
							pos++;
						}
						if (pos >= 0 && pos <= strBuffer.Length) {
							indentString = SmartIndenter.TabsToSpaces(strBuffer.Substring(0, pos));
						}
					}
					strBuffer = SmartIndenter.TabsToSpaces(strBuffer).Substring(indentString.Length);
					sbText.AppendLine(strBuffer);
				}
				return sbText.ToString();
			} catch (Exception e) {
				Common.LogEntry(ClassName, "GetText", e, Common.enErrorLvl.Error);
				return string.Empty;
			}
		}

		/// <summary>
		/// Retrieve a HiddenRegionData object from an IVsHiddenRegion object
		/// </summary>
		/// <param name="hiddenRegion"></param>
		/// <returns></returns>
		public HiddenRegionData GetHiddenRegionData(IVsHiddenRegion hiddenRegion) {
			try {
				foreach (HiddenRegionData region in lstHiddenRegions) {
					if (region.HiddenRegion == hiddenRegion) {
						return region;
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "GetHiddenRegionData", e, Common.enErrorLvl.Error);
			}
			return null;
		}
	}
}
