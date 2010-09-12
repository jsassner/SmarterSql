// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using System;
using System.Diagnostics;

namespace Sassner.SmarterSql.Utils {
	public class PopupLastShown {
		#region Member variables

		private DateTime tmLastShown;
		private int line;
		private int column;
		private bool canShowPopup;

		#endregion

		public PopupLastShown() {
			canShowPopup = true;
			tmLastShown = DateTime.Now;
		}

		public PopupLastShown(DateTime tmLastShown, int line, int column, bool canShowPopup) {
			this.tmLastShown = tmLastShown;
			this.line = line;
			this.column = column;
			this.canShowPopup = canShowPopup;
		}

		#region Public properties

		public bool CanShowPopup {
			[DebuggerStepThrough]
			get { return canShowPopup; }
			set { canShowPopup = value; }
		}

		public DateTime LastShown {
			[DebuggerStepThrough]
			get { return tmLastShown; }
			[DebuggerStepThrough]
			set { tmLastShown = value; }
		}

		public int Line {
			[DebuggerStepThrough]
			get { return line; }
			[DebuggerStepThrough]
			set { line = value; }
		}

		public int Column {
			[DebuggerStepThrough]
			get { return column; }
			[DebuggerStepThrough]
			set { column = value; }
		}

		#endregion

		internal bool ShallAbort(int intCursorLine, int intCursorColumn) {
			if (canShowPopup) {
				return false;
			}
			if (DateTime.Now.Subtract(tmLastShown).Seconds > 10) {
				return false;
			}
			if (intCursorLine != line) {
				return false;
			}
			if (intCursorColumn - 1 == column) {
				return true;
			}
			return false;
		}
	}
}
