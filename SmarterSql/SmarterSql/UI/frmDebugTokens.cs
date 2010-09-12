// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.UI.Subclassing;
using Sassner.SmarterSql.Utils;
using Sassner.SmarterSql.Utils.Marker;
using Label=System.Windows.Forms.Label;

namespace Sassner.SmarterSql.UI {
	public partial class frmDebugTokens : Form {
		private const string ClassName = "frmDebugTokens";
		private readonly Markers testMarkers = new Markers();
		private Squiggle squiggleToFind;
		private TextEditor textEditor;

		public frmDebugTokens() {
			InitializeComponent();

			try {
				string path = Assembly.GetExecutingAssembly().Location;
				path = Path.GetDirectoryName(path);

				// Load changelog text
				try {
					string filename = Path.Combine(path, "Changelog.txt");
					StreamReader srFile = new StreamReader(filename);
					txtChangelog.Text = srFile.ReadToEnd();
				} catch (Exception e) {
					txtChangelog.Text = e.ToString();
				}

				// Load todo text
				try {
					string filename = Path.Combine(path, "Todo.txt");
					StreamReader srFile = new StreamReader(filename);
					txtTodo.Text = srFile.ReadToEnd();
				} catch (Exception e) {
					txtTodo.Text = e.ToString();
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "Constructor", e, Common.enErrorLvl.Error);
				Common.ErrorMsg("Unable to load changelog/todo textfiles.");
			}

			listView1.Columns.Add("Active");
			listView1.Columns.Add("DocCookie");
			listView1.Columns.Add("Parser");
			listView1.Columns.Add("ActiveActiveView");
			listView1.Columns.Add("PrimaryActiveView");
			listView1.Columns.Add("SecondaryActiveView");
			listView1.Columns.Add("ActiveHwnd");
			listView1.Columns.Add("PrimaryHwndVsEditPane");
			listView1.Columns.Add("PrimaryHwndVsTextEditPane");
			listView1.Columns.Add("SecondaryHwndVsEditPane");
			listView1.Columns.Add("SecondaryHwndVsTextEditPane");

			string[] arr = {
				"None"
				, "{030EF597-983E-4DC8-BFE2-A57508256636}"
				, "{095C01F2-B8AB-4FBB-9404-6EFCD125E4F8}"
				, "{1915461E-0F61-44C5-8F2C-9CE3CA05B2C1}"
				, "{1B7D8015-15FF-4EC9-9C8C-5B83499C8468}"
				, "{1E36D6FC-803F-4FD3-BD00-435A4EEF8911}"
				, "{1F66BCD8-0239-4A9B-A3F9-F51321D3368C}"
				, "{2138B8A9-9D8F-4073-90AC-88678AC755A5}"
				, "{237C7135-D8A7-48C4-832C-DF1FB8CEFD0E}"
				, "{2DAA2998-36A1-4B85-B8E9-4CDA438DDF8C}"
				, "{2FA576FC-EA49-49B0-93D6-85133168BA82}"
				, "{37469172-F83A-4BFA-864B-6900D34EA6F0}"
				, "{387D9BB6-2845-46f3-AC93-55EDC2DE7282}"
				, "{3DDBBA3A-4C4A-493B-BAD1-70E2B47B421C}"
				, "{454B4B0E-829F-4e1a-9BB9-118AEF41F8D6}"
				, "{546FAD73-6000-4138-BA73-9B4746C38324}"
				, "{554B4B0E-829F-4e1a-9BB9-118AEF41F8D6}"
				, "{59E4355F-5663-45B8-B4F3-51ED22561DED}"
				, "{61DF72D6-D8F7-472E-A78D-4DDF06ED7391}"
				, "{69DA1285-D454-4DA1-AB7A-1039C71F3F80}"
				, "{6D799B25-7F32-4173-A4EB-521827D4AE4F}"
				, "{7E2ED083-0516-4366-A84E-3FECB4369799}"
				, "{9D00A12E-9336-4585-A4A6-27B87383312D}"
				, "{9E96A88C-A98E-4711-B610-F2783EEFD731}"
				, "{A5998F84-BB45-4885-B552-0A9A6D98BE26}"
				, "{C362E2C6-C242-425D-95D4-9A52B27096A8}"
				, "{CCB8B9D5-1643-4B41-8395-53C5B5ED5284}"
				, "{E3AA0043-8E3E-4441-BDC5-79F72387E807}"
				, "{E52A924F-0A81-4CE0-B5A2-C4BAE0BF29E3}"
				, "{F7218A30-4648-4994-8A38-2FAF8A992E4F}"
			};
			foreach (string guid in arr) {
				lstMarkers.Items.Add(guid);
			}
			lstMarkers.SelectedIndex = 0;

			for (int i = 0; i < Enum.GetNames(typeof (TokenContextType)).Length; i++) {
				string tokenContextType = Enum.GetNames(typeof (TokenContextType))[i];
				lstKnownTokenTypes.Items.Add(tokenContextType);
				lstKnownTokenTypes.SetSelected(i, true);
			}
		}

		public TextEditor TextEditor {
			get { return textEditor; }
			set { textEditor = value; }
		}

		public ListBox ListBox1 {
			get { return listBox1; }
		}

		public ListBox ListBox2 {
			get { return listBox2; }
		}

		public ListBox ListBox3 {
			get { return listBox3; }
		}

		public ListBox ListBox4 {
			get { return listBox4; }
		}

		public ListBox ListBox5 {
			get { return listBox5; }
		}

		public ListBox ListBox6 {
			get { return listBox6; }
		}

		public ListBox ListBox7 {
			get { return listBox7; }
		}

		public ListBox ListBox8 {
			get { return listBox8; }
		}

		public Label LblConnection {
			get { return lblConnection; }
		}

		public Label LblCurrentToken {
			get { return lblCurrentToken; }
		}

		public Label LblPreviousToken {
			get { return lblPreviousToken; }
		}

		public Label LblCurrentPos {
			get { return lblCurrentPos; }
		}

		public Label LblInComment {
			get { return lblInComment; }
		}

		public Label LblInString {
			get { return lblInString; }
		}

		public Label LblIntellisensWindowVisible {
			get { return lblIntellisensWindowVisible; }
		}

		// Segment ------------------------

		private void listBox3_DoubleClick(object sender, EventArgs e) {
			if (listBox3.SelectedIndex >= 0) {
				SelectSegment(listBox3.SelectedIndex);
			}
		}

		private void cmdSegmentPrevious_Click(object sender, EventArgs e) {
			if (listBox3.SelectedIndex > 0) {
				listBox3.SelectedIndex--;
				SelectSegment(listBox3.SelectedIndex);
			}
		}

		private void cmdSegmentNext_Click(object sender, EventArgs e) {
			if (listBox3.SelectedIndex >= 0 && listBox3.SelectedIndex + 1 < listBox3.Items.Count) {
				listBox3.SelectedIndex++;
				SelectSegment(listBox3.SelectedIndex);
			}
		}

		private void cmdSegmentSegment_Click(object sender, EventArgs e) {
			if (listBox3.SelectedIndex >= 0) {
				SelectSegment(listBox3.SelectedIndex);
			}
		}

		private void cmdSegmentClear_Click(object sender, EventArgs e) {
			TextEditor.CurrentWindowData.Parser.SegmentUtils.InvalidateSegmentSquiggle();
		}

		private void cmdSegmentSelectTokens_Click(object sender, EventArgs e) {
			if (listBox3.SelectedIndex >= 0) {
				TextEditor.CurrentWindowData.Parser.SegmentUtils.selectTokensInSegment(listBox3.SelectedIndex, Instance.TextEditor.RawTokens);
			}
		}

		private void SelectSegment(int index) {
			TextEditor.CurrentWindowData.Parser.SegmentUtils.selectSegment(index, Instance.TextEditor.RawTokens);
			listBox6.Items.Clear();
			StatementSpans ss = TextEditor.CurrentWindowData.Parser.SegmentUtils.StartTokenIndexes[index];
			foreach (TableSource tableSource in ss.TableSources) {
				listBox6.Items.Add(tableSource);
			}
		}

		// Table ------------------------

		private void listBox4_DoubleClick(object sender, EventArgs e) {
			if (listBox4.SelectedIndex >= 0) {
				textEditor.selectTable(listBox4.SelectedIndex);
			}
		}

		private void cmdTableSelect_Click(object sender, EventArgs e) {
			if (listBox4.SelectedIndex >= 0) {
				textEditor.selectTable(listBox4.SelectedIndex);
			}
		}

		private void cmdTableClear_Click(object sender, EventArgs e) {
			textEditor.clearTable();
		}

		private void cmdTablePrevious_Click(object sender, EventArgs e) {
			if (listBox4.SelectedIndex > 0) {
				listBox4.SelectedIndex--;
				textEditor.selectTable(listBox4.SelectedIndex);
			}
		}

		private void cmdTableNext_Click(object sender, EventArgs e) {
			if (listBox4.SelectedIndex >= 0 && listBox4.SelectedIndex + 1 < listBox4.Items.Count) {
				listBox4.SelectedIndex++;
				textEditor.selectTable(listBox4.SelectedIndex);
			}
		}

		// Matching braces ------------------------

		private void listBox5_DoubleClick(object sender, EventArgs e) {
			if (listBox5.SelectedIndex >= 0) {
				textEditor.selectMatchingBraces(listBox5.SelectedIndex);
			}
		}

		private void cmdParenSelect_Click(object sender, EventArgs e) {
			if (listBox5.SelectedIndex >= 0) {
				textEditor.selectMatchingBraces(listBox5.SelectedIndex);
			}
		}

		private void cmdParenClear_Click(object sender, EventArgs e) {
			textEditor.clearMatchingBraces();
		}

		private void cmdParenPrevious_Click(object sender, EventArgs e) {
			if (listBox5.SelectedIndex > 0) {
				listBox5.SelectedIndex--;
				textEditor.selectMatchingBraces(listBox5.SelectedIndex);
			}
		}

		private void cmdParenNext_Click(object sender, EventArgs e) {
			if (listBox5.SelectedIndex >= 0 && listBox5.SelectedIndex + 1 < listBox5.Items.Count) {
				listBox5.SelectedIndex++;
				textEditor.selectMatchingBraces(listBox5.SelectedIndex);
			}
		}

		private void frmDebugTokens_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Escape) {
				textEditor.KeypressEscape();
			}
		}

		private void cmdMarkersShowMarkerOnToken_Click(object sender, EventArgs e) {
			SelectTestMarker(lstMarkers.SelectedIndex);
		}

		private void SelectTestMarker(int index) {
			testMarkers.RemoveAll();

			if (0 != index) {
				IVsTextLines ppBuffer;
				TextEditor.CurrentWindowData.ActiveView.GetBuffer(out ppBuffer);

				// Retrieve the markers
				Guid pguidMarker = new Guid(lstMarkers.Items[index].ToString());
				int piMarkerTypeID;
				Instance.VsTextMgr.GetRegisteredMarkerTypeID(ref pguidMarker, out piMarkerTypeID);

				txtMarkersFind.Text = piMarkerTypeID.ToString();

				int indexToken = Convert.ToInt32(txtMarkerTokenIndex.Text);
				TokenInfo token = textEditor.RawTokens[index];
				testMarkers.CreateMarker(ppBuffer, "test_marker_on_" + token.Token.UnqoutedImage, textEditor.RawTokens, indexToken, indexToken, token.Token.UnqoutedImage, null, piMarkerTypeID);
			}
		}

		private void cmdMarkersClear_Click(object sender, EventArgs e) {
			testMarkers.RemoveAll();
		}

		private void cmdMarkersSelectNext_Click(object sender, EventArgs e) {
			if (lstMarkers.SelectedIndex >= 0 && lstMarkers.SelectedIndex + 1 < lstMarkers.Items.Count) {
				lstMarkers.SelectedIndex++;
				SelectTestMarker(lstMarkers.SelectedIndex);
			}
		}

		private void cmdMarkersSelectPrevious_Click(object sender, EventArgs e) {
			if (lstMarkers.SelectedIndex > 0) {
				lstMarkers.SelectedIndex--;
				SelectTestMarker(lstMarkers.SelectedIndex);
			}
		}

		private void lstMarkers_DoubleClick(object sender, EventArgs e) {
			if (lstMarkers.SelectedIndex >= 0) {
				SelectTestMarker(lstMarkers.SelectedIndex);
			}
		}

		private void cmdMarkersFind_Click(object sender, EventArgs e) {
			IVsTextLines ppBuffer;
			TextEditor.CurrentWindowData.ActiveView.GetBuffer(out ppBuffer);

			if (null != squiggleToFind) {
				squiggleToFind.Invalidate();
			}

			int indexToken = Convert.ToInt32(txtMarkerTokenIndex.Text);
			int indexMarker = Convert.ToInt32(txtMarkersFind.Text);
			TokenInfo token = textEditor.RawTokens[0];
			squiggleToFind = testMarkers.CreateMarker(ppBuffer, "test_marker_on_" + token.Token.UnqoutedImage, textEditor.RawTokens, indexToken, indexToken, token.Token.UnqoutedImage, null, indexMarker);
			txtMarkersFind.Text = (indexMarker + 1).ToString();
		}

		private void cmdMiscClearKnownTokens_Click(object sender, EventArgs e) {
			testMarkers.RemoveAll();
		}

		private void cmdMiscShowKnownTokens_Click(object sender, EventArgs e) {
			SelectMiscShowKnownTokens();
		}

		private void SelectMiscShowKnownTokens() {
			IVsTextLines ppBuffer;
			TextEditor.CurrentWindowData.ActiveView.GetBuffer(out ppBuffer);
			testMarkers.RemoveAll();
			for (int i = 0; i < textEditor.RawTokens.Count; i++) {
				TokenInfo token = textEditor.RawTokens[i];
				TokenInfo nextToken = (i + 1 < textEditor.RawTokens.Count ? textEditor.RawTokens[i + 1] : null);
				if (Common.IsIdentifier(token, nextToken)) {
					ListBox.SelectedObjectCollection items = lstKnownTokenTypes.SelectedItems;
					foreach (object strTokenContextType in items) {
						TokenContextType tokenContextType = (TokenContextType)Enum.Parse(typeof (TokenContextType), strTokenContextType.ToString());
						if (token.TokenContextType == tokenContextType) {
							testMarkers.CreateMarker(ppBuffer, "marker_" + i + "_" + token.Token.UnqoutedImage, textEditor.RawTokens, i, i, token.Token.UnqoutedImage, null);
							break;
						}
					}
				}
			}
		}

		private void lstKnownTokenTypes_DoubleClick(object sender, EventArgs e) {
			SelectMiscShowKnownTokens();
		}

		// --------- WindowData
		public void SetWindowDataList(List<WindowData> lstWindowData, WindowData currentWindowData) {
			listView1.BeginUpdate();
			listView1.Items.Clear();
			foreach (WindowData windowData in lstWindowData) {
				string isActive = "";
				if (null != currentWindowData && currentWindowData == windowData) {
					isActive = "*";
				}
				ListViewItem value = new ListViewItem(isActive);
				AddWindowDataItem(value, windowData.DocCookie);
				AddWindowDataItem(value, windowData.Parser);
				AddWindowDataItem(value, windowData.SplitterRoot.ActiveActiveView);
				AddWindowDataItem(value, windowData.SplitterRoot.PrimaryActiveView);
				AddWindowDataItem(value, windowData.SplitterRoot.SecondaryActiveView);
				AddWindowDataItem(value, windowData.SplitterRoot.ActiveHwnd);
				AddWindowDataItem(value, windowData.SplitterRoot.PrimaryHwndVsEditPane);
				AddWindowDataItem(value, windowData.SplitterRoot.PrimaryHwndVsTextEditPane);
				AddWindowDataItem(value, windowData.SplitterRoot.SecondaryHwndVsEditPane);
				AddWindowDataItem(value, windowData.SplitterRoot.SecondaryHwndVsTextEditPane);
				listView1.Items.Add(value);
			}
			listView1.EndUpdate();
		}

		private static void AddWindowDataItem(ListViewItem listItem, Object value) {
			listItem.SubItems.Add(new ListViewItem.ListViewSubItem(listItem, (null == value ? "NULL" : String.Format("{0:X2}", value.GetHashCode()))));
		}

		// Case segment ----------------------------------------

		private void cmdSelectCaseSegment_Click(object sender, EventArgs e) {
			if (listBox7.SelectedIndex >= 0) {
				TextEditor.CurrentWindowData.Parser.SegmentUtils.SelectCaseSegment(listBox7.SelectedIndex, Instance.TextEditor.RawTokens);
			}
		}

		private void cmdClearCaseSegment_Click(object sender, EventArgs e) {
			TextEditor.CurrentWindowData.Parser.SegmentUtils.InvalidateCaseSegmentSquiggle();
		}

		private void cmdPreviousCaseSegment_Click(object sender, EventArgs e) {
			if (listBox7.SelectedIndex > 0) {
				listBox7.SelectedIndex--;
				TextEditor.CurrentWindowData.Parser.SegmentUtils.SelectCaseSegment(listBox7.SelectedIndex, Instance.TextEditor.RawTokens);
			}
		}

		private void cmdNextCaseSegment_Click(object sender, EventArgs e) {
			if (listBox7.SelectedIndex >= 0 && listBox7.SelectedIndex + 1 < listBox7.Items.Count) {
				listBox7.SelectedIndex++;
				TextEditor.CurrentWindowData.Parser.SegmentUtils.SelectCaseSegment(listBox7.SelectedIndex, Instance.TextEditor.RawTokens);
			}
		}

		private void listBox7_DoubleClick(object sender, EventArgs e) {
			if (listBox7.SelectedIndex >= 0) {
				TextEditor.CurrentWindowData.Parser.SegmentUtils.SelectCaseSegment(listBox7.SelectedIndex, Instance.TextEditor.RawTokens);
			}
		}
	}
}