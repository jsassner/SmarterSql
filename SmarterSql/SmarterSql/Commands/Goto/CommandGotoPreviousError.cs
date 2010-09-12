// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using EnvDTE;
using Sassner.SmarterSql.Commands.CommandAttributes;
using Sassner.SmarterSql.Utils.Menu;

namespace Sassner.SmarterSql.Commands.Goto {
	[CommandMenuItem(Menus.MenuGroups.Goto, "&Previous error", "Previous error", "SQL Query Editor::Shift+Alt+F12", 3)]
	[CommandVisibleMenuAttribute(true)]
	internal class CommandGotoPreviousError : CommandBase {
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
			TextEditor.CurrentWindowData.Parser.ListOfScannedSqlErrors.GotoPreviousError();
		}
	}
}