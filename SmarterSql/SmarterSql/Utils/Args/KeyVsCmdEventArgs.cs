// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Sassner.SmarterSql.Utils.Args {
	public class KeyVsCmdEventArgs {
		#region Member variables

		private readonly uint nCmdID;
		private readonly Guid pguidCmdGroup;
		private readonly IVsTextView activeView;
		private readonly Common.enVsCmd vsCmd;
		private bool cancel;

		#endregion

		public KeyVsCmdEventArgs(IVsTextView activeView, Common.enVsCmd vsCmd, Guid pguidCmdGroup, uint nCmdID) {
			this.activeView = activeView;
			this.vsCmd = vsCmd;
			this.pguidCmdGroup = pguidCmdGroup;
			this.nCmdID = nCmdID;
		}

		#region Public properties

		public IVsTextView ActiveView {
			[DebuggerStepThrough]
			get { return activeView; }
		}

		public bool Cancel {
			[DebuggerStepThrough]
			get { return cancel; }
			[DebuggerStepThrough]
			set { cancel = value; }
		}

		public Guid PguidCmdGroup {
			[DebuggerStepThrough]
			get { return pguidCmdGroup; }
		}

		public uint NCmdID {
			[DebuggerStepThrough]
			get { return nCmdID; }
		}

		public Common.enVsCmd VsCmd {
			[DebuggerStepThrough]
			get { return vsCmd; }
		}

		#endregion
	}
}
