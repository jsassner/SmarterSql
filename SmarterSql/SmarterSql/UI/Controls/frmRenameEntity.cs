// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.PInvoke;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.UI.Controls {
	public partial class frmRenameEntity : Form {
		#region Member variables

		private readonly string ClassName = "frmRenameEntity";

		private readonly int currentIndex;
		private readonly List<TokenInfo> lstTokens;
		private DialogResult dialogResult = DialogResult.None;

		#endregion

		public frmRenameEntity(List<TokenInfo> lstTokens, int currentIndex, string textToRename) {
			InitializeComponent();

			this.lstTokens = lstTokens;
			this.currentIndex = currentIndex;

			txtToRename.Text = textToRename;
			txtToRename.SelectionStart = 0;
			txtToRename.SelectionLength = txtToRename.TextLength;
		}

		#region Public properties

		public string renamedText {
			get { return txtToRename.Text; }
		}

		#endregion

		public DialogResult ShowDialog(IVsTextView activeView, IntPtr hwndCurrentCodeEditor, TextSelection Selection) {
			try {
				TokenInfo firstToken = lstTokens[currentIndex];
				int startPos = firstToken.Span.iStartIndex;
				EditPoint epSelection = Selection.ActivePoint.CreateEditPoint();
				int charsToMoveLeft = epSelection.LineCharOffset - 1 - startPos;

				int x;
				int y;
				int intCursorLine;
				int intCursorColumn;
				Common.GetCoordinates(activeView, charsToMoveLeft, out x, out y, out intCursorLine, out intCursorColumn, hwndCurrentCodeEditor);

				// Move the window a bit
				int intFontHeight = label1.Font.Height;
				y += intFontHeight;

				int screenHeight = NativeWIN32.GetScreenHeight();
				if (y + Height > screenHeight) {
					y = y - (int)(intFontHeight * 1.5) - Height;
				}

				Location = new Point(x, y);

				DialogResult dialog = ShowDialog();
				return (dialogResult != DialogResult.None ? dialogResult : dialog);
			} catch (Exception e) {
				Common.LogEntry(ClassName, "ShowDialog", e, Common.enErrorLvl.Error);
				return DialogResult.Cancel;
			}
		}

		private void frmRenameEntity_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Escape) {
				Close();
			} else if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter) {
				dialogResult = DialogResult.OK;
				Close();
			}
		}
	}
}