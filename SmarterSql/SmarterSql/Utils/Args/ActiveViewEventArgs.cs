// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Diagnostics;
using Sassner.SmarterSql.UI.Subclassing;

namespace Sassner.SmarterSql.Utils.Args {
	public class ActiveViewEventArgs {
		#region Member variables

		private readonly WindowData windowData;

		#endregion

		public ActiveViewEventArgs(WindowData windowData) {
			this.windowData = windowData;
		}

		#region Public properties

		public WindowData WindowData {
			[DebuggerStepThrough]
			get { return windowData; }
		}

		#endregion
	}
}