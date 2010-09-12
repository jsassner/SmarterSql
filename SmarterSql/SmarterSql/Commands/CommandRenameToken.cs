// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using EnvDTE;
using Sassner.SmarterSql.Commands.CommandAttributes;
using Sassner.SmarterSql.Utils.Menu;

namespace Sassner.SmarterSql.Commands {
	[CommandMenuItem(Menus.MenuGroups.Root, "&Rename token", "Rename token", "SQL Query Editor::Shift+F6", 1)]
	[CommandVisibleMenuAttribute(true)]
	internal class CommandRenameToken : CommandBase {
		#region Public properties

		/// <summary>
		/// Should this menu entry be shown in the context menu?
		/// </summary>
		/// <returns></returns>
		public override bool ShowMenuEntryInContextMenu {
			get { return (Instance.ApplicationObject.ActiveWindow.Type == vsWindowType.vsWindowTypeDocument); }
		}

		#endregion

		/// <summary>
		/// Performs the command
		/// </summary>
		public override void Perform() {
			Instance.TextEditor.UserWantToRenameToken();
		}
	}
}
