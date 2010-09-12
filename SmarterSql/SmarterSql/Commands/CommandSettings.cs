// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using Sassner.SmarterSql.Commands.CommandAttributes;
using Sassner.SmarterSql.UI;
using Sassner.SmarterSql.Utils.Menu;
using Sassner.SmarterSql.Utils.Settings;

namespace Sassner.SmarterSql.Commands {
	[CommandMenuItem(Menus.MenuGroups.Root, "S&ettings", "Set options", "", 6)]
	[CommandVisibleMenuAttribute(true)]
	internal class CommandSettings : CommandBase {
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
			frmSettings objFrmSettings = new frmSettings();
			objFrmSettings.SettingsUpdated += objFrmSettings_SettingsUpdated;
			objFrmSettings.Servers = Instance.Servers;
			objFrmSettings.ShowDialog();
			objFrmSettings.SettingsUpdated -= objFrmSettings_SettingsUpdated;
			objFrmSettings.Dispose();
		}

		private static void objFrmSettings_SettingsUpdated(Settings settings) {
			Instance.SettingsUpdated(settings);
		}
	}
}
