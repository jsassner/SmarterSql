// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using EnvDTE;
using Sassner.SmarterSql.Commands.CommandAttributes;
using Sassner.SmarterSql.Utils.Menu;

namespace Sassner.SmarterSql.Commands {
	[CommandMenuItem(Menus.MenuGroups.Root, "Clean up code", "Clean up code", "SQL Query Editor::Ctrl+Alt+F", 3)]
	[CommandVisibleMenuAttribute(true)]
	internal class CommandCleanUpCode : CommandBase {
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
			TextEditor textEditor = Instance.TextEditor;
			textEditor.ScanningThread.StartAsynchroneFullParse(10, () => textEditor.CleanUpCode.RunCleanUpCodeComplete());
		}
	}
}
