// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Windows.Forms;
using Sassner.SmarterSql.PInvoke;
using Sassner.SmarterSql.Utils;
using Sassner.SmarterSql.Utils.Settings;

namespace Sassner.SmarterSql.UI.Subclassing {
	public class VsGenericPaneChild : NativeWindow, IDisposable {
		#region Member variables

		private const string ClassName = "VsGenericPaneChild";

		private readonly IntPtr hwndGenericPaneChild;
		private readonly IntPtr hwndGenericPane;
		private bool showConnectionColoringStrip = true;
		private ConnectionColoring connectionColoring;

		#endregion

		public VsGenericPaneChild(IntPtr hwndVsSplitterRoot) {
			hwndGenericPaneChild = Common.FindChildToGenericPaneWindow(hwndVsSplitterRoot);
			if (!NativeWIN32.IsValidWindowHandle(hwndGenericPaneChild)) {
				throw new ApplicationException("Couldn't find child window to GenericPane window.");
			}
			hwndGenericPane = Common.GetGenericPaneWindow(hwndVsSplitterRoot);
			if (!NativeWIN32.IsValidWindowHandle(hwndGenericPane)) {
				throw new ApplicationException("Couldn't find GenericPane window.");
			}

			connectionColoring = new ConnectionColoring(hwndGenericPane, hwndGenericPaneChild);
			connectionColoring.NewPosition += ConnectionColoringOnNewPosition;

			AssignHandle(hwndGenericPaneChild);

			showConnectionColoringStrip = Instance.Settings.ShowConnectionColorStrip;
		}

		#region WndProc

		///<summary>
		/// Invokes the default window procedure associated with this window. 
		///</summary>
		///<param name="m">A <see cref="T:System.Windows.Forms.Message"></see> that is associated with the current Windows message. </param>
		protected override void WndProc(ref Message m) {
			try {
				NativeWIN32.WindowsMessages msg = (NativeWIN32.WindowsMessages)m.Msg;
				switch (msg) {
					case NativeWIN32.WindowsMessages.WM_SIZE:
						if (showConnectionColoringStrip) {
							SetBounds();
						}
						break;

/*
					case NativeWIN32.WindowsMessages.WM_PAINT:
						NativeWIN32.PAINTSTRUCT ps;
						IntPtr hdc = NativeWIN32.BeginPaint(hwndGenericPaneChild, out ps);
						try {
							NativeWIN32.RECT rect;
							NativeWIN32.GetClientRect(hwndGenericPaneChild, out rect);
							rect.Bottom = 20;
							NativeWIN32.DrawFrameControl(hdc, ref rect, NativeWIN32.DFC_BUTTON, NativeWIN32.DFCS_BUTTONPUSH);
						} catch (Exception e) {
							Common.LogEntry(ClassName, "OnPaint", e, Common.enErrorLvl.Error);
						} finally {
							if (hdc != IntPtr.Zero) {
								NativeWIN32.EndPaint(hwndGenericPaneChild, ref ps);
							}
						}
						break;
*/
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "WndProc", e, Common.enErrorLvl.Error);
			}

			base.WndProc(ref m);
		}

		#endregion

		#region Events

		private void ConnectionColoringOnNewPosition(object sender, ConnectionColoring.NewPositionHandlerArgs args) {
			SetBounds();
		}

		#endregion

		#region Utils

		public void SetBounds() {
			NativeWIN32.RECT rect;
			NativeWIN32.GetClientRect(hwndGenericPane, out rect);

			if (showConnectionColoringStrip && connectionColoring.HasMatchedConnection) {
				int width;
				int height;
				int x;
				int y;
				switch (Instance.Settings.ConnectionColoringStripPosition) {
					case Settings.StripPosition.Top:
						width = rect.Right - rect.Left;
						height = rect.Bottom - rect.Top - Common.ConnectionColorStripHeight;
						x = rect.Left;
						y = rect.Top + Common.ConnectionColorStripHeight;
						break;
					case Settings.StripPosition.Left:
						width = rect.Right - rect.Left - Common.ConnectionColorStripHeight;
						height = rect.Bottom - rect.Top;
						x = rect.Left + Common.ConnectionColorStripHeight;
						y = rect.Top;
						break;
					case Settings.StripPosition.Bottom:
						width = rect.Right - rect.Left;
						height = rect.Bottom - rect.Top - Common.ConnectionColorStripHeight;
						x = rect.Left;
						y = rect.Top;
						break;
					case Settings.StripPosition.Right:
						width = rect.Right - rect.Left - Common.ConnectionColorStripHeight;
						height = rect.Bottom - rect.Top;
						x = rect.Left;
						y = rect.Top;
						break;
					default:
						return;
				}

				NativeWIN32.SetWindowPos(Handle, IntPtr.Zero, x, y, width, height, NativeWIN32.SWP_NOOWNERZORDER | NativeWIN32.SWP_NOCOPYBITS | NativeWIN32.SWP_NOZORDER | NativeWIN32.SWP_NOACTIVATE);
			} else {
				NativeWIN32.SetWindowPos(Handle, IntPtr.Zero, rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top, NativeWIN32.SWP_NOOWNERZORDER | NativeWIN32.SWP_NOCOPYBITS | NativeWIN32.SWP_NOZORDER | NativeWIN32.SWP_NOACTIVATE);
			}

			connectionColoring.SetBounds(hwndGenericPaneChild);
		}

		public void Initialize() {
			showConnectionColoringStrip = Instance.Settings.ShowConnectionColorStrip;
			if (showConnectionColoringStrip) {
				connectionColoring.CreateStrip();
			}
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
				if (null != connectionColoring) {
					connectionColoring.Dispose();
					connectionColoring = null;
				}
				ReleaseHandle();

			} catch (Exception e) {
				Common.LogEntry(ClassName, "Dispose", e, Common.enErrorLvl.Error);
			}
		}

		#endregion
	}
}
