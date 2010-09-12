// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using Sassner.SmarterSql.Commands.CommandAttributes;
using Sassner.SmarterSql.UI;
using Sassner.SmarterSql.Utils.Menu;

namespace Sassner.SmarterSql.Commands {
	[CommandMenuItem(Menus.MenuGroups.Root, "&License agreement...", "License agreement...", "", 4)]
	[CommandVisibleMenuAttribute(true)]
	internal class CommandLicense : CommandBase {
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
		/// Performs the command : Show license
		/// </summary>
		public override void Perform() {
			frmShowLicens frmLicens = new frmShowLicens();
			frmLicens.ShowReadOnly();
		}
	}
}
