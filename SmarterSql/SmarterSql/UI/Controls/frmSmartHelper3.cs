// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using EnvDTE;
using GlacialComponents.Controls;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.Parsing;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.PInvoke;
using Sassner.SmarterSql.UI.Subclassing;
using Sassner.SmarterSql.Utils;
using Sassner.SmarterSql.Utils.Args;
using Sassner.SmarterSql.Utils.Helpers;
using Sassner.SmarterSql.Utils.Marker;
using Sassner.SmarterSql.Utils.SqlErrors;
using Sassner.SmarterSql.Utils.Tooltips;
using KeyEventArgs=System.Windows.Forms.KeyEventArgs;
using Thread=System.Threading.Thread;
using Timer=System.Windows.Forms.Timer;

namespace Sassner.SmarterSql.UI.Controls {
	public partial class frmSmartHelper3 : Form {
		#region Member variables

		private const string ClassName = "frmSmartHelper3";
		private const int HeaderIconHeight = 16;
		private const int intIconWidth = 22;
		private const int intMaxItemsToDisplay = 11;
		private readonly ScrollBarMovedEventArgs[] currentcrollBarMovedEventArgs = new ScrollBarMovedEventArgs[2];
		private readonly TextEditor textEditor;

		private readonly ToolTipWindow toolTipWindow;
		private IVsTextView activeView;
		private bool blnToolTipVisible;
		private IntPtr hwndCurrentCodeEditor;
		private int intX;
		private int intY;

		private List<SysObject> lstSysObjects;
		private List<TokenInfo> lstTokens;
		private int mostLeftPosition;
		private bool ResizeListDownwards = true;
		private int ScreenHeight;
		private TextSelection Selection;
		private bool showList;
		private Squiggle squiggle;
		private bool startedWithKeyboard;

		private float textEditorCharactherWidth;
		private int textEditorLineHeight;

		private int currentLine;
		private int currentColumn;

		BackgroundWorker backgroundWorker;

		#endregion

		public frmSmartHelper3(TextEditor textEditor) {
			this.textEditor = textEditor;
			InitializeComponent();

			textEditor.KeyDownVsCommands += textEditor_KeyDownVsCommands;
			textEditor.LostFocus += textEditor_LostFocus;
			textEditor.ScrollBarMoved += textEditor_ScrollBarMoved;

			toolTipWindow = new ToolTipWindow(textEditor, Instance.FontTooltip);
			timer1.Tick += timer1_Tick;

			backgroundWorker = new BackgroundWorker {
				WorkerReportsProgress = true,
				WorkerSupportsCancellation = true
			};
			backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;
			backgroundWorker.DoWork += backgroundWorker_DoWork;
		}

		#region Public properties

		public Squiggle Squiggle {
			[DebuggerStepThrough]
			get { return squiggle; }
		}

		/// <summary>
		/// Show window without activating it
		/// </summary>
		protected override bool ShowWithoutActivation {
			[DebuggerStepThrough]
			get { return true; }
		}

		#endregion

		private bool ShowHint(HintToShow hintToShow) {
			if (TextEditor.CurrentWindowData.ActiveView != hintToShow.ActiveView || TextEditor.CurrentWindowData.ActiveHwnd != hintToShow.ActiveHwnd) {
				return false;
			}
			activeView = hintToShow.ActiveView;
			hwndCurrentCodeEditor = hintToShow.ActiveHwnd;
			squiggle = hintToShow.Squiggle;

			if (!GetCoordinates(out intX, out intY)) {
				return false;
			}
			if (currentLine != hintToShow.Line || currentColumn != hintToShow.Column) {
				return false;
			}

			// Get the current position in the code window
			int piMinUnit;
			int piMaxUnit;
			int piVisibleUnits;
			int piFirstVisibleUnit;
			if (NativeMethods.Succeeded(activeView.GetScrollInfo(NativeWIN32.SB_VERT, out piMinUnit, out piMaxUnit, out piVisibleUnits, out piFirstVisibleUnit))) {
				currentcrollBarMovedEventArgs[NativeWIN32.SB_VERT] = new ScrollBarMovedEventArgs(NativeWIN32.SB_VERT, piMinUnit, piMaxUnit, piVisibleUnits, piFirstVisibleUnit);
			}
			if (NativeMethods.Succeeded(activeView.GetScrollInfo(NativeWIN32.SB_HORZ, out piMinUnit, out piMaxUnit, out piVisibleUnits, out piFirstVisibleUnit))) {
				currentcrollBarMovedEventArgs[NativeWIN32.SB_HORZ] = new ScrollBarMovedEventArgs(NativeWIN32.SB_HORZ, piMinUnit, piMaxUnit, piVisibleUnits, piFirstVisibleUnit);
			}

			// Get line height in text editor
			activeView.GetLineHeight(out textEditorLineHeight);
			// Get characther width in text editor
			textEditorCharactherWidth = Common.GetCharactherWidth(hintToShow.FontEditor, Graphics.FromHwnd(hwndCurrentCodeEditor), ' ');

			intX += (int)(currentcrollBarMovedEventArgs[NativeWIN32.SB_HORZ].FirstVisibleUnit * textEditorCharactherWidth);
			ResizeListDownwards = true;
			ShowList(false);

			lblIcon.ImageKey = "Active";
			startedWithKeyboard = false;

			return true;
		}

		private bool GetCoordinates(out int x, out int y) {
			// Get active line and col
			int lineHeight;
			int editorLeftMargin;
			int firstLineEditor;

			activeView.GetCaretPos(out currentLine, out currentColumn);

			if (!Common.GetTextEditorInfo(activeView, hwndCurrentCodeEditor, out lineHeight, out editorLeftMargin, out firstLineEditor)) {
				x = 0;
				y = 0;
				return false;
			}

			// Transform the line and col into coordinates. These coordinates are relative to the edit window.
			POINT[] pt = new POINT[1];
			activeView.GetPointOfLineColumn(currentLine, 0, pt);

			// Transform the point into our client screen
			Point pt2 = new Point(pt[0].x, pt[0].y);
			NativeWIN32.ClientToScreen(hwndCurrentCodeEditor, ref pt2);
			x = pt2.X;
			mostLeftPosition = x;
			y = pt2.Y + lineHeight - HeaderIconHeight - 1;
			return true;
		}

		private void ShowList(bool ShowList) {
			showList = ShowList;

			glacialList1.Visible = showList;
			ResizeListView();

			NativeWIN32.SetWindowPos(Handle, new IntPtr(NativeWIN32.HWND_TOPMOST), 0, 0, 0, 0, NativeWIN32.SWP_NOACTIVATE | NativeWIN32.SWP_NOMOVE | NativeWIN32.SWP_NOSIZE);
			Show();
		}

		/// <summary>
		/// Show a list of entries to choose from
		/// </summary>
		/// <returns></returns>
		public bool ShowFullList(bool StartedWithKeyboard) {
			try {
				if (null == squiggle || null == squiggle.ScannedSqlError) {
					return false;
				}
				squiggle.ScannedSqlError.ConstructChoices();

				startedWithKeyboard = StartedWithKeyboard;
				lstSysObjects = textEditor.ActiveConnection.GetSysObjects();
				Selection = Instance.ApplicationObject.ActiveDocument.Selection as TextSelection;
				lstTokens = textEditor.RawTokens;
				glacialList1.Font = textEditor.FontEditor;

				glacialList1.Items.Clear();
				for (int i = 0; i < squiggle.ScannedSqlError.Choices.Count; i++) {
					Choice choice = squiggle.ScannedSqlError.Choices[i];
					glacialList1.Items.Add(GetItem(i, choice.Text));
				}

				// Set line height
				int intMaxHeight = glacialList1.Font.Height;
				if (imageList1.ImageSize.Height > intMaxHeight) {
					intMaxHeight = imageList1.ImageSize.Height;
				}
				int ItemHeight = intMaxHeight + glacialList1.CellPaddingSize * 2 + 1;
				glacialList1.ItemHeight = ItemHeight;

				// Calculate start position. If the list will overflow the screen, start it above the start point instead
				ScreenHeight = NativeWIN32.GetScreenHeight();
				ResizeListDownwards = (Top + intMaxItemsToDisplay * ItemHeight <= ScreenHeight);

				ShowList(true);

				Focus();

				glacialList1.Items[0].Selected = true;
				glacialList1.FocusedItem = glacialList1.Items[0];

				return true;
			} catch (Exception e) {
				Common.LogEntry(ClassName, "Show", e, Common.enErrorLvl.Error);
				return false;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		private static GLItem GetItem(int index, string text) {
			GLItem glItem = new GLItem {
				Tag = index
			};

			// Main item
			GLSubItem subItem1 = new GLSubItem {
				ImageIndex = 0,
				Text = text
			};

			// Add items
			glItem.SubItems.AddRange(new[] {
				subItem1
			});

			return glItem;
		}

		/// <summary>
		/// Resize the form
		/// </summary>
		private void ResizeListView() {
			try {
				if (glacialList1.Visible) {
					int intColMainMaxWidth;

					GetColumnWidths(out intColMainMaxWidth);
					glacialList1.Columns[0].Width = intColMainMaxWidth;

					int intNbOfRows = squiggle.ScannedSqlError.Choices.Count;
					if (intNbOfRows > intMaxItemsToDisplay) {
						intNbOfRows = intMaxItemsToDisplay;
					}
					glacialList1.VisibleRowsCount = intNbOfRows;
					glacialList1.ResizeToFullColumnWidth();

					// Resize the parent form
					Width = glacialList1.Width;
					Height = glacialList1.Height + HeaderIconHeight;
				} else {
					Width = lblIcon.Width;
					Height = lblIcon.Height;
				}

				SetLocation();
			} catch (Exception e) {
				Common.LogEntry(ClassName, "ResizeListView", e, Common.enErrorLvl.Error);
			}
		}

		/// <summary>
		/// Get the width of the columns in the grid.
		/// Obs: Doesn't work with variable width sized fonts
		/// </summary>
		/// <param name="intColMain"></param>
		private void GetColumnWidths(out int intColMain) {
			intColMain = 1;

			foreach (Choice choice in squiggle.ScannedSqlError.Choices) {
				int colWidth = TextRenderer.MeasureText(choice.Text + "X", glacialList1.Font, Size.Empty, TextFormatFlags.LeftAndRightPadding).Width + intIconWidth;
				if (colWidth > intColMain) {
					intColMain = colWidth;
				}
			}
		}

		/// <summary>
		/// If active already, show it
		/// </summary>
		private void PositionToolTip() {
			if (blnToolTipVisible) {
				ShowToolTip();
			}
		}

		private void ShowToolTip() {
			try {
				ToolTipLiveTemplate objTTLT = new ToolTipLiveTemplateInfo(toolTipWindow);
				toolTipWindow.Initialize(TextEditor.CurrentWindowData.ActiveView, Left + lblIcon.Left + lblIcon.Width + 4, Top + lblIcon.Height / 2, "Click (or press alt+enter) to show smart help", objTTLT, true, Common.enPosition.Center);
				toolTipWindow.Show();
				blnToolTipVisible = true;
			} catch (Exception) {
				// Do nothing
			}
		}

		private void SetLocation() {
			NativeWIN32.RECT lpRect;
			// Get code window rect
			NativeWIN32.GetWindowRect(hwndCurrentCodeEditor, out lpRect);

			int xPosition = intX;
			int yPosition = intY;

			// Validate the position
			if (xPosition < mostLeftPosition) {
				xPosition = mostLeftPosition;
			} else if (xPosition > lpRect.Right - Width) {
				xPosition = lpRect.Right - Width;
			}
			if (yPosition < lpRect.Top) {
				yPosition = lpRect.Top;
			} else if (yPosition > lpRect.Bottom - Height) {
				yPosition = lpRect.Bottom - Height;
				if (yPosition + Height > intY) {
					yPosition = intY - Height;
				}
			}

			// Set the position of the list
			Location = (ResizeListDownwards ? new Point(xPosition, yPosition) : new Point(xPosition, yPosition - Height));
		}

		private void CheckForKeys(KeyEventArgs e) {
			if (e.KeyCode == Keys.Escape) {
				HideWindow();
			} else if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter) {
				ExecuteAction();
			}
		}

		/// <summary>
		/// Execute the action tied to this particular SqlError
		/// </summary>
		private void ExecuteAction() {
			List<int> indicies = glacialList1.SelectedIndicies;
			squiggle.ScannedSqlError.Execute(lstSysObjects, lstTokens, squiggle, Selection, indicies[0]);
			Instance.TextEditor.ScheduleFullReparse();
			HideWindow();
		}

		public void HideWindow() {
			squiggle = null;
			HideToolTip();
			Hide();
		}

		private void HideToolTip() {
			blnToolTipVisible = false;
			timer1.Enabled = false;
			toolTipWindow.HideToolTip();
		}

		internal static void HandleSmartHelper(frmSmartHelper3 smartHelper, Parser parser, WindowData windowData, int newLine, int newColumn) {
			if (null == smartHelper || null == parser || null == windowData || null == windowData.SplitterRoot) {
				return;
			}

			try {
				// Is the SmartHelper window visible? has the cursor moved?
				if (newLine != smartHelper.currentLine || newColumn != smartHelper.currentColumn) {
					smartHelper.HideWindow();
				} else {
					if (smartHelper.Visible) {
						bool hideSmartHelper = true;
						foreach (Squiggle squiggle in parser.Markers) {
							if (TextSpanHelper.ContainsInclusive(squiggle.Span, newLine, newColumn)) {
								if (squiggle == smartHelper.Squiggle) {
									hideSmartHelper = false;
								}
								break;
							}
						}
						if (hideSmartHelper) {
							smartHelper.HideWindow();
						}
					}
				}

				// Show smart helper window?
				if (!smartHelper.Visible) {
					foreach (Squiggle squiggle in parser.Markers) {
						if (TextSpanHelper.ContainsInclusive(squiggle.Span, newLine, newColumn)) {
							squiggle.ScannedSqlError.ConstructChoices();
							if (squiggle.ScannedSqlError.Choices.Count > 0) {
								HintToShow hintToShow = new HintToShow(smartHelper, parser, windowData.SplitterRoot.ActiveActiveView, windowData.SplitterRoot.ActiveHwnd, squiggle, Instance.FontEditor, newLine, newColumn);
								smartHelper.StartNewWaitForShowingHint(hintToShow);
							}
							break;
						}
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "HandleSmartHelper", e, Common.enErrorLvl.Error);
			}
		}

		private void StartNewWaitForShowingHint(HintToShow hintToShow) {
			if (!backgroundWorker.IsBusy) {
				backgroundWorker.RunWorkerAsync(hintToShow);
				return;
			}
			backgroundWorker.CancelAsync();
			Timer timerRetryUntilWorkerNotBusy = new Timer {
				Interval = 1
			};
			timerRetryUntilWorkerNotBusy.Tick +=
				delegate {
					if (backgroundWorker.IsBusy) {
						return;
					}
					timerRetryUntilWorkerNotBusy.Stop();
					backgroundWorker.RunWorkerAsync(hintToShow);
				};
			timerRetryUntilWorkerNotBusy.Start();
		}

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public new void Dispose() {
			base.Dispose();

			if (null != backgroundWorker) {
				backgroundWorker.ProgressChanged -= backgroundWorker_ProgressChanged;
				backgroundWorker.DoWork -= backgroundWorker_DoWork;
				backgroundWorker.CancelAsync();
				backgroundWorker = null;
			}

			textEditor.KeyDownVsCommands -= textEditor_KeyDownVsCommands;
			textEditor.LostFocus -= textEditor_LostFocus;
			textEditor.ScrollBarMoved -= textEditor_ScrollBarMoved;
		}

		#region Event implementations

		private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
			HintToShow hintToShow = (HintToShow)e.Argument;

			if (Instance.Settings.SmartHelperInitialDelay > 0) {
				// Wait some time before scanning for errors
				int counter = Instance.Settings.SmartHelperInitialDelay / 10;
				while (counter > 0) {
					if (null == backgroundWorker || backgroundWorker.CancellationPending) {
						return;
					}
					Thread.Sleep(10);
					counter--;
				}
			}
			backgroundWorker.ReportProgress(100, hintToShow);
		}

		private static void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
			if (!Common.IsTextEditorActiveWindow()) {
				return;
			}
			HintToShow hintToShow = (HintToShow)e.UserState;
			hintToShow.SmartHelper.ShowHint(hintToShow);
		}

		/// <summary>
		/// Close the window when we loose focus on it
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void frmSmartHelper2_Deactivate(object sender, EventArgs e) {
			HideWindow();
		}

		private void glacialList1_KeyDown(object sender, KeyEventArgs e) {
			CheckForKeys(e);
		}

		private void frmSmartHelper2_KeyDown(object sender, KeyEventArgs e) {
			CheckForKeys(e);
		}

		private void glacialList1_Click(object sender, EventArgs e) {
			ExecuteAction();
		}

		private void lblIcon_Click(object sender, EventArgs e) {
			if (!showList) {
				ShowFullList(false);
				HideToolTip();
			}
		}

		private void timer1_Tick(object sender, EventArgs e) {
			if (!toolTipWindow.Visible) {
				ShowToolTip();
			}
		}

		private void lblIcon_MouseEnter(object sender, EventArgs e) {
			if (!showList) {
				lblIcon.ImageKey = "NonActive";
				timer1.Interval = 500;
				timer1.Enabled = true;
			}
		}

		private void lblIcon_MouseLeave(object sender, EventArgs e) {
			if (!showList) {
				lblIcon.ImageKey = "Active";
			}
			HideToolTip();
		}

		private void textEditor_LostFocus(object sender, LostFocusEventArgs e) {
			if (!Focused && !startedWithKeyboard) {
				HideWindow();
			}
		}

		private void textEditor_KeyDownVsCommands(object sender, KeyVsCmdEventArgs e) {
			if (e.VsCmd == Common.enVsCmd.Escape && showList) {
				HideWindow();
			}
		}

		private bool textEditor_ScrollBarMoved(object sender, ScrollBarMovedEventArgs e) {
			if (null != currentcrollBarMovedEventArgs[e.Bar]) {
				int diff = currentcrollBarMovedEventArgs[e.Bar].FirstVisibleUnit - e.FirstVisibleUnit;
				if (e.Bar == NativeWIN32.SB_HORZ) {
					intX += (int)(textEditorCharactherWidth * diff);
				} else {
					intY += textEditorLineHeight * diff;
				}
				currentcrollBarMovedEventArgs[e.Bar] = e;

				SetLocation();
				PositionToolTip();
			}

			return false;
		}

		#endregion
	}
}
