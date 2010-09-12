// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Diagnostics;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Sassner.SmarterSql.Utils.Args {
	public class SplitWindowEventArgs {
		#region Member variables

		private readonly IVsTextView primaryActiveView;
		private readonly IVsTextView secondaryActiveView;

		#endregion

		public SplitWindowEventArgs(IVsTextView primaryActiveView, IVsTextView secondaryActiveView) {
			this.primaryActiveView = primaryActiveView;
			this.secondaryActiveView = secondaryActiveView;
		}

		#region Public properties

		public IVsTextView PrimaryActiveView {
			[DebuggerStepThrough]
			get { return primaryActiveView; }
		}

		public IVsTextView SecondaryActiveView {
			[DebuggerStepThrough]
			get { return secondaryActiveView; }
		}

		#endregion
	}
}
