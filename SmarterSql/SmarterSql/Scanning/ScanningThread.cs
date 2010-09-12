// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.Parsing;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.PInvoke;
using Sassner.SmarterSql.UI.Controls;
using Sassner.SmarterSql.Utils;
using Sassner.SmarterSql.Utils.Args;
using Sassner.SmarterSql.Utils.HiddenRegions;
using Sassner.SmarterSql.Utils.Marker;
using Sassner.SmarterSql.Utils.SqlErrors;
using Thread = System.Threading.Thread;
using Timer = System.Windows.Forms.Timer;

namespace Sassner.SmarterSql.Scanning {
	public class ScanningThread : IDisposable {
		#region Member variables

		private const string ClassName = "Scanning";

		private readonly BackgroundWorker bgWorkerDoFullParse;
		private readonly BackgroundWorker bgWorkerScanForErrors;

		private readonly TextEditor textEditor;
		private bool isDisposed;
		private bool IsParsing;
		private bool IsScanningForErrors;
		private Scanner scanner;

		#endregion

		#region Events

		#region Delegates

		public delegate void ErrorScanningDoneHandler(object sender, ErrorScanningDoneEventArgs e);

		#endregion

		public event ErrorScanningDoneHandler ErrorScanningDone;

		#endregion

		public ScanningThread(TextEditor textEditor) {
			this.textEditor = textEditor;

			scanner = new Scanner();

			bgWorkerDoFullParse = new BackgroundWorker {
				WorkerReportsProgress = true,
				WorkerSupportsCancellation = true
			};
			bgWorkerDoFullParse.DoWork += bgWorkerDoFullParse_DoWork;
			bgWorkerDoFullParse.ProgressChanged += bgWorkerDoFullParse_ProgressChanged;

			bgWorkerScanForErrors = new BackgroundWorker {
				WorkerReportsProgress = true,
				WorkerSupportsCancellation = true
			};
			bgWorkerScanForErrors.DoWork += bgWorkerScanForErrors_DoWork;
			bgWorkerScanForErrors.ProgressChanged += bgWorkerScanForErrors_ProgressChanged;
		}

		#region Public properties

		#endregion

		public void AbortAll() {
			AbortScanForErrors();

			if (!bgWorkerDoFullParse.IsBusy) {
				return;
			}
			bgWorkerDoFullParse.CancelAsync();
		}

		#region DoFullParse

		#region Delegates

		public delegate void AsynchroneFullParse();

		#endregion

		/// <summary>
		/// Asynchron scanning wanting feedback
		/// </summary>
        public void StartSynchroneFullParse(AsynchroneFullParse asynchroneFullParse) {
		    if (isDisposed) {
		        return;
		    }

		    AbortScanForErrors();

            if (DoFullParse(null, asynchroneFullParse)) {
            }
		}

	    /// <summary>
		/// Asynchron scanning wanting feedback
		/// </summary>
		public void StartAsynchroneFullParse(AsynchroneFullParse asynchroneFullParse) {
			StartAsynchroneFullParse(Instance.Settings.FullParseInitialDelay, asynchroneFullParse);
	    }

	    /// <summary>
		/// Asynchron scanning wanting feedback
		/// </summary>
		public void StartAsynchroneFullParse(int fullParseInitialDelay, AsynchroneFullParse asynchroneFullParse) {
			if (isDisposed) {
				return;
			}

			AbortScanForErrors();

			if (!bgWorkerDoFullParse.IsBusy) {
				bgWorkerDoFullParse.RunWorkerAsync(new ParseParameters(fullParseInitialDelay, asynchroneFullParse));
				return;
			}
			bgWorkerDoFullParse.CancelAsync();
			//Debug.WriteLine(DateTime.Now + ": AbortFullParse - " + Thread.CurrentThread.ManagedThreadId);
			Timer timerRetryUntilWorkerNotBusy = new Timer {
				Interval = 1
			};
			timerRetryUntilWorkerNotBusy.Tick +=
				delegate {
					if (bgWorkerDoFullParse.IsBusy) {
						return;
					}
					timerRetryUntilWorkerNotBusy.Stop();
					bgWorkerDoFullParse.RunWorkerAsync(new ParseParameters(fullParseInitialDelay, asynchroneFullParse));
					// Debug.WriteLine(DateTime.Now + ": StartAsynchroneFullParse - " + Thread.CurrentThread.ManagedThreadId);
				};
			timerRetryUntilWorkerNotBusy.Start();
		}

		/// <summary>
		/// Do full parse
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bgWorkerDoFullParse_DoWork(object sender, DoWorkEventArgs e) {
			//Debug.WriteLine(DateTime.Now + ": Start doing full parse " + Thread.CurrentThread.ManagedThreadId);
			try {
				ParseParameters parseParameters = (ParseParameters)e.Argument;
				BackgroundWorker backgroundWorker = ((BackgroundWorker)sender);

				int fullParseInitialDelay = parseParameters.FullParseInitialDelay;
				if (fullParseInitialDelay > 0) {
					// Wait some time before scanning
					int counter = fullParseInitialDelay / 5;
					while (counter > 0) {
						if (backgroundWorker.CancellationPending) {
							//Debug.WriteLine(DateTime.Now + ": Aborting full parse " + Thread.CurrentThread.ManagedThreadId);
							return;
						}
						Thread.Sleep(5);
						counter--;
					}
				}
				if (DoFullParse(backgroundWorker, parseParameters.AsynchroneFullParse)) {
				}
			} catch (Exception ex) {
				Common.LogEntry(ClassName, "bgWorkerDoFullParse_DoWork", ex, Common.enErrorLvl.Error);
			}
			//Debug.WriteLine(DateTime.Now + ": Done doing full parse " + Thread.CurrentThread.ManagedThreadId);
		}

		/// <summary>
		/// Execute in the same thread that started the async process. Shouled be GUI thread
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bgWorkerDoFullParse_ProgressChanged(object sender, ProgressChangedEventArgs e) {
			if (isDisposed) {
				return;
			}

			if (Instance.PrimaryThread != Thread.CurrentThread) {
				Common.LogEntry(ClassName, "bgWorkerDoFullParse_ProgressChanged", "Not run in GUI thread. " + Instance.PrimaryThread + " != " + Thread.CurrentThread, Common.enErrorLvl.Error);
			}

			AsynchroneFullParse asynchroneFullParse = (AsynchroneFullParse)e.UserState;
			if (e.ProgressPercentage == 100) {
				if (null != asynchroneFullParse) {
					asynchroneFullParse();
				}

				scanner.UpdateMissmatchingBracesMarkers();
				StartScanForErrors();
			}
		}

		/// <summary>
		/// Parse the whole code window
		/// </summary>
		/// <returns></returns>
		private bool DoFullParse(BackgroundWorker backgroundWorker, AsynchroneFullParse asynchroneFullParse) {
			try {
				if (isDisposed || IsParsing || textEditor.PopupWindow.IsPopupVisible || null == scanner) {
					return false;
				}

				//Common.LogEntry(ClassName, "DoFullParse", "Starting DoFullParse (" + Thread.CurrentThread.ManagedThreadId + ")", Common.enErrorLvl.Information);
				IsParsing = true;
				long startTime;
				long endTime;
				long lpFrequency;

				NativeWIN32.QueryPerformanceFrequency(out lpFrequency);
				NativeWIN32.QueryPerformanceCounter(out startTime);

				Parser newParser = new Parser();
				List<TokenInfo> tokens;
				if (!scanner.ParseSqlCode(newParser, backgroundWorker, out tokens) || (null != backgroundWorker && backgroundWorker.CancellationPending) || null == tokens) {
					return false;
				}
				newParser.RawTokens = tokens;

				if (null != textEditor.ActiveConnection) {
					List<SysObject> lstSysObjects = textEditor.ActiveConnection.GetSysObjects();
					newParser.ParseTokens(lstSysObjects, backgroundWorker);
                    if (null != backgroundWorker && backgroundWorker.CancellationPending) {
						return false;
					}
				}

				NativeWIN32.QueryPerformanceCounter(out endTime);
				double timeToParse = ((endTime - startTime) / (double)lpFrequency * 1000);
				if (timeToParse > 100) {
					Common.LogEntry(ClassName, "DoFullParse", "All parsing took " + (long)timeToParse + " msec", Common.enErrorLvl.Information);
				}

				Parser oldParser = TextEditor.CurrentWindowData.Parser;
				if (null != oldParser) {
					newParser.Markers = oldParser.Markers;
					oldParser.Dispose();
				}
				TextEditor.CurrentWindowData.Parser = newParser;

				textEditor.NeedToRedrawDebugWindow = true;
				textEditor.NeedToGetDBConnection = true;

				if (null != backgroundWorker && !backgroundWorker.CancellationPending) {
					backgroundWorker.ReportProgress(100, asynchroneFullParse);
				}
				return true;

			} catch (Exception e) {
				Common.LogEntry(ClassName, "DoFullParse", e, Common.enErrorLvl.Error);
				return false;
			} finally {
				if (null != backgroundWorker && backgroundWorker.CancellationPending) {
					Common.LogEntry(ClassName, "DoFullParse", "Aborting ParseSqlCode (" + Thread.CurrentThread.ManagedThreadId + ")", Common.enErrorLvl.Information);
				}
				IsParsing = false;
			}
		}

		#endregion

		#region ScanForErrors

		private void StartScanForErrors() {
			if (isDisposed) {
				return;
			}

			if (!bgWorkerScanForErrors.IsBusy) {
				bgWorkerScanForErrors.RunWorkerAsync();
				return;
			}
			bgWorkerScanForErrors.CancelAsync();
			Timer timerRetryUntilWorkerNotBusy = new Timer {
				Interval = 1
			};
			timerRetryUntilWorkerNotBusy.Tick +=
				delegate {
					if (bgWorkerScanForErrors.IsBusy) {
						return;
					}
					timerRetryUntilWorkerNotBusy.Stop();
					bgWorkerScanForErrors.RunWorkerAsync();
				};
			timerRetryUntilWorkerNotBusy.Start();
		}

		private void bgWorkerScanForErrors_DoWork(object sender, DoWorkEventArgs e) {
			try {
				BackgroundWorker backgroundWorker = ((BackgroundWorker)sender);
				// Wait a second before scanning for errors
				int counter = 100;
				while (counter > 0) {
					if (backgroundWorker.CancellationPending) {
						return;
					}
					Thread.Sleep(10);
					counter--;
				}
				// Debug.WriteLine(DateTime.Now + ": Start doing scanning for errors");
				DoErrorScanning(backgroundWorker);
				// Debug.WriteLine(DateTime.Now + ": Done doing scanning for errors");
			} catch (Exception e1) {
				Common.LogEntry(ClassName, "bgWorkerScanForErrors_DoWork", e1, Common.enErrorLvl.Error);
			}
		}

		private void AbortScanForErrors() {
			try {
				bgWorkerScanForErrors.CancelAsync();
			} catch (Exception) {
			}
		}

		internal void DoErrorScanning(BackgroundWorker backgroundWorker) {
			try {
				if (!IsParsing && !IsScanningForErrors && !textEditor.PopupWindow.IsPopupVisible && null != TextEditor.CurrentWindowData && null != TextEditor.CurrentWindowData.Parser && null != Instance.TextEditor.SysObjects) {
					try {
						long startTime;
						long endTime;
						long lpFrequency;

						IsScanningForErrors = true;

						NativeWIN32.QueryPerformanceFrequency(out lpFrequency);
						NativeWIN32.QueryPerformanceCounter(out startTime);

						TextEditor.CurrentWindowData.Parser.ScanForErrors(backgroundWorker);
						if (backgroundWorker.CancellationPending) {
							return;
						}

						NativeWIN32.QueryPerformanceCounter(out endTime);
						double timeToParse = ((endTime - startTime) / (double)lpFrequency * 1000);
						if (timeToParse > 100) {
							Common.LogEntry(ClassName, "DoErrorScanning", "Scanning for errors took " + (long)timeToParse + " msec", Common.enErrorLvl.Information);
						}

						if (null != TextEditor.CurrentWindowData && null != TextEditor.CurrentWindowData.Parser) {
							backgroundWorker.ReportProgress(100, TextEditor.CurrentWindowData.Parser);
						}
					} finally {
						IsScanningForErrors = false;
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "DoErrorScanning", e, Common.enErrorLvl.Error);
			}
		}

		private void bgWorkerScanForErrors_ProgressChanged(object sender, ProgressChangedEventArgs e) {
			if (isDisposed) {
				return;
			}

			if (e.ProgressPercentage == 100 && null != textEditor.RawTokens) {
				Parser parser = (Parser)e.UserState;

				Markers oldMarkers = new Markers();
				if (null != parser.Markers) {
					foreach (Squiggle squiggle in parser.Markers) {
						oldMarkers.addSquiggle(squiggle);
					}
				}

				// Mark the errors
				foreach (ScannedSqlError scannedSqlError in parser.ScannedSqlErrors) {
					Squiggle squiggle = Markers.CreateMarker(textEditor.RawTokens, scannedSqlError, parser.Markers);
					scannedSqlError.Squiggle = squiggle;
					if (null != squiggle) {
						oldMarkers.Remove(squiggle, false);
					}
				}
				if (parser.Markers != null) {
					parser.Markers.SortSquiggles();
				}

				// Clear all old markers
				oldMarkers.RemoveAll();

				// Set error markers on the error stripe
				int nbOfRows = (textEditor.RawTokens.Count > 0 ? textEditor.RawTokens[textEditor.RawTokens.Count - 1].Span.iEndLine : 0);
				TextEditor.CurrentWindowData.SplitterRoot.SetErrors(parser.Markers, nbOfRows);

				if (null != TextEditor.CurrentWindowData) {
					// Show/hide hidden regions
					IVsTextView activeView = TextEditor.CurrentWindowData.ActiveView;
					if (null != activeView) {
						IVsTextLines ppBuffer;
						activeView.GetBuffer(out ppBuffer);
						HiddenRegion.ConstructHiddenRegions(textEditor, textEditor.RawTokens, ppBuffer);

						int piLine;
						int piColumn;
						activeView.GetCaretPos(out piLine, out piColumn);
						frmSmartHelper3.HandleSmartHelper(textEditor.SmartHelper, parser, TextEditor.CurrentWindowData, piLine, piColumn);
					}
				}

				if (null != ErrorScanningDone) {
					ErrorScanningDone(this, new ErrorScanningDoneEventArgs());
				}
			}
		}

		#endregion

		#region IDisposable Members

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose() {
			try {
				isDisposed = true;

				if (null != TextEditor.CurrentWindowData && null != TextEditor.CurrentWindowData.Parser && null != TextEditor.CurrentWindowData.Parser.Markers) {
					TextEditor.CurrentWindowData.Parser.Dispose();
					TextEditor.CurrentWindowData.Parser = null;
				}

				if (null != scanner) {
					scanner.Dispose();
					scanner = null;
				}
				bgWorkerDoFullParse.CancelAsync();
				bgWorkerScanForErrors.CancelAsync();
			} catch (Exception e) {
				Common.LogEntry(ClassName, "Dispose", e, Common.enErrorLvl.Error);
			}
		}

		#endregion

		internal class ParseParameters {
			private readonly int fullParseInitialDelay;
			private readonly AsynchroneFullParse asynchroneFullParse;

			public ParseParameters(int fullParseInitialDelay, AsynchroneFullParse asynchroneFullParse) {
				this.fullParseInitialDelay = fullParseInitialDelay;
				this.asynchroneFullParse = asynchroneFullParse;
			}

			public int FullParseInitialDelay {
				get { return fullParseInitialDelay; }
			}

			public AsynchroneFullParse AsynchroneFullParse {
				get { return asynchroneFullParse; }
			}
		}
	}
}
