// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using EnvDTE;
using Sassner.SmarterSql.Commands.CommandAttributes;
using Sassner.SmarterSql.Utils.Menu;

namespace Sassner.SmarterSql.Commands.Search {
	[CommandMenuItem(Menus.MenuGroups.Search, "Goto &previous", "Goto previous", "SQL Query Editor::Ctrl+Alt+Up Arrow", 3)]
	[CommandVisibleMenuAttribute(true)]
	internal class CommandHighlightGotoPrevious : CommandBase {
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
			Instance.TextEditor.TokenUsage.HighlightTokenGotoPrevious();
		}
	}
}