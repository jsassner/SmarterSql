// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.PInvoke;
using Sassner.SmarterSql.Utils;
using Sassner.SmarterSql.Utils.Settings;

namespace Sassner.SmarterSql.UI.Subclassing {
	public class ConnectionColoring : WindowHandler, IDisposable {
		#region Member variables

		private const string ClassName = "ConnectionColoring";

		#region Events

		public delegate void NewPositionHandler(object sender, NewPositionHandlerArgs args);

		public event NewPositionHandler NewPosition;

		public class NewPositionHandlerArgs {
			// ReSharper disable once UnusedAutoPropertyAccessor.Global
			public Settings.StripPosition StripPosition { get; set; }

			public NewPositionHandlerArgs(Settings.StripPosition stripPosition) {
				StripPosition = stripPosition;
			}
		}

		#endregion

		private readonly object paintLock = new object();
		private readonly IntPtr hwndGenericPane;
		private readonly IntPtr hwndGenericPaneChild;
		private Brush brush;
		private readonly IntPtr arrowCursor = NativeWIN32.LoadCursor(IntPtr.Zero, NativeWIN32.StockCursors.IDC_ARROW);
		private readonly IntPtr handCursor = NativeWIN32.LoadCursor(IntPtr.Zero, NativeWIN32.StockCursors.IDC_HAND);
		private bool isInWindow;

		#endregion

		public ConnectionColoring(IntPtr hwndGenericPane, IntPtr hwndGenericPaneChild) {
			this.hwndGenericPane = hwndGenericPane;
			this.hwndGenericPaneChild = hwndGenericPaneChild;
		}

		public bool HasMatchedConnection {
			get {
				return (null != brush);
			}
		}

		#region Painting/creating methods

		public void CreateStrip() {
			ActiveConnection activeConnection = Instance.FuncActiveConnection();
			brush = Instance.Settings.GetConnectionColor(activeConnection);

			if (IntPtr.Zero != Handle || !HasMatchedConnection) {
				Common.LogEntry(ClassName, "CreateStrip", "Had existing brush", Common.enErrorLvl.Information);
				Invalidate();
				return;
			}

			NativeWIN32.RECT rect;
			NativeWIN32.GetClientRect(hwndGenericPane, out rect);

			int width;
			int height;
			int x;
			int y;
			CalculateStripePosition(rect, out width, out height, out x, out y);

			CreateParams createParams = new CreateParams {
				Parent = hwndGenericPane,
				Caption = "SmarterSql connection coloring strip",
				ClassName = null,
				Width = width,
				Height = height,
				X = x,
				Y = y,
				Style = ((int)(NativeWIN32.WindowStyles.WS_CHILD | NativeWIN32.WindowStyles.WS_VISIBLE)),
				ExStyle = ((int)(NativeWIN32.WindowStylesEx.WS_EX_NOPARENTNOTIFY)),
				ClassStyle = ((int)(NativeWIN32.ClassStyles.CS_OWNDC | NativeWIN32.ClassStyles.CS_HREDRAW | NativeWIN32.ClassStyles.CS_VREDRAW))
			};

			CreateHandle(createParams);
		}

		private static void CalculateStripePosition(NativeWIN32.RECT rect, out int width, out int height, out int x, out int y) {
			switch (Instance.Settings.ConnectionColoringStripPosition) {
				case Settings.StripPosition.Top:
					width = rect.Right - rect.Left;
					height = Common.ConnectionColorStripHeight;
					x = rect.Left;
					y = rect.Top;
					return;
				case Settings.StripPosition.Left:
					width = Common.ConnectionColorStripHeight;
					height = rect.Bottom - rect.Top;
					x = rect.Left;
					y = rect.Top;
					return;
				case Settings.StripPosition.Bottom:
					width = rect.Right - rect.Left;
					height = Common.ConnectionColorStripHeight;
					x = rect.Left;
					y = rect.Bottom;
					return;
				case Settings.StripPosition.Right:
					width = Common.ConnectionColorStripHeight;
					height = rect.Bottom - rect.Top;
					x = rect.Right - rect.Left;
					y = rect.Top;
					return;
			}
			width = 0;
			height = 0;
			x = 0;
			y = 0;
		}

		public void SetBounds(IntPtr hwnd) {
			if (!HasMatchedConnection) {
				return;
			}

			NativeWIN32.RECT rect;
			NativeWIN32.GetClientRect(hwnd, out rect);

			if (Handle != IntPtr.Zero) {
				int width;
				int height;
				int x;
				int y;
				CalculateStripePosition(rect, out width, out height, out x, out y);
				NativeWIN32.SetWindowPos(Handle, IntPtr.Zero, x, y, width, height, NativeWIN32.SWP_NOOWNERZORDER | NativeWIN32.SWP_NOCOPYBITS | NativeWIN32.SWP_NOZORDER | NativeWIN32.SWP_NOACTIVATE);
			}
		}

		public void Invalidate() {
			NativeWIN32.InvalidateRect(Handle, IntPtr.Zero, true);
		}

		#endregion

		#region WndProc methods

		protected override bool OnKeyDown(NativeWIN32.VirtualKeys keys, bool shift, bool ctrl, bool alt) {
			return false;
		}

		protected override bool OnKeyUp(NativeWIN32.VirtualKeys keys, bool shift, bool ctrl, bool alt) {
			return false;
		}

		protected override bool OnMouseDown(int x, int y, Common.enMouseButtons mouseButton) {
			if (mouseButton == Common.enMouseButtons.Right) {
				ShowSettingsContextMenu(x, y);

				return true;
			}
			return false;
		}

		protected override bool OnMouseLeave(int x, int y) {
			isInWindow = false;
			NativeWIN32.SetCursor(arrowCursor);
			return false;
		}

		protected override bool OnMouseHover(int x, int y) {
			return false;
		}

		protected override bool OnGotFocus(IntPtr hwnd) {
			return false;
		}

		protected override bool OnLostFocus(IntPtr hwnd) {
			return false;
		}

		protected override bool OnWindowPosChanging(IntPtr windowPos) {
			return false;
		}

		protected override bool OnPaint(ref Message m) {
			return PaintStrip(m.HWnd);
		}

		protected override bool OnWindowSize(IntPtr windowPos) {
			return false;
		}

		protected override bool OnMouseWheel(int linesToScroll, int pixelsToScroll, int newYPos) {
			return false;
		}

		protected override bool OnMouseMove(int X, int Y) {
			if (!isInWindow) {
				isInWindow = true;
				NativeWIN32.SetCursor(handCursor);
			}
			return false;
		}

		#endregion

		#region Util methods

		private void ShowSettingsContextMenu(int x, int y) {
			ContextMenuStrip menu = new ContextMenuStrip {
				ShowImageMargin = false
			};

			ToolStripItem toolStripItem = new ToolStripButton("Set color") {
				Tag = null
			};
			menu.Items.Add(toolStripItem);
			toolStripItem = new ToolStripButton("Top") {
				Tag = Settings.StripPosition.Top
			};
			menu.Items.Add(toolStripItem);
			toolStripItem = new ToolStripButton("Left") {
				Tag = Settings.StripPosition.Left
			};
			menu.Items.Add(toolStripItem);
			toolStripItem = new ToolStripButton("Bottom") {
				Tag = Settings.StripPosition.Bottom
			};
			menu.Items.Add(toolStripItem);
			toolStripItem = new ToolStripButton("Right") {
				Tag = Settings.StripPosition.Right
			};
			menu.Items.Add(toolStripItem);

			ConnectionColoring thisReference = this;
			menu.ItemClicked += (sender, args) => {
				if (null != args.ClickedItem.Tag) {
					Instance.Settings.ConnectionColoringStripPosition = (Settings.StripPosition)args.ClickedItem.Tag;
					Instance.Settings.StoreSettings();
					if (null != NewPosition) {
						NewPosition(thisReference, new NewPositionHandlerArgs(Instance.Settings.ConnectionColoringStripPosition));
					}
				} else {
					ColorDialog colorDialog = new ColorDialog();
					if (DialogResult.OK == colorDialog.ShowDialog()) {
						ActiveConnection activeConnection = Instance.FuncActiveConnection();
						Settings.ConnectionColorSetting connectionColor = Instance.Settings.GetConnectionColorSetting(activeConnection);
						if (null != connectionColor) {
							connectionColor.Color = colorDialog.Color;
							brush = new SolidBrush(connectionColor.Color);
							Instance.Settings.StoreSettings();
							Invalidate();
						}
					}
				}
			};

			Point pt = new Point(x, y);
			NativeWIN32.ClientToScreen(Handle, ref pt);
			menu.Show(pt.X, pt.Y);
		}

		/// <summary>
		/// Paint the error strip
		/// </summary>
		/// <param name="hwnd"></param>
		private bool PaintStrip(IntPtr hwnd) {
			if (!HasMatchedConnection) {
				return true;
			}

			Monitor.Enter(paintLock);

			NativeWIN32.PAINTSTRUCT ps;
			IntPtr hdc = NativeWIN32.BeginPaint(hwnd, out ps);
			try {
				NativeWIN32.RECT rect;
				NativeWIN32.GetClientRect(hwnd, out rect);
				Graphics g = Graphics.FromHdc(hdc);

				// Validate width and height
				if (rect.Right - rect.Left <= 0 || rect.Bottom - rect.Top <= 0) {
					return false;
				}

				// Draw background
				g.FillRectangle(brush, new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top));

			} catch (Exception e) {
				Common.LogEntry(ClassName, "OnPaint", e, Common.enErrorLvl.Error);
				return false;

			} finally {
				if (hdc != IntPtr.Zero) {
					NativeWIN32.EndPaint(hwnd, ref ps);
				}

				Monitor.Exit(paintLock);
			}
			return true;
		}

		#endregion

		#region IDisposable Members

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public new void Dispose() {
			try {
				// Debug.WriteLine("ErrorStrip dispose " + Handle.GetHashCode() + " " + Handle.ToInt32());
				base.Dispose();
				DestroyHandle();

			} catch (Exception e) {
				Common.LogEntry(ClassName, "Dispose", e, Common.enErrorLvl.Error);
			}
		}

		#endregion
	}
}
