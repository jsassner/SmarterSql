// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using Sassner.SmarterSql.Commands.CommandAttributes;
using Sassner.SmarterSql.UI;
using Sassner.SmarterSql.Utils.Menu;

namespace Sassner.SmarterSql.Commands {
	[CommandMenuItem(Menus.MenuGroups.Root, "&About", "About", "", 7)]
	[CommandVisibleMenuAttribute(true)]
	internal class CommandAbout : CommandBase {
		#region Public properties

		/// <summary>
		/// Should this menu entry be shown in the context menu?
		/// </summary>
		/// <returns></returns>
		public override bool ShowMenuEntryInContextMenu {
			get { return true; }
		}

		#endregion

		/// <summary>
		/// Performs the command : Launch the application in Windows Explorer
		/// </summary>
		public override void Perform() {
			frmAbout frmAbout = new frmAbout();
			frmAbout.ShowDialog();
		}
	}
}
