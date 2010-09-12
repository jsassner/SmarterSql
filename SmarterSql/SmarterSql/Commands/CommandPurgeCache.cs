// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using Sassner.SmarterSql.Commands.CommandAttributes;
using Sassner.SmarterSql.Utils;
using Sassner.SmarterSql.Utils.Menu;

namespace Sassner.SmarterSql.Commands {
	[CommandMenuItem(Menus.MenuGroups.Root, "&Purge cache", "Purge cache", "Global::Shift+Ctrl+Alt+F12", 2)]
	[CommandVisibleMenuAttribute(true)]
	internal class CommandPurgeCache : CommandBase {
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
		/// Performs the command
		/// </summary>
		public override void Perform() {
			Instance.TextEditor.PurgeCache();
			Instance.TextEditor.ScheduleFullReparse();
			Common.InfoMsg("Cache purged");
		}
	}
}
