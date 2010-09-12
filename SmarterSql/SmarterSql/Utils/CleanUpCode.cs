// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Parsing;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Threads;

namespace Sassner.SmarterSql.Utils {
	public class CleanUpCode : IDisposable {
		#region Member variables

		private const string ClassName = "CleanUpCode";

		private readonly List<CodeChange> codeChanges;
		private bool isDisposed;

		#endregion

		public CleanUpCode() {
			codeChanges = new List<CodeChange>(50);
		}

		#region IDisposable Members

		public void Dispose() {
			isDisposed = true;
		}

		#endregion

		/// <summary>
		/// Add a job to the queue cleaning up the code changes
		/// </summary>
		public void QueueCleanUpCode() {
			if (codeChanges.Count > 0) {
				Instance.BackgroundTask.QueueTask(new TaskScanForCodeChanges(this));
			}
		}

		/// <summary>
		/// Clean up code, using the list of changed tokens
		/// </summary>
		internal void RunCleanUpCodeForChanges() {
			TokenInfo lastToken = null;

			Instance.ApplicationObject.UndoContext.Open("CleanUpCode.RunCleanUpCodeForChanges", true);
			try {
				lock (codeChanges) {
					Parser parser = TextEditor.CurrentWindowData.Parser;
					List<TokenInfo> tokens = parser.RawTokens;
					IVsTextLines ppBuffer;
					TextEditor.CurrentWindowData.ActiveView.GetBuffer(out ppBuffer);

					foreach (CodeChange codeChange in codeChanges) {
						if (null != lastToken && !(lastToken.Span.iStartLine == codeChange.Line && lastToken.Span.iStartIndex <= codeChange.Column && lastToken.Span.iEndIndex >= codeChange.Column)) {
							lastToken = null;
						}
						if (null == lastToken) {
							for (int i = 0; i < tokens.Count; i++) {
								TokenInfo token = tokens[i];
								if (token.Span.iStartLine == codeChange.Line && token.Span.iStartIndex <= codeChange.Column && token.Span.iEndIndex >= codeChange.Column) {
									lastToken = token;
									FormatToken(ppBuffer, token, Instance.Settings);
								}
							}
						}
					}
					ppBuffer.Reload(0);
					codeChanges.Clear();
				}
			} finally {
				if (Instance.ApplicationObject.UndoContext.IsOpen) {
					Instance.ApplicationObject.UndoContext.Close();
                }
			}
			if (null != lastToken) {
				Instance.TextEditor.ScheduleFullReparse();
			}
		}

		/// <summary>
		/// Clean up all tokens
		/// </summary>
		internal void RunCleanUpCodeComplete() {
			Instance.ApplicationObject.UndoContext.Open("CleanUpCode.RunCleanUpCodeComplete", true);
			try {
				lock (codeChanges) {
					Parser parser = TextEditor.CurrentWindowData.Parser;
					List<TokenInfo> tokens = parser.RawTokens;

					IVsTextLines ppBuffer;
					TextEditor.CurrentWindowData.ActiveView.GetBuffer(out ppBuffer);

					foreach (TokenInfo token in tokens) {
						FormatToken(ppBuffer, token, Instance.Settings);
					}
					ppBuffer.Reload(0);
				}
				codeChanges.Clear();
			} finally {
				if (Instance.ApplicationObject.UndoContext.IsOpen) {
					Instance.ApplicationObject.UndoContext.Close();
				}
			}
		}

		/// <summary>
		/// Clean up the supplied token
		/// </summary>
		/// <param name="ppBuffer"></param>
		/// <param name="token"></param>
		/// <param name="settings"></param>
		private static void FormatToken(IVsTextLines ppBuffer, TokenInfo token, Settings.Settings settings) {
		    bool shouldBeUpperCase;

            switch (token.Type) {
                case TokenType.DataType:
                    if (settings.DatatypesShouldBeUpperCase == Settings.Settings.ProperCase.Disabled) {
                        return;
                    }
                    shouldBeUpperCase = (settings.DatatypesShouldBeUpperCase == Settings.Settings.ProperCase.Upper);
                    break;
                case TokenType.Keyword:
                    if (settings.KeywordsShouldBeUpperCase == Settings.Settings.ProperCase.Disabled) {
                        return;
                    }
                    shouldBeUpperCase = (settings.KeywordsShouldBeUpperCase == Settings.Settings.ProperCase.Upper);
                    break;
                default:
                    return;
            }

		    string newImage = string.Empty;
            if (shouldBeUpperCase) {
                string imageToUpper = token.Token.ScannedText.ToUpper();
                if (!token.Token.ScannedText.Equals(imageToUpper, StringComparison.Ordinal)) {
                    newImage = imageToUpper;
                }
            } else {
                string imageToLower = token.Token.ScannedText.ToLower();
                if (!token.Token.ScannedText.Equals(imageToLower, StringComparison.Ordinal)) {
                    newImage = imageToLower;
                }
            }

            if (newImage.Length > 0) {
                ReplaceToken(ppBuffer, token, newImage);
            }
		}

	    /// <summary>
		/// Replace the text of the supplied token with the new image
		/// </summary>
		/// <param name="ppBuffer"></param>
		/// <param name="token"></param>
		/// <param name="newImage"></param>
		private static void ReplaceToken(IVsTextLines ppBuffer, TokenInfo token, string newImage) {
			// Replace the text in the textbuffer
			IntPtr newLine = Marshal.StringToCoTaskMemAuto(newImage);
			TextSpan[] pChangedSpan = null;
			try {
				if (!NativeMethods.Succeeded(ppBuffer.ReloadLines(token.Span.iStartLine, token.Span.iStartIndex, token.Span.iEndLine, token.Span.iEndIndex, newLine, newImage.Length, pChangedSpan))) {

				}
			} finally {
				Marshal.FreeCoTaskMem(newLine);
			}
		}

		/// <summary>
		/// Add the current cursor position to the list of changed items
		/// </summary>
		public void AddTextBufferChange() {
			try {
				if (isDisposed || null == TextEditor.CurrentWindowData) {
					return;
				}

				IVsTextView activeView = TextEditor.CurrentWindowData.ActiveView;
				int piLine;
				int piColumn;
				if (!NativeMethods.Succeeded(activeView.GetCaretPos(out piLine, out piColumn))) {
					return;
				}

				lock (codeChanges) {
					codeChanges.Add(new CodeChange(piLine, piColumn));
				}

			} catch (Exception e) {
				Common.LogEntry(ClassName, "AddTextBufferChange", e, Common.enErrorLvl.Error);
			}
		}
	}
}
