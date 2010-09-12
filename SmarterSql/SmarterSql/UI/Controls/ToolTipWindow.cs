// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.PInvoke;
using Sassner.SmarterSql.Utils;
using Sassner.SmarterSql.Utils.Args;
using Sassner.SmarterSql.Utils.Tooltips;
using KeyEventArgs=Sassner.SmarterSql.Utils.Args.KeyEventArgs;

namespace Sassner.SmarterSql.UI.Controls {
	public partial class ToolTipWindow : Form {
		#region Privat members

		private const string ClassName = "ToolTipWindow";

		private readonly TextEditor textEditor;
		private bool blnMouseInWindow;
		private Font fontTooltip;
		private int initialX;
		private int initialY;
		private int lineHeight;
		private ToolTipLiveTemplate liveTemplate;

		#endregion

		/// <summary>
		/// Public constructor
		/// </summary>
		/// <param name="textEditor"></param>
		/// <param name="fontTooltip"></param>
		public ToolTipWindow(TextEditor textEditor, Font fontTooltip) {
			this.textEditor = textEditor;
			try {
				this.fontTooltip = fontTooltip;
				InitializeComponent();

				textEditor.KeyDownVsCommands += textEditor_KeyDownVsCommands;
				textEditor.KeyDown += textEditor_KeyDown;
				textEditor.LostFocus += textEditor_LostFocus;
				textEditor.MouseDown += textEditor_MouseDown;
				textEditor.MouseWheel += textEditor_MouseWheel;

				SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.DoubleBuffer, true);

				richTextBox1.AutoSize = true;
				richTextBox1.Font = fontTooltip;
				Text = "ToolTipWindow";
			} catch (Exception e) {
				Common.LogEntry(ClassName, "Constructor", e, Common.enErrorLvl.Error);
			}
		}

		#region Utils

		public static void ShowIntelligentToolTipWindow(ToolTipWindow toolTipMethodInfo, ToolTipLiveTemplateIntelligent liveTemplate, int initialSelectedParameter, int moveXChars, int endTokenIndex, Font fontEditor) {
			ShowToolTipWindow(toolTipMethodInfo, liveTemplate.GetToolTipText(initialSelectedParameter), liveTemplate, 0, false, Common.enPosition.Bottom, -2, moveXChars, fontEditor);
			//liveTemplate.CreateSquiggle(RawTokens, endTokenIndex);
		}

		public static void ShowNormalToolTipWindow(ToolTipWindow toolTip, string Text, ToolTipLiveTemplate liveTemplate, Font fontEditor) {
			ShowToolTipWindow(toolTip, Text, liveTemplate, 700, true, Common.enPosition.Top, 1, 0, fontEditor);
		}

		public static void ShowToolTipWindow(ToolTipWindow toolTip, string text, int timeout, bool CancelOnKey, Common.enPosition relativePosition, int xfactor, int moveXChars, Font fontEditor) {
			ShowToolTipWindow(toolTip, text, new ToolTipLiveTemplateInfo(toolTip), timeout, CancelOnKey, relativePosition, xfactor, moveXChars, fontEditor);
		}

		public bool Initialize(IVsTextView ActiveView, int X, int Y, string ToolTipText, ToolTipLiveTemplate _liveTemplate, bool CancelOnKey, Common.enPosition pos) {
			try {
				liveTemplate = _liveTemplate;
				initialX = X;
				initialY = Y;

				if (!NativeMethods.Succeeded(TextEditor.CurrentWindowData.ActiveView.GetLineHeight(out lineHeight))) {
					lineHeight = FontHeight;
				}

				// Set text last, since it resizes the tooltip
				Text = ToolTipText;

				PositionToolTip(pos);
				NativeWIN32.SetWindowPos(Handle, new IntPtr(NativeWIN32.HWND_TOPMOST), 0, 0, 0, 0, NativeWIN32.SWP_NOACTIVATE | NativeWIN32.SWP_NOMOVE | NativeWIN32.SWP_NOSIZE);
			} catch (Exception e) {
				Common.LogEntry(ClassName, "Initialize", e, "Error when initializing: ", Common.enErrorLvl.Error);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Show a tooltip window
		/// </summary>
		/// <param name="toolTipWindow"></param>
		/// <param name="text"></param>
		/// <param name="liveTemplate"></param>
		/// <param name="timeout"></param>
		/// <param name="CancelOnKey"></param>
		/// <param name="relativePosition"></param>
		/// <param name="xfactor"></param>
		/// <param name="moveXChars"></param>
		/// <param name="fontEditor"></param>
		public static void ShowToolTipWindow(ToolTipWindow toolTipWindow, string text, ToolTipLiveTemplate liveTemplate, int timeout, bool CancelOnKey, Common.enPosition relativePosition, int xfactor, int moveXChars, Font fontEditor) {
			int x;
			int y;
			Common.GetCoordinates(out x, out y, moveXChars);

			// Move the tooltip window a bit
			int intWidth = TextRenderer.MeasureText("X", fontEditor, Size.Empty, TextFormatFlags.NoPadding).Width;
			x += (intWidth / xfactor);

			toolTipWindow.Initialize(TextEditor.CurrentWindowData.ActiveView, x, y, text, liveTemplate, CancelOnKey, relativePosition);
			toolTipWindow.Show(timeout);
		}

		public void PositionToolTip(Common.enPosition pos) {
			int y = initialY + lineHeight;

			switch (pos) {
				case Common.enPosition.Top:
					y = initialY - lineHeight - richTextBox1.Height;
					break;

				case Common.enPosition.Center:
					y = initialY - richTextBox1.Height / 2;
					break;

				default:
					break;
			}

			int screenHeight = NativeWIN32.GetScreenHeight();

			// See that the window isn't shown outside the monitor.
			// TODO: See to that it always fits (completly new ui needed - have to be able to scroll).
			if (y < 10) {
				y = 10;
			} else if (y + richTextBox1.Height > screenHeight) {
				y = screenHeight - richTextBox1.Height;
			}

			SetLocation(initialX, y);
		}

		public void HideToolTip() {
			StopTimer();
			Hide();
		}

		private void SetLocation(int X, int Y) {
			Location = new Point(X, Y);
		}

		private void timer1_Tick(object sender, EventArgs e) {
			timer1.Enabled = false;
			Show();
		}

		public bool StopTimer() {
			bool blnTimerIsRunning = (timer1.Enabled);
			timer1.Enabled = false;
			return blnTimerIsRunning;
		}

		#endregion

		#region Public properties

		public ToolTipLiveTemplate LiveTemplate {
			get { return liveTemplate; }
		}

		public bool MouseInWindow {
			get { return blnMouseInWindow; }
			set { blnMouseInWindow = value; }
		}

		public Font FontTooltip {
			get { return fontTooltip; }
			set {
				fontTooltip = value;
				richTextBox1.Font = fontTooltip;
				ResizeTooltip();
			}
		}

		public new string Text {
			get { return richTextBox1.Text; }
			set {
				try {
					string strFontName = FontTooltip.Name;
					// The parameter for the font-size command is in half-points
					int intFontSize = (int)(FontTooltip.SizeInPoints * 2);
					richTextBox1.Rtf = @"{\rtf1\ansi{\fonttbl{\f0\fnil\fcharset0 " + strFontName + @";}}\fs" + intFontSize + " " + value.Replace("\n", @"\par ") + "}";

					ResizeTooltip();
				} catch (Exception e) {
					Common.LogEntry(ClassName, "Text", e, Common.enErrorLvl.Error);
				}
			}
		}

		public bool ToolTipEnabled {
			get { return timer1.Enabled; }
		}

		/// <summary>
		/// Show window without activating it
		/// </summary>
		protected override bool ShowWithoutActivation {
			get { return true; }
		}

		public void Show(int intTimeOut) {
			if (intTimeOut > 0) {
				timer1.Interval = intTimeOut;
				timer1.Enabled = true;
			} else {
				Show();
			}
		}

		private void ResizeTooltip() {
			try {
				int textLength = richTextBox1.Text.Length;
				int textLines = richTextBox1.GetLineFromCharIndex(textLength) + 1;
				int maxWidth = 0;
				foreach (string line in richTextBox1.Lines) {
					int width = TextRenderer.MeasureText(line, richTextBox1.Font).Width;
					if (width > maxWidth) {
						maxWidth = width;
					}
				}
				richTextBox1.Width = maxWidth + 2;
				int newHeight = TextRenderer.MeasureText("X", richTextBox1.Font).Height * textLines + 2;
				richTextBox1.Height = newHeight;

				// +2 for the border (1 pixel width on each side)
				Width = richTextBox1.Width + 2;
				Height = newHeight + 2;
			} catch (Exception e) {
				Common.LogEntry(ClassName, "ResizeTooltip", e, "Error when resizing: ", Common.enErrorLvl.Error);
			}
		}

		#endregion

		#region Event implemenations

		public void OnCaretMoved(int oldLine, int oldColumn, int newLine, int newColumn) {
			if (null != LiveTemplate) {
				LiveTemplate.OnCaretMoved(oldLine, oldColumn, newLine, newColumn);
			}
		}

		private bool textEditor_MouseWheel(object sender, MouseWheelEventArgs e) {
			SetLocation(Left, Top + e.PixelsToScroll);
			return true;
		}

		private void textEditor_MouseDown(object sender, MouseDownEventArgs e) {
			if (null != LiveTemplate) {
				LiveTemplate.OnMouseDown(sender, e);
			}
		}

		private void textEditor_LostFocus(object sender, LostFocusEventArgs e) {
			if (null != LiveTemplate) {
				LiveTemplate.OnLostFocus(sender, e);
			}
			HideToolTip();
		}

		private void textEditor_KeyDownVsCommands(object sender, KeyVsCmdEventArgs e) {
			if (null != LiveTemplate) {
				LiveTemplate.OnKeyDownVsCommands(sender, e);
			}
		}

		private void textEditor_KeyDown(object sender, KeyEventArgs e) {
			if (null != LiveTemplate) {
				liveTemplate.OnKeyDown(sender, e);
			}
		}

		private void richTextBox1_MouseEnter(object sender, EventArgs e) {
			MouseInWindow = true;
		}

		private void richTextBox1_MouseLeave(object sender, EventArgs e) {
			MouseInWindow = false;
		}

		#endregion

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public new void Dispose() {
			base.Dispose();

			textEditor.KeyDownVsCommands -= textEditor_KeyDownVsCommands;
			textEditor.KeyDown -= textEditor_KeyDown;
			textEditor.LostFocus -= textEditor_LostFocus;
			textEditor.MouseDown -= textEditor_MouseDown;
			textEditor.MouseWheel -= textEditor_MouseWheel;
		}
	}
}