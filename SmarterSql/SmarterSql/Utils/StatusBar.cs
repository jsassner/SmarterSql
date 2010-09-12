// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using Microsoft.VisualStudio.Shell.Interop;

namespace Sassner.SmarterSql.Utils {
	public class StatusBar {
		private static readonly IVsStatusbar statusBar;

		static StatusBar() {
			statusBar = (IVsStatusbar)Instance.Sp.GetService(typeof (SVsStatusbar));
		}

		public static void SetText(string text) {
			int frozen;
			statusBar.IsFrozen(out frozen);

			if (frozen == 0) {
				statusBar.SetColorText(text, 0, 0);
			}
		}

		public static void Animate(bool Enable) {
			object icon = (short)Constants.SBAI_General;
			statusBar.Animation((Enable ? 1 : 0), ref icon);
		}
	}
}