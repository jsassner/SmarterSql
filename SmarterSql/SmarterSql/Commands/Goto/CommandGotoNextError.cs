// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using EnvDTE;
using Sassner.SmarterSql.Commands.CommandAttributes;
using Sassner.SmarterSql.Utils.Menu;

namespace Sassner.SmarterSql.Commands.Goto {
	[CommandMenuItem(Menus.MenuGroups.Goto, "&Next error", "Next error", "SQL Query Editor::Alt+F12", 2)]
	[CommandVisibleMenuAttribute(true)]
	internal class CommandGotoNextError : CommandBase {
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
			TextEditor.CurrentWindowData.Parser.ListOfScannedSqlErrors.GotoNextError();
		}
	}
}