// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Diagnostics;

namespace Sassner.SmarterSql.Utils.Args {
	public class MouseWheelEventArgs {
		#region Public properties

		private readonly IntPtr hwnd;
		private readonly int linesToScroll;
		private readonly int newYPos;
		private readonly int pixelsToScroll;
		private bool cancel;

		#endregion

		public MouseWheelEventArgs(IntPtr hwnd, int linesToScroll, int pixelsToScroll, int newYPos) {
			this.hwnd = hwnd;
			this.linesToScroll = linesToScroll;
			this.pixelsToScroll = pixelsToScroll;
			this.newYPos = newYPos;
		}

		#region Public properties

		public IntPtr Hwnd {
			[DebuggerStepThrough]
			get { return hwnd; }
		}

		public int PixelsToScroll {
			[DebuggerStepThrough]
			get { return pixelsToScroll; }
		}

		public int NewYPos {
			[DebuggerStepThrough]
			get { return newYPos; }
		}

		public bool Cancel {
			[DebuggerStepThrough]
			get { return cancel; }
			set { cancel = value; }
		}

		public int LinesToScroll {
			[DebuggerStepThrough]
			get { return linesToScroll; }
		}

		#endregion
	}
}