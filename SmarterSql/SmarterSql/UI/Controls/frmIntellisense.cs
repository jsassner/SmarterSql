// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EnvDTE;
using GlacialComponents.Controls;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.PInvoke;
using Sassner.SmarterSql.Utils;
using Sassner.SmarterSql.Utils.Args;
using Sassner.SmarterSql.Utils.Tooltips;

namespace Sassner.SmarterSql.UI.Controls {
	public partial class frmIntellisense : Form {
		#region Member variables

		private const string ClassName = "frmIntellisense";
		private const int MaxItemsToDisplay = 11;
		private static int iconWidth = 22;
		private readonly ScrollBarMovedEventArgs[] currentcrollBarMovedEventArgs = new ScrollBarMovedEventArgs[2];

		private readonly List<GLItem> lstGLItems = new List<GLItem>();
		private readonly List<int> lstPointers = new List<int>();
		private readonly ToolTipWindow toolTip;
		public OutputWindowPane _outputWindowPane;
		private IVsTextView activeView;
		private bool blnEnableToolTips = true;
		private bool blnToolTipVisible;
		private EditPoint epSelection;
		private Font fontEditor;
		private Font fontTooltip;
		private IntPtr hwndCurrentCodeEditor;
		private int intPreSelectPosition;
		private int intX;
		private int intY;
		private EditPoint ipSelection;
		private int itemHeight;
		private int intScrollWheelLines;
		private bool mouseInWindow;
		private double opacityHidden = 0.15;
		private string postSelectText;
		private string preSelectText;
		private TokenInfo prevToken;
		private bool resizeListDownwards = true;
		private int screenHeight;
		private EditPoint spSelection;

		private float textEditorCharactherWidth;
		private int textEditorLineHeight;

		#endregion

		public frmIntellisense(TextEditor textEditor, Font fontEditor, Font fontTooltip) {
			this.fontEditor = fontEditor;
			this.fontTooltip = fontTooltip;
			InitializeComponent();

			toolTip = new ToolTipWindow(textEditor, fontTooltip);

			tmrToolTip.Interval = Common.intTimeoutLong;
			tmrToolTip.Enabled = false;

			glacialList1.Top = 0;
			glacialList1.Left = 0;
			glacialList1.Visible = true;
			lblNoMatchingEntries.Top = 0;
			lblNoMatchingEntries.Left = 0;
			lblNoMatchingEntries.Visible = false;

			iconWidth = imageList1.ImageSize.Width + glacialList1.CellPaddingSize * 2;

			// Set line height
			int intMaxHeight = fontEditor.Height;
			if (imageList1.ImageSize.Height > intMaxHeight) {
				intMaxHeight = imageList1.ImageSize.Height;
			}
			itemHeight = intMaxHeight + glacialList1.CellPaddingSize * 2;

			SetPointers(string.Empty);
			glacialList1.VirtualMode = true;
			glacialList1.ResizeToFullColumnWidth();
			glacialList1.RetrieveVirtualItem += glacialList1_RetrieveVirtualItem;
			glacialList1.OnScrollContent += glacialList1_OnScrollContent;
			glacialList1.MouseEnter += glacialList1_MouseEnter;
			glacialList1.MouseLeave += glacialList1_MouseLeave;
			glacialList1.OnScrollBarMouseEnter += glacialList1_MouseEnter;
			glacialList1.OnScrollBarMouseLeave += glacialList1_MouseLeave;
			glacialList1.KeyPress += glacialList1_KeyPress;
			textEditor.MouseWheel += textEditor_MouseWheel;

			SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.DoubleBuffer, true);
		}

		#region Public properties

		public bool EnableToolTips {
			get { return blnEnableToolTips; }
			set { blnEnableToolTips = value; }
		}

		public double OpacityHidden {
			get { return opacityHidden; }
			set { opacityHidden = value; }
		}

		public Font FontEditor {
			get { return fontEditor; }
			set {
				fontEditor = value;
				glacialList1.Font = fontEditor;
				lblNoMatchingEntries.Font = fontEditor;

				// Set line height
				int intMaxHeight = fontEditor.Height;
				if (imageList1.ImageSize.Height > intMaxHeight) {
					intMaxHeight = imageList1.ImageSize.Height;
				}
				itemHeight = intMaxHeight + glacialList1.CellPaddingSize * 2;
			}
		}

		public Font FontTooltip {
			get { return fontTooltip; }
			set {
				fontTooltip = value;
				toolTip.FontTooltip = fontTooltip;
			}
		}

		public string PreSelectText {
			get { return preSelectText; }
		}

		public string PostSelectText {
			get { return postSelectText; }
		}

		/// <summary>
		/// Show window without activating it
		/// </summary>
		protected override bool ShowWithoutActivation {
			get { return true; }
		}

		public bool MouseInWindow {
			get {
				return mouseInWindow;
			}
		}

		public bool NoMatches {
			get { return lblNoMatchingEntries.Visible; }
		}

		#endregion

		/// <summary>
		/// Initialize the intellisense function
		/// </summary>
		/// <param name="ActiveView"></param>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		/// <param name="lstItems"></param>
		/// <param name="PreSelect"></param>
		/// <param name="PostSelect"></param>
		/// <param name="Selection"></param>
		/// <param name="previousToken"></param>
		/// <param name="_hwndCurrentCodeEditor"></param>
		/// <returns></returns>
		public bool InitNewOperation(IVsTextView ActiveView, int X, int Y, List<IntellisenseData> lstItems, string PreSelect, string PostSelect, EditPoint Selection, TokenInfo previousToken, IntPtr _hwndCurrentCodeEditor) {
			try {
				activeView = ActiveView;
				prevToken = previousToken;
				hwndCurrentCodeEditor = _hwndCurrentCodeEditor;
				mouseInWindow = false;
				intX = X;
				intY = Y;

				// Get the number of lines the user scrolls each tick with the mouse wheel button
				IntPtr ptrScrollWheelLines = IntPtr.Zero;
				NativeWIN32.SystemParametersInfo(NativeWIN32.SPI.SPI_GETWHEELSCROLLLINES, 0, ptrScrollWheelLines, 0);
				intScrollWheelLines = (ptrScrollWheelLines != IntPtr.Zero ? ptrScrollWheelLines.ToInt32() : 3);

				lstItems.Sort();

				preSelectText = PreSelect;
				postSelectText = PostSelect;
				intPreSelectPosition = preSelectText.Length;
				spSelection = Selection;
				ipSelection = Selection.CreateEditPoint();
				ipSelection.CharRight(intPreSelectPosition);
				epSelection = ipSelection.CreateEditPoint();
				lstGLItems.Clear();
				glacialList1.Items.Clear();

				glacialList1.ItemHeight = itemHeight;

				// Calculate start position. If the list will overflow the screen, start it above the start point instead
				screenHeight = NativeWIN32.GetScreenHeight();

				// Get line height in text editor
				activeView.GetLineHeight(out textEditorLineHeight);
				// Get characther width in text editor
				textEditorCharactherWidth = Common.GetCharactherWidth(fontEditor, Graphics.FromHwnd(hwndCurrentCodeEditor), ' ');

				glacialList1.Visible = true;
				lblNoMatchingEntries.Visible = false;

				// Copy the items to the list
				if (lstItems.Count > 0) {
					foreach (IntellisenseData IdItem in lstItems) {
						GLItem glItem = IdItem.GetItem();
						glacialList1.Items.Add(glItem);
						lstGLItems.Add(glItem);
					}
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

				tmrToolTip.Interval = Common.intTimeoutLong;
				HideToolTip();
				SetPointers(preSelectText);
				NativeWIN32.SetWindowPos(Handle, new IntPtr(NativeWIN32.HWND_TOPMOST), 0, 0, 0, 0, NativeWIN32.SWP_NOACTIVATE | NativeWIN32.SWP_NOMOVE | NativeWIN32.SWP_NOSIZE);
				Opacity = 1.0;
				Show();

				return true;

			} catch (Exception e) {
				Common.LogEntry(ClassName, "InitNewOperation", e, "Error when initializing intellisense window: ", Common.enErrorLvl.Error);
				return false;
			}
		}

		/// <summary>
		/// Abort the selection of an item in the list
		/// </summary>
		public void StopOperation() {
			try {
				Visible = false;
				HideToolTip();

				glacialList1.BeginUpdate();
				glacialList1.VirtualListSize = 0;
				lstPointers.Clear();
				lstGLItems.Clear();
				glacialList1.EndUpdate();
			} catch (Exception e) {
				Common.LogEntry(ClassName, "StopOperation", e, "Error when unloading intellisense window. ", Common.enErrorLvl.Error);
			}
		}

		/// <summary>
		/// Add a characther to the "filter"
		/// </summary>
		/// <param name="strChar"></param>
		/// <returns></returns>
		public bool AddChar(string strChar) {
			preSelectText = preSelectText.Insert(intPreSelectPosition, strChar);
			ipSelection.CharRight(strChar.Length);
			epSelection.CharRight(strChar.Length);
			intPreSelectPosition += strChar.Length;

			return SetPointers(preSelectText);
		}

		/// <summary>
		/// The user just deleted the characters supplied. 
		/// </summary>
		/// <param name="strTextDeleted">The characters to remove</param>
		/// <param name="blnMovePointers">True if we should move pointers where we are in the string (default), and false if not. Set to false
		/// when the user removes a number of characters in a selection.</param>
		/// <returns>True if there still are matches in the intellisense list, else false.</returns>
		public bool DeleteChar(string strTextDeleted, bool blnMovePointers) {
			if (blnMovePointers) {
				intPreSelectPosition -= strTextDeleted.Length;
				if (intPreSelectPosition < 0) {
					return false;
				}
				ipSelection.CharLeft(strTextDeleted.Length);
			}
			epSelection.CharRight(strTextDeleted.Length);

			if (preSelectText.Length < intPreSelectPosition + strTextDeleted.Length) {
				return false;
			}
			preSelectText = preSelectText.Remove(intPreSelectPosition, strTextDeleted.Length);

			return SetPointers(preSelectText);
		}

		/// <summary>
		/// Choose the active item
		/// </summary>
		/// <param name="idItem"></param>
		/// <param name="PreSelect"></param>
		/// <param name="PostSelect"></param>
		/// <param name="Selection"></param>
		/// <param name="previousToken"></param>
		/// <returns></returns>
		public bool SelectItem(out IntellisenseData idItem, out string PreSelect, out string PostSelect, out EditPoint Selection, out TokenInfo previousToken) {
			idItem = null;
			PreSelect = "";
			PostSelect = "";
			Selection = null;
			previousToken = prevToken;

			List<GLItem> lstSelectedItem = glacialList1.SelectedItems;
			if (0 == lstSelectedItem.Count || !glacialList1.Visible) {
				return false;
			}

			idItem = (IntellisenseData)lstSelectedItem[0].Tag;
			PreSelect = preSelectText.Substring(0, intPreSelectPosition);
			PostSelect = postSelectText;
			Selection = spSelection;

			return true;
		}

		/// <summary>
		/// Retrieve the current selected index in the list
		/// </summary>
		/// <returns></returns>
		private int GetSelectedIndex() {
			List<int> lstSelectedItem = glacialList1.SelectedIndicies;
			if (0 == lstSelectedItem.Count) {
				return -1;
			}
			if (1 != lstSelectedItem.Count) {
				Common.LogEntry(ClassName, "GetSelectedIndex", "More than one item selected", Common.enErrorLvl.Error);
			}

			return lstSelectedItem[0];
		}

		/// <summary>
		/// Select the item in the list with the supplied index
		/// </summary>
		/// <param name="intIndex"></param>
		private void SelectItemInList(int intIndex) {
			glacialList1.Items[intIndex].Selected = true;
			glacialList1.EnsureVisible(intIndex);

			HideToolTip();
			StartTimer(Common.intTimeoutShort);
		}

		/// <summary>
		/// If active already, show it
		/// </summary>
		private void PositionToolTip() {
			if (blnToolTipVisible) {
				ShowToolTip();
			}
		}

		/// <summary>
		/// Hide tooltip
		/// </summary>
		private void HideToolTip() {
			if (blnEnableToolTips) {
				blnToolTipVisible = false;
				tmrToolTip.Enabled = false;
				toolTip.Hide();
			}
		}

		/// <summary>
		/// Start the timer for when showing the tooltip
		/// </summary>
		private void StartTimer(int intTimeOut) {
			if (blnEnableToolTips) {
				tmrToolTip.Interval = intTimeOut;
				tmrToolTip.Enabled = true;
			}
		}

		/// <summary>
		/// Gets the selected item. Shows that IntellisenseData object ToolTip text
		/// </summary>
		private void ShowToolTip() {
			try {
				int intIndex = GetSelectedIndex();
				if (intIndex < 0) {
					return;
				}

				// Get the selected item
				GLItem glItem = GetItem(intIndex);
				IntellisenseData idItem = (IntellisenseData)glItem.Tag;
				if (idItem.GetToolTip.Length > 0) {
					// Calculate the Y-offset
					int intPos = (intIndex - glacialList1.TopIndex) * glacialList1.ItemHeight + (glacialList1.ItemHeight / 2) + glacialList1.CellPaddingSize;

					// Show the tooltip
					ToolTipLiveTemplate objTTLT = new ToolTipLiveTemplateInfo(toolTip);
					toolTip.Initialize(activeView, Left + Width, Top + intPos, idItem.GetToolTip, objTTLT, false, Common.enPosition.Center);
					toolTip.Show();
					blnToolTipVisible = true;
				}
			} catch (Exception) {
				// Log
			}
		}

		/// <summary>
		/// Timer ticks after a specified time. Show tooltip
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tmrToolTip_Tick(object sender, EventArgs e) {
			ShowToolTip();
			tmrToolTip.Enabled = false;
		}

		private void glacialList1_RetrieveVirtualItem(object sender, RetrieveVirtualItem2EventArgs e) {
			e.Item = GetItem(e.ItemIndex);
		}

		/// <summary>
		/// Get item on the intItemIndex position from lstGLItems list, taking in
		/// consideration the virtual list.
		/// </summary>
		/// <param name="intItemIndex"></param>
		/// <returns></returns>
		private GLItem GetItem(int intItemIndex) {
			try {
				if (lstPointers.Count <= intItemIndex) {
					return null;
				}
				if (lstPointers[intItemIndex] >= lstGLItems.Count) {
					return null;
				}
				return lstGLItems[lstPointers[intItemIndex]];
			} catch (Exception e) {
				Common.LogEntry(ClassName, "GetItem", e, "Error when getting item (intItemIndex=" + intItemIndex + "): ", Common.enErrorLvl.Error);
				return null;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strTextToMatch"></param>
		/// <returns>True if items are shown or if the "no matches" is shown for the first time, else false</returns>
		private bool SetPointers(string strTextToMatch) {
			try {
				tmrToolTip.Enabled = false;
				// Clear the pointer list
				lstPointers.Clear();
				int startPosMinLength = 0;
				int atStartstartPosMinLength = 0;
				int minLengthMinLength = 10000;
				int atStartMinLengthMinLength = 10000;
				int startPosMatchesEnd = -1;
				int minLengthMatchesEnd = 10000;

				//Common.LogChar(false, "SetPointers: " + strTextToMatch + ", " + strTextToMatch.Substring(0, intPreSelectPosition) + ", " + intPreSelectPosition);

				strTextToMatch = strTextToMatch.Substring(0, intPreSelectPosition);
				Regex regExp = CamelCaseMatcher.CreateCamelCaseRegExp(strTextToMatch);

				// Loop all items to find those to include
				for (int i = 0; i < lstGLItems.Count; i++) {
					GLItem glItem = lstGLItems[i];
					IntellisenseData idItem = (IntellisenseData)glItem.Tag;

					Match match = CamelCaseMatcher.GetMatchCamelCase(regExp, idItem.MainText);
					if (match.Success) {
						if (idItem.MainText.Length < minLengthMinLength) {
							minLengthMinLength = idItem.MainText.Length;
							startPosMinLength = lstPointers.Count;
						}
						if (idItem.MainText.Length < atStartMinLengthMinLength && idItem.MainText.StartsWith(strTextToMatch, StringComparison.OrdinalIgnoreCase)) {
							atStartMinLengthMinLength = idItem.MainText.Length;
							atStartstartPosMinLength = lstPointers.Count;
						}
						if (postSelectText.Length > 0 && idItem.MainText.EndsWith(postSelectText, StringComparison.OrdinalIgnoreCase)) {
							if (idItem.MainText.Length < minLengthMatchesEnd) {
								minLengthMatchesEnd = idItem.MainText.Length;
								startPosMatchesEnd = lstPointers.Count;
							}
						}

						lstPointers.Add(i);
					}
//						if (idItem.MainText.StartsWith(strTextToMatch, StringComparison.OrdinalIgnoreCase)) {
//							// Find the shortest string. Make that the start position
//							if (idItem.MainText.Length < minLengthMinLength) {
//								minLengthMinLength = idItem.MainText.Length;
//								startPosMinLength = lstPointers.Count;
//							}
//							if (idItem.MainText.EndsWith(postSelectText, StringComparison.OrdinalIgnoreCase)) {
//								if (idItem.MainText.Length < minLengthMatchesEnd) {
//									minLengthMatchesEnd = idItem.MainText.Length;
//									startPosMatchesEnd = lstPointers.Count;
//								}
//							}
//
//							// We are showing this item - add it to our pointers list
//							lstPointers.Add(i);
//						} else {
//							// Check camel casing
//							if (idItem.UpperCaseLetters.Length > 0) {
//								string strNewTextToMatch = strTextToMatch;
//								// Remove prefix (if match)
//								if (strNewTextToMatch.StartsWith(idItem.TypePrefix)) {
//									strNewTextToMatch = strNewTextToMatch.Substring(idItem.TypePrefix.Length);
//								}
//								if (idItem.UpperCaseLetters.StartsWith(strNewTextToMatch, StringComparison.OrdinalIgnoreCase)) {
//									// We are showing this item - add it to our pointers list
//									lstPointers.Add(i);
//								}
//							}
//						}
				}
				if (atStartstartPosMinLength != 0) {
					startPosMinLength = atStartstartPosMinLength;
				}

				try {
					glacialList1.BeginUpdate();

					if (lstPointers.Count > 0) {
						glacialList1.Visible = true;
						lblNoMatchingEntries.Visible = false;

						if (glacialList1.Items.Count > 0) {
							// First make sure that the first item in the list is selected
							// First execution of this line, check if any items in the list
							glacialList1.EnsureVisible(0);
						}

						// Then set the new size of the list
						glacialList1.VirtualListSize = lstPointers.Count;

						// Then select the first item
						glacialList1.Items.ClearSelection();
						if (-1 != startPosMatchesEnd) {
							glacialList1.Items[startPosMatchesEnd].Selected = true;
							glacialList1.EnsureVisible(startPosMatchesEnd);
						} else {
							glacialList1.Items[startPosMinLength].Selected = true;
							glacialList1.EnsureVisible(startPosMinLength);
						}

						ResizeListView();

						HideToolTip();
						// Enable timer to show it soon
						StartTimer(Common.intTimeoutLong);

						return true;
					} else {
						glacialList1.VirtualListSize = 0;
						// Is this the first time we hide the popup window?
						bool blnFirstTime = glacialList1.Visible;

						// No items in list
						glacialList1.Visible = false;
						lblNoMatchingEntries.Visible = true;

						// Resize the parent form
						Width = lblNoMatchingEntries.Width;
						Height = lblNoMatchingEntries.Height;

						SetListPosition();
						HideToolTip();

						return blnFirstTime;
					}
				} catch (Exception e) {
					Common.LogEntry(ClassName, "SetPointers", e, "Error when showing/hiding list: ", Common.enErrorLvl.Error);
				} finally {
					glacialList1.EndUpdate();
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "SetPointers", e, "Error when building list to show: ", Common.enErrorLvl.Error);
			}
			return false;
		}

		private void SetListPosition() {
			NativeWIN32.RECT lpRect;
			// Get code window rect
			NativeWIN32.GetWindowRect(hwndCurrentCodeEditor, out lpRect);

			int xPosition = intX;
			int yPosition = intY;
			int height = GetHeight();

			resizeListDownwards = (yPosition + height <= screenHeight);

			// Validate the position
			if (xPosition < lpRect.Left) {
				xPosition = lpRect.Left;
			} else if (xPosition > lpRect.Right - GetWidth()) {
				xPosition = lpRect.Right - GetWidth();
			}

			if (resizeListDownwards) {
				if (yPosition < 1) {
					yPosition = 1;
				} else if (yPosition > screenHeight - height) {
					yPosition = screenHeight - height;
				}
			} else {
				if (yPosition < 1 + height) {
					yPosition = 1 + height;
				} else if (yPosition > screenHeight - 1) {
					yPosition = screenHeight - 1;
				}
			}

			// Set the position of the list
			Location = (resizeListDownwards ? new Point(xPosition, yPosition) : new Point(xPosition, yPosition - height - itemHeight));
		}

		/// <summary>
		/// Resize the form
		/// </summary>
		private void ResizeListView() {
			try {
				int intColMainMaxWidth;
				int intColSubMaxWidth;

				GetColumnWidths(out intColMainMaxWidth, out intColSubMaxWidth);
				glacialList1.Columns[0].Width = intColMainMaxWidth;
				glacialList1.Columns[1].Width = intColSubMaxWidth;

				int intNbOfRows = lstPointers.Count;
				if (intNbOfRows > MaxItemsToDisplay) {
					intNbOfRows = MaxItemsToDisplay;
				}
				glacialList1.VisibleRowsCount = intNbOfRows;
				glacialList1.ResizeToFullColumnWidth();

				// Resize the parent form
				Width = glacialList1.Width;
				Height = glacialList1.Height;

				SetListPosition();
			} catch (Exception) {
				// Do nothing
			}
		}

		/// <summary>
		/// Get the width of the columns in the grid.
		/// Obs: Doesn't work with variable width sized fonts
		/// </summary>
		/// <param name="intColMain"></param>
		/// <param name="intColSub"></param>
		private void GetColumnWidths(out int intColMain, out int intColSub) {
			intColMain = 1;
			intColSub = 1;

			foreach (int pointer in lstPointers) {
				GLItem glItem = lstGLItems[pointer];

				// Get main column max width
				if (glItem.SubItems[0].Text.Length > intColMain) {
					intColMain = glItem.SubItems[0].Text.Length;
				}

				// Get sub column max width
				if (glItem.SubItems[1].Text.Length > intColSub) {
					intColSub = glItem.SubItems[1].Text.Length;
				}
			}

			// Measure the widest number of characters in both columns, using the supplied font
			intColMain = TextRenderer.MeasureText(new string('X', intColMain + 2), fontEditor, Size.Empty, TextFormatFlags.LeftAndRightPadding).Width + iconWidth;
			intColSub = TextRenderer.MeasureText(new string('X', intColSub + 2), fontEditor, Size.Empty, TextFormatFlags.LeftAndRightPadding).Width;

			return;
		}

		private int GetWidth() {
			return (glacialList1.Visible ? glacialList1.Width : lblNoMatchingEntries.Width);
		}

		public int GetHeight() {
			int intNbOfRows = lstPointers.Count;
			if (intNbOfRows > MaxItemsToDisplay) {
				intNbOfRows = MaxItemsToDisplay;
			}
			return (glacialList1.Visible ? glacialList1.Height : (lblNoMatchingEntries.Visible ? lblNoMatchingEntries.Height : itemHeight * intNbOfRows));
		}

		private void glacialList1_MouseEnter(object sender, EventArgs e) {
			mouseInWindow = true;
		}

		private void glacialList1_MouseLeave(object sender, EventArgs e) {
			mouseInWindow = false;
		}

		bool textEditor_MouseWheel(object sender, MouseWheelEventArgs e) {
			return MouseInWindow;
		}

		internal bool OnScrollBarMoved(object sender, ScrollBarMovedEventArgs e) {
			if (null == currentcrollBarMovedEventArgs[e.Bar]) {
				return false;
			}

			if (MouseInWindow) {
				return true;
			}

			int diff = currentcrollBarMovedEventArgs[e.Bar].FirstVisibleUnit - e.FirstVisibleUnit;
			if (e.Bar == NativeWIN32.SB_HORZ) {
				intX += (int)(textEditorCharactherWidth * diff);
			} else {
				intY += textEditorLineHeight * diff;
			}
			currentcrollBarMovedEventArgs[e.Bar] = e;

			SetListPosition();
			PositionToolTip();

			return false;
		}

		private void glacialList1_OnScrollContent(object sender, MouseEventArgs e) {
			try {
				int intIndex = GetSelectedIndex();
				if (intIndex < 0) {
					return;
				}
				intIndex -= (Math.Sign(e.Delta) * intScrollWheelLines);
				if (intIndex < 0) {
					intIndex = 0;
				} else if (intIndex > glacialList1.Items.Count - 1) {
					intIndex = glacialList1.Items.Count - 1;
				}
				SelectItemInList(intIndex);

			} catch (Exception ex) {
				Common.LogEntry(ClassName, "ScrollContent", ex, Common.enErrorLvl.Error);
			}
		}

		public bool ScrollContent(MouseWheelEventArgs e) {
			if (!MouseInWindow) {
				return false;
			}
			try {
				int intIndex = GetSelectedIndex();
				if (intIndex < 0) {
					return true;
				}
				intIndex -= e.LinesToScroll;
				if (intIndex < 0) {
					intIndex = 0;
				} else if (intIndex > glacialList1.Items.Count - 1) {
					intIndex = glacialList1.Items.Count - 1;
				}
				SelectItemInList(intIndex);

			} catch (Exception ex) {
				Common.LogEntry(ClassName, "ScrollContent", ex, Common.enErrorLvl.Error);
			}

			// Cancel the scroll bar event, which makes the text editor scroll bar not move
			e.Cancel = true;

			return true;
		}

		public void TemporaryHide(bool blnHide) {
			try {
				if (blnHide) {
					Opacity = opacityHidden;
					HideToolTip();
				} else {
					Opacity = 1.0;
					StartTimer(Common.intTimeoutLong);
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "TemporaryHide", e, "Error when temporary hiding window: ", Common.enErrorLvl.Error);
			}
		}

		#region Move cursor

		public void MoveCursorUp() {
			try {
				int intIndex = GetSelectedIndex();
				if (intIndex < 0) {
					return;
				}
				if (intIndex > 0) {
					intIndex--;
					SelectItemInList(intIndex);
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "MoveCursorUp", e, Common.enErrorLvl.Error);
			}
		}

		public void MoveCursorDown() {
			try {
				int intIndex = GetSelectedIndex();
				if (intIndex < 0) {
					return;
				}
				if (intIndex < glacialList1.Items.Count - 1) {
					intIndex++;
					SelectItemInList(intIndex);
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "MoveCursorDown", e, Common.enErrorLvl.Error);
			}
		}

		public void MoveCursorPageDown() {
			try {
				int intIndex = GetSelectedIndex();
				if (intIndex < 0) {
					return;
				}
				intIndex += glacialList1.VisibleRowsCount;
				if (intIndex > glacialList1.Items.Count - 1) {
					intIndex = glacialList1.Items.Count - 1;
				}
				SelectItemInList(intIndex);
			} catch (Exception e) {
				Common.LogEntry(ClassName, "MoveCursorPageDown", e, Common.enErrorLvl.Error);
			}
		}

		public void MoveCursorPageUp() {
			try {
				int intIndex = GetSelectedIndex();
				if (intIndex < 0) {
					return;
				}
				intIndex -= glacialList1.VisibleRowsCount;
				if (intIndex < 0) {
					intIndex = 0;
				}
				SelectItemInList(intIndex);
			} catch (Exception e) {
				Common.LogEntry(ClassName, "MoveCursorPageUp", e, Common.enErrorLvl.Error);
			}
		}

		public void MoveCursorTop() {
			try {
				if (lstPointers.Count > 0) {
					SelectItemInList(0);
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "MoveCursorTop", e, Common.enErrorLvl.Error);
			}
		}

		public void MoveCursorBottom() {
			try {
				if (lstPointers.Count > 0) {
					SelectItemInList(lstPointers.Count - 1);
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "MoveCursorBottom", e, Common.enErrorLvl.Error);
			}
		}

		public bool MoveCursorRight() {
			intPreSelectPosition++;
			if (intPreSelectPosition > preSelectText.Length) {
				return false;
			}
			ipSelection.CharRight(1);
			return SetPointers(preSelectText);
		}

		public bool MoveCursorLeft() {
			intPreSelectPosition--;
			if (intPreSelectPosition <= 0) {
				return false;
			}
			ipSelection.CharLeft(1);
			return SetPointers(preSelectText);
		}

		#endregion

		private void glacialList1_KeyPress(object sender, KeyPressEventArgs e) {
			Debug.WriteLine(e.KeyChar);
			NativeWIN32.PostMessage(hwndCurrentCodeEditor, (uint)NativeWIN32.WindowsMessages.WM_CHAR, e.KeyChar, 0);
		}

		private void glacialList1_DoubleClick(object sender, EventArgs e) {
//			NativeWIN32.PostMessage(hwndCurrentCodeEditor, (uint)NativeWIN32.WindowsMessages.WM_CHAR, 10, 0);
		}

		private void glacialList1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
		}

		private void glacialList1_Click(object sender, EventArgs e) {
			Opacity = 1.0;
		}

		private void frmIntellisense_Deactivate(object sender, EventArgs e) {
			;
		}
	}
}
