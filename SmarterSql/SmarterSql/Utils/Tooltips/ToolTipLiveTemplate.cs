// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using EnvDTE;
using Sassner.SmarterSql.UI.Controls;
using Sassner.SmarterSql.Utils.Args;

namespace Sassner.SmarterSql.Utils.Tooltips {
	public abstract class ToolTipLiveTemplate {
		#region Member variables

		protected ToolTipWindow toolTipWindow;

		#endregion

		protected ToolTipLiveTemplate(ToolTipWindow toolTipWindow) {
			this.toolTipWindow = toolTipWindow;
		}

		public abstract void Execute(TextSelection Selection);

		#region Window functions

		public virtual void OnMouseDown(object sender, MouseDownEventArgs e) {
			toolTipWindow.HideToolTip();
			return;
		}

		public virtual void OnKeyDownVsCommands(object sender, KeyVsCmdEventArgs e) {
			switch (e.VsCmd) {
				case Common.enVsCmd.Escape:
				case Common.enVsCmd.Delete:
				case Common.enVsCmd.ArrowUp:
				case Common.enVsCmd.ArrowDown:
				case Common.enVsCmd.PageUp:
				case Common.enVsCmd.PageDown:
				case Common.enVsCmd.Home:
				case Common.enVsCmd.End:
				case Common.enVsCmd.Right:
				case Common.enVsCmd.Left:
					toolTipWindow.HideToolTip();
					break;
			}
			return;
		}

		public virtual void OnKeyDown(object sender, KeyEventArgs e) {
			toolTipWindow.HideToolTip();
		}

		public virtual void OnCaretMoved(int oldLine, int oldColumn, int newLine, int newColumn) {
		}

		public virtual void OnLostFocus(object sender, LostFocusEventArgs e) {
			toolTipWindow.HideToolTip();
		}

		#endregion
	}
}