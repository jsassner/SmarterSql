// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Diagnostics;
using Sassner.SmarterSql.UI.Subclassing;

namespace Sassner.SmarterSql.Utils.Args {
	public class GotFocusEventArgs {
		#region Member variables

		private readonly Common.enActiveWindow activeWindow;
		private readonly IntPtr hwnd;
		private WindowData currentWindowData;

		#endregion

		public GotFocusEventArgs(IntPtr hwnd, Common.enActiveWindow activeWindow) {
			this.hwnd = hwnd;
			this.activeWindow = activeWindow;
		}

		#region Public properties

		public IntPtr Hwnd {
			[DebuggerStepThrough]
			get { return hwnd; }
		}

		public Common.enActiveWindow ActiveWindow {
			[DebuggerStepThrough]
			get { return activeWindow; }
		}

		public WindowData CurrentWindowData {
			get { return currentWindowData; }
			set { currentWindowData = value; }
		}

		#endregion
	}
}