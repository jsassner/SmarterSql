// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Windows.Forms;
using Sassner.SmarterSql.PInvoke;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.UI.Controls {
	/// <summary>
	/// Show a window with an information text and a progress bar
	/// </summary>
	public partial class frmShowInformation : Form {
		#region Member variables

		private const string ClassName = "frmShowInformation";

		#endregion

		public frmShowInformation() {
			InitializeComponent();

			SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.DoubleBuffer, true);
			Opacity = 0;
		}

		public void Show(string information, IntPtr hwndCodeWindow) {
			try {
				if (IntPtr.Zero != hwndCodeWindow && NativeWIN32.IsWindow(hwndCodeWindow)) {
					NativeWIN32.RECT lpRect;
					// Get code window rect
					NativeWIN32.GetWindowRect(hwndCodeWindow, out lpRect);

					int x = lpRect.Right - Width - 10;
					int y = lpRect.Bottom - Height - 10;
					NativeWIN32.SetWindowPos(Handle, new IntPtr(NativeWIN32.HWND_TOPMOST), x, y, 0, 0, NativeWIN32.SWP_NOACTIVATE | NativeWIN32.SWP_NOSIZE);
					Opacity = 85;
				}

				lblInformation.Text = information;
				Show();
				//				Application.DoEvents();
			} catch (Exception e) {
				Common.LogEntry(ClassName, "Show", e, Common.enErrorLvl.Error);
			}
		}

		public void HideWindow() {
			try {
				Close();
				//				Application.DoEvents();
			} catch (Exception e) {
				Common.LogEntry(ClassName, "HideWindow", e, Common.enErrorLvl.Error);
			}
		}

		#region Public properties

		/// <summary>
		/// Show window without activating it
		/// </summary>
		protected override bool ShowWithoutActivation {
			get { return true; }
		}

		#endregion
	}
}