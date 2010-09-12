// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.PInvoke;
using Sassner.SmarterSql.Utils;
using Sassner.SmarterSql.Utils.Helpers;

namespace Sassner.SmarterSql.UI.Subclassing {
	public class VsEditPane : NativeWindow, IDisposable {
		#region Member variables

		private const string ClassName = "VsEditPane";

		private readonly IntPtr hwndVsEditPane;
		private readonly IntPtr hwndVsTextEditPane;
		private Common.enActiveWindow activeWindow;
		private ScrollBar horizontalScrollBar;
		private bool showErrorStrip = true;
		private ScrollBar verticalScrollBar;
		private VsTextEditPane vsTextEditPane;

		#endregion

		public VsEditPane(Common.enActiveWindow activeWindow, IntPtr hwndVsEditPane, IntPtr hwndVsTextEditPane, List<Stripe> stripes, int nbOfRowsInEditor) {
			this.activeWindow = activeWindow;
			this.hwndVsEditPane = hwndVsEditPane;
			this.hwndVsTextEditPane = hwndVsTextEditPane;

			AssignHandle(hwndVsEditPane);

			// Create a VsTextEditPane, i.e. subclass the text editor
			vsTextEditPane = new VsTextEditPane(this, hwndVsTextEditPane, stripes, nbOfRowsInEditor);

			showErrorStrip = Instance.Settings.ShowErrorStrip;

			IntPtr hwndVerticalScrollBar = IntPtr.Zero;
			IntPtr hwndHorizontalScrollBar = IntPtr.Zero;
			NativeWIN32.EnumChildWindows(hwndVsEditPane, delegate(IntPtr hwnd, IntPtr pointer) {
				if (NativeWIN32.IsScrollBar(hwnd)) {
					if (NativeWIN32.IsScrollBarVertical(hwnd)) {
						hwndVerticalScrollBar = hwnd;
					} else {
						hwndHorizontalScrollBar = hwnd;
					}
				}
				return true;
			}
				, IntPtr.Zero);

			SubclassScrollBars(hwndVerticalScrollBar, hwndHorizontalScrollBar);
		}

		#region Public properties

		public bool ShowErrorStrip {
			get { return showErrorStrip; }
			set { showErrorStrip = value; }
		}

		public ScrollBar VerticalScrollBar {
			get { return verticalScrollBar; }
		}

		public ScrollBar HorizontalScrollBar {
			get { return horizontalScrollBar; }
		}

		public Common.enActiveWindow ActiveWindow {
			[DebuggerStepThrough]
			get { return activeWindow; }
			set { activeWindow = value; }
		}

		public VsTextEditPane VsTextEditPane {
			[DebuggerStepThrough]
			get { return vsTextEditPane; }
		}

		public IntPtr HwndVsEditPane {
			[DebuggerStepThrough]
			get { return hwndVsEditPane; }
		}

		public IntPtr HwndVsTextEditPane {
			[DebuggerStepThrough]
			get { return hwndVsTextEditPane; }
		}

		public int CurrentXPos {
			[DebuggerStepThrough]
			get { return vsTextEditPane.CurrentXPos; }
		}

		public int CurrentYPos {
			[DebuggerStepThrough]
			get { return vsTextEditPane.CurrentYPos; }
		}

		#endregion

		#region WndProc

		///<summary>
		/// Invokes the default window procedure associated with this window. 
		///</summary>
		///<param name="m">A <see cref="T:System.Windows.Forms.Message"></see> that is associated with the current Windows message. </param>
		protected override void WndProc(ref Message m) {
			try {
				NativeWIN32.WindowsMessages msg = (NativeWIN32.WindowsMessages)m.Msg;
				NativeWIN32.RECT rect;
				NativeWIN32.GetClientRect(m.HWnd, out rect);
				int splitterHeight = GetSplitterHeight();

				if (showErrorStrip && msg >= NativeWIN32.WindowsMessages.WM_MOUSEFIRST && msg <= NativeWIN32.WindowsMessages.WM_MOUSELAST && 0 != splitterHeight) {
					int x = NativeWIN32.GET_X_LPARAM(m.LParam);
					int y = NativeWIN32.GET_Y_LPARAM(m.LParam);
					NativeWIN32.RECT splitterRect = GetSplitterRect(rect, splitterHeight);
					if (RectHelper.ContainsInclusive(splitterRect, x, y)) {
						// "Move" the mouse event to the right, the width of the ErrorStrip
						m.LParam = NativeWIN32.MakeLParam(x + Common.ErrorStripWidth(), y);
						Cursor.Current = Cursors.HSplit;
					} else {
						Cursor.Current = Cursors.Default;
					}
				}

				switch (msg) {
					case NativeWIN32.WindowsMessages.WM_WINDOWPOSCHANGING:
						NativeWIN32.InvalidateRect(m.HWnd, IntPtr.Zero, false);
						break;

					case NativeWIN32.WindowsMessages.WM_PAINT:
						if (showErrorStrip) {
							OnPaint(m.HWnd, splitterHeight);
						}
						break;

					case NativeWIN32.WindowsMessages.WM_SIZE:
						if (showErrorStrip) {
							base.WndProc(ref m);
							NativeWIN32.InvalidateRect(m.HWnd, IntPtr.Zero, false);
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

		private void OnPaint(IntPtr hwnd, int splitterHeight) {
			NativeWIN32.PAINTSTRUCT ps;
			IntPtr hdc = NativeWIN32.BeginPaint(hwnd, out ps);
			try {
				NativeWIN32.RECT rect;
				NativeWIN32.GetClientRect(hwnd, out rect);
				Graphics g = Graphics.FromHdc(hdc);

				// Draw background
				g.FillRectangle(SystemBrushes.Control, new Rectangle(rect.Right - Common.ErrorStripWidth() - SystemInformation.VerticalScrollBarWidth, rect.Bottom - SystemInformation.HorizontalScrollBarHeight, SystemInformation.VerticalScrollBarWidth, SystemInformation.HorizontalScrollBarHeight));

				// Need to draw the splitter button?
				if (null != verticalScrollBar) {
					NativeWIN32.RECT splitterRect = GetSplitterRect(rect, splitterHeight);
					if (0 != splitterHeight) {
						NativeWIN32.DrawFrameControl(hdc, ref splitterRect, NativeWIN32.DFC_BUTTON, NativeWIN32.DFCS_BUTTONPUSH);
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "OnPaint", e, Common.enErrorLvl.Error);
			} finally {
				if (hdc != IntPtr.Zero) {
					NativeWIN32.EndPaint(hwnd, ref ps);
				}
			}
		}

		private int GetSplitterHeight() {
			if (null == verticalScrollBar) {
				return 0;
			}
			return verticalScrollBar.SplitterHeight;
		}

		/// <summary>
		/// Get the rect of the splitter
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="splitterHeight"></param>
		/// <returns></returns>
		private static NativeWIN32.RECT GetSplitterRect(NativeWIN32.RECT rect, int splitterHeight) {
			NativeWIN32.RECT splitterRect = new NativeWIN32.RECT {
				Top = rect.Top,
				Bottom = splitterHeight,
				Left = (rect.Right - Common.ErrorStripWidth() - SystemInformation.VerticalScrollBarWidth),
				Right = (rect.Right - Common.ErrorStripWidth())
			};
			return splitterRect;
		}

		private void SubclassScrollBars(IntPtr hwndVerticalScrollBar, IntPtr hwndHorizontalScrollBar) {
			verticalScrollBar = new ScrollBar(hwndVerticalScrollBar, true, showErrorStrip);
			horizontalScrollBar = new ScrollBar(hwndHorizontalScrollBar, false, showErrorStrip);
		}

		public void SetBounds() {
			vsTextEditPane.SetBounds(hwndVsEditPane);
			if (null != verticalScrollBar) {
				verticalScrollBar.SetBounds(hwndVsEditPane);
			}
		}

		public void Initialize(IVsTextView activeView) {
			vsTextEditPane.Initialize(activeView);
			showErrorStrip = Instance.Settings.ShowErrorStrip;
			SetBounds();
		}

		#endregion

		#region IDisposable Members

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose() {
			try {
				if (null != horizontalScrollBar) {
					horizontalScrollBar.Dispose();
					horizontalScrollBar = null;
				}
				if (null != verticalScrollBar) {
					verticalScrollBar.Dispose();
					verticalScrollBar = null;
				}
				if (null != vsTextEditPane) {
					vsTextEditPane.Dispose();
					vsTextEditPane = null;
				}
				ReleaseHandle();
			} catch (Exception e) {
				Common.LogEntry(ClassName, "Dispose", e, Common.enErrorLvl.Error);
			}
		}

		#endregion
	}
}