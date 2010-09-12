// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Diagnostics;

namespace Sassner.SmarterSql.Utils.Args {
	public class MouseDownEventArgs {
		#region Member variables

		private readonly Common.enMouseButtons mouseButtons;
		private readonly int x;
		private readonly int y;
		private bool cancel;

		#endregion

		public MouseDownEventArgs(int x, int y, Common.enMouseButtons mouseButtons) {
			this.x = x;
			this.y = y;
			this.mouseButtons = mouseButtons;
		}

		#region Public properties

		public bool Cancel {
			[DebuggerStepThrough]
			get { return cancel; }
			set { cancel = value; }
		}

		public int X {
			[DebuggerStepThrough]
			get { return x; }
		}

		public int Y {
			[DebuggerStepThrough]
			get { return y; }
		}

		public Common.enMouseButtons MouseButtons {
			[DebuggerStepThrough]
			get { return mouseButtons; }
		}

		#endregion
	}
}