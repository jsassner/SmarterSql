// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using EnvDTE;
using Sassner.SmarterSql.Commands.CommandAttributes;
using Sassner.SmarterSql.Utils.Menu;

namespace Sassner.SmarterSql.Commands.Goto {
	[CommandMenuItem(Menus.MenuGroups.Goto, "Goto &declaration", "Goto declaration", "SQL Query Editor::Ctrl+B", 1)]
	[CommandVisibleMenuAttribute(true)]
	internal class CommandGotoDeclaration : CommandBase {
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
			Instance.TextEditor.TokenUsage.GotoDeclaration(TextEditor.CurrentWindowData.Parser);
		}
	}
}