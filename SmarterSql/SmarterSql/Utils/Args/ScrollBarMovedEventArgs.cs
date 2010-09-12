// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Diagnostics;

namespace Sassner.SmarterSql.Utils.Args {
	public class ScrollBarMovedEventArgs {
		#region Member variables

		private readonly int iBar;
		private readonly int iFirstVisibleUnit;
		private readonly int iMaxUnits;
		private readonly int iMinUnit;
		private readonly int iVisibleUnits;
		private bool cancel;

		#endregion

		public ScrollBarMovedEventArgs(int iBar, int iMinUnit, int iMaxUnits, int iVisibleUnits, int iFirstVisibleUnit) {
			this.iBar = iBar;
			this.iMinUnit = iMinUnit;
			this.iMaxUnits = iMaxUnits;
			this.iVisibleUnits = iVisibleUnits;
			this.iFirstVisibleUnit = iFirstVisibleUnit;
		}

		#region Public properties

		public bool Cancel {
			[DebuggerStepThrough]
			get { return cancel; }
			[DebuggerStepThrough]
			set { cancel = value; }
		}
		public int Bar {
			[DebuggerStepThrough]
			get { return iBar; }
		}

		public int MinUnit {
			[DebuggerStepThrough]
			get { return iMinUnit; }
		}

		public int MaxUnits {
			[DebuggerStepThrough]
			get { return iMaxUnits; }
		}

		public int VisibleUnits {
			[DebuggerStepThrough]
			get { return iVisibleUnits; }
		}

		public int FirstVisibleUnit {
			[DebuggerStepThrough]
			get { return iFirstVisibleUnit; }
		}

		#endregion

		public bool IsSame(ScrollBarMovedEventArgs bar2) {
			if (null == bar2) {
				return false;
			}
			return (iBar == bar2.iBar && iMinUnit == bar2.iMinUnit && iMaxUnits == bar2.iMaxUnits && iVisibleUnits == bar2.iVisibleUnits && iFirstVisibleUnit == bar2.iFirstVisibleUnit);
		}
	}
}
