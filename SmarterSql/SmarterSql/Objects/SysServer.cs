// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Diagnostics;

namespace Sassner.SmarterSql.Objects {
	public class SysServer : IntellisenseData {
		#region Member variables

		private readonly string strServerName;

		#endregion

		public SysServer(string strServerName)
			: base(strServerName) {
				this.strServerName = strServerName;
		}

		#region Public properties

		protected override enSortOrder SortLevel {
			[DebuggerStepThrough]
			get { return enSortOrder.SysServer; }
		}

		public override string MainText {
			[DebuggerStepThrough]
			get { return strServerName; }
		}

		public override int ImageKey {
			[DebuggerStepThrough]
			// TODO: Fix icon
			get { return (int)ImageKeys.None; }
		}

		public override string GetToolTip {
			[DebuggerStepThrough]
			get { return "Server"; }
		}

		public override string GetSelectedData {
			[DebuggerStepThrough]
			get { return MainText; }
		}

		public string ServerName {
			[DebuggerStepThrough]
			get { return strServerName; }
		}

		#endregion
	}
}
