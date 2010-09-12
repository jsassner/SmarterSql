// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.PInvoke;
using Sassner.SmarterSql.Utils;
using Sassner.SmarterSql.Utils.Args;
using KeyEventArgs=Sassner.SmarterSql.Utils.Args.KeyEventArgs;

namespace Sassner.SmarterSql.UI.Subclassing {
	public class VsTextEditPane : WindowHandler, IVsTextViewEvents, IDisposable {
		#region Member variables

		private const string ClassName = "VsTextEditPane";
		private readonly ScrollBarMovedEventArgs[] scrollBarMovedEventArgses = new ScrollBarMovedEventArgs[2];
		private uint cookie;
		private IConnectionPoint cp;

		private int currentXPos;
		private int currentYPos;
		private ErrorStrip errorStrip;
		private bool isPaintingCurrentLine;
		private bool showErrorStrip = true;
		private VsEditPane vsEditPane;

		#endregion

		public VsTextEditPane(VsEditPane vsEditPane, IntPtr hwndVsTextEditPane, List<Stripe> stripes, int nbOfRowsInEditor) : base(hwndVsTextEditPane) {
			this.vsEditPane = vsEditPane;
			showErrorStrip = Instance.Settings.ShowErrorStrip;

			if (showErrorStrip) {
				// Add the errorstrip window
				errorStrip = new ErrorStrip(this, stripes, nbOfRowsInEditor);
				errorStrip.CreateErrorStrip();
			}
		}

		#region Public properties

		public int CurrentXPos {
			[DebuggerStepThrough]
			get { return currentXPos; }
		}

		public int CurrentYPos {
			[DebuggerStepThrough]
			get { return currentYPos; }
		}

		#endregion

		#region Utils

		/// <summary>
		/// Start listening for IVsTextViewEvents. Must be done some time after the window has been created/subclassed
		/// </summary>
		/// <param name="activeView"></param>
		public void Initialize(IVsTextView activeView) {
			IConnectionPointContainer cpContainer = activeView as IConnectionPointContainer;
			if (cpContainer != null) {
				Guid textViewGuid = typeof (IVsTextViewEvents).GUID;
				cpContainer.FindConnectionPoint(ref textViewGuid, out cp);
				cp.Advise(this, out cookie);
			}
			showErrorStrip = Instance.Settings.ShowErrorStrip;
		}

		public void OnFontChange(Font fontEditor) {
			NativeWIN32.InvalidateRect(Handle, IntPtr.Zero, true);
		}

		public void SetErrorStripStripes(List<Stripe> stripes, int nbOfRowsInEditor) {
			if (null != errorStrip) {
				errorStrip.Stripes = stripes;
				errorStrip.NbOfRowsInEditor = nbOfRowsInEditor;
			}
		}

		public IntPtr GetParentHwnd() {
			return (null != vsEditPane ? vsEditPane.HwndVsEditPane : IntPtr.Zero);
		}

		public void SetErrorBounds(Rectangle rectangle) {
			if (null != errorStrip && showErrorStrip) {
				errorStrip.SetBounds(rectangle);
			}
		}

		#endregion

		#region IDisposable Members

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public new void Dispose() {
			try {
				if (null != cp) {
					cp.Unadvise(cookie);
				}

				base.Dispose();
				ReleaseHandle();

				vsEditPane = null;

				if (null != errorStrip) {
					errorStrip.Dispose();
					errorStrip = null;
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "Dispose", e, Common.enErrorLvl.Error);
			}
		}

		#endregion

		#region WndProc methods

		protected override bool OnKeyDown(NativeWIN32.VirtualKeys keys, bool shift, bool ctrl, bool alt) {
			return FireKeyDown(new KeyEventArgs(keys, shift, ctrl, alt));
		}

		protected override bool OnKeyUp(NativeWIN32.VirtualKeys keys, bool shift, bool ctrl, bool alt) {
			return FireKeyUp(new KeyEventArgs(keys, shift, ctrl, alt));
		}

		protected override bool OnPaint(ref Message m) {
			if (Instance.Settings.HighlightCurrentLine) {
				PaintCurrentLineHighlighted(ref m);
			}
			base.WndProc(ref m);
			return true;
		}

		protected override bool OnMouseDown(int x, int y, Common.enMouseButtons mouseButton) {
			return FireMouseDown(new MouseDownEventArgs(x, y, mouseButton));
		}

		protected override bool OnMouseLeave(int x, int y) {
			return false;
		}

		protected override bool OnMouseHover(int x, int y) {
			FireMouseHover(new MouseMoveEventArgs(x, y));
			return false;
		}

		protected override bool OnMouseMove(int x, int y) {
			currentXPos = x;
			currentYPos = y;
			return false;
		}

		protected override bool OnGotFocus() {
			FireGotFocus(new GotFocusEventArgs(HwndVsTextEditPane, (null != vsEditPane ? vsEditPane.ActiveWindow : Common.enActiveWindow.Main)));
			return false;
		}

		protected override bool OnLostFocus() {
			FireLostFocus(new LostFocusEventArgs());
			return false;
		}

		protected override bool OnWindowSize(IntPtr windowPos) {
			return false;
		}

		protected override bool OnMouseWheel(int linesToScroll, int pixelsToScroll, int newYPos) {
			return FireMouseWheel(new MouseWheelEventArgs(Handle, linesToScroll, pixelsToScroll, newYPos));
		}

		protected override bool OnWindowPosChanging(IntPtr windowPos) {
			if (showErrorStrip) {
				NativeWIN32.WINDOWPOS winPos = (NativeWIN32.WINDOWPOS)Marshal.PtrToStructure(windowPos, typeof (NativeWIN32.WINDOWPOS));
				winPos.cx -= Common.ErrorStripWidth();
				Marshal.StructureToPtr(winPos, windowPos, false);

				if (null != errorStrip) {
					Rectangle rectangle = new Rectangle(winPos.x + winPos.cx + SystemInformation.VerticalScrollBarWidth, winPos.y, Common.ErrorStripWidth(), winPos.cy);
					// Debug.WriteLine("vsTextEditPane OnWindowPosChanging " + Handle.GetHashCode() + " (" + rectangle.Left + "," + winPos.x + "," + winPos.cx + ")");
					errorStrip.SetBounds(rectangle);
				}
			}
			return false;
		}

		#endregion

		#region Painting methods

		/// <summary>
		/// Paint the current line background
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		public bool PaintCurrentLineHighlighted(ref Message m) {
			NativeWIN32.RECT updateRect;
			NativeWIN32.RECT lineRect;
			NativeWIN32.RECT clientRect;
			bool hiddenCaret = false;

			if (isPaintingCurrentLine) {
				return true;
			}

			// Is the update rect empty?
			NativeWIN32.GetUpdateRect(m.HWnd, out updateRect, false);
			if (NativeWIN32.IsRectEmpty(ref updateRect)) {
				return true;
			}
			// Get current line & client rect
			if (!GetCurrentLineRect(m.HWnd, out lineRect, out clientRect)) {
				return false;
			}

			isPaintingCurrentLine = true;
			try {
				// Hide (if visible) the mouse pointer (caret)
				if (NativeWIN32.GetFocus() == Handle) {
					hiddenCaret = true;
					NativeWIN32.HideCaret(Handle);
				}

				// Paint the hightlighted row on the background
				Bitmap image = null;
				Graphics g = null;
				try {
					image = new Bitmap(clientRect.Right - clientRect.Left, clientRect.Bottom - clientRect.Top);
					g = Graphics.FromImage(image);
					IntPtr hdc = g.GetHdc();
					NativeWIN32.PrintWindow(Handle, hdc, NativeWIN32.PW_CLIENTONLY);
					g.ReleaseHdc(hdc);

					PaintCurrentLineBackground(g, lineRect);

					Graphics.FromHwnd(Handle).DrawImage(image, new Point(0, 0));
					NativeWIN32.ValidateRect(Handle, ref clientRect);
				} finally {
					if (g != null) {
						g.Dispose();
					}
					if (image != null) {
						image.Dispose();
					}
				}
			} finally {
				// Show (if previous visible) the mouse pointer (caret)
				if (hiddenCaret) {
					NativeWIN32.ShowCaret(Handle);
				}
				isPaintingCurrentLine = false;
			}
			return true;
		}

		/// <summary>
		/// Paint background on the current line
		/// </summary>
		/// <param name="g"></param>
		/// <param name="lineRect"></param>
		private static void PaintCurrentLineBackground(Graphics g, NativeWIN32.RECT lineRect) {
			Brush brush = null;
			try {
				brush = new SolidBrush(Color.FromArgb(Instance.Settings.AlphaHighlightCurrentLine, Instance.Settings.ColorHighlightCurrentLine));
				Rectangle rectangle = new Rectangle(lineRect.Left, lineRect.Top, lineRect.Right - lineRect.Left, lineRect.Bottom - lineRect.Top);
				g.FillRectangle(brush, rectangle);
			} finally {
				if (brush != null) {
					brush.Dispose();
				}
			}
		}

		/// <summary>
		/// Get both client rect (whole text window) and the current line rect
		/// </summary>
		/// <param name="hwnd"></param>
		/// <param name="rect"></param>
		/// <param name="clientRect"></param>
		/// <returns></returns>
		private static bool GetCurrentLineRect(IntPtr hwnd, out NativeWIN32.RECT rect, out NativeWIN32.RECT clientRect) {
			try {
				if (null != TextEditor.CurrentWindowData) {
					IVsTextView textView = TextEditor.CurrentWindowData.ActiveView;

					int line;
					if (Common.FindActualLineNumber(textView, out line)) {
						int firstLineInEditor;
						int lineHeight;
						int editorLeftMargin;
						if (Common.GetTextEditorInfo(textView, hwnd, out lineHeight, out editorLeftMargin, out firstLineInEditor)) {
							// Adjust for the user scrolling in the texteditor
							line -= firstLineInEditor;

							// Construct drawing area
							NativeWIN32.GetClientRect(hwnd, out clientRect);
							rect.Top = clientRect.Top + line * lineHeight;
							rect.Bottom = rect.Top + lineHeight;
							rect.Left = editorLeftMargin;
							rect.Right = clientRect.Right;

							return true;
						}
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "OnPaint", e, Common.enErrorLvl.Error);
			}

			rect = new NativeWIN32.RECT();
			clientRect = new NativeWIN32.RECT();
			return false;
		}

		/// <summary>
		/// Set bounds of the text editor
		/// </summary>
		/// <param name="hwndVsEditPane"></param>
		public void SetBounds(IntPtr hwndVsEditPane) {
			NativeWIN32.RECT rect;
			NativeWIN32.GetClientRect(hwndVsEditPane, out rect);

			if (showErrorStrip) {
				NativeWIN32.SetWindowPos(Handle, IntPtr.Zero, rect.Left, rect.Top, rect.Right - rect.Left - Common.ErrorStripWidth(), rect.Bottom - rect.Top - SystemInformation.HorizontalScrollBarHeight, NativeWIN32.SWP_NOOWNERZORDER | NativeWIN32.SWP_NOCOPYBITS | NativeWIN32.SWP_NOZORDER | NativeWIN32.SWP_NOACTIVATE);
			} else {
				NativeWIN32.SetWindowPos(Handle, IntPtr.Zero, rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top - SystemInformation.HorizontalScrollBarHeight, NativeWIN32.SWP_NOOWNERZORDER | NativeWIN32.SWP_NOCOPYBITS | NativeWIN32.SWP_NOZORDER | NativeWIN32.SWP_NOACTIVATE);
			}
		}

		public void InvalidateCurrentLine() {
			NativeWIN32.RECT rect;
			NativeWIN32.GetClientRect(Handle, out rect);
			NativeWIN32.InvalidateRect(Handle, rect, false);
		}

		#endregion

		#region IVsTextViewEvents Members

		///<summary>
		///Notifies the client when a change of caret line occurs.
		///</summary>
		///
		///<param name="iNewLine">[in] Integer containing the new line.</param>
		///<param name="pView">[in] Pointer to a view object.</param>
		///<param name="iOldLine">[in] Integer containing the old line.</param>
		void IVsTextViewEvents.OnChangeCaretLine(IVsTextView pView, int iNewLine, int iOldLine) {
			// TODO: Jämför rects
			InvalidateCurrentLine();
		}

		///<summary>
		///Notifies a client when a view receives focus.
		///</summary>
		///
		///<param name="pView">[in] Pointer to the <see cref="T:Microsoft.VisualStudio.TextManager.Interop.IVsTextView"></see> interface.</param>
		void IVsTextViewEvents.OnSetFocus(IVsTextView pView) {
		}

		///<summary>
		///Notifies a client when a view loses focus.
		///</summary>
		///
		///<param name="pView">[in] Pointer to the <see cref="T:Microsoft.VisualStudio.TextManager.Interop.IVsTextView"></see> interface.</param>
		void IVsTextViewEvents.OnKillFocus(IVsTextView pView) {
		}

		///<summary>
		///Notifies a client when a view is attached to a new buffer.
		///</summary>
		///
		///<param name="pView">[in] Pointer to the <see cref="T:Microsoft.VisualStudio.TextManager.Interop.IVsTextView"></see> interface.</param>
		///<param name="pBuffer">[in] Pointer to the <see cref="T:Microsoft.VisualStudio.TextManager.Interop.IVsTextLines"></see> interface.</param>
		void IVsTextViewEvents.OnSetBuffer(IVsTextView pView, IVsTextLines pBuffer) {
		}

		///<summary>
		///Notifies a client when the scrolling information is changed.
		///</summary>
		///
		///<param name="iMinUnit">[in] Integer value for the minimum units.</param>
		///<param name="pView">[in] Pointer to a view object.</param>
		///<param name="iFirstVisibleUnit">[in] Integer value for the first visible unit.</param>
		///<param name="iMaxUnits">[in] Integer value for the maximum units.</param>
		///<param name="iVisibleUnits">[in] Integer value for the visible units.</param>
		///<param name="iBar">[in] Integer value referring to the bar.</param>
		void IVsTextViewEvents.OnChangeScrollInfo(IVsTextView pView, int iBar, int iMinUnit, int iMaxUnits, int iVisibleUnits, int iFirstVisibleUnit) {
			ScrollBarMovedEventArgs scrollBarMovedEventArgs = new ScrollBarMovedEventArgs(iBar, iMinUnit, iMaxUnits, iVisibleUnits, iFirstVisibleUnit);

			if (null == scrollBarMovedEventArgses[iBar] || !scrollBarMovedEventArgses[iBar].IsSame(scrollBarMovedEventArgs)) {
				scrollBarMovedEventArgses[iBar] = scrollBarMovedEventArgs;

				FireScrollBarMoved(scrollBarMovedEventArgs);
			}
		}

		#endregion
	}
}