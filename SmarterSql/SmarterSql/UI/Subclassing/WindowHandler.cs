// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.PInvoke;
using Sassner.SmarterSql.Utils;
using Sassner.SmarterSql.Utils.Args;
using KeyEventArgs=Sassner.SmarterSql.Utils.Args.KeyEventArgs;

namespace Sassner.SmarterSql.UI.Subclassing {
	public abstract class WindowHandler : NativeWindow, IDisposable {
		#region Member variables

		private const string ClassName = "WindowHandler";

		private readonly IntPtr hwndVsTextEditPane;
		private readonly int intScrollWheelLines = 3;
		private double fMouseWheelDelta;
		private bool isInWindow;
		private bool isPainting;
		private bool needToReRegisterHover;
		private int prevXHover = -1;
		private int prevYHover = -1;

		#endregion

		protected WindowHandler() {
		}

		protected WindowHandler(IntPtr hwndVsTextEditPane) {
			this.hwndVsTextEditPane = hwndVsTextEditPane;

			AssignHandle(hwndVsTextEditPane);

			// Get the number of lines the user scrolls each tick with the mouse wheel button
			IntPtr ptrScrollWheelLines = IntPtr.Zero;
			NativeWIN32.SystemParametersInfo(NativeWIN32.SPI.SPI_GETWHEELSCROLLLINES, 0, ptrScrollWheelLines, 0);
			if (ptrScrollWheelLines != IntPtr.Zero) {
				intScrollWheelLines = ptrScrollWheelLines.ToInt32();
			}
		}

		#region Events

		#region Delegates

		public delegate void GotFocusHandler(object sender, GotFocusEventArgs e);

		public delegate bool KeyHandler(object sender, KeyEventArgs e);

		public delegate void LostFocusHandler(object sender, LostFocusEventArgs e);

		public delegate bool MouseDownHandler(object sender, MouseDownEventArgs e);

		public delegate void MouseHoverHandler(object sender, MouseMoveEventArgs e);

		public delegate bool MouseWheelHandler(object sender, MouseWheelEventArgs e);

		public delegate bool ScrollBarMovedHandler(object sender, ScrollBarMovedEventArgs e);

		#endregion

		public event KeyHandler KeyDown;
		public event KeyHandler KeyUp;

		public event MouseHoverHandler MouseHover;

		public event GotFocusHandler GotFocus;

		public event LostFocusHandler LostFocus;

		public event MouseDownHandler MouseDown;

		public event MouseWheelHandler MouseWheel;

		public event ScrollBarMovedHandler ScrollBarMoved;

		#endregion

		#region Public properties

		public IntPtr HwndVsTextEditPane {
			get { return hwndVsTextEditPane; }
		}

		#endregion

		#region Fire event methods

		protected bool FireKeyDown(KeyEventArgs e) {
			return (null != KeyDown && KeyDown(this, e));
		}

		protected bool FireKeyUp(KeyEventArgs e) {
			return (null != KeyUp && KeyUp(this, e));
		}

		protected void FireMouseHover(MouseMoveEventArgs e) {
			if (null != MouseHover) {
				MouseHover(this, e);
			}
		}

		protected void FireGotFocus(GotFocusEventArgs e) {
			if (null != GotFocus) {
				GotFocus(this, e);
			}
		}

		protected void FireLostFocus(LostFocusEventArgs e) {
			if (null != LostFocus) {
				LostFocus(this, e);
			}
		}

		protected bool FireMouseDown(MouseDownEventArgs e) {
			return (null != MouseDown && MouseDown(this, e));
		}

		protected bool FireMouseWheel(MouseWheelEventArgs e) {
			return (null != MouseWheel && MouseWheel(this, e));
		}

		protected bool FireScrollBarMoved(ScrollBarMovedEventArgs e) {
			if (null != ScrollBarMoved) {
				ScrollBarMoved(this, e);
				if (e.Cancel) {
					return true;
				}
			}

			return false;
		}

		#endregion

		#region Wndproc event methods

		protected abstract bool OnKeyDown(NativeWIN32.VirtualKeys keys, bool shift, bool ctrl, bool alt);
		protected abstract bool OnKeyUp(NativeWIN32.VirtualKeys keys, bool shift, bool ctrl, bool alt);
		protected abstract bool OnPaint(ref Message m);
		protected abstract bool OnMouseDown(int x, int y, Common.enMouseButtons mouseButton);
		protected abstract bool OnMouseLeave(int x, int y);
		protected abstract bool OnMouseHover(int x, int y);
		protected abstract bool OnMouseMove(int x, int y);
		protected abstract bool OnGotFocus();
		protected abstract bool OnLostFocus();
		protected abstract bool OnWindowPosChanging(IntPtr windowPos);
		protected abstract bool OnWindowSize(IntPtr windowPos);
		protected abstract bool OnMouseWheel(int linesToScroll, int pixelsToScroll, int newYPos);

		#endregion

		#region WndProc

		///<summary>
		/// Invokes the default window procedure associated with this window. 
		///</summary>
		///<param name="m">A <see cref="T:System.Windows.Forms.Message"></see> that is associated with the current Windows message. </param>
		protected override void WndProc(ref Message m) {
			try {
				int X;
				int Y;
				NativeWIN32.WindowsMessages msg = (NativeWIN32.WindowsMessages)m.Msg;

				bool IsShift = (NativeWIN32.GetKeyState((int)NativeWIN32.VirtualKeys.Shift) & 0x8000) > 0;
				bool IsCtrl = (NativeWIN32.GetKeyState((int)NativeWIN32.VirtualKeys.Control) & 0x8000) > 0;
				bool IsAlt = (NativeWIN32.GetKeyState((int)NativeWIN32.VirtualKeys.Menu) & 0x8000) > 0;
				NativeWIN32.VirtualKeys wParam = (NativeWIN32.VirtualKeys)m.WParam;

				switch (msg) {


					case NativeWIN32.WindowsMessages.WM_MOUSEWHEEL:
					case NativeWIN32.WindowsMessages.WM_MOUSEHWHEEL:
						// Calculate the number of rows to scroll
						int intZDelta = NativeWIN32.GET_WHEEL_DELTA_WPARAM(m.WParam);
						fMouseWheelDelta += intZDelta;
						int linesToScroll = (int)(fMouseWheelDelta * intScrollWheelLines / NativeWIN32.WHEEL_DELTA);
						fMouseWheelDelta = fMouseWheelDelta - (1.0 * linesToScroll * NativeWIN32.WHEEL_DELTA / intScrollWheelLines);
						int pixelsToScroll;
						int newYPos;
						if (CalcYPositionToScrollTo(TextEditor.CurrentWindowData.ActiveView, linesToScroll, out pixelsToScroll, out newYPos)) {
							if (OnMouseWheel(linesToScroll, pixelsToScroll, newYPos)) {
								m.Result = IntPtr.Zero;
								return;
							}
						}
						break;

					case NativeWIN32.WindowsMessages.WM_WINDOWPOSCHANGING:
						OnWindowPosChanging(m.LParam);
						break;

					case NativeWIN32.WindowsMessages.WM_SIZE:
						OnWindowSize(m.LParam);
						break;

					case NativeWIN32.WindowsMessages.WM_SETFOCUS:
						OnGotFocus();
						break;

					case NativeWIN32.WindowsMessages.WM_KILLFOCUS:
						OnLostFocus();
						break;

					case NativeWIN32.WindowsMessages.WM_MOUSEMOVE:
						X = NativeWIN32.GET_X_LPARAM(m.LParam);
						Y = NativeWIN32.GET_Y_LPARAM(m.LParam);
						// Need to register for hover events?
						if (!isInWindow || needToReRegisterHover) {
							isInWindow = true;
							needToReRegisterHover = false;
							TrackMouseEvent(NativeWIN32.TME_HOVER | NativeWIN32.TME_LEAVE);
						}
						if (OnMouseMove(X, Y)) {
							m.Result = (IntPtr)1;
							return;
						}
						break;

					case NativeWIN32.WindowsMessages.WM_MOUSEHOVER:
						needToReRegisterHover = true;
						X = NativeWIN32.GET_X_LPARAM(m.LParam);
						Y = NativeWIN32.GET_Y_LPARAM(m.LParam);
						if (prevXHover != X || prevYHover != Y) {
							prevXHover = X;
							prevYHover = Y;
							if (OnMouseHover(X, Y)) {
								if (null != MouseHover) {
									MouseHover(this, new MouseMoveEventArgs(X, Y));
								}
								m.Result = (IntPtr)1;
								return;
							}
						}
						break;

					case NativeWIN32.WindowsMessages.WM_MOUSELEAVE:
						isInWindow = false;
						X = NativeWIN32.GET_X_LPARAM(m.LParam);
						Y = NativeWIN32.GET_Y_LPARAM(m.LParam);
						if (OnMouseLeave(X, Y)) {
							m.Result = (IntPtr)1;
							return;
						}
						break;

					case NativeWIN32.WindowsMessages.WM_RBUTTONDOWN:
					case NativeWIN32.WindowsMessages.WM_XBUTTONDOWN:
					case NativeWIN32.WindowsMessages.WM_LBUTTONDOWN:
						X = NativeWIN32.GET_X_LPARAM(m.LParam);
						Y = NativeWIN32.GET_Y_LPARAM(m.LParam);
						Common.enMouseButtons mouseButton = (msg == NativeWIN32.WindowsMessages.WM_RBUTTONDOWN ? Common.enMouseButtons.Right : (msg == NativeWIN32.WindowsMessages.WM_LBUTTONDOWN ? Common.enMouseButtons.Left : Common.enMouseButtons.Middle));

						if (OnMouseDown(X, Y, mouseButton)) {
							m.Result = (IntPtr)1;
							return;
						}
						break;

					case NativeWIN32.WindowsMessages.WM_ERASEBKGND:
						m.Result = (IntPtr)1;
						return;

					case NativeWIN32.WindowsMessages.WM_PAINT:
						if (!isPainting) {
							isPainting = true;
							if (OnPaint(ref m)) {
								m.Result = (IntPtr)1;
								isPainting = false;
								return;
							}
						}
						isPainting = false;
						break;

					case NativeWIN32.WindowsMessages.WM_SYSKEYDOWN:
					case NativeWIN32.WindowsMessages.WM_KEYDOWN:
						if (OnKeyDown(wParam, IsShift, IsCtrl, IsAlt)) {
							m.Result = IntPtr.Zero;
							return;
						}
						break;

					case NativeWIN32.WindowsMessages.WM_SYSKEYUP:
					case NativeWIN32.WindowsMessages.WM_KEYUP:
						if (OnKeyUp(wParam, IsShift, IsCtrl, IsAlt)) {
							m.Result = IntPtr.Zero;
							return;
						}
						break;
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "WndProc", e, Common.enErrorLvl.Error);
			}

			base.WndProc(ref m);
		}

		#endregion

		#region Utils

		/// <summary>
		/// Start to listen to WM_MOUSEHOVER and WM_MOUSELEAVE event
		/// </summary>
		private void TrackMouseEvent(uint dwFlags) {
			NativeWIN32.TRACKMOUSEEVENT tme = new NativeWIN32.TRACKMOUSEEVENT();
			tme.cbSize = Marshal.SizeOf(tme);
			tme.hwndTrack = Handle;
			tme.dwHoverTime = 200;
			tme.dwFlags = dwFlags;
			NativeWIN32.TrackMouseEvent(ref tme);
		}

		/// <summary>
		/// Calculate the Y position to scroll
		/// </summary>
		/// <param name="activeView"></param>
		/// <param name="linesToScroll"></param>
		/// <param name="pixelsToScroll"></param>
		/// <param name="newYPos"></param>
		/// <returns></returns>
		private static bool CalcYPositionToScrollTo(IVsTextView activeView, int linesToScroll, out int pixelsToScroll, out int newYPos) {
			pixelsToScroll = 0;
			newYPos = 0;

			// Get the current position in the code window
			int piMinUnit;
			int piMaxUnit;
			int piVisibleUnits;
			int piFirstVisibleUnit;
			if (!NativeMethods.Succeeded(activeView.GetScrollInfo(NativeWIN32.SB_VERT, out piMinUnit, out piMaxUnit, out piVisibleUnits, out piFirstVisibleUnit))) {
				return false;
			}

			// Get line height
			int lineHeight;
			if (!NativeMethods.Succeeded(activeView.GetLineHeight(out lineHeight))) {
				return false;
			}

			// Get cursor position
			int piLine;
			int piColumn;
			if (!NativeMethods.Succeeded(activeView.GetCaretPos(out piLine, out piColumn))) {
				return false;
			}

			// Since this event is triggered before the code window is updated, subtract the number of rows we just scrolled
			piFirstVisibleUnit -= linesToScroll;
			int actualScrolledLines = linesToScroll;

			// Validate the data
			if (piFirstVisibleUnit < 0) {
				actualScrolledLines += piFirstVisibleUnit;
				piFirstVisibleUnit = 0;
			} else if (piFirstVisibleUnit > piMaxUnit) {
				actualScrolledLines -= (piFirstVisibleUnit - piMaxUnit);
				piFirstVisibleUnit = piMaxUnit;
			}

			// Calculate the position of the window. Handle that the current caret pos is scrolled out of window range
			if (piLine < piFirstVisibleUnit) {
				newYPos = 0;
			} else if (piLine > piVisibleUnits + piFirstVisibleUnit) {
				newYPos = lineHeight * piMaxUnit;
			} else {
				newYPos = lineHeight * (piLine - piFirstVisibleUnit + 1);
			}

			pixelsToScroll = actualScrolledLines * lineHeight;

			return true;
		}

		#endregion

		#region IDisposable Members

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose() {
			try {
				TrackMouseEvent(NativeWIN32.TME_CANCEL);
			} catch (Exception e) {
				Common.LogEntry(ClassName, "Dispose", e, Common.enErrorLvl.Error);
			}
		}

		#endregion
	}
}