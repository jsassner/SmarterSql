// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Sassner.SmarterSql.PInvoke;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.UI.Subclassing {
	public class ScrollBar : NativeWindow, IDisposable {
		#region Member variables

		private const string ClassName = "ScrollBar";

		private readonly bool isVertical;
		private readonly bool showErrorStrip;
		private int splitterHeight;

		#endregion

		public ScrollBar(IntPtr hwndScrollBar, bool isVertical, bool showErrorStrip) {
			this.isVertical = isVertical;
			this.showErrorStrip = showErrorStrip;

			AssignHandle(hwndScrollBar);
		}

		#region Public properties

		public int SplitterHeight {
			get { return splitterHeight; }
		}

		#endregion

		#region IDisposable Members

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose() {
			try {
				ReleaseHandle();
			} catch (Exception e) {
				Common.LogEntry(ClassName, "Dispose", e, Common.enErrorLvl.Error);
			}
		}

		#endregion

		protected override void WndProc(ref Message m) {
			if (showErrorStrip && m.Msg == (int)NativeWIN32.WindowsMessages.WM_WINDOWPOSCHANGING) {
				IntPtr windowPos = m.LParam;
				NativeWIN32.WINDOWPOS winPos = (NativeWIN32.WINDOWPOS)Marshal.PtrToStructure(windowPos, typeof (NativeWIN32.WINDOWPOS));

				if (isVertical) {
					splitterHeight = winPos.y;
					// Debug.WriteLine("scrollbar WM_WINDOWPOSCHANGING " + m.HWnd.GetHashCode() + ", height " + splitterHeight);
					winPos.x -= Common.ErrorStripWidth();
				} else {
					winPos.cx -= Common.ErrorStripWidth();
				}
				Marshal.StructureToPtr(winPos, windowPos, false);
			}

			base.WndProc(ref m);
		}

		public void SetBounds(IntPtr hwndVsEditPane) {
			if (isVertical) {
				if (Handle != IntPtr.Zero) {
					NativeWIN32.RECT rect;
					NativeWIN32.GetClientRect(hwndVsEditPane, out rect);

					if (showErrorStrip) {
						NativeWIN32.SetWindowPos(Handle, IntPtr.Zero, rect.Right - Common.ErrorStripWidth(), splitterHeight, SystemInformation.VerticalScrollBarWidth, rect.Bottom - rect.Top - SystemInformation.HorizontalScrollBarHeight - splitterHeight, NativeWIN32.SWP_NOOWNERZORDER | NativeWIN32.SWP_NOCOPYBITS | NativeWIN32.SWP_NOZORDER | NativeWIN32.SWP_NOACTIVATE);
					} else {
						NativeWIN32.SetWindowPos(Handle, IntPtr.Zero, rect.Right, splitterHeight, SystemInformation.VerticalScrollBarWidth, rect.Bottom - rect.Top - SystemInformation.HorizontalScrollBarHeight - splitterHeight, NativeWIN32.SWP_NOOWNERZORDER | NativeWIN32.SWP_NOCOPYBITS | NativeWIN32.SWP_NOZORDER | NativeWIN32.SWP_NOACTIVATE);
					}
				}
			}
		}
	}
}