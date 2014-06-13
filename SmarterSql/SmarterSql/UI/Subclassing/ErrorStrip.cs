// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.PInvoke;
using Sassner.SmarterSql.UI.Controls;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.UI.Subclassing {
	public class ErrorStrip : WindowHandler, IDisposable {
		#region Member variables

		private const string ClassName = "ErrorStrip";

		private const int imageHeight = 11;
		private const int symbolsOffset = 3 + imageHeight + 1;
		private readonly IntPtr arrowCursor = NativeWIN32.LoadCursor(IntPtr.Zero, NativeWIN32.StockCursors.IDC_ARROW);
		private readonly IntPtr handCursor = NativeWIN32.LoadCursor(IntPtr.Zero, NativeWIN32.StockCursors.IDC_HAND);
		private readonly object paintLock = new object();
		private readonly VsTextEditPane vsTextEditPane;
		private int nbOfRowsInEditor;
		private int prevX = -1;
		private int prevY = -1;
		private List<Stripe> stripes = new List<Stripe>();

		// Tooltip
		private ToolTipWindow toolTip;

		#endregion

		public ErrorStrip(VsTextEditPane vsTextEditPane, List<Stripe> stripes, int nbOfRowsInEditor) {
			this.vsTextEditPane = vsTextEditPane;
			this.stripes = stripes;
			Stripe.Sort(this.stripes);
			this.nbOfRowsInEditor = nbOfRowsInEditor;
		}

		#region Public properties

		public int NbOfRowsInEditor {
			get { return nbOfRowsInEditor; }
			set { nbOfRowsInEditor = value; }
		}

		public List<Stripe> Stripes {
			get { return stripes; }
			set {
				stripes = value;
				Stripe.Sort(stripes);
				Invalidate();
			}
		}

		#endregion

		#region Painting/creating methods

		public void CreateErrorStrip() {
			NativeWIN32.RECT rect;
			NativeWIN32.GetClientRect(GetParentHwnd(), out rect);
			rect.Right -= Common.ErrorStripWidth();

			CreateParams createParams = new CreateParams {
				Parent = GetParentHwnd(),
				Caption = "SmarterSql error strip",
				ClassName = null,
				Width = Common.ErrorStripWidth(),
				Height = (rect.Bottom - rect.Top),
				X = rect.Right,
				Y = rect.Top,
				Style = ((int)(NativeWIN32.WindowStyles.WS_CHILD | NativeWIN32.WindowStyles.WS_VISIBLE)),
				ExStyle = ((int)(NativeWIN32.WindowStylesEx.WS_EX_NOPARENTNOTIFY)),
				ClassStyle = ((int)(NativeWIN32.ClassStyles.CS_OWNDC | NativeWIN32.ClassStyles.CS_HREDRAW | NativeWIN32.ClassStyles.CS_VREDRAW))
			};

			CreateHandle(createParams);
		}

		private IntPtr GetParentHwnd() {
			return vsTextEditPane.GetParentHwnd();
		}

		public void SetBounds(Rectangle bounds) {
			if (Handle != IntPtr.Zero) {
				NativeWIN32.SetWindowPos(Handle, IntPtr.Zero, bounds.Left, bounds.Top, bounds.Width, bounds.Height, NativeWIN32.SWP_NOOWNERZORDER | NativeWIN32.SWP_NOCOPYBITS | NativeWIN32.SWP_NOZORDER | NativeWIN32.SWP_NOACTIVATE);
			}
		}

		public void Invalidate() {
			NativeWIN32.InvalidateRect(Handle, IntPtr.Zero, true);
		}

		public void SetErrorStripStripes(List<Stripe> _stripes, int _nbOfRowsInEditor, Common.StripeType stripeType) {
			nbOfRowsInEditor = _nbOfRowsInEditor;

			// Remove old stripes of supplied type
			for (int i = stripes.Count - 1; i >= 0; i--) {
				if (stripes[i].StripeType == stripeType) {
					stripes.RemoveAt(i);
				}
			}
			// Add new stripes
			foreach (Stripe stripe in _stripes) {
				stripes.Add(stripe);
			}

			// Redraw
			Invalidate();
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
			ClickStripe(GetStripe(y));
			return true;
		}

		protected override bool OnMouseLeave(int x, int y) {
			HideToolTip();
			return false;
		}

		protected override bool OnMouseHover(int x, int y) {
			ShowTooltip(y);
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

		/// <summary>
		/// Handle OnMouseMove event
		/// </summary>
		/// <param name="Y"></param>
		/// <param name="X"></param>
		/// <returns></returns>
		protected override bool OnMouseMove(int X, int Y) {
			if (Y != prevY || X != prevX) {
				// If the user has moved the mouse pointer in the Y axle, hide the tooltip
				if (prevY != Y) {
					HideToolTip();
				}

				// Set cursor
				SetCursor(Y);

				prevY = Y;
				prevX = X;

				return true;
			}

			return false;
		}

		#endregion

		#region Util methods

		/// <summary>
		/// Hide tooltip
		/// </summary>
		private void HideToolTip() {
			try {
				if (null != toolTip) {
					toolTip.Hide();
					toolTip = null;
				}
			} catch (Exception) {
				// Do nothing
			}
		}

		/// <summary>
		/// Show tooltip for the supplied stripe
		/// </summary>
		/// <param name="yPos"></param>
		private void ShowTooltip(int yPos) {
			string tooltipText;
			if (yPos < symbolsOffset) {
				tooltipText = (null != stripes && stripes.Count > 0 ? stripes.Count + " errors found" : "No errors found");
			} else {
				Stripe stripe = GetStripe(yPos);
				if (null == stripe) {
					return;
				}
				tooltipText = stripe.Tooltip;
			}
			if (null != toolTip && toolTip.Visible) {
				toolTip.Text = tooltipText;
			} else {
				HideToolTip();

				// Transform the mouse pointer position to client universe
				Point pt2 = new Point(Common.ErrorStripWidth() + 5, yPos);
				NativeWIN32.ClientToScreen(Handle, ref pt2);
				int x = pt2.X;
				int y = pt2.Y;

				// Show tooltip
				toolTip = new ToolTipWindow(Instance.TextEditor, Instance.FontTooltip);
				toolTip.Initialize(null, x, y, tooltipText, null, false, Common.enPosition.Center);
				toolTip.Show(0);
			}
		}

		/// <summary>
		/// Click on the supplied stripe
		/// </summary>
		/// <param name="stripe"></param>
		private static void ClickStripe(Stripe stripe) {
			if (null == stripe) {
				return;
			}
			IVsTextView activeView = TextEditor.CurrentWindowData.ActiveView;
			activeView.SetCaretPos(stripe.EditorLine, stripe.EditorColumn);
			Common.MakeSureCursorIsVisible(activeView, stripe.EditorLine, Common.enPosition.Center);
		}

		/// <summary>
		/// Set cursor
		/// </summary>
		/// <param name="yPos"></param>
		private void SetCursor(int yPos) {
			IntPtr currentCursor = arrowCursor;
			if (yPos < symbolsOffset) {
				currentCursor = handCursor;
			} else {
				if (null != GetStripe(yPos)) {
					currentCursor = handCursor;
				}
			}
			NativeWIN32.SetCursor(currentCursor);
		}

		/// <summary>
		/// Get a Stripe object for the supplied Y-position, or null if none found
		/// </summary>
		/// <param name="yPos"></param>
		/// <returns></returns>
		private Stripe GetStripe(int yPos) {
			if (null != stripes) {
				foreach (Stripe stripe in stripes) {
					if ((int)stripe.PositionY + 3 > yPos && (int)stripe.PositionY - 2 < yPos) {
						return stripe;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Paint the error strip
		/// </summary>
		/// <param name="hwnd"></param>
		private bool PaintStrip(IntPtr hwnd) {
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
				g.FillRectangle(SystemBrushes.Control, new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top));

				// Draw status icon
				Image image;
				if (null != stripes && stripes.Count > 0) {
					image = Resources.ErrorStrip.errorstrip_red;
				} else {
					image = Resources.ErrorStrip.errorstrip_green;
				}
				g.DrawImage(image, 3, 3);
				image.Dispose();

				// Draw symbols
				int height = rect.Bottom - rect.Top - symbolsOffset - SystemInformation.HorizontalScrollBarHeight;
				int width = rect.Right - rect.Left - 3 - 3 - 1;

				if (null != stripes && nbOfRowsInEditor > 0) {
					// Calculate positions of stripes
					foreach (Stripe stripe in stripes) {
						float index = (float)(1.0 * stripe.RowNb / nbOfRowsInEditor * height + symbolsOffset);
						stripe.PositionY = index;
					}

					// Draw stripes
					var uniqueRowStrips = Stripe.GetUniqueRowStrips(stripes);
					foreach (Stripe stripe in uniqueRowStrips) {
						g.DrawRectangle(Stripe.GetPen(stripe), 3, (int)stripe.PositionY, width, 1);
					}

				}
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
				HideToolTip();
				base.Dispose();
				DestroyHandle();
			} catch (Exception e) {
				Common.LogEntry(ClassName, "Dispose", e, Common.enErrorLvl.Error);
			}
		}

		#endregion
	}
}