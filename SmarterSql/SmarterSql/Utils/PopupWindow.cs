// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.Parsing;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.PInvoke;
using Sassner.SmarterSql.UI.Controls;
using Sassner.SmarterSql.Utils.Args;
using Sassner.SmarterSql.Utils.Helpers;
using KeyEventArgs=Sassner.SmarterSql.Utils.Args.KeyEventArgs;

namespace Sassner.SmarterSql.Utils {
	public class PopupWindow : IDisposable {
		#region Member variables

		private const string ClassName = "PopupWindow";

		private readonly DTE2 _applicationObject;
		private readonly frmIntellisense objfrmIntellisense;
		private readonly TextEditor textEditor;
		private IVsTextView activeView;

		private PopupLastShown popupLastShown;
		private bool isDisposed;

		private Font fontEditor;
		private Font fontTooltip;

		private IntPtr hwndCurrentCodeEditor = IntPtr.Zero;

		#endregion

		#region Events

		#region Delegates

		public delegate void PopupHidingHandler();

		#endregion

		public event PopupHidingHandler PopupHiding;

		#endregion

		public PopupWindow(TextEditor textEditor, DTE2 _applicationObject, Font fontEditor, Font fontTooltip) {
			this.textEditor = textEditor;
			this._applicationObject = _applicationObject;
			this.fontEditor = fontEditor;
			this.fontTooltip = fontTooltip;

			textEditor.KeyDownVsCommands += textEditor_KeyDownVsCommands;
			textEditor.LostFocus += textEditor_LostFocus;
			textEditor.MouseDown += textEditor_MouseDown;
			textEditor.MouseWheel += textEditor_MouseWheel;
			textEditor.ScrollBarMoved += textEditor_ScrollBarMoved;

			// Create the window
			objfrmIntellisense = new frmIntellisense(textEditor, fontEditor, fontTooltip);

			popupLastShown = new PopupLastShown();
		}

		#region Public properties

		public Font FontEditor {
			[DebuggerStepThrough]
			get { return fontEditor; }
			set { fontEditor = value; }
		}

		public Font FontTooltip {
			[DebuggerStepThrough]
			get { return fontTooltip; }
			set { fontTooltip = value; }
		}

		public bool IsPopupVisible {
			[DebuggerStepThrough]
			get { return objfrmIntellisense.Visible; }
		}

		public bool CanShowPopup {
			[DebuggerStepThrough]
			set { popupLastShown.CanShowPopup = value; }
		}

		public string PreSelectText {
			[DebuggerStepThrough]
			get { return objfrmIntellisense.PreSelectText; }
		}

		public string PostSelectText {
			[DebuggerStepThrough]
			get { return objfrmIntellisense.PostSelectText; }
		}

		#endregion

		#region Show popup (static) methods

		public static void ShowPopup(TextEditor textEditor, List<IntellisenseData> lstItems, string PreSelect, string PostSelect, EditPoint epSelection, TokenInfo token) {
			if (textEditor.ToolTipMethodInfo.ToolTipEnabled || textEditor.ToolTipMethodInfo.Visible) {
				textEditor.ToolTipMethodInfo.PositionToolTip(Common.enPosition.Top);
			}
			textEditor.PopupWindow.ShowPopup(lstItems, PreSelect, PostSelect, epSelection, token);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="textEditor"></param>
		/// <param name="epSelection"></param>
		/// <param name="prevToken"></param>
		/// <param name="preSelect"></param>
		/// <param name="postSelect"></param>
		public static void ShowPopupWithDataTypes(TextEditor textEditor, EditPoint epSelection, TokenInfo prevToken, string preSelect, string postSelect) {
			List<IntellisenseData> lstItems = new List<IntellisenseData>();
			foreach (DataType dataType in textEditor.StaticData.DataTypes.Values) {
				lstItems.Add(dataType);
			}
			ShowPopup(textEditor, lstItems, preSelect, postSelect, epSelection, prevToken);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="textEditor"></param>
		/// <param name="epSelection"></param>
		/// <param name="prevToken"></param>
		/// <param name="preSelect"></param>
		/// <param name="postSelect"></param>
		public static void ShowPopupWithDataBases(TextEditor textEditor, EditPoint epSelection, TokenInfo prevToken, string preSelect, string postSelect) {
			List<Database> lstDataBases = textEditor.ActiveServer.GetDataBases();
			List<IntellisenseData> lstItems = new List<IntellisenseData>();
			foreach (Database database in lstDataBases) {
				lstItems.Add(database);
			}
			ShowPopup(textEditor, lstItems, preSelect, postSelect, epSelection, prevToken);
		}

		public static void ShowPopupWithSetTypes(TextEditor textEditor, EditPoint epSelection, TokenInfo prevToken, string preSelect, string postSelect, int tokenIndex) {
			List<IntellisenseData> lstItems = new List<IntellisenseData>();
			foreach (SetType setType in textEditor.StaticData.SetTypes) {
				lstItems.Add(setType);
			}
			List<LocalVariable> allVariablesInBatch = LocalVariable.GetAllVariablesInBatch(TextEditor.CurrentWindowData.Parser, tokenIndex);
			foreach (LocalVariable variable in allVariablesInBatch) {
				lstItems.Add(variable);
			}
			ShowPopup(textEditor, lstItems, preSelect, postSelect, epSelection, prevToken);
		}

		public static void ShowPopupWithDateParameters(TextEditor textEditor, EditPoint epSelection, TokenInfo prevToken, string preSelect, string postSelect) {
			List<IntellisenseData> lstItems = new List<IntellisenseData>();
			foreach (MethodParameter methodParameter in textEditor.StaticData.MethodParameters) {
				lstItems.Add(methodParameter);
			}
			ShowPopup(textEditor, lstItems, preSelect, postSelect, epSelection, prevToken);
		}

		/// <summary>
		/// Show a popup containing sysobjects
		/// </summary>
		/// <param name="textEditor"></param>
		/// <param name="parser"></param>
		/// <param name="foundConnection"></param>
		/// <param name="epSelection"></param>
		/// <param name="indexOfToken"></param>
		/// <param name="prevToken"></param>
		/// <param name="preSelect"></param>
		/// <param name="postSelect"></param>
		/// <param name="lstSqlTypes"></param>
		/// <param name="strSchema"></param>
		/// <param name="blnIncludeSchemas"></param>
		public static void ShowPopupWithSysObjects(TextEditor textEditor, Parser parser, Connection foundConnection, EditPoint epSelection, int indexOfToken, TokenInfo prevToken, string preSelect, string postSelect, IEnumerable<Common.enSqlTypes> lstSqlTypes, string strSchema, bool blnIncludeSchemas) {
			// List tables
			List<SysObject> lstSysObjects = foundConnection.GetSysObjects();
			List<IntellisenseData> lstItems = new List<IntellisenseData>();
			StatementSpans ss;
			List<TableSource> lstFoundTableSources;

			if (parser.SegmentUtils.GetStatementSpanTablesources(indexOfToken, out lstFoundTableSources, out ss)) {
				foreach (SysObject sysObject in lstSysObjects) {
					if (Common.enSqlTypes.DerivedTable != sysObject.SqlType && (0 == strSchema.Length || (null != sysObject.Schema && sysObject.Schema.Schema.Equals(strSchema, StringComparison.OrdinalIgnoreCase)))) {
						foreach (Common.enSqlTypes type in lstSqlTypes) {
							if (type == sysObject.SqlType) {
								bool addIt = true;
								if (Common.enSqlTypes.Temporary == sysObject.SqlType) {
									foreach (TemporaryTable temporaryTable in parser.TemporaryTables) {
										if (temporaryTable.SysObject == sysObject) {
											if (TextSpanHelper.StartsBeforeStartOf(prevToken.Span, temporaryTable.Span)) {
												addIt = false;
											}
											break;
										}
									}
								}
								if (addIt) {
									lstItems.Add(sysObject);
								}
								break;
							}
						}
					}
				}

				foreach (ColumnAlias columnAlias in parser.DeclaredColumnAliases) {
					if (columnAlias.StatementSpan == ss) {
						lstItems.Add(columnAlias);
					}
				}
			}

			if (blnIncludeSchemas) {
				List<SysObjectSchema> lstSysObjectSchemas = foundConnection.GetSysObjectSchemas();
				foreach (SysObjectSchema sysObjectSchema in lstSysObjectSchemas) {
					lstItems.Add(sysObjectSchema);
				}
			}

			ShowPopup(textEditor, lstItems, preSelect, postSelect, epSelection, prevToken);
		}

		/// <summary>
		/// Show a popup containing schemas
		/// </summary>
		/// <param name="textEditor"></param>
		/// <param name="foundConnection"></param>
		/// <param name="epSelection"></param>
		/// <param name="prevToken"></param>
		/// <param name="preSelect"></param>
		/// <param name="postSelect"></param>
		public static void ShowPopupWithSchemas(TextEditor textEditor, Connection foundConnection, EditPoint epSelection, TokenInfo prevToken, string preSelect, string postSelect) {
			List<IntellisenseData> lstItems = new List<IntellisenseData>();

			List<SysObjectSchema> lstSysObjectSchemas = foundConnection.GetSysObjectSchemas();
			foreach (SysObjectSchema sysObjectSchema in lstSysObjectSchemas) {
				lstItems.Add(sysObjectSchema);
			}

			ShowPopup(textEditor, lstItems, preSelect, postSelect, epSelection, prevToken);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="textEditor"></param>
		/// <param name="epSelection"></param>
		/// <param name="prevToken"></param>
		/// <param name="preSelect"></param>
		/// <param name="postSelect"></param>
		/// <param name="tokenIndex"></param>
		public static void ShowPopupWithVariables(TextEditor textEditor, EditPoint epSelection, TokenInfo prevToken, string preSelect, string postSelect, int tokenIndex) {
			List<IntellisenseData> lstItems = new List<IntellisenseData>();

			// Add local variables
			List<LocalVariable> allVariablesInBatch = LocalVariable.GetAllVariablesInBatch(TextEditor.CurrentWindowData.Parser, tokenIndex);
			foreach (LocalVariable variable in allVariablesInBatch) {
				lstItems.Add(variable);
			}

			// Add global variables
			foreach (KeyValuePair<string, GlobalVariable> objGlobalVariable in textEditor.StaticData.GlobalVariables) {
				lstItems.Add(objGlobalVariable.Value);
			}

			ShowPopup(textEditor, lstItems, preSelect, postSelect, epSelection, prevToken);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="textEditor"></param>
		/// <param name="parser"></param>
		/// <param name="epSelection"></param>
		/// <param name="preSelect"></param>
		/// <param name="postSelect"></param>
		/// <param name="tableName"></param>
		/// <param name="aliasName"></param>
		/// <param name="indexOfToken"></param>
		/// <param name="prevToken"></param>
		public static void ShowPopupWithColumns(TextEditor textEditor, Parser parser, EditPoint epSelection, string preSelect, string postSelect, string tableName, string aliasName, int indexOfToken, TokenInfo prevToken) {
			StatementSpans ss;
			List<TableSource> lstFoundTableSources;

			if (parser.SegmentUtils.GetUniqueStatementSpanTablesources(indexOfToken, out lstFoundTableSources, out ss, false)) {
				List<IntellisenseData> lstItems = new List<IntellisenseData>();
				foreach (TableSource tableSource in lstFoundTableSources) {
					if ((tableName.Length == 0 || tableSource.Table.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase)) && (0 == aliasName.Length || tableSource.Table.Alias.Equals(aliasName, StringComparison.OrdinalIgnoreCase) || tableSource.Table.TableName.Equals(aliasName, StringComparison.OrdinalIgnoreCase))) {
						foreach (SysObjectColumn sysObjectColumn in tableSource.Table.SysObject.Columns) {
							lstItems.Add(sysObjectColumn);
						}
					}
				}
				if (0 == tableName.Length && 0 == aliasName.Length) {
					ColumnAlias.AddColumnAliasesInSegment(parser, ss, lstItems);
				}

				ShowPopup(textEditor, lstItems, preSelect, postSelect, epSelection, prevToken);
			}
		}

		/// <summary>
		/// List table alias found in the document in a popup window
		/// </summary>
		/// <param name="textEditor"></param>
		/// <param name="parser"></param>
		/// <param name="epSelection"></param>
		/// <param name="preSelect"></param>
		/// <param name="postSelect"></param>
		/// <param name="prevToken"></param>
		public static void ShowPopupWithScannedTables(TextEditor textEditor, Parser parser, EditPoint epSelection, string preSelect, string postSelect, TokenInfo prevToken) {
			StatementSpans ss;
			List<TableSource> lstFoundTableSources;
			int indexOfToken = -1;
			for (int i = 0; i < textEditor.RawTokens.Count; i++) {
				if (textEditor.RawTokens[i] == prevToken) {
					indexOfToken = i;
					break;
				}
			}
			if (parser.SegmentUtils.GetUniqueStatementSpanTablesources(indexOfToken, out lstFoundTableSources, out ss, false)) {
				//				List<SysObject> lstSysObjects = objActiveConnection.GetSysObjects();

				List<IntellisenseData> lstItems = new List<IntellisenseData>();
				foreach (TableSource tableSource in lstFoundTableSources) {
					if (tableSource.Table.Alias.Length > 0) {
						lstItems.Add(tableSource.Table);
					}
					// Add the columns for this object
					foreach (SysObjectColumn column in tableSource.Table.SysObject.Columns) {
						lstItems.Add(column);
					}
					//					foreach (SysObject sysObject in lstSysObjects) {
					//						if (sysObject.ObjectName.Equals(tableSource.Table.TableName, StringComparison.OrdinalIgnoreCase)) {
					//							foreach (SysObjectColumn column in sysObject.Columns) {
					//								lstItems.Add(column);
					//							}
					//							break;
					//						}
					//					}
				}

				ColumnAlias.AddColumnAliasesInSegment(parser, ss, lstItems);

				ShowPopup(textEditor, lstItems, preSelect, postSelect, epSelection, prevToken);
			}
		}

		#endregion

		#region IDisposable Members

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose() {
			isDisposed = true;

			textEditor.KeyDownVsCommands -= textEditor_KeyDownVsCommands;
			textEditor.LostFocus -= textEditor_LostFocus;
			textEditor.MouseDown -= textEditor_MouseDown;
			textEditor.MouseWheel -= textEditor_MouseWheel;
			textEditor.ScrollBarMoved -= textEditor_ScrollBarMoved;

			HidePopupWindow();
		}

		#endregion

		/// <summary>
		/// Create a popup window
		/// </summary>
		/// <param name="lstItems"></param>
		/// <param name="PreSelect"></param>
		/// <param name="PostSelect"></param>
		/// <param name="epSelection"></param>
		/// <param name="prevToken"></param>
		public void ShowPopup(List<IntellisenseData> lstItems, string PreSelect, string PostSelect, EditPoint epSelection, TokenInfo prevToken) {
			try {
				if (!popupLastShown.CanShowPopup || !Common.IsTextEditorActiveWindow()) {
					return;
				}

				activeView = TextEditor.CurrentWindowData.ActiveView;
				hwndCurrentCodeEditor = TextEditor.CurrentWindowData.ActiveHwnd;

				int intX;
				int intY;
				int intCursorColumn;
				int intCursorLine;
				GetCoordinates(PreSelect.Length, out intX, out intY, out intCursorLine, out intCursorColumn);

				if (null != popupLastShown && popupLastShown.ShallAbort(intCursorLine, intCursorColumn)) {
					return;
				}

				// Move the cursor the number of chars to the left that the preselect text are long
				epSelection.CharLeft(PreSelect.Length);

				objfrmIntellisense.FontEditor = fontEditor;
				objfrmIntellisense.FontTooltip = fontTooltip;

				// Open the intellisens window
				if (!objfrmIntellisense.InitNewOperation(activeView, intX, intY, lstItems, PreSelect, PostSelect, epSelection, prevToken, hwndCurrentCodeEditor)) {
					objfrmIntellisense.Visible = false;
				} else {
					TextEditor.CurrentWindowData.SplitterRoot.KeyDown += SplitterRoot_KeyDown;
					TextEditor.CurrentWindowData.SplitterRoot.KeyUp += SplitterRoot_KeyUp;
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "ShowPopup", e, Common.enErrorLvl.Error);
			}
		}

		/// <summary>
		/// Hide popup window
		/// </summary>
		private void HidePopupWindow() {
			HidePopupWindow(true);
		}

		/// <summary>
		/// Hide popup window
		/// </summary>
		public void HidePopupWindow(bool canShowPopup) {
			if (!isDisposed && null != TextEditor.CurrentWindowData && null != TextEditor.CurrentWindowData.ActiveView) {
				activeView = TextEditor.CurrentWindowData.ActiveView;
				int line;
				int column;
				activeView.GetCaretPos(out line, out column);
				popupLastShown = new PopupLastShown(DateTime.Now, line, column, canShowPopup);
			}

			if (null != PopupHiding) {
				PopupHiding();
			}

			if (null != objfrmIntellisense) {
				objfrmIntellisense.StopOperation();
				objfrmIntellisense.Hide();
			}

			return;
		}

		private void textEditor_KeyDownVsCommands(object sender, KeyVsCmdEventArgs e) {
			if (!IsPopupVisible) {
				return;
			}

			if (e.ActiveView != activeView) {
				Common.LogEntry(ClassName, "textEditor_KeyDownVsCommands", "Wrong active view send keydown", Common.enErrorLvl.Error);
				return;
			}

			switch (e.VsCmd) {
				case Common.enVsCmd.Cut:
					TextSelection Selection = (TextSelection)_applicationObject.ActiveDocument.Selection;
					if (!DeleteChar(Selection.Text, false)) {
						HidePopupWindow();
						e.Cancel = true;
					}
					break;

				case Common.enVsCmd.Paste:
					if (!AddChar(Clipboard.GetText())) {
						HidePopupWindow();
						e.Cancel = true;
					}
					break;

				case Common.enVsCmd.ArrowDown:
					if (objfrmIntellisense.NoMatches) {
						HidePopupWindow();
					} else {
						objfrmIntellisense.MoveCursorDown();
						e.Cancel = true;
					}
					break;

				case Common.enVsCmd.ArrowUp:
					if (objfrmIntellisense.NoMatches) {
						HidePopupWindow();
					} else {
						objfrmIntellisense.MoveCursorUp();
						e.Cancel = true;
					}
					break;

				case Common.enVsCmd.Home:
				case Common.enVsCmd.End:
				case Common.enVsCmd.Undo:
				case Common.enVsCmd.Redo:
				case Common.enVsCmd.Escape:
				case Common.enVsCmd.Delete:
					HidePopupWindow();
					break;

				case Common.enVsCmd.PageUp:
					if (objfrmIntellisense.NoMatches) {
						HidePopupWindow();
					} else {
						objfrmIntellisense.MoveCursorPageUp();
						e.Cancel = true;
					}
					break;

				case Common.enVsCmd.PageDown:
					if (objfrmIntellisense.NoMatches) {
						HidePopupWindow();
					} else {
						objfrmIntellisense.MoveCursorPageDown();
						e.Cancel = true;
					}
					break;

				case Common.enVsCmd.Right:
					if (!objfrmIntellisense.MoveCursorRight()) {
						HidePopupWindow();
					}
					break;

				case Common.enVsCmd.Left:
					if (!objfrmIntellisense.MoveCursorLeft()) {
						HidePopupWindow();
					}
					break;

				case Common.enVsCmd.WordLeft:
				case Common.enVsCmd.WordRight:
				case Common.enVsCmd.SelectWordRight:
				case Common.enVsCmd.SelectWordLeft:
					HidePopupWindow();
					break;
			}
		}

		private bool SplitterRoot_KeyDown(object sender, KeyEventArgs e) {
			if (IsPopupVisible) {
				switch (e.VK) {
					case NativeWIN32.VirtualKeys.Shift:
						break;
					case NativeWIN32.VirtualKeys.Control:
						objfrmIntellisense.TemporaryHide(true);
						break;

					default:
						if (e.IsCtrl || e.IsAlt) {
							if (e.VK >= NativeWIN32.VirtualKeys.A && e.VK <= NativeWIN32.VirtualKeys.Z) {
								HidePopupWindow();
							}
						}
						break;
				}
			}
			return false;
		}

		private bool SplitterRoot_KeyUp(object sender, KeyEventArgs e) {
			if (IsPopupVisible) {
				switch (e.VK) {
					case NativeWIN32.VirtualKeys.Control:
						objfrmIntellisense.TemporaryHide(false);
						break;
				}
			}
			return false;
		}

		private void textEditor_LostFocus(object sender, LostFocusEventArgs e) {
			if (!IsPopupVisible) {
				return;
			}

			if (!objfrmIntellisense.MouseInWindow) {
				HidePopupWindow();
			}
		}

		private void textEditor_MouseDown(object sender, MouseDownEventArgs e) {
			if (!IsPopupVisible) {
				return;
			}

			if (!objfrmIntellisense.MouseInWindow) {
				HidePopupWindow();
			}
		}

		private bool textEditor_MouseWheel(object sender, MouseWheelEventArgs e) {
			if (!IsPopupVisible) {
				return true;
			}

			return objfrmIntellisense.ScrollContent(e);
		}

		private bool textEditor_ScrollBarMoved(object sender, ScrollBarMovedEventArgs e) {
			if (!IsPopupVisible) {
				return false;
			}

			return objfrmIntellisense.OnScrollBarMoved(sender, e);
		}

		/// <summary>
		/// Get x and y positions for the cursor relative to the screen
		/// </summary>
		/// <param name="moveColumn"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="intCursorLine"></param>
		/// <param name="intCursorColumn"></param>
		private void GetCoordinates(int moveColumn, out int x, out int y, out int intCursorLine, out int intCursorColumn) {
			Common.GetCoordinates(activeView, moveColumn, out x, out y, out intCursorLine, out intCursorColumn, hwndCurrentCodeEditor);
			intCursorColumn += moveColumn;

			// Move the window a bit
			int intFontHeight = fontEditor.Height;
			x -= (22 + 2); // popup window listview icon column width = 22
			y += intFontHeight;
		}

		public bool DeleteChar(string strTextDeleted) {
			return DeleteChar(strTextDeleted, true);
		}

		public bool DeleteChar(string strTextDeleted, bool blnMovePointers) {
			if (!IsPopupVisible) {
				return false;
			}
			return objfrmIntellisense.DeleteChar(strTextDeleted, blnMovePointers);
		}

		public bool SelectItem(out IntellisenseData idData, out string preSelect, out string postSelect, out EditPoint epSel, out TokenInfo prevToken) {
			if (!IsPopupVisible) {
				idData = null;
				preSelect = string.Empty;
				postSelect = string.Empty;
				epSel = null;
				prevToken = null;
				return false;
			}
			return objfrmIntellisense.SelectItem(out idData, out preSelect, out postSelect, out epSel, out prevToken);
		}

		public bool AddChar(string keypress) {
			if (!IsPopupVisible) {
				return false;
			}
			return objfrmIntellisense.AddChar(keypress);
		}
	}
}
