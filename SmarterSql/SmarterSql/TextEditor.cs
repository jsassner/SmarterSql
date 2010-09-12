// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.Objects.Functions;
using Sassner.SmarterSql.Parsing;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.PInvoke;
using Sassner.SmarterSql.Scanning;
using Sassner.SmarterSql.Threads;
using Sassner.SmarterSql.UI;
using Sassner.SmarterSql.UI.Controls;
using Sassner.SmarterSql.UI.Subclassing;
using Sassner.SmarterSql.Utils;
using Sassner.SmarterSql.Utils.Args;
using Sassner.SmarterSql.Utils.Helpers;
using Sassner.SmarterSql.Utils.Marker;
using Sassner.SmarterSql.Utils.Segment;
using Sassner.SmarterSql.Utils.Settings;
using Sassner.SmarterSql.Utils.Tooltips;
using KeyEventArgs=Sassner.SmarterSql.Utils.Args.KeyEventArgs;

namespace Sassner.SmarterSql {
	public class TextEditor : IDisposable {
		#region Member variables

		private const string ClassName = "TextEditor";
		private static Object s_InternalSyncObject;

		private readonly DTE2 _applicationObject;
		private readonly List<Server> servers;
		private readonly Markers missmatchingRegionMarkers = new Markers();
		private readonly Instance objInstance;
		private readonly frmDebugTokens objDebugTokens;
		private readonly StaticData objStaticData;
		private readonly TokenInfo unknownPrevToken = new TokenInfo(new TextSpan(), TokenType.Unknown);
		private TextDocumentKeyPressEvents _textDocumentKeyPressEvents;

		private bool blnIsInComment;
		private bool blnIsInString;

		// DTE ui events

		private bool blnNeedToGetDBConnection;
		private bool blnNeedToRedrawDebugWindow;
		private bool blnUserPressedAltEnter;
		private bool blnUserPressedCompleteWord;
		private bool blnUserPressedRenameToken;
		private readonly List<WindowData> lstWindowData = new List<WindowData>();
		private static WindowData currentWindowData;
		private DebugMatchingParen debugMatchingParen;

		private Font fontEditor;
		private Font fontTooltip;
		private bool isDisposed;

		private MatchingParen matchingParen;
		private MouseOverToken mouseOverToken;
		private MyRunningDocumentTable myRunningDocumentTable;
		private Connection objActiveConnection;
		private Server objActiveServer;
		private PopupWindow objPopupWindow;

		private ScanningThread scanningThread;

		private frmSmartHelper3 smartHelper;
		private TableUtils tableUtils;
		private TokenUsage tokenUsage;
		private ToolTipWindow toolTip;
		private ToolTipWindow toolTipMethodInfo;
		private Intellisense intellisense;
		private CleanUpCode cleanUpCode;

		#endregion

		#region Constructor and destructor

		public TextEditor(Instance objInstance, DTE2 _applicationObject, StaticData objStaticData, List<Server> lstServers, frmDebugTokens objDebugTokens, Font fontEditor, Font fontTooltip) {
			this.objInstance = objInstance;
			this._applicationObject = _applicationObject;
			this.objStaticData = objStaticData;
			servers = lstServers;
			this.objDebugTokens = objDebugTokens;
			this.fontEditor = fontEditor;
			this.fontTooltip = fontTooltip;

			cleanUpCode = new CleanUpCode();
			intellisense = new Intellisense(this);

			objPopupWindow = new PopupWindow(this, _applicationObject, fontEditor, fontTooltip);
			objPopupWindow.PopupHiding += objPopupWindow_PopupHiding;

			objInstance.OleComponent.OnCaretMoved += OleComponent_OnCaretMoved;
			objInstance.OleComponent.OnPeriodicIdle += OleComponent_OnPeriodicIdle;
			objInstance.OleComponent.OnIdle += OleComponent_OnIdle;

			toolTip = new ToolTipWindow(this, fontTooltip);
			toolTipMethodInfo = new ToolTipWindow(this, fontTooltip);

			myRunningDocumentTable = new MyRunningDocumentTable();
			myRunningDocumentTable.NewConnection += myRunningDocumentTable_NewConnection;
			myRunningDocumentTable.TextViewShown += myRunningDocumentTable_TextViewShown;
			myRunningDocumentTable.TextViewCreated += myRunningDocumentTable_TextViewCreated;
			myRunningDocumentTable.TextViewClosed += myRunningDocumentTable_TextViewClosed;
			myRunningDocumentTable.KeyDown += myRunningDocumentTable_KeyDown;
			myRunningDocumentTable.KeyUp += myRunningDocumentTable_KeyUp;
			myRunningDocumentTable.GotFocus += myRunningDocumentTable_GotFocus;
			myRunningDocumentTable.LostFocus += myRunningDocumentTable_LostFocus;
			myRunningDocumentTable.MouseHover += myRunningDocumentTable_MouseHover;
			myRunningDocumentTable.MouseWheel += myRunningDocumentTable_MouseWheel;
			myRunningDocumentTable.MouseDown += myRunningDocumentTable_MouseDown;
			myRunningDocumentTable.ScrollBarMoved += myRunningDocumentTable_ScrollBarMoved;
			//myRunningDocumentTable.SplitWindow += myRunningDocumentTable_SplitWindow;

			tokenUsage = new TokenUsage(_applicationObject, this);
			tableUtils = new TableUtils(this);

			debugMatchingParen = new DebugMatchingParen();
			matchingParen = new MatchingParen();

			mouseOverToken = new MouseOverToken(this);
			scanningThread = new ScanningThread(this);
			scanningThread.ErrorScanningDone += scanningThread_ErrorScanningDone;

			smartHelper = new frmSmartHelper3(this);
		}

		public void Dispose() {
			isDisposed = true;

			if (null != cleanUpCode) {
				cleanUpCode.Dispose();
				cleanUpCode = null;
			}

			if (null != intellisense) {
				intellisense.Dispose();
				intellisense = null;
			}

			if (null != objPopupWindow) {
				objPopupWindow.PopupHiding -= objPopupWindow_PopupHiding;
				objPopupWindow.Dispose();
				objPopupWindow = null;
			}

			if (null != scanningThread) {
				scanningThread.ErrorScanningDone -= scanningThread_ErrorScanningDone;
				scanningThread.Dispose();
				scanningThread = null;
			}

			if (null != mouseOverToken) {
				mouseOverToken.InvalidateSquiggleUnderMouseCursor();
				mouseOverToken.Dispose();
				mouseOverToken = null;
			}

			lock (lstWindowData) {
				for (int i = lstWindowData.Count - 1; i >= 0; i--) {
					WindowData windowData = lstWindowData[i];
					DestroyWindowDataEntry(windowData);
					lstWindowData.RemoveAt(i);
				}
			}

			if (null != myRunningDocumentTable) {
				myRunningDocumentTable.NewConnection -= myRunningDocumentTable_NewConnection;
				myRunningDocumentTable.TextViewShown -= myRunningDocumentTable_TextViewShown;
				myRunningDocumentTable.TextViewCreated -= myRunningDocumentTable_TextViewCreated;
				myRunningDocumentTable.TextViewClosed -= myRunningDocumentTable_TextViewClosed;
				myRunningDocumentTable.KeyDown -= myRunningDocumentTable_KeyDown;
				myRunningDocumentTable.KeyUp -= myRunningDocumentTable_KeyUp;
				myRunningDocumentTable.GotFocus -= myRunningDocumentTable_GotFocus;
				myRunningDocumentTable.LostFocus -= myRunningDocumentTable_LostFocus;
				myRunningDocumentTable.MouseHover -= myRunningDocumentTable_MouseHover;
				myRunningDocumentTable.MouseWheel -= myRunningDocumentTable_MouseWheel;
				myRunningDocumentTable.MouseDown -= myRunningDocumentTable_MouseDown;
				myRunningDocumentTable.ScrollBarMoved -= myRunningDocumentTable_ScrollBarMoved;
				//myRunningDocumentTable.SplitWindow -= myRunningDocumentTable_SplitWindow;
				myRunningDocumentTable.Dispose();
				myRunningDocumentTable = null;
			}

			if (null != matchingParen) {
				matchingParen = null;
			}

			if (null != debugMatchingParen) {
				debugMatchingParen = null;
			}

			if (null != tableUtils) {
				tableUtils = null;
			}

			if (null != tokenUsage) {
				tokenUsage.Dispose();
				tokenUsage = null;
			}

			if (null != toolTip) {
				HideToolTipWindow();
				toolTip = null;
			}

			if (null != toolTipMethodInfo) {
				HideToolTipWindow();
				toolTipMethodInfo = null;
			}

			if (null != smartHelper) {
				smartHelper.Dispose();
				smartHelper = null;
			}

			objInstance.OleComponent.OnCaretMoved -= OleComponent_OnCaretMoved;
			objInstance.OleComponent.OnPeriodicIdle -= OleComponent_OnPeriodicIdle;
			objInstance.OleComponent.OnIdle -= OleComponent_OnIdle;

			if (_textDocumentKeyPressEvents != null) {
				_textDocumentKeyPressEvents.BeforeKeyPress -= _textDocumentKeyPressEvents_BeforeKeyPress;
				_textDocumentKeyPressEvents.AfterKeyPress -= _textDocumentKeyPressEvents_AfterKeyPress;
			}
		}

		public bool Initialize() {
			try {
				Events events = _applicationObject.Events;
				// Must save _textDocumentKeyPressEvents globally, or GC will remove it soon
				_textDocumentKeyPressEvents = ((Events2)events).get_TextDocumentKeyPressEvents(null);
				_textDocumentKeyPressEvents.BeforeKeyPress += _textDocumentKeyPressEvents_BeforeKeyPress;
				_textDocumentKeyPressEvents.AfterKeyPress += _textDocumentKeyPressEvents_AfterKeyPress;
			} catch (Exception e) {
				Common.LogEntry(ClassName, "OnConnection", e, "Error while hooking into DTE events.", Common.enErrorLvl.Error);
				return false;
			}
			return true;
		}

		#endregion

		#region Events

		#region Delegates

		public delegate void GotFocusHandler(object sender, GotFocusEventArgs e);

		public delegate void KeyHandler(object sender, KeyEventArgs e);

		public delegate void KeyVsCommandsHandler(object sender, KeyVsCmdEventArgs e);

		public delegate void LostFocusHandler(object sender, LostFocusEventArgs e);

		public delegate void MouseDownHandler(object sender, MouseDownEventArgs e);

		public delegate void MouseHoverHandler(object sender, MouseMoveEventArgs e);

		public delegate bool MouseWheelHandler(object sender, MouseWheelEventArgs e);

		public delegate void NewConnectionHandler(object sender, NewConnectionEventArgs e);

		public delegate void OnCaretMovedHandler(int oldLine, int oldColumn, int newLine, int newColumn);

		public delegate bool ScrollBarMovedHandler(object sender, ScrollBarMovedEventArgs e);

		#endregion

		public event OnCaretMovedHandler OnCaretMoved;
		public event KeyVsCommandsHandler KeyDownVsCommands;

		public event NewConnectionHandler NewConnection;

		public event KeyHandler KeyDown;
		public event KeyHandler KeyUp;

		public event MouseHoverHandler MouseHover;

		public event GotFocusHandler GotFocus;

		public event LostFocusHandler LostFocus;

		public event MouseWheelHandler MouseWheel;

		public event MouseDownHandler MouseDown;

		public event ScrollBarMovedHandler ScrollBarMoved;

		#endregion

		#region Public properties

		public ScanningThread ScanningThread {
			[DebuggerStepThrough]
			get { return scanningThread; }
		}

		public CleanUpCode CleanUpCode {
			[DebuggerStepThrough]
			get { return cleanUpCode; }
		}

		public ToolTipWindow ToolTip {
			[DebuggerStepThrough]
			get { return toolTip; }
		}

		public bool IsInComment {
			[DebuggerStepThrough]
			get { return blnIsInComment; }
			[DebuggerStepThrough]
			set { blnIsInComment = value; }
		}

		public bool IsInString {
			[DebuggerStepThrough]
			get { return blnIsInString; }
			[DebuggerStepThrough]
			set { blnIsInString = value; }
		}

		private static Object InternalSyncObject {
			[DebuggerStepThrough]
			get {
				if (s_InternalSyncObject == null) {
					Object obj = new Object();
					Interlocked.CompareExchange(ref s_InternalSyncObject, obj, null);
				}
				return s_InternalSyncObject;
			}
		}

		public frmSmartHelper3 SmartHelper {
			[DebuggerStepThrough]
			get { return smartHelper; }
		}

		public bool NeedToGetDBConnection {
			[DebuggerStepThrough]
			get { return blnNeedToGetDBConnection; }
			set { blnNeedToGetDBConnection = value; }
		}

		public bool NeedToRedrawDebugWindow {
			[DebuggerStepThrough]
			get { return blnNeedToRedrawDebugWindow; }
			set { blnNeedToRedrawDebugWindow = value; }
		}

		public static WindowData CurrentWindowData {
			[DebuggerStepThrough]
			get { return currentWindowData; }
			set { currentWindowData = value; }
		}

		public Markers MissmatchingRegionMarkers {
			[DebuggerStepThrough]
			get { return missmatchingRegionMarkers; }
		}

		public ToolTipWindow ToolTipMethodInfo {
			[DebuggerStepThrough]
			get { return toolTipMethodInfo; }
		}

		public PopupWindow PopupWindow {
			[DebuggerStepThrough]
			get { return objPopupWindow; }
		}

		public Server ActiveServer {
			[DebuggerStepThrough]
			get { return objActiveServer; }
		}

		public Connection ActiveConnection {
			[DebuggerStepThrough]
			get { return objActiveConnection; }
		}

		public StaticData StaticData {
			[DebuggerStepThrough]
			get { return objStaticData; }
		}

		public List<Function> SysObjectScalarFunctions {
			get { return (null != objActiveConnection ? objActiveConnection.GetScalarSysObjects() : null); }
		}

		public List<SysObject> SysObjects {
			[DebuggerStepThrough]
			get { return (null != objActiveConnection ? objActiveConnection.GetSysObjects() : null); }
		}

		public TokenUsage TokenUsage {
			[DebuggerStepThrough]
			get { return tokenUsage; }
		}

		public List<TokenInfo> RawTokens {
			[DebuggerStepThrough]
			get {
				if (null != currentWindowData && null != currentWindowData.Parser) {
					return currentWindowData.Parser.RawTokens;
				}
				return null;
			}
		}

		public Font FontEditor {
			[DebuggerStepThrough]
			get { return fontEditor; }
			set {
				fontEditor = value;
				objPopupWindow.FontEditor = fontEditor;
				currentWindowData.SplitterRoot.OnFontChange(fontEditor);
			}
		}

		public Font FontTooltip {
			[DebuggerStepThrough]
			get { return fontTooltip; }
			set {
				fontTooltip = value;
				objPopupWindow.FontTooltip = fontTooltip;
			}
		}

		#endregion

		#region OleComponent events

		/// <summary>
		/// 
		/// </summary>
		/// <param name="line"></param>
		/// <param name="column"></param>
		private void OleComponent_OnPeriodicIdle(int line, int column) {
			if (isDisposed || !Instance.Settings.EnableAddin) {
				return;
			}

			try {
			} catch (Exception e) {
				Common.LogEntry(ClassName, "OleComponent_OnPeriodicIdle", e, Common.enErrorLvl.Error);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="line"></param>
		/// <param name="column"></param>
		private void OleComponent_OnIdle(int line, int column) {
			if (isDisposed || !Instance.Settings.EnableAddin) {
				return;
			}

			try {
				if (null != currentWindowData) {
					if (blnNeedToGetDBConnection) {
						try {
							// If the new window hasn't got any cached SysObjects because it has been purged, re-cache
							if (Server.GetActiveServer(_applicationObject, servers, out objActiveServer, out objActiveConnection)) {
								if (null != objActiveServer && null != objActiveConnection) {
									objDebugTokens.LblConnection.Text = "Server: " + objActiveServer.SqlServer + ", database: " + objActiveConnection.ActiveConnection.DatabaseName;
									if (!objActiveConnection.HasSysObjects || objActiveConnection.HasBeenPurged) {
										if (!objActiveConnection.IsScanning) {
											IntPtr hwnd = (null != currentWindowData.SplitterRoot.PrimaryVsEditPane ? currentWindowData.SplitterRoot.PrimaryVsEditPane.HwndVsTextEditPane : IntPtr.Zero);
											TaskGetSysObjects taskGetSysObjects = new TaskGetSysObjects(objActiveConnection, hwnd);
											Instance.BackgroundTask.QueueTask(taskGetSysObjects);
										}
									}
									blnNeedToGetDBConnection = false;
								}
							}
						} catch (Exception e) {
							Common.LogEntry(ClassName, "OleComponent_OnIdle", e, "Error when retrieving sysobject data from database", Common.enErrorLvl.Error);
						}
					}

					if (Instance.Settings.ShowDebugWindow) {
						try {
							bool IsInsideToken;
							TokenInfo prevToken = null;
							TokenInfo prevprevToken = null;
							int tokenIndex = GetTokenIndex(RawTokens, line, column, out IsInsideToken);
							if (-1 != tokenIndex) {
								lock (RawTokens) {
									prevToken = RawTokens[tokenIndex];
									if (tokenIndex > 0) {
										prevprevToken = RawTokens[tokenIndex - 1];
									}
									blnIsInComment = false;
									blnIsInString = false;

									if (prevToken.Kind == TokenKind.SingleLineComment && line == prevToken.Span.iStartLine) {
										blnIsInComment = true;
									} else if (prevToken.Kind == TokenKind.MultiLineComment) {
										blnIsInComment = true;
									} else if (prevToken.Kind == TokenKind.ValueString && IsInsideToken) {
										blnIsInString = true;
									}
								}
							}

							// Show debug info
							objDebugTokens.LblCurrentToken.Text = "Current: " + prevToken ?? "None" + (IsInsideToken ? ", Inside" : ", Outside");
							objDebugTokens.LblPreviousToken.Text = "Previous: " + prevprevToken ?? "None";
							objDebugTokens.LblCurrentPos.Text = "Pos: (" + line + ", " + column + ")";
							objDebugTokens.LblIntellisensWindowVisible.Text = "Intellisense window is: " + (objPopupWindow.IsPopupVisible ? "visible" : "not visible");
							objDebugTokens.Text = (null != RawTokens ? RawTokens.Count.ToString() : "no") + " tokens";
							objDebugTokens.LblInString.Text = "In str (" + blnIsInString + ")";
							objDebugTokens.LblInComment.Text = "In Comment (" + blnIsInComment + ")";
						} catch (Exception e) {
							Common.LogEntry(ClassName, "OleComponent_OnIdle", e, "Error when filling debug window: ", Common.enErrorLvl.Warning);
						}
					}

					try {
						if (Instance.Settings.EnableMouseOverTokenTooltip) {
							mouseOverToken.HandleMouseCursorOverToken(CurrentWindowData);
						}
					} catch (Exception e) {
						Common.LogEntry(ClassName, "OleComponent_OnIdle", e, "Error when handling mouse-over-token: ", Common.enErrorLvl.Error);
					}

					if (blnUserPressedAltEnter) {
						try {
							if (smartHelper.Visible) {
								smartHelper.ShowFullList(true);
							}
						} catch (Exception e) {
							Common.LogEntry(ClassName, "OleComponent_OnIdle", e, "Error when handling alt+enter: ", Common.enErrorLvl.Error);
						}
						blnUserPressedAltEnter = false;
					}

					if (blnUserPressedRenameToken) {
						try {
							RenameToken(line, column);
						} catch (Exception e) {
							Common.LogEntry(ClassName, "OleComponent_OnIdle", e, "Error when handling shift+f6: ", Common.enErrorLvl.Error);
						}
						blnUserPressedRenameToken = false;
					}

					if (blnUserPressedCompleteWord) {
						try {
							HandleCompleteWord();
						} catch (Exception e) {
							Common.LogEntry(ClassName, "OleComponent_OnIdle", e, "Error when handling ctrl+space: ", Common.enErrorLvl.Error);
						}
						blnUserPressedCompleteWord = false;
					}
				} else {
					if (Instance.Settings.ShowDebugWindow) {
						try {
							// Show debug info
							objDebugTokens.LblCurrentToken.Text = "Current: No code window";
							objDebugTokens.LblPreviousToken.Text = "Previous: No code window";
							objDebugTokens.LblCurrentPos.Text = "Pos: (0,0)";
							objDebugTokens.LblIntellisensWindowVisible.Text = "Intellisense window is: No code window";
							objDebugTokens.Text = "0 tokens";
							objDebugTokens.LblInString.Text = "In str (" + blnIsInString + ")";
							objDebugTokens.LblInComment.Text = "In Comment (" + blnIsInComment + ")";
						} catch (Exception e) {
							Common.LogEntry(ClassName, "OleComponent_OnIdle", e, "Error when filling debug window: ", Common.enErrorLvl.Warning);
						}
					}
				}

				if (blnNeedToRedrawDebugWindow && Instance.Settings.ShowDebugWindow) {
					try {
						RedrawListBox2(line);
					} catch (Exception e) {
						Common.LogEntry(ClassName, "OleComponent_OnIdle", e, "Error when redrawing debugwindow: ", Common.enErrorLvl.Warning);
					}
					blnNeedToRedrawDebugWindow = false;
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "OleComponent_OnIdle", e, Common.enErrorLvl.Error);
			}
		}

		private void OleComponent_OnCaretMoved(int oldLine, int oldColumn, int newLine, int newColumn) {
			if (isDisposed || !Instance.Settings.EnableAddin) {
				return;
			}

			try {
				if (null != OnCaretMoved) {
					OnCaretMoved(oldLine, oldColumn, newLine, newColumn);
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "OleComponent_OnCaretMoved", e, Common.enErrorLvl.Error);
			}

			try {
				bool IsInsideToken;
				TokenInfo currentToken = null;
				int tokenIndex = GetTokenIndex(RawTokens, newLine, newColumn, out IsInsideToken);
				if (-1 != tokenIndex) {
					currentToken = RawTokens[tokenIndex];
				}

				frmSmartHelper3.HandleSmartHelper(smartHelper, CurrentWindowData.Parser, currentWindowData, newLine, newColumn);

				// Matching braces?
				if (Instance.Settings.ShowMatchingBraces && null != currentToken) {
					if (tokenIndex + 1 < RawTokens.Count) {
						// Handle if token adjacent to left paranthesis
						if (RawTokens[tokenIndex + 1].Kind == TokenKind.LeftParenthesis) {
							currentToken = RawTokens[tokenIndex + 1];
						}
					}
					if (-1 != currentToken.MatchingParenToken && currentToken.Span.iStartLine == newLine && (currentToken.Kind == TokenKind.LeftParenthesis && currentToken.Span.iStartIndex == newColumn || currentToken.Kind == TokenKind.RightParenthesis && currentToken.Span.iEndIndex == newColumn)) {
						matchingParen.selectMatchingTable((currentToken.Kind == TokenKind.LeftParenthesis ? currentToken : RawTokens[currentToken.MatchingParenToken]), RawTokens);
					} else {
						matchingParen.InvalidateMatchingTableSquiggle();
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "OleComponent_OnCaretMoved", e, Common.enErrorLvl.Error);
			}

			// Update tooltip position (in method parameters)
			try {
				if (null != toolTipMethodInfo && (toolTipMethodInfo.ToolTipEnabled || toolTipMethodInfo.Visible)) {
					toolTipMethodInfo.OnCaretMoved(oldLine, oldColumn, newLine, newColumn);
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "OleComponent_OnCaretMoved", e, Common.enErrorLvl.Error);
			}
		}

		#endregion

		#region IOleCommandTarget

		protected bool WindowData_KeyDownVsCommands(object sender, KeyVsCmdEventArgs e) {
			if (!Instance.Settings.EnableAddin) {
				return false;
			}

			if (null != KeyDownVsCommands) {
				KeyDownVsCommands(sender, e);
				if (e.Cancel) {
					return true;
				}
			}

			if (objPopupWindow.IsPopupVisible) {
				return false;
			}

			switch (e.VsCmd) {
				case Common.enVsCmd.Cut:
				case Common.enVsCmd.Paste:
				case Common.enVsCmd.Undo:
				case Common.enVsCmd.Redo:
				case Common.enVsCmd.Comment:
				case Common.enVsCmd.Uncomment:
					ScheduleFullReparse();
					break;

				case Common.enVsCmd.Escape:
					KeypressEscape();
					break;

				case Common.enVsCmd.CompleteWord:
					blnUserPressedCompleteWord = true;
					break;
			}
			return false;
		}

		#endregion

		#region RunningDocumentTable

		/// <summary>
		/// Take care of when a new active view has been visible
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void myRunningDocumentTable_TextViewCreated(object sender, ActiveViewEventArgs e) {
			NewActiveView(e.WindowData);

			if (!Instance.Settings.EnableAddin || null == e.WindowData || isDisposed) {
				return;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void myRunningDocumentTable_TextViewShown(object sender, ActiveViewEventArgs e) {
			if (isDisposed || !Instance.Settings.EnableAddin) {
				return;
			}

			lock (lstWindowData) {
				foreach (WindowData windowData in lstWindowData) {
					if (windowData == e.WindowData) {
						SetNewActiveView(windowData);

//						Common.LogEntry(ClassName, "myRunningDocumentTable_TextViewShown", "change to " + windowData.ActiveView.GetHashCode(), Common.enErrorLvl.Information);
						return;
					}
				}
			}

			Common.LogEntry(ClassName, "myRunningDocumentTable_TextViewShown", "Unable to find window to change to " + e.WindowData.ActiveView.GetHashCode(), Common.enErrorLvl.Error);
		}

		/// <summary>
		/// Close active window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void myRunningDocumentTable_TextViewClosed(object sender, ActiveViewEventArgs e) {
			if (isDisposed || !Instance.Settings.EnableAddin) {
				return;
			}

//			Common.LogEntry(ClassName, "myRunningDocumentTable_TextViewClosed", "close " + e.WindowData.ActiveView.GetHashCode(), Common.enErrorLvl.Information);

			lock (lstWindowData) {
				if (null != e.WindowData) {
					foreach (WindowData windowData in lstWindowData) {
						if (e.WindowData == windowData) {
							DestroyWindowDataEntry(windowData);
							break;
						}
					}
					if (0 == lstWindowData.Count) {
						currentWindowData = null;
					}
				} else {
					if (null != currentWindowData) {
						currentWindowData.KeyDownVsCommands -= WindowData_KeyDownVsCommands;
					}
					currentWindowData = null;
				}
			}
		}

		/// <summary>
		/// Destroy a WindowData object, i.e. a window was closed
		/// </summary>
		/// <param name="windowData"></param>
		private void DestroyWindowDataEntry(WindowData windowData) {
			try {
				lstWindowData.Remove(windowData);
				windowData.Dispose();
			} catch (Exception e) {
				Common.LogEntry(ClassName, "DestroyWindowDataEntry", e, Common.enErrorLvl.Error);
			}
		}

		/// <summary>
		/// Activate a WindowData, i.e. a windows has been activated/got focus
		/// </summary>
		/// <param name="windowData"></param>
		private void SetNewActiveView(WindowData windowData) {
			try {
				if (currentWindowData != windowData) {
//					Common.LogEntry(ClassName, "SetNewActiveView", "Activating active view: " + windowData.ActiveView.GetHashCode(), Common.enErrorLvl.Information);

					if (null != currentWindowData) {
						currentWindowData.KeyDownVsCommands -= WindowData_KeyDownVsCommands;
					}
					currentWindowData = windowData;
					currentWindowData.KeyDownVsCommands += WindowData_KeyDownVsCommands;

					blnIsInComment = false;
					blnIsInString = false;
					blnNeedToGetDBConnection = true;

					SysObject.RemoveTemporarySysObjects(SysObjects);

					ScheduleFullReparse();
				}

				if (null != RawTokens && RawTokens.Count < 50000 && Instance.Settings.ShowDebugWindow && Instance.Settings.EnableAddin) {
					objDebugTokens.SetWindowDataList(lstWindowData, currentWindowData);
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "SetNewActiveView", e, Common.enErrorLvl.Error);
			}
		}

		/// <summary>
		/// A new WindowData object added, i.e. a new window is opened
		/// </summary>
		/// <param name="windowData"></param>
		private void NewActiveView(WindowData windowData) {
			try {
//				Common.LogEntry(ClassName, "NewActiveView", "Added new active view: " + windowData.ActiveView.GetHashCode(), Common.enErrorLvl.Information);
				lstWindowData.Add(windowData);

				SysObject.RemoveTemporarySysObjects(SysObjects);
			} catch (Exception e) {
				Common.LogEntry(ClassName, "NewActiveView", e, Common.enErrorLvl.Error);
			}
		}

		/// <summary>
		/// A connection from one database to another has changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void myRunningDocumentTable_NewConnection(object sender, NewConnectionEventArgs e) {
			if (isDisposed || !Instance.Settings.EnableAddin) {
				return;
			}

			if (null != NewConnection) {
				NewConnection(sender, e);
			}

			blnNeedToGetDBConnection = true;
			ScheduleFullReparse();
		}

		/// <summary>
		/// An existing window has recieved focus
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void myRunningDocumentTable_GotFocus(object sender, GotFocusEventArgs e) {
			if (null != GotFocus) {
				GotFocus(sender, e);
			}

			SetNewActiveView(e.CurrentWindowData);

			int line;
			int column;
			e.CurrentWindowData.ActiveView.GetCaretPos(out line, out column);
			frmSmartHelper3.HandleSmartHelper(smartHelper, CurrentWindowData.Parser, currentWindowData, line, column);
		}

		/// <summary>
		/// An existing window has lost focus
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void myRunningDocumentTable_LostFocus(object sender, LostFocusEventArgs e) {
			if (null != LostFocus) {
				LostFocus(sender, e);
			}
		}

		private bool myRunningDocumentTable_MouseDown(object sender, MouseDownEventArgs e) {
			if (null != MouseDown) {
				MouseDown(sender, e);
				if (e.Cancel) {
					return true;
				}
			}
			return false;
		}

		private bool myRunningDocumentTable_ScrollBarMoved(object sender, ScrollBarMovedEventArgs e) {
			if (null != ScrollBarMoved) {
				ScrollBarMoved(sender, e);
				if (e.Cancel) {
					return true;
				}
			}

			return false;
		}

		private bool myRunningDocumentTable_MouseWheel(object sender, MouseWheelEventArgs e) {
			if (null != MouseWheel) {
				MouseWheel(sender, e);
				if (e.Cancel) {
					return true;
				}
			}

			return false;
		}

		private void myRunningDocumentTable_MouseHover(object sender, MouseMoveEventArgs e) {
			if (null != MouseHover) {
				MouseHover(sender, e);
			}
		}

		/// <summary>
		/// Handle keydown event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private bool myRunningDocumentTable_KeyDown(object sender, KeyEventArgs e) {
			if (isDisposed || !Instance.Settings.EnableAddin) {
				return false;
			}

			if (null != KeyDown) {
				KeyDown(sender, e);
				if (e.Cancel) {
					return true;
				}
			}

			switch (e.VK) {
				case NativeWIN32.VirtualKeys.Return:
					if (!e.IsShift && e.IsAlt && !e.IsCtrl) {
						ShowSmartHelper();
					}
					break;
			}
			return false;
		}

		/// <summary>
		/// Handle keyup event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		private bool myRunningDocumentTable_KeyUp(object sender, KeyEventArgs e) {
			if (null != KeyUp) {
				KeyUp(sender, e);
				if (e.Cancel) {
					return true;
				}
			}

			return false;
		}

		#endregion

		#region _textDocumentKeyPressEvents_* events

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Keypress"></param>
		/// <param name="Selection"></param>
		/// <param name="InStatementCompletion">If True the keystroke causes Intellisense to show a popup menu</param>
		/// <param name="CancelKeypress"></param>
		private void _textDocumentKeyPressEvents_BeforeKeyPress(string Keypress, TextSelection Selection, bool InStatementCompletion, ref bool CancelKeypress) {
			if (isDisposed || !Common.IsAddinEnabled(Instance.Settings) || null == currentWindowData) {
				return;
			}

			try {
				if (null != toolTip) {
					if (toolTip.ToolTipEnabled || toolTip.Visible) {
						HideToolTipWindow();
						if (Keypress[0] == Common.chrKeyTab) {
							toolTip.LiveTemplate.Execute(Selection);
							ScheduleFullReparse();
							CancelKeypress = true;
							return;
						}
					}
				}
				if (null != toolTipMethodInfo) {
					if (toolTipMethodInfo.ToolTipEnabled || toolTipMethodInfo.Visible) {
						// Do nothing
					}
				}

				if (objPopupWindow.IsPopupVisible) {
					HandleBeforeKeyPressWithIntellisense(Keypress, Selection, ref CancelKeypress);
				} else if (Instance.Settings.AutomaticallyShowCompletionWindow) {
					HandleBeforeKeyPress(Keypress, Selection, ref CancelKeypress);
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "_textDocumentKeyPressEvents_BeforeKeyPress", e, Common.enErrorLvl.Error);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Keypress"></param>
		/// <param name="Selection"></param>
		/// <param name="InStatementCompletion"></param>
		private void _textDocumentKeyPressEvents_AfterKeyPress(string Keypress, TextSelection Selection, bool InStatementCompletion) {
			if (isDisposed || null == currentWindowData || !Common.IsAddinEnabled(Instance.Settings)) {
				return;
			}

			// If committed, just ignore the key just entered and scan for code to cleanup
			if (Keypress[0] == Common.chrKeyEnter && Instance.Settings.CommittedByEnter
					|| Keypress[0] == Common.chrKeyTab && Instance.Settings.CommittedByTab
					|| Keypress[0] == Common.chrKeySpace && Instance.Settings.CommittedBySpaceBar) {
				scanningThread.AbortAll();
				scanningThread.StartAsynchroneFullParse(() => {
					cleanUpCode.QueueCleanUpCode();
				});
				return;
			}

			cleanUpCode.AddTextBufferChange();

			int piLine;
			int piColumn;
			if (!NativeMethods.Succeeded(CurrentWindowData.ActiveView.GetCaretPos(out piLine, out piColumn))) {
				piLine = -1;
			}

			if (!objPopupWindow.IsPopupVisible && Instance.Settings.AutomaticallyShowCompletionWindow) {
				int initialDelay = Instance.Settings.FullParseInitialDelay;
				if (Instance.Settings.CommittedByCharacters.IndexOf(Keypress[0]) > 0) {
					initialDelay = 10;
				}
				scanningThread.StartAsynchroneFullParse(initialDelay, () => {
					cleanUpCode.QueueCleanUpCode();

					int piLine2;
					int piColumn2;
					if (!NativeMethods.Succeeded(CurrentWindowData.ActiveView.GetCaretPos(out piLine2, out piColumn2))) {
						piLine2 = -1;
					}
					if (piLine == piLine2) {
						intellisense.HandleIntellisenseItem(Keypress, Selection);
					}
				});
			}
		}

		#endregion

		#region HandleBeforeKeyPress

		private void HandleBeforeKeyPress(string Keypress, TextSelection Selection, ref bool CancelKeypress) {
			try {
				if (HandleRemoveSpecialChars(Keypress, Selection)) {
					return;
				}

				if (blnIsInString || blnIsInComment) {
					objPopupWindow.CanShowPopup = false;
				} else {
					if (Keypress[0] == Common.chrKeyEnter) {
						#region Handle smart indenting after press of enter

						if (Instance.Settings.SmartIndenting) {
							objPopupWindow.CanShowPopup = true;
							IVsTextView activeView = CurrentWindowData.ActiveView;

							try {
								Instance.ApplicationObject.UndoContext.Open("HandleBeforeKeyPress.SmartIndenting", true);

								int oldColumn;
								int oldLine;
								int column;
								int line;
								activeView.GetCaretPos(out oldLine, out oldColumn);
								if (SmartIndenter.GetSmartIndentColumn(activeView, out line, out column)) {
									IVsTextLines ppBuffer;
									activeView.GetBuffer(out ppBuffer);
									int lineLength;
									string buffer;
									ppBuffer.GetLengthOfLine(line, out lineLength);
									ppBuffer.GetLineText(line, 0, line, lineLength, out buffer);
									string leftOfCursor = string.Empty;
									string rightOfCursor = string.Empty;
									if (oldColumn <= buffer.Length) {
										leftOfCursor = buffer.Substring(0, oldColumn);
										rightOfCursor = buffer.Substring(oldColumn);
									}

									if (0 != column) {
										// Clear the previous line if it's just whitespace
										if (0 == leftOfCursor.Trim().Length && 0 == rightOfCursor.Trim().Length) {
											// Replace the text in the textbuffer
											activeView.SetCaretPos(line, 0);
											Selection.Insert("\r\n", (int)vsInsertFlags.vsInsertFlagsCollapseToEnd);
											// Position the cursor at the new column
											activeView.SetCaretPos(line + 1, column);
										} else {
											Selection.Insert("\r\n", (int)vsInsertFlags.vsInsertFlagsCollapseToEnd);
											if (0 != rightOfCursor.Length) {
												string indentString = SmartIndenter.FormatIndentString(column);
												Selection.Insert(indentString, (int)vsInsertFlags.vsInsertFlagsCollapseToEnd);
											} else {
												activeView.SetCaretPos(line + 1, column);
											}
										}
									} else {
										Selection.Insert("\r\n", (int)vsInsertFlags.vsInsertFlagsCollapseToEnd);
									}

									Common.MakeSureCursorIsVisible(activeView);
									CancelKeypress = true;
									ScheduleFullReparse();
								}
							} finally {
								if (Instance.ApplicationObject.UndoContext.IsOpen) {
									Instance.ApplicationObject.UndoContext.Close();
								}
							}
						}

						#endregion
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "HandleBeforeKeyPress", e, Common.enErrorLvl.Error);
			}
		}

		private bool HandleRemoveSpecialChars(string Keypress, TextSelection Selection) {
			if (Instance.Settings.EnableAutoInsertPairParanthesesAndQuotes && !blnIsInComment) {
				if (Keypress[0] == Common.chrKeyBackSpace) {
					#region Handle remove of (, [ and '

					int currentColumn = Selection.ActivePoint.VirtualCharOffset - 1;
					EditPoint epSel = Selection.ActivePoint.CreateEditPoint();
					EditPoint spSel = Selection.ActivePoint.CreateEditPoint();
					spSel.StartOfLine();
					epSel.EndOfLine();
					string previousChars = spSel.GetText(epSel);
					if (previousChars.Length >= currentColumn) {
						string leftOfCursor = previousChars.Substring(0, currentColumn);
						string rightOfCursor = previousChars.Substring(currentColumn);

						if (leftOfCursor.EndsWith("(") && rightOfCursor.StartsWith(")")) {
							Selection.Delete(1);
							return true;
						}
						if (leftOfCursor.EndsWith("[") && rightOfCursor.StartsWith("]")) {
							Selection.Delete(1);
							return true;
						}
						if (leftOfCursor.EndsWith("'")) {
							int nbOfCitationLeft = leftOfCursor.Length - leftOfCursor.Replace("'", string.Empty).Length;
							if (nbOfCitationLeft % 2 == 1 && rightOfCursor.StartsWith("'")) {
								Selection.Delete(1);
							}
							return true;
						}
					}

					#endregion
				}
			}

			return false;
		}

		#endregion

		#region HandleBeforeKeyPressWithIntellisense

		private void HandleBeforeKeyPressWithIntellisense(string Keypress, TextSelection Selection, ref bool CancelKeypress) {
			try {
				int currentLine = Selection.CurrentLine - 1;
				int currentColumn = Selection.ActivePoint.VirtualCharOffset - 1;

				if (blnIsInString || blnIsInComment) {
					HidePopupWindow(false);
				} else {
					if (Keypress[0] == Common.chrKeyBackSpace) {
						string strTextDeleted = Selection.Text;
						if (0 == strTextDeleted.Length) {
							strTextDeleted = Keypress;
							if (!objPopupWindow.DeleteChar(strTextDeleted)) {
								// The user deleted to much. Abort
								HidePopupWindow(false);
							}
						} else {
							if (!objPopupWindow.DeleteChar(strTextDeleted, false)) {
								// The user deleted to much. Abort
								HidePopupWindow(false);
							}
						}
						if (HandleRemoveSpecialChars(Keypress, Selection)) {
							return;
						}

					} else if (Keypress[0] == Common.chrKeyEnter && Instance.Settings.CommittedByEnter
								|| Keypress[0] == Common.chrKeyTab && Instance.Settings.CommittedByTab
								|| Keypress[0] == Common.chrKeySpace && Instance.Settings.CommittedBySpaceBar
								|| Instance.Settings.CommittedByCharacters.IndexOf(Keypress[0]) > 0) {
						IntellisenseData idData;
						EditPoint epSelection;
						string preSelect;
						string postSelect;
						TokenInfo prevToken;
						int prevTokenIndex = -1;

						if (objPopupWindow.SelectItem(out idData, out preSelect, out postSelect, out epSelection, out prevToken)) {
							for (int i = 0; i < RawTokens.Count; i++) {
								if (prevToken == RawTokens[i]) {
									prevTokenIndex = i;
									break;
								}
							}
							// Get the text the user selected
							string strSelectedText = idData.GetSelectedData;
							EditPoint epStartInsertPoint = epSelection.CreateEditPoint();

							try {
								Instance.ApplicationObject.UndoContext.Open("LiveTemplate", true);

								epSelection.Delete(preSelect.Length);

								#region Live template

								if (idData is LiveTemplate) {
									int startColumn = epSelection.DisplayColumn - 1;

									// Split into rows, and find end position
									string[] rows = strSelectedText.Split('\n');
									int endCol = -1;
									int endRow = currentLine;
									for (int i = 0; i < rows.Length; i++) {
										string row = rows[i];
										endCol = row.IndexOf("$end$");
										if (endCol >= 0) {
											endCol = SmartIndenter.TabsToSpaces(startColumn, row).IndexOf("$end$");
											rows[i] = row.Replace("$end$", string.Empty);
											break;
										}
										endCol = -1;
										endRow++;
									}

									// Add the start position to the end column
									endCol += startColumn;
									// Create the indent string for multiline inserts
									string indentString = string.Empty;
									if (rows.Length > 1) {
										indentString = SmartIndenter.FormatIndentString(startColumn);
									}

									// If the text contains some keywords, act on them
									if (strSelectedText.EndsWith(Common.cstrIntellisenseExpandSysObjects)) {
										// Tables
										rows[rows.Length - 1] = rows[rows.Length - 1].Substring(0, rows[rows.Length - 1].Length - 2);
										for (int i = 0; i < rows.Length; i++) {
											string row = rows[i];
											if (0 != i) {
												epSelection.Insert("\n" + indentString);
											}
											epSelection.Insert(row);
										}

										int newEndRow;
										if (Common.FindActualLineNumber(CurrentWindowData.ActiveView, endRow, out newEndRow)) {
											Selection.MoveToDisplayColumn(newEndRow + 1, endCol + 1, false);
										}

										// List tables in a new window
										List<Common.enSqlTypes> lstSqlTypes = new List<Common.enSqlTypes> {
											Common.enSqlTypes.Tables,
											Common.enSqlTypes.DerivedTable,
											Common.enSqlTypes.CTE,
											Common.enSqlTypes.Temporary,
											Common.enSqlTypes.Views,
											Common.enSqlTypes.TableValuedFunctions,
											Common.enSqlTypes.ScalarValuedFunctions
										};

										HidePopupWindow(true);
										bool IsInsideToken;
										int tokenIndex = GetTokenIndex(RawTokens, currentLine, currentColumn, out IsInsideToken);
										if (-1 != tokenIndex) {
											prevTokenIndex = tokenIndex;
											prevToken = RawTokens[tokenIndex];
										}

										scanningThread.StartAsynchroneFullParse(10, () =>
											PopupWindow.ShowPopupWithSysObjects(this, CurrentWindowData.Parser, objActiveConnection, epSelection, prevTokenIndex, prevToken, string.Empty, string.Empty, lstSqlTypes, "dbo", false)
										);
										CancelKeypress = true;

									} else if (strSelectedText.EndsWith(Common.cstrIntellisenseExpandAlias)) {
										// Table Aliases
										rows[rows.Length - 1] = rows[rows.Length - 1].Substring(0, rows[rows.Length - 1].Length - 2);
										for (int i = 0; i < rows.Length; i++) {
											string row = rows[i];
											if (0 != i) {
												epSelection.Insert("\n" + indentString);
											}
											epSelection.Insert(row);
										}

										int newEndRow;
										if (Common.FindActualLineNumber(CurrentWindowData.ActiveView, endRow, out newEndRow)) {
											Selection.MoveToDisplayColumn(newEndRow + 1, endCol + 1, false);
										}

										HidePopupWindow(true);

										scanningThread.StartAsynchroneFullParse(10, () =>
											// List table alias found in the document in a new window
											PopupWindow.ShowPopupWithScannedTables(this, CurrentWindowData.Parser, epSelection, string.Empty, string.Empty, unknownPrevToken)
										);
										CancelKeypress = true;

									} else {
										for (int i = 0; i < rows.Length; i++) {
											string row = rows[i];
											if (0 != i) {
												epSelection.Insert("\n" + indentString);
											}
											epSelection.Insert(row);
										}

										int newEndRow;
										if (Common.FindActualLineNumber(CurrentWindowData.ActiveView, endRow, out newEndRow)) {
											Selection.MoveToDisplayColumn(newEndRow + 1, endCol + 1, false);
										}

										HidePopupWindow(true);
										CancelKeypress = true;
									}

									return;
								}

								#endregion

								// Insert the new text
								epSelection.Insert(strSelectedText);

								if (Keypress[0] == Common.chrKeyTab && Instance.Settings.CommittedByTab) {
									// Overwrite
									epSelection.Delete(postSelect.Length);
								} else if (Keypress[0] == Common.chrKeySpace && Instance.Settings.CommittedBySpaceBar) {
									// Insert and add a space
									epSelection.Insert(" ");
								} else if (Keypress[0] == Common.chrKeyEnter && Instance.Settings.CommittedByEnter) {
									// Insert
								}
							} finally {
								if (Instance.ApplicationObject.UndoContext.IsOpen) {
									Instance.ApplicationObject.UndoContext.Close();
								}
							}

							if (Keypress[0] == Common.chrKeyEnter && Instance.Settings.CommittedByEnter
								|| Keypress[0] == Common.chrKeyTab && Instance.Settings.CommittedByTab
								|| Keypress[0] == Common.chrKeySpace && Instance.Settings.CommittedBySpaceBar) {
								HidePopupWindow(true);

								scanningThread.StartAsynchroneFullParse(10, () => {
									// If it's a table we just intellisene'd, create an alias for it
									if (idData is SysObject) {
										SysObject sysObject = (SysObject)idData;
										if (sysObject.SqlType == Common.enSqlTypes.Tables || sysObject.SqlType == Common.enSqlTypes.DerivedTable || sysObject.SqlType == Common.enSqlTypes.Temporary || sysObject.SqlType == Common.enSqlTypes.CTE || sysObject.SqlType == Common.enSqlTypes.Views || sysObject.SqlType == Common.enSqlTypes.TableValuedFunctions) {
											bool isInsideToken;
											int tokenIndex = GetTokenIndex(RawTokens, currentLine, currentColumn, out isInsideToken);
											StatementSpans span = CurrentWindowData.Parser.SegmentUtils.GetStatementSpan(tokenIndex);
											if (null != span && (span.SegmentStartToken.StartToken == Tokens.kwSelectToken || (-1 != span.FromIndex && span.FromIndex < tokenIndex && !(span.SegmentStartToken.StartToken == Tokens.kwDeleteToken && span.FromIndex - 1 == span.StartIndex)))) {
												foreach (TableSource tableSource in CurrentWindowData.Parser.TableSources) {
													if (tableSource.Table.SysObject == sysObject && TextSpanHelper.ContainsInclusive(RawTokens[tableSource.Table.TableIndex].Span, currentLine, currentColumn)) {
														if (Instance.Settings.AutoInsertSysobjectSchema) {
															SysObject.InsertSchema(sysObject, epStartInsertPoint);
														}
														if (Instance.Settings.AutoInsertSysobjectAlias) {
															SysObject.CreateTableAlias(CurrentWindowData.Parser, sysObject, Instance.Settings.AutoInsertTokenAS, epSelection, prevToken.ParenLevel, false, tableSource.StartIndex);
														}
														ScheduleFullReparse();
														break;
													}
												}
											}
										} else if (sysObject.SqlType == Common.enSqlTypes.SPs) {
											if (sysObject.Parameters.Count > 0) {
												ToolTipWindow.ShowNormalToolTipWindow(toolTip, "Press Tab to insert stored procedure parameters.", new ToolTipLiveTemplateInsertSprocParams(toolTip, sysObject), FontEditor);
											}
										}
									}
								});
								CancelKeypress = true;

							} else if (Keypress[0] == Common.chrKeyDot) {
								if (idData is SysObject) {
									SysObject sysObject = (SysObject)idData;
									if (sysObject.SqlType == Common.enSqlTypes.Tables || sysObject.SqlType == Common.enSqlTypes.DerivedTable || sysObject.SqlType == Common.enSqlTypes.CTE || sysObject.SqlType == Common.enSqlTypes.Temporary) {
										// We'll handle the dot ourselves, so the event AfterKeyPress doesn't cut in,
										// since we want the popup to show again right away with new content
										epSelection.Insert(".");
										CancelKeypress = true;
										HidePopupWindow(true);
										scanningThread.StartAsynchroneFullParse(10, () =>
											PopupWindow.ShowPopupWithColumns(this, CurrentWindowData.Parser, epSelection, strSelectedText, string.Empty, string.Empty, string.Empty, prevTokenIndex, prevToken)
										);
									} else {
										CancelKeypress = true;
										HidePopupWindow(true);
									}
								} else if (idData is Database) {
									Database selectedDatabase = idData as Database;
									// We'll handle the dot ourselves, so the event AfterKeyPress doesn't cut in,
									// since we want the popup to show again right away with new content
									epSelection.Insert(".");
									CancelKeypress = true;

									Connection foundConnection = null;
									foreach (Connection connection in objActiveServer.Connections) {
										if (connection.ActiveConnection.DatabaseName.Equals(selectedDatabase.MainText, StringComparison.OrdinalIgnoreCase)) {
											foundConnection = connection;
										}
									}

									if (null == foundConnection) {
										Server.AddNewConnection(objActiveServer, selectedDatabase.MainText, out foundConnection);
									}

									if (null != foundConnection) {
										PopupWindow.ShowPopupWithSchemas(this, foundConnection, epSelection, prevToken, string.Empty, string.Empty);
									}
								} else if (idData is Table) {
									Table table = (Table)idData;
									// We'll handle the dot ourselves, so the event AfterKeyPress doesn't cut in,
									// since we want the popup to show again right away with new content
									epSelection.Insert(".");
									CancelKeypress = true;
									HidePopupWindow(true);
									scanningThread.StartAsynchroneFullParse(10, () =>
										PopupWindow.ShowPopupWithColumns(this, CurrentWindowData.Parser, epSelection, string.Empty, string.Empty, table.TableName, table.Alias, prevTokenIndex, prevToken)
									);

								} else if (idData is SysObjectSchema) {
									SysObjectSchema sysSchema = idData as SysObjectSchema;
									// We'll handle the dot ourselves, so the event AfterKeyPress doesn't cut in,
									// since we want the popup to show again right away with new content
									epSelection.Insert(".");
									CancelKeypress = true;

									List<Common.enSqlTypes> lstSqlTypes = Intellisense.ConstructSqlTypesList(unknownPrevToken);
									HidePopupWindow(true);
									scanningThread.StartAsynchroneFullParse(10, () =>
										PopupWindow.ShowPopupWithSysObjects(this, CurrentWindowData.Parser, sysSchema.Connection, epSelection, prevTokenIndex, prevToken, string.Empty, string.Empty, lstSqlTypes, sysSchema.Schema, false)
									);
								} else {
									CancelKeypress = true;
									HidePopupWindow(true);
								}
							} else if (Instance.Settings.CommittedByCharacters.IndexOf(Keypress[0]) > 0) {
								HidePopupWindow(true);
								epSelection.Insert(Keypress[0].ToString());
								CancelKeypress = true;
							}
						} else {
							HidePopupWindow(false);
						}
					} else if (Keypress[0] == Common.chrKeyEnter && !Instance.Settings.CommittedByEnter
								|| Keypress[0] == Common.chrKeyTab && !Instance.Settings.CommittedByTab
								|| Keypress[0] == Common.chrKeySpace && !Instance.Settings.CommittedBySpaceBar) {
						CancelKeypress = true;

					} else if (Keypress[0] == '*' && 0 == objPopupWindow.PreSelectText.Length) {
						HidePopupWindow(false);
						ToolTipLiveTemplate objTTLT = new ToolTipLiveTemplateExpandTableAlias(toolTip, this);
						ToolTipWindow.ShowNormalToolTipWindow(toolTip, "Press Tab to expand columns.", objTTLT, FontEditor);
						EditPoint epSelection = Selection.ActivePoint.CreateEditPoint();
						epSelection.Insert(Keypress[0].ToString());
						CancelKeypress = true;
					} else if (Keypress[0] == Common.chrKeyCitation) {
						// A string is starting. Hide the popup. Let the HandleAfterKeypress handle the rest
						HidePopupWindow(false);
					} else if (Keypress[0] == '-' || Keypress[0] == '*' || Keypress[0] == '/') {
						// A comment is starting. Hide the popup
						HidePopupWindow(false);

						EditPoint epSelection = Selection.ActivePoint.CreateEditPoint();
						epSelection.Insert(Keypress[0].ToString());
						CancelKeypress = true;
					} else {
						if (!objPopupWindow.AddChar(Keypress)) {
							// No items was selectable in the popup window list. Abort.
							//HidePopupWindow(false);
						}
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "HandleBeforeKeyPressWithIntellisense", e, Common.enErrorLvl.Error);
			}
		}

		#endregion

		#region Utils

		public void ScheduleFullReparse() {
			scanningThread.StartAsynchroneFullParse(() => cleanUpCode.QueueCleanUpCode());
		}

		/// <summary>
		/// Executed when a error scan is done.
		/// NOTE - Not executed on the UI thread
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void scanningThread_ErrorScanningDone(object sender, ErrorScanningDoneEventArgs e) {
		}

		internal void ShowSmartHelper() {
			blnUserPressedAltEnter = true;
		}

		internal void PurgeCache() {
			foreach (Server server in servers) {
				try {
					server.PurgeCache();
				} catch (Exception e) {
					Common.LogEntry(ClassName, "PurgeCache", e, Common.enErrorLvl.Error);
				}
			}
		}

		internal void KeypressEscape() {
			try {
				tokenUsage.RemoveAll();
				// Debug code
				if (null != CurrentWindowData.Parser && null != CurrentWindowData.Parser.SegmentUtils) {
					CurrentWindowData.Parser.SegmentUtils.clearSegment();
					CurrentWindowData.Parser.SegmentUtils.ClearCaseSegment();
				}
				tableUtils.clearTables();
				debugMatchingParen.InvalidateMatchingTableSquiggle();
				// Debug code
			} catch (Exception e) {
				Common.LogEntry(ClassName, "KeypressEscape", e, Common.enErrorLvl.Error);
			}
		}

		/// <summary>
		/// New settings exists. Handle changes
		/// </summary>
		/// <param name="oldSettings"></param>
		/// <param name="newSettings"></param>
		public void SettingsUpdated(Settings oldSettings, Settings newSettings) {
			try {
				if (oldSettings.EnableAddin != newSettings.EnableAddin || oldSettings.ShowErrorStrip != newSettings.ShowErrorStrip || oldSettings.HighlightCurrentLine != newSettings.HighlightCurrentLine) {
					// Enable / disable error strip window
					foreach (WindowData windowData in lstWindowData) {
						windowData.SplitterRoot.EnableErrorStrip(newSettings);
					}
				}
				
				// Show debug window?
				objDebugTokens.Visible = newSettings.ShowDebugWindow;
				
				// Matching braces
				if (!newSettings.ShowMatchingBraces) {
					matchingParen.InvalidateMatchingTableSquiggle();
				}
				// Enable / disable hidden regions?
				if (null != CurrentWindowData && null != CurrentWindowData.HiddenRegion) {
					if (newSettings.EnableOutlining) {
						CurrentWindowData.HiddenRegion.DoOutlining = true;
					} else {
						CurrentWindowData.HiddenRegion.DisableOutlining();
					}
				}

				ScheduleFullReparse();

			} catch (Exception e) {
				Common.LogEntry(ClassName, "SettingsUpdated", e, Common.enErrorLvl.Error);
			}
		}

		/// <summary>
		/// Get token at the line and column
		/// </summary>
		/// <param name="IsInsideToken"></param>
		/// <param name="tokens"></param>
		/// <param name="line"></param>
		/// <param name="col"></param>
		/// <returns></returns>
		public static int GetTokenIndex(List<TokenInfo> tokens, int line, int col, out bool IsInsideToken) {
			int tokenIndex = -1;

			try {
				if (null != tokens) {
					lock (tokens) {
						for (int i = 0; i < tokens.Count; i++) {
							TokenInfo info = tokens[i];
							if (TextSpanHelper.ContainsInclusive(info.Span, line, col)) {
								IsInsideToken = true;
								return i;
							}
							if (TextSpanHelper.IsAfterEndOf(info.Span, line, col)) {
								tokenIndex = i;
							}
						}
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "GetTokenIndex", e, Common.enErrorLvl.Error);
			}

			IsInsideToken = false;
			return tokenIndex;
		}

		#region Handle insert paranthesis

		internal static void HandleInsertLeftParanthesis(TextSelection Selection, EditPoint epSelection, IVsTextView activeView, string leftOfCursor, string rightOfCursor) {
			try {
				Instance.ApplicationObject.UndoContext.Open("HandleInsertLeftParanthesis", true);

				if (rightOfCursor.StartsWith(")")) {
					int nbOfParanLeft = leftOfCursor.Length - leftOfCursor.Replace("(", string.Empty).Length;
					int nbOfParanRight = rightOfCursor.Length - rightOfCursor.Replace(")", string.Empty).Length;
					if (nbOfParanLeft % 2 == nbOfParanRight % 2) {
						Selection.CharRight(false, 1);
						Selection.DeleteLeft(1);
					}
				}
			} finally {
				if (Instance.ApplicationObject.UndoContext.IsOpen) {
					Instance.ApplicationObject.UndoContext.Close();
				}
			}
		}

		internal static void HandleInsertRightParanthesis(string character, TextSelection Selection, EditPoint epSelection, IVsTextView activeView, out int line, out int col, out int charsToMoveLeft, out int lengthToRight) {
			try {
				Instance.ApplicationObject.UndoContext.Open("HandleInsertRightParanthesis", true);

				EditPoint spSel = Selection.ActivePoint.CreateEditPoint();
				EditPoint epSel = Selection.ActivePoint.CreateEditPoint();
				epSel.EndOfLine();
				string restOfTheLine = spSel.GetText(epSel);

				// End of row?
				if (0 == restOfTheLine.Length) {
					epSelection.Insert(character);
					charsToMoveLeft = 1;
					lengthToRight = 0;
				} else if (restOfTheLine.StartsWith(" ") || restOfTheLine.StartsWith("\t")) {
					epSelection.Insert(character);
					charsToMoveLeft = 1;
					lengthToRight = 0;
				} else {
					charsToMoveLeft = 0;
					lengthToRight = restOfTheLine.Length;
				}
				activeView.GetCaretPos(out line, out col);
				activeView.SetCaretPos(line, col - charsToMoveLeft);
			} finally {
				if (Instance.ApplicationObject.UndoContext.IsOpen) {
					Instance.ApplicationObject.UndoContext.Close();
				}
			}
		}

		#endregion

		#region Rename token methods

		public void UserWantToRenameToken() {
			blnUserPressedRenameToken = true;
		}

		private void RenameToken(int line, int column) {
			try {
				TextSelection Selection = _applicationObject.ActiveDocument.Selection as TextSelection;
				if (null != Selection) {
					// Parse initally to make sure everything is up to date
					scanningThread.StartAsynchroneFullParse(10, () => {
						List<int> lstUsages;
						int declarationIndex;
						if (tokenUsage.FindTokenUsage(CurrentWindowData.Parser, out declarationIndex, out lstUsages, line, column)) {
							lstUsages.Sort();

							bool IsInsideToken;
							int tokenIndex = GetTokenIndex(RawTokens, line, column, out IsInsideToken);
							if (-1 != tokenIndex) {
								string oldText = RawTokens[lstUsages[0]].Token.UnqoutedImage;
								// Open a rename window
								frmRenameEntity formRenameEntity = new frmRenameEntity(RawTokens, tokenIndex, oldText);
								IVsTextView activeView = CurrentWindowData.ActiveView;
								IntPtr hwndCurrentCodeEditor = activeView.GetWindowHandle();
								if (DialogResult.OK == formRenameEntity.ShowDialog(activeView, hwndCurrentCodeEditor, Selection)) {
									// Start the undo transaction
									Instance.ApplicationObject.UndoContext.Open("RenameToken", true);

									try {
										IVsTextLines ppBuffer;
										activeView.GetBuffer(out ppBuffer);
										string newText = formRenameEntity.renamedText;

										// Loop each usage backwards and rename the token
										for (int i = lstUsages.Count - 1; i >= 0; i--) {
											TokenInfo currentToken = RawTokens[lstUsages[i]];
											int lineNb = currentToken.Span.iStartLine;
											int intLineLength;
											ppBuffer.GetLengthOfLine(lineNb, out intLineLength);
											string strBuffer;
											ppBuffer.GetLineText(lineNb, 0, lineNb, intLineLength, out strBuffer);
											// Remove the old text and insert the new one
											strBuffer = strBuffer.Remove(currentToken.Span.iStartIndex, currentToken.Span.iEndIndex - currentToken.Span.iStartIndex);
											strBuffer = strBuffer.Insert(currentToken.Span.iStartIndex, newText);

											// Replace the text in the textbuffer
											IntPtr newLine = Marshal.StringToHGlobalUni(strBuffer);
											TextSpan[] pChangedSpan = null;
											ppBuffer.ReplaceLines(lineNb, 0, lineNb, intLineLength, newLine, strBuffer.Length, pChangedSpan);
											Marshal.Release(newLine);
										}

										// Put the cursor
										Selection.MoveToLineAndOffset(line + 1, column + 1, false);
									} catch (Exception e) {
										_applicationObject.UndoContext.SetAborted();
										Common.LogEntry(ClassName, "RenameToken", e, Common.enErrorLvl.Error);
									} finally {
										if (Instance.ApplicationObject.UndoContext.IsOpen) {
											Instance.ApplicationObject.UndoContext.Close();
										}
									}

									// Parse
									ScheduleFullReparse();
								}
							}
						}
					});
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "RenameToken", e, Common.enErrorLvl.Error);
			}
		}

		#endregion

		#region HandleCompleteWord

		private void HandleCompleteWord() {
			try {
				if (!objPopupWindow.IsPopupVisible) {
					TextSelection Selection = _applicationObject.ActiveDocument.Selection as TextSelection;
					if (null == Selection) {
						return;
					}

					scanningThread.StartAsynchroneFullParse(10, () => {
						int currentLine;
						int currentColumn;
						IVsTextView activeView = CurrentWindowData.ActiveView;
						activeView.GetCaretPos(out currentLine, out currentColumn);
						IVsTextLines ppBuffer;
						activeView.GetBuffer(out ppBuffer);
						int lineLength;
						string currentLineText;
						ppBuffer.GetLengthOfLine(currentLine, out lineLength);
						ppBuffer.GetLineText(currentLine, 0, currentLine, lineLength, out currentLineText);
						string Keypress = string.Empty;
						if (currentColumn - 1 > 0) {
							Keypress = currentLineText.Substring(currentColumn - 1, 1);
						}

						if (Keypress.Length > 0) {
							intellisense.HandleIntellisenseItem(Keypress, Selection);
						}
					});
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "HandleCompleteWord", e, Common.enErrorLvl.Error);
			}
		}

		#endregion

		#endregion

		#region Tool tip methods

		/// <summary>
		/// Hide the tooltip window
		/// </summary>
		private void HideToolTipWindow() {
			if (null != toolTip) {
				toolTip.HideToolTip();
			}
		}

		#endregion

		#region Popup methods

		internal void HidePopupWindow(bool canShowPopup) {
			objPopupWindow.HidePopupWindow(canShowPopup);

			ScheduleFullReparse();

			if (toolTipMethodInfo.ToolTipEnabled || toolTipMethodInfo.Visible) {
				// If tooltip for methods is visible, move it to the bottom again
				toolTipMethodInfo.PositionToolTip(Common.enPosition.Bottom);
			} else if (toolTip.ToolTipEnabled || toolTip.Visible) {
				HideToolTipWindow();
			}
		}

		private void objPopupWindow_PopupHiding() {
			try {
				ScheduleFullReparse();
				HideToolTipWindow();
			} catch (Exception e) {
				Common.LogEntry(ClassName, "objPopupWindow_PopupHiding", e, Common.enErrorLvl.Error);
			}
		}

		#endregion

		#region Paren level markers (debug)

		public void selectMatchingBraces(int selectedIndex) {
			debugMatchingParen.selectMatchingTable(selectedIndex, RawTokens);
		}

		public void clearMatchingBraces() {
			debugMatchingParen.InvalidateMatchingTableSquiggle();
		}

		#endregion

		#region Table markers (debug)

		public void clearTable() {
			tableUtils.InvalidateTableSquiggle();
		}

		public void selectTable(int selectedIndex) {
			tableUtils.selectTable(selectedIndex, RawTokens);
		}

		#endregion

		#region RedrawListBox2 (Debug)

		internal void RedrawListBox2(int currentLine) {
			if (!isDisposed && null != objDebugTokens && !objDebugTokens.IsDisposed && objDebugTokens.Visible && null != RawTokens && RawTokens.Count < 50000 && Instance.Settings.ShowDebugWindow && Instance.Settings.EnableAddin) {
				objDebugTokens.ListBox2.BeginUpdate();
				objDebugTokens.ListBox2.Items.Clear();
				objDebugTokens.ListBox1.BeginUpdate();
				objDebugTokens.ListBox1.Items.Clear();
				objDebugTokens.ListBox5.BeginUpdate();
				objDebugTokens.ListBox5.Items.Clear();
				int activeLine = 0;
				lock (InternalSyncObject) {
					int lineCounter = -1;
					lock (RawTokens) {
						for (int i = 0; i < RawTokens.Count; i++) {
							TokenInfo info = RawTokens[i];
							if (lineCounter != info.Span.iStartLine) {
								lineCounter = info.Span.iStartLine;
								if (currentLine == lineCounter) {
									objDebugTokens.ListBox1.Items.Add(i + ": " + info);
									if (0 == activeLine) {
										activeLine = objDebugTokens.ListBox2.Items.Count;
									}
								}
								objDebugTokens.ListBox2.Items.Add(string.Format("----- {0}-----------", lineCounter));
							}
							objDebugTokens.ListBox2.Items.Add(i + ": " + info);

							// Paren level
							if (-1 != info.MatchingParenToken && info.MatchingParenToken > RawTokens[info.MatchingParenToken].MatchingParenToken) {
								objDebugTokens.ListBox5.Items.Add("(" + i + "," + info.MatchingParenToken + "), " + info + " ------- " + RawTokens[info.MatchingParenToken]);
							}
						}
					}
				}
				if (objDebugTokens.ListBox2.Items.Count > 0) {
					objDebugTokens.ListBox2.SelectedIndex = activeLine;
				}
				objDebugTokens.ListBox5.EndUpdate();
				objDebugTokens.ListBox1.EndUpdate();
				objDebugTokens.ListBox2.EndUpdate();

				// Update Segment tab
				objDebugTokens.ListBox6.BeginUpdate();
				objDebugTokens.ListBox6.Items.Clear();
				objDebugTokens.ListBox6.EndUpdate();
				objDebugTokens.ListBox3.BeginUpdate();
				objDebugTokens.ListBox3.Items.Clear();
				if (null != CurrentWindowData.Parser.SegmentUtils) {
					foreach (StatementSpans ss in CurrentWindowData.Parser.SegmentUtils.StartTokenIndexes) {
						objDebugTokens.ListBox3.Items.Add(ss.ToString());
					}
				}
				objDebugTokens.ListBox3.EndUpdate();

				// Case segments
				objDebugTokens.ListBox7.BeginUpdate();
				objDebugTokens.ListBox7.Items.Clear();
				if (null != CurrentWindowData.Parser.SegmentUtils) {
					foreach (StatementSpans ss in CurrentWindowData.Parser.SegmentUtils.CaseStartTokenIndexes) {
						objDebugTokens.ListBox7.Items.Add(ss.ToString());
					}
				}
				objDebugTokens.ListBox7.EndUpdate();
				// If segments
				objDebugTokens.ListBox8.BeginUpdate();
				objDebugTokens.ListBox8.Items.Clear();
				if (null != CurrentWindowData.Parser.SegmentUtils) {
					foreach (StatementSpans ss in CurrentWindowData.Parser.SegmentUtils.IfStartTokenIndexes) {
						objDebugTokens.ListBox8.Items.Add(ss.ToString());
					}
				}
				objDebugTokens.ListBox8.EndUpdate();

				// Update scanned tables tab
				objDebugTokens.ListBox4.BeginUpdate();
				objDebugTokens.ListBox4.Items.Clear();
				if (null != CurrentWindowData.Parser && null != CurrentWindowData.Parser.TableSources) {
					foreach (TableSource tableSource in CurrentWindowData.Parser.TableSources) {
						objDebugTokens.ListBox4.Items.Add(tableSource.ToString());
					}
				}
				if (null != CurrentWindowData.Parser && null != CurrentWindowData.Parser.TableSources) {
					objDebugTokens.ListBox4.Items.Add("------------");
					foreach (TemporaryTable tempTable in CurrentWindowData.Parser.TemporaryTables) {
						objDebugTokens.ListBox4.Items.Add(tempTable.ToString());
					}
				}
				objDebugTokens.ListBox4.EndUpdate();
				// WindowData
				objDebugTokens.SetWindowDataList(lstWindowData, currentWindowData);
			}
		}

		#endregion
	}
}
