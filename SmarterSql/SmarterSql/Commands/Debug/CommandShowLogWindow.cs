// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using Sassner.SmarterSql.Commands.CommandAttributes;
using Sassner.SmarterSql.Utils.Menu;

namespace Sassner.SmarterSql.Commands.Debug {
	[CommandMenuItem(Menus.MenuGroups.Debug, "&Show log window", "Show log window", "", 1)]
	[CommandVisibleMenu(true)]
	internal class CommandShowLogWindow : CommandBase {
		#region Public properties

		public override bool ShowMenuEntryInContextMenu {
			get { return true; }
		}

		#endregion

		public override void Perform() {
			Instance.LogWindow.ShowWindow();
		}
	}
}
