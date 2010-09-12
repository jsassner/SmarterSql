// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Diagnostics;
using Sassner.SmarterSql.PInvoke;

namespace Sassner.SmarterSql.Utils.Args {
	public class KeyEventArgs {
		#region Member variables

		private readonly bool isAlt;
		private readonly bool isCtrl;
		private readonly bool isShift;
		private readonly NativeWIN32.VirtualKeys vk;
		private bool cancel;

		#endregion

		public KeyEventArgs(NativeWIN32.VirtualKeys vk, bool isShift, bool isCtrl, bool isAlt) {
			this.vk = vk;
			this.isShift = isShift;
			this.isCtrl = isCtrl;
			this.isAlt = isAlt;
		}

		#region Public properties

		public bool Cancel {
			[DebuggerStepThrough]
			get { return cancel; }
			set { cancel = value; }
		}

		public NativeWIN32.VirtualKeys VK {
			[DebuggerStepThrough]
			get { return vk; }
		}

		public bool IsShift {
			[DebuggerStepThrough]
			get { return isShift; }
		}

		public bool IsCtrl {
			[DebuggerStepThrough]
			get { return isCtrl; }
		}

		public bool IsAlt {
			[DebuggerStepThrough]
			get { return isAlt; }
		}

		#endregion
	}
}